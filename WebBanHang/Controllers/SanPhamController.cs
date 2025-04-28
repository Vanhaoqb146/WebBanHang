using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }
        public SanPhamController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("sanpham")]
        public IActionResult Index(int? page, string? searchKey, string? sort, string? priceRange, int? minPrice, int? maxPrice)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 9;

                // Lấy danh sách sản phẩm
                IQueryable<SanPham> query = _context.SanPhams
                    .AsNoTracking()
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation);

                // Tìm kiếm theo từ khóa
                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(x => x.TenSp.ToLower().Contains(searchKey.ToLower()));
                }

                // Lọc theo khoảng giá
                if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value >= 0 && maxPrice.Value >= minPrice.Value)
                {
                    query = query.Where(p => p.GiaGiam >= minPrice.Value && p.GiaGiam <= maxPrice.Value);
                }
                else if (!string.IsNullOrEmpty(priceRange) && priceRange != "all")
                {
                    try
                    {
                        var ranges = priceRange.Split('-');
                        if (ranges.Length == 2)
                        {
                            int min = int.Parse(ranges[0]);
                            int max = int.Parse(ranges[1]);
                            query = query.Where(p => p.GiaGiam >= min && p.GiaGiam <= max);
                        }
                        else if (ranges.Length == 1 && priceRange.EndsWith("-"))
                        {
                            int min = int.Parse(ranges[0]);
                            query = query.Where(p => p.GiaGiam >= min);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nếu cần
                    }
                }

                // Sắp xếp (áp dụng sau cùng)
                string sortValue = sort ?? "0"; // Sử dụng chuỗi để đảm bảo so sánh nhất quán

                // Sắp xếp dựa trên giá trị sort
                switch (sortValue)
                {
                    case "1":
                        query = query.OrderBy(x => x.GiaGiam);
                        break;
                    case "2":
                        query = query.OrderByDescending(x => x.GiaGiam);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.MaSp);
                        break;
                }

                // Phân trang
                PagedList<SanPham> models = new PagedList<SanPham>(query, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;

                // Lưu trạng thái bộ lọc vào ViewBag
                ViewBag.PriceRange = priceRange ?? "all";
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SearchKey = searchKey;
                ViewBag.Sort = sortValue; // Đảm bảo lưu giá trị sortValue

                // Lấy danh mục sản phẩm
                var dmsp = _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai)
                    .ToList();
                ViewBag.DMSP = dmsp;

                // Lấy sản phẩm mới
                var spmoi = _context.SanPhams
                    .AsNoTracking()
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation)
                    .OrderByDescending(x => x.MaSp)
                    .Take(3)
                    .ToList();
                ViewBag.SPM = spmoi;

                return View(models);
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ở đây để debug
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("sanpham/sort")]
        public IActionResult Sort(int sort)
        {
            var url = $"/sanpham?sort={sort}";
            if (sort == 0)
            {
                url = $"/sanpham";
            }
            else
            {
                //if(maLoai==0) url = $"/Admin/SanPhams?maTh={maTh}&stt={stt}";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }

        [Route("sanphams/{MaLoai}")]
        public IActionResult List(int MaLoai, int page = 1, string? searchKey = null, string? sort = null,
    string? priceRange = null, int? minPrice = null, int? maxPrice = null)
        {
            try
            {
                var pageNumber = page;
                var pageSize = 9;

                // Lấy danh mục sản phẩm
                var dmsp = _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai).ToList();
                ViewBag.DMSP = dmsp;

                var danhmuc = _context.LoaiSanPhams
                    .AsNoTracking()
                    .SingleOrDefault(x => x.MaLoai == MaLoai);

                // Lấy danh sách sản phẩm với điều kiện MaLoai
                IQueryable<SanPham> query = _context.SanPhams
                    .AsNoTracking()
                    .Where(x => x.MaLoai == MaLoai)
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation);

                // Tìm kiếm theo từ khóa
                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(x => x.TenSp.ToLower().Contains(searchKey.ToLower()));
                }

                // Lọc theo khoảng giá
                if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value >= 0 && maxPrice.Value >= minPrice.Value)
                {
                    query = query.Where(p => p.GiaGiam >= minPrice.Value && p.GiaGiam <= maxPrice.Value);
                }
                else if (!string.IsNullOrEmpty(priceRange) && priceRange != "all")
                {
                    try
                    {
                        var ranges = priceRange.Split('-');
                        if (ranges.Length == 2)
                        {
                            int min = int.Parse(ranges[0]);
                            int max = int.Parse(ranges[1]);
                            query = query.Where(p => p.GiaGiam >= min && p.GiaGiam <= max);
                        }
                        else if (ranges.Length == 1 && priceRange.EndsWith("-"))
                        {
                            int min = int.Parse(ranges[0]);
                            query = query.Where(p => p.GiaGiam >= min);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nếu cần
                    }
                }

                // Sắp xếp (áp dụng sau cùng)
                string sortValue = sort ?? "0"; // Sử dụng chuỗi để đảm bảo so sánh nhất quán

                // Sắp xếp dựa trên giá trị sort
                switch (sortValue)
                {
                    case "1":
                        query = query.OrderBy(x => x.GiaGiam);
                        break;
                    case "2":
                        query = query.OrderByDescending(x => x.GiaGiam);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.MaSp);
                        break;
                }

                // Phân trang
                PagedList<SanPham> models = new PagedList<SanPham>(query, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentLoaiSP = danhmuc;

                // Lưu trạng thái bộ lọc vào ViewBag
                ViewBag.PriceRange = priceRange ?? "all";
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SearchKey = searchKey;
                ViewBag.Sort = sortValue;

                // Lấy sản phẩm mới cho sidebar
                var spmoi = _context.SanPhams
     .AsNoTracking()
     .Include(s => s.MaLoaiNavigation)
     .Include(s => s.MaThNavigation)
     .OrderByDescending(x => x.MaSp)
     .Take(3)
     .ToList() ?? new List<SanPham>();
                ViewBag.SPM = spmoi;


                return View(models);
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ở đây để debug
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("sanpham/{id}")]
        public IActionResult Detail(int id)
        {
            try
            {
                var product = _context.SanPhams
                .Include(x => x.MaLoaiNavigation)
                .Include(x => x.MaThNavigation)
                .FirstOrDefault(x => x.MaSp == id);

                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.SanPhams
                    .AsNoTracking()
                    .Where(x => x.MaLoai == product.MaLoai && x.MaSp != id)
                    .Take(4)
                    .OrderByDescending(x => x.MaSp)
                    .ToList();
                ViewBag.SanPham = lsProduct;

                var lsDG = _context.DanhGiaSanPhams
                    .Include(x => x.MaKhNavigation)
                    .AsNoTracking()
                    .Where(x => x.MaSp == id)
                    .OrderByDescending(x => x.MaDg)
                    .Take(5)
                    .ToList();
                ViewBag.DanhGia = lsDG;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        [Route("sanpham/danhgia")]
        public async Task<IActionResult> DanhGia(int? id, byte diem, string noiDung)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "SanPham");
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("DangNhap", "Accounts");
                }
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    return NotFound();
                }
                //

                //
                DanhGiaSanPham dgsp = new DanhGiaSanPham();
                dgsp.MaKh = khachhang.MaKh;
                dgsp.MaSp = id;
                dgsp.Diem = diem;
                dgsp.NoiDung = noiDung;
                dgsp.ThoiGian = DateTime.Now;

                _context.Add(dgsp);
                _context.SaveChanges();

                _notyfService.Success("Đánh giá thành công");

                return RedirectToAction("Detail", "SanPham", new { id = id });

            }
            catch
            {
                return RedirectToAction("Index", "SanPham", new { id = id });
            }

        }

    }
}

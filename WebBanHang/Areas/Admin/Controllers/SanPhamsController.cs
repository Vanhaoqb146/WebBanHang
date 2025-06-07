using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanHang.Helper;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamsController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public SanPhamsController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/SanPhams
        // SỬA LẠI ACTION INDEX
        public async Task<IActionResult> Index(int page = 1, int MaLoai = 0, int? GiaTu = null, int? GiaDen = null, string trangThai = "all", string searchKey = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }

            var pageNumber = page;
            var pageSize = 10;

            var query = _context.SanPhams
                .AsNoTracking()
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .AsQueryable();

            // THÊM LẠI LOGIC TÌM KIẾM (MỚI)
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.TenSp.ToLower().Contains(searchKey.ToLower()));
            }

            if (trangThai == "active")
            {
                query = query.Where(p => p.DaXoa == false);
            }
            else if (trangThai == "inactive")
            {
                query = query.Where(p => p.DaXoa == true);
            }

            if (MaLoai > 0)
            {
                query = query.Where(x => x.MaLoai == MaLoai);
            }
            if (GiaTu.HasValue && GiaTu > 0)
            {
                query = query.Where(x => x.GiaBan.HasValue && x.GiaBan >= GiaTu.Value);
            }
            if (GiaDen.HasValue && GiaDen > 0)
            {
                query = query.Where(x => x.GiaBan.HasValue && x.GiaBan <= GiaDen.Value);
            }

            var lsSanPham = await query.OrderByDescending(x => x.MaSp).ToListAsync();
            PagedList<SanPham> models = new PagedList<SanPham>(lsSanPham.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentSearchKey = searchKey; // (MỚI)
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentMaLoai = MaLoai;
            ViewBag.CurrentGiaTu = GiaTu;
            ViewBag.CurrentGiaDen = GiaDen;
            ViewBag.CurrentTrangThai = trangThai;

            if (!lsSanPham.Any())
            {
                ViewBag.Message = "Không tìm thấy sản phẩm nào thỏa mãn điều kiện lọc.";
            }

            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh");

            return View(models);
        }

        // Filter
        // SỬA LẠI ACTION FILTER
        public IActionResult Filter(int MaLoai = 0, int? GiaTu = null, int? GiaDen = null, string trangThai = "all", string searchKey = null)
        {
            var url = $"/Admin/SanPhams?page=1&trangThai={trangThai}";

            if (MaLoai > 0)
            {
                url += $"&MaLoai={MaLoai}";
            }
            if (GiaTu.HasValue && GiaTu > 0)
            {
                url += $"&GiaTu={GiaTu.Value}";
            }
            if (GiaDen.HasValue && GiaDen > 0)
            {
                url += $"&GiaDen={GiaDen.Value}";
            }
            // THÊM LẠI LOGIC TÌM KIẾM (MỚI)
            if (!string.IsNullOrEmpty(searchKey))
            {
                url += $"&searchKey={searchKey}";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create()
        {
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh");
            return View();
        }

        // POST: Admin/SanPhams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,GiaBan,GiaGiam,SoLuongCo,Anh,CongSuat,KhoiLuong,MoTa,BaoHanh,MaLoai,MaTh")] SanPham sanPham, Microsoft.AspNetCore.Http.IFormFile fAnh)
        {
            if (ModelState.IsValid)
            {
                if (sanPham.GiaGiam >= sanPham.GiaBan)
                {
                    _notyfService.Warning("Giá giảm phải nhỏ hơn giá bán");
                    return View(sanPham);
                }
                sanPham.TenSp = Utilities.ToTitleCase(sanPham.TenSp);
                if (fAnh != null)
                {
                    string extension = Path.GetExtension(fAnh.FileName);
                    string img = Utilities.SEOUrl(sanPham.TenSp) + "-" + Utilities.RandomGuid() + extension;
                    sanPham.Anh = await Utilities.UploadFile(fAnh, @"sanpham", img.ToLower());
                }
                if (string.IsNullOrEmpty(sanPham.TenSp)) sanPham.Anh = "default.jpg";

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,TenSp,GiaBan,GiaGiam,SoLuongCo,Anh,CongSuat,KhoiLuong,MoTa,BaoHanh,MaLoai,MaTh")] SanPham sanPham, Microsoft.AspNetCore.Http.IFormFile fAnh)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Bước 1: Tải đối tượng gốc từ Database
                    var sanPhamToUpdate = await _context.SanPhams.FindAsync(id);
                    if (sanPhamToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Bước 2: Cập nhật các thuộc tính từ form vào đối tượng gốc
                    sanPhamToUpdate.TenSp = Utilities.ToTitleCase(sanPham.TenSp);
                    sanPhamToUpdate.GiaBan = sanPham.GiaBan;
                    sanPhamToUpdate.GiaGiam = sanPham.GiaGiam;
                    sanPhamToUpdate.SoLuongCo = sanPham.SoLuongCo;
                    sanPhamToUpdate.MoTa = sanPham.MoTa;
                    sanPhamToUpdate.BaoHanh = sanPham.BaoHanh;
                    sanPhamToUpdate.MaLoai = sanPham.MaLoai;
                    sanPhamToUpdate.MaTh = sanPham.MaTh;


                    if (fAnh != null)
                    {
                        string extension = Path.GetExtension(fAnh.FileName);
                        string img = Utilities.SEOUrl(sanPham.TenSp) + "-" + Utilities.RandomGuid() + extension; // Tạo tên file ảnh mới để tránh cache
                        sanPhamToUpdate.Anh = await Utilities.UploadFile(fAnh, @"sanpham", img.ToLower());
                    }

                    _context.Update(sanPhamToUpdate); // Bước 3: Đánh dấu đối tượng là đã thay đổi
                    await _context.SaveChangesAsync(); // Bước 4: Lưu vào DB
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var daPhatSinhGiaoDich = await _context.ChiTietDonHangs.AsNoTracking().AnyAsync(ct => ct.MaSp == id);

            if (daPhatSinhGiaoDich)
            {
                try
                {
                    var sanPhamToUpdate = await _context.SanPhams.FindAsync(id);
                    if (sanPhamToUpdate != null)
                    {
                        sanPhamToUpdate.DaXoa = true;
                        _context.Update(sanPhamToUpdate);
                        await _context.SaveChangesAsync();
                        _notyfService.Success("Sản phẩm đã được chuyển sang trạng thái Ngừng kinh doanh.");
                    }
                }
                catch (Exception ex)
                {
                    _notyfService.Error("Lỗi khi cập nhật trạng thái sản phẩm: " + ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var sanPhamToDelete = await _context.SanPhams.FindAsync(id);
                if (sanPhamToDelete != null)
                {
                    _context.SanPhams.Remove(sanPhamToDelete);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Đã xóa sản phẩm thành công vì chưa có trong đơn hàng.");
                }
            }
            catch (Exception ex)
            {
                _notyfService.Error("Lỗi khi xóa sản phẩm: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // THÊM MỚI ACTION RESTORE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                sanPham.DaXoa = false; // Chuyển trạng thái về đang bán
                _context.Update(sanPham);
                await _context.SaveChangesAsync();
                _notyfService.Success("Đã phục hồi sản phẩm thành công!");
            }
            else
            {
                _notyfService.Error("Không tìm thấy sản phẩm.");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }

    }
}
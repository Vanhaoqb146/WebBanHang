using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers
{
    public class DonHangController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public DonHangController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string getLocation(string maxa, string maqh, string matp)
        {
            try
            {
                var xa = _context.XaPhuongThiTrans.AsNoTracking()
                    .SingleOrDefault(x => x.Maxa == maxa);
                var qh = _context.QuanHuyens.AsNoTracking()
                    .SingleOrDefault(x => x.Maqh == maqh);
                var tp = _context.TinhThanhPhos.AsNoTracking()
                    .SingleOrDefault(x => x.Matp == matp);

                if (xa != null && qh != null && tp != null)
                {
                    return $"{xa.Name}, {qh.Name}, {tp.Name}";
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
                var donhang = await _context.DonHangs
                    .Include(x => x.MaTtNavigation)
                    .Include(x => x.MaKhNavigation)
                    .Include(x => x.MaShipperNavigation)
                    .FirstOrDefaultAsync(m => m.MaDh == id && Convert.ToInt32(taikhoanID) == m.MaKh);
                if (donhang == null)
                {
                    return NotFound();
                }

                var ctdh = _context.ChiTietDonHangs
                    .AsNoTracking()
                    .Where(x => x.MaDh == id)
                    .Include(x => x.MaSpNavigation)
                    .OrderBy(x => x.MaSp)
                    .ToList();

                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = ctdh;

                string fullAddress = $"{donhang.DiaChi}, {getLocation(donhang.Maxa, donhang.Maqh, donhang.Matp)}";
                ViewBag.FullAddress = fullAddress;

                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }

        }
        [HttpPost]
        public IActionResult XacNhanTrangThai([FromBody] XacNhanTrangThaiRequest request)
        {
            var donHang = _context.DonHangs
                .Include(d => d.MaTtNavigation)
                .FirstOrDefault(d => d.MaDh == request.MaDonHang);

            if (donHang == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            // Cập nhật trạng thái đơn hàng
            donHang.TrangThaiXacNhan = request.DaNhan ? 1 : -1; // 1: Đã nhận hàng, -1: Chưa nhận hàng

            // Cập nhật NgayShip
            //if (request.DaNhan)
            //{
            //    donHang.NgayShip = DateTime.Now; // Đặt ngày giao hàng là ngày hiện tại
            //}
            //else
            //{
            //    donHang.NgayShip = null; // Xóa ngày giao hàng nếu chưa nhận hàng
            //}

            _context.SaveChanges();

            return Ok(new { message = (request.DaNhan ? "Cập nhật trạng thái thành công." : "Đã gửi báo cáo đến cửa hàng.\n Vui lòng đợi liên hệ") });
        }


        public class XacNhanTrangThaiRequest
        {
            public int MaDonHang { get; set; }
            public bool DaNhan { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> Huy(int? madh)
        {
            if (madh == null)
            {
                return RedirectToAction("Dashboard", "Accounts");
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

                var donhang = await _context.DonHangs
                    .FirstOrDefaultAsync(m => m.MaDh == madh && Convert.ToInt32(taikhoanID) == m.MaKh);
                if (donhang == null)
                {
                    return RedirectToAction("Dashboard", "Accounts");
                }
                if (donhang.MaTt != 1)
                {
                    _notyfService.Warning("Hủy thất bại");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                var ctdh = _context.ChiTietDonHangs
                    .AsNoTracking()
                    .Where(x => x.MaDh == madh)
                    .Include(x => x.MaSpNavigation)
                    .OrderBy(x => x.MaSp)
                    .ToList();
                foreach (var item in ctdh)
                {
                    SanPham hh = _context.SanPhams.SingleOrDefault(p => p.MaSp == item.MaSp);
                    hh.SoLuongCo += item.SoLuong;
                    _context.Update(hh);
                }

                donhang.MaTt = 6;

                _context.Update(donhang);
                _context.SaveChanges();

                _notyfService.Success("Hủy thành công");

                return RedirectToAction("Dashboard", "Accounts");

            }
            catch
            {
                return RedirectToAction("Dashboard", "Accounts");
            }

        }


    }
}

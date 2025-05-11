using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebBanHang.Models;
namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }
            var lsp = _context.LoaiSanPhams.Count();
            ViewBag.LSP = lsp;
            var sp = _context.SanPhams.Count();
            ViewBag.SP = sp;
            var dh = _context.DonHangs.Count();
            ViewBag.DH = dh;
            var kh = _context.KhachHangs.Count();
            ViewBag.KH = kh;

            // Lấy thông tin doanh thu
            var salesData = _context.DonHangs
                .Where(s => s.NgayDat >= DateTime.Today.AddMonths(-12) && s.MaTt == 4)
                .GroupBy(s => s.NgayDat.Value.Month)
                .Select(g => new { Month = g.Key, SalesTotal = g.Sum(s => s.TongTien) })
                .OrderBy(g => g.Month)
                .ToList();
            ViewBag.SalesData = salesData;

            // Lấy đầy đủ 6 trạng thái đơn hàng
            var choXuLy = _context.DonHangs.Count(x => x.MaTt == 1);
            var daXacNhan = _context.DonHangs.Count(x => x.MaTt == 2);
            var dangGiao = _context.DonHangs.Count(x => x.MaTt == 3);
            var thanhCong = _context.DonHangs.Count(x => x.MaTt == 4);
            var khongThanhCong = _context.DonHangs.Count(x => x.MaTt == 5);
            var daHuy = _context.DonHangs.Count(x => x.MaTt == 6);

            // Đưa dữ liệu vào ViewBag
            ViewBag.ChoXuLy = choXuLy;
            ViewBag.DaXacNhan = daXacNhan;
            ViewBag.DangGiao = dangGiao;
            ViewBag.ThanhCong = thanhCong;
            ViewBag.KhongThanhCong = khongThanhCong;
            ViewBag.DaHuy = daHuy;

            // Lấy thông tin top sản phẩm bán chạy
            var topSellingProducts = _context.ChiTietDonHangs
     .Include(x => x.MaSpNavigation)
     .Where(od => od.MaDhNavigation.MaTt == 4)
     .GroupBy(od => new { od.MaSp, TenSp = od.MaSpNavigation.TenSp })
     .Select(g => new {
         ProductId = g.Key.MaSp,
         ProductName = g.Key.TenSp,
         Quantity = g.Sum(od => od.SoLuong ?? 0)
     })
     .OrderByDescending(g => g.Quantity)
     .Take(5)
     .ToList();

            ViewBag.TopSellingProducts = topSellingProducts;

            return View();
        }
    }
}
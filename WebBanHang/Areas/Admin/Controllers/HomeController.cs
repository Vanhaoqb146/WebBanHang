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

            //
            var salesData = _context.DonHangs
            .Where(s => s.NgayDat >= DateTime.Today.AddMonths(-12) && s.MaTt == 4)
            .GroupBy(s => s.NgayDat.Value.Month)
            .Select(g => new { Month = g.Key, SalesTotal = g.Sum(s => s.TongTien) })
            .OrderBy(g => g.Month)
            .ToList();

            ViewBag.SalesData = salesData;

            var dh1 = _context.DonHangs
                .Where(x => x.MaTt == 1)
                .Count();
            var dh2 = _context.DonHangs
                .Where(x => x.MaTt == 4)
                .Count();
            var dh3 = _context.DonHangs
                .Where(x => x.MaTt == 6)
                .Count();
            ViewBag.DH1 = dh1;
            ViewBag.DH2 = dh2;
            ViewBag.DH3 = dh3;
            ViewBag.Ten1 = "Chờ xử lý";
            ViewBag.Ten2 = "Giao thành công";
            ViewBag.Ten3 = "Đã hủy";


            var topSellingProducts = _context.ChiTietDonHangs
                .Include(x => x.MaSpNavigation)
            .GroupBy(od => od.MaSp)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(od => od.SoLuong) })
            .OrderByDescending(g => g.Quantity)
            .Take(5)
            .ToList();

            var productIds = topSellingProducts.Select(p => p.ProductId).ToList();

            var productsName = _context.SanPhams
                .Where(p => productIds.Contains(p.MaSp))
                .Select(g => new { Name = g.TenSp })
                .ToList();

            ViewBag.TopSellingProducts = topSellingProducts;
            ViewBag.ProductName = productsName;


            return View();
        }
    }
}

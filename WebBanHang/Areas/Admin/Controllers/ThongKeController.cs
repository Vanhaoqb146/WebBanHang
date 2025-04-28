using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {
        private readonly dbBanHangContext _context;

        public INotyfService _notyfService { get; }

        public ThongKeController(dbBanHangContext context, INotyfService notyfService)
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
            return View();
        }

        [HttpPost]
        public IActionResult Index(DateTime? tuNgay, DateTime? denNgay)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }
            var tk = _context.DonHangs
                .Where(x => x.NgayDat >= tuNgay && x.NgayDat <= denNgay && x.MaTt == 4)
            .GroupBy(o => o.NgayDat.Value.Date)
            .Select(g => new { NgayDat = g.Key, TongDH = g.Count(), TongDT = g.Sum(o => o.TongTien) })
            .ToList();


            List<ThongKeVM> tklist = new List<ThongKeVM>();
            foreach (var item in tk)
            {
                ThongKeVM tkvm = new ThongKeVM();
                tkvm.NgayDat = (DateTime)item.NgayDat;
                tkvm.TongDH = item.TongDH;
                tkvm.TongDT = (int)item.TongDT;
                tklist.Add(tkvm);
            }

            return View(tklist);
        }
    }
}

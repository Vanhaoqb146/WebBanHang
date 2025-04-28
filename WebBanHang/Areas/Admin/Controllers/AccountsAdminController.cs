using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanHang.Extension;
using WebBanHang.Helper;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsAdminController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }
        public AccountsAdminController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        //[AllowAnonymous]
        //[Route("dangnhap")]
        public IActionResult DangNhap()
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (taikhoanID != null)
            {

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        //[Route("dangnhap")]
        public async Task<IActionResult> DangNhap(LoginAdminVM customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.QuanTriViens.AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null)
                    {
                        _notyfService.Warning("Thông tin đăng nhập không chính xác");
                        return View(customer);
                    }
                    

                    string pass = customer.Password;
                    if (khachhang.MatKhau != pass)
                    {
                        _notyfService.Warning("Thông tin đăng nhập không chính xác");
                        return View(customer);
                    }

                    // lưu session
                    HttpContext.Session.SetString("AdminId", khachhang.MaQtv.ToString());
                    var taikhoanID = HttpContext.Session.GetString("AdminId");

                    //Identity?
                    //var claims = new List<Claim>
                    //    {
                    //        new Claim(ClaimTypes.Name, khachhang.TenKh),
                    //        new Claim("CustomerId", khachhang.MaKh.ToString())
                    //    };
                    //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    //await HttpContext.SignInAsync(claimsPrincipal);

                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");

                }
            }
            catch
            {
                return View(customer);

            }
            return View(customer);
        }
        [HttpGet]
        public IActionResult DangXuat()
        {
            //HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public IActionResult Info()
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (taikhoanID != null)
            {
                var khachhang = _context.QuanTriViens.AsNoTracking()
                    .SingleOrDefault(x => x.MaQtv == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {

                    return View(khachhang);
                }

            }

            return RedirectToAction("DangNhap");
        }
        [HttpPost]
        public IActionResult Info(QuanTriVien model, string MKC,string MKM, string NLMKM)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AdminId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("DangNhap");
                }

                if (ModelState.IsValid)
                {
                    var taikhoan = _context.QuanTriViens.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("DangNhap");

                    var pass = (MKC.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    if (pass == taikhoan.MatKhau)
                    {
                        if (MKM != null)
                        {
                            string passnew = (MKM.Trim() + taikhoan.Salt.Trim()).ToMD5();
                            taikhoan.MatKhau = passnew;
                        }

                        taikhoan.TenQtv = model.TenQtv;

                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Cập nhật thành công");

                        return View();
                    }
                }

            }
            catch
            {
                _notyfService.Warning("Cập nhật không thành công");
                return View();
            }
            _notyfService.Warning("Cập nhật không thành công");
            return View();
        }
    }
}

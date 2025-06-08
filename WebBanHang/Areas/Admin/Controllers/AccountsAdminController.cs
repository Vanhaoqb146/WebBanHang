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
        public async Task<IActionResult> Info(QuanTriVien model, string MKC, string MKM, string NLMKM)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (taikhoanID == null)
            {
                return RedirectToAction("DangNhap");
            }

            var taikhoan = await _context.QuanTriViens.FindAsync(Convert.ToInt32(taikhoanID));
            if (taikhoan == null)
            {
                _notyfService.Error("Không tìm thấy tài khoản");
                return RedirectToAction("DangNhap");
            }

            // ==========================================================
            // BẮT ĐẦU THAY ĐỔI LOGIC
            // ==========================================================

            // Bước 1: So sánh trực tiếp mật khẩu cũ người dùng nhập (MKC) với mật khẩu trong DB
            if (MKC.Trim() == taikhoan.MatKhau)
            {
                try
                {
                    // Cập nhật thông tin cá nhân khác
                    taikhoan.TenQtv = model.TenQtv;

                    // Cập nhật mật khẩu mới nếu người dùng có nhập
                    if (!string.IsNullOrEmpty(MKM))
                    {
                        if (MKM != NLMKM)
                        {
                            _notyfService.Error("Mật khẩu mới không trùng khớp");
                            // Trả về View với thông tin tài khoản để người dùng không phải nhập lại tên
                            return View(taikhoan);
                        }
                        // Gán trực tiếp mật khẩu mới (dạng plain text) vào database
                        taikhoan.MatKhau = MKM.Trim();
                    }

                    _context.Update(taikhoan);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (Exception ex)
                {
                    _notyfService.Error("Đã xảy ra lỗi: " + ex.Message);
                }
            }
            else
            {
                // Nếu mật khẩu cũ không chính xác, thông báo lỗi cụ thể
                _notyfService.Error("Mật khẩu hiện tại không chính xác");
                return View(taikhoan);
            }

            // Tải lại trang Info sau khi cập nhật
            return RedirectToAction(nameof(Info));
        }
    }
}
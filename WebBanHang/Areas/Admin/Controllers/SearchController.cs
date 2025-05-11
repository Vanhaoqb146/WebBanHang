using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly dbBanHangContext _context;

        public SearchController(dbBanHangContext context)
        {
            _context = context;
        }

        // GET: Search/TimSanPham
        public IActionResult TimSanPham(string searchKey)
        {
            List<SanPham> ls = new List<SanPham>();
            if (string.IsNullOrEmpty(searchKey) || searchKey.Length < 1)
            {
                ls = _context.SanPhams
                .AsNoTracking()
                .Include(x => x.MaLoaiNavigation)
                .Include(x => x.MaThNavigation)
                .OrderByDescending(x => x.TenSp)
                .Take(10)
                .ToList();
            }
            else
            {
                ls = _context.SanPhams
                .AsNoTracking()
                .Include(x => x.MaLoaiNavigation)
                .Include(x => x.MaThNavigation)
                .Where(x => x.TenSp.Contains(searchKey))
                .OrderByDescending(x => x.TenSp)
                .Take(10)
                .ToList();
            }

            if (ls == null)
            {
                return PartialView("ListSanPhamSearchPartial", null);
            }
            else
            {
                return PartialView("ListSanPhamSearchPartial", ls);

            }
        }

        public IActionResult TimDonHang(string searchKey)
        {
            List<DonHang> ls = new List<DonHang>();

            if (string.IsNullOrEmpty(searchKey) || searchKey.Length < 1)
            {
                // Lấy danh sách đơn hàng mặc định
                ls = _context.DonHangs
                    .AsNoTracking()
                    .Include(x => x.MaTtNavigation)
                    .OrderByDescending(x => x.NgayDat)
                    .Take(10)
                    .ToList();
            }
            else
            {
                // Kiểm tra xem searchKey có phải là số hợp lệ không
                if (int.TryParse(searchKey, out int maDh))
                {
                    // Tìm kiếm theo mã đơn hàng
                    ls = _context.DonHangs
                        .AsNoTracking()
                        .Include(x => x.MaTtNavigation)
                        .Where(x => x.MaDh == maDh)
                        .OrderByDescending(x => x.NgayDat)
                        .Take(10)
                        .ToList();
                }
                else
                {
                    // Nếu searchKey không phải là số, trả về danh sách rỗng hoặc thông báo lỗi
                    return PartialView("TimDonHangSearchPartial", null);
                }
            }

            if (ls == null || !ls.Any())
            {
                return PartialView("TimDonHangSearchPartial", null);
            }
            else
            {
                return PartialView("TimDonHangSearchPartial", ls);
            }
        }

        public IActionResult TimKH(string searchKey)
        {
            List<KhachHang> ls = new List<KhachHang>();
            if (string.IsNullOrEmpty(searchKey) || searchKey.Length < 1)
            {
                ls = _context.KhachHangs
                    .AsNoTracking()
                    .OrderByDescending(x => x.MaKh)
                    .Take(10)
                    .ToList();
            }
            else
            {
                ls = _context.KhachHangs
                .AsNoTracking()
                .Where(x => x.Email.Contains(searchKey) || x.Sdt.Contains(searchKey))
                .OrderByDescending(x => x.MaKh)
                .Take(10)
                .ToList();
            }

            if (ls == null)
            {
                return PartialView("TimKHSearchPartial", null);
            }
            else
            {
                return PartialView("TimKHSearchPartial", ls);

            }
        }
    }
}

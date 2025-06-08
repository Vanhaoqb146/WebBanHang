using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public KhachHangsController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/KhachHangs
        // SỬA LẠI ACTION INDEX
        public async Task<IActionResult> Index(int page = 1, int TrangThai = -1, string searchKey = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "Accounts", new { Area = "" });
            }

            var pageNumber = page;
            var pageSize = 10;

            IQueryable<KhachHang> query = _context.KhachHangs.AsNoTracking();

            // Lọc theo trạng thái
            if (TrangThai != -1)
            {
                bool isKhoa = Convert.ToBoolean(TrangThai);
                query = query.Where(x => x.Khoa == isKhoa);
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.Email.Contains(searchKey) || x.Sdt.Contains(searchKey) || x.TenKh.Contains(searchKey));
            }

            var lsKH = await query.OrderByDescending(x => x.MaKh).ToListAsync();
            PagedList<KhachHang> models = new PagedList<KhachHang>(lsKH.AsQueryable(), pageNumber, pageSize);

            // Giữ lại trạng thái lọc
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentTrangThai = TrangThai;
            ViewBag.CurrentSearchKey = searchKey;

            // Tạo danh sách cho dropdown
            ViewData["lsTrangThai"] = new List<SelectListItem>
            {
                new SelectListItem { Text = "Tất cả", Value = "-1" },
                new SelectListItem { Text = "Hoạt động", Value = "0" },
                new SelectListItem { Text = "Khóa", Value = "1" }
            };

            return View(models);
        }
        public IActionResult Filter(int TrangThai = -1)
        {
            var url = $"/Admin/KhachHangs?TrangThai={TrangThai}";
            if (TrangThai == -1)
            {
                url = $"/Admin/KhachHangs";
            }
            else
            {
                //if(maLoai==0) url = $"/Admin/SanPhams?maTh={maTh}&stt={stt}";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }

        // GET: Admin/KhachHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
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


        // THÊM ACTION TOGGLESTATUS MỚI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                _notyfService.Error("Không tìm thấy khách hàng");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                khachHang.Khoa = !khachHang.Khoa;
                _context.Update(khachHang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thay đổi trạng thái thành công!");
            }
            catch
            {
                _notyfService.Error("Có lỗi xảy ra khi thay đổi trạng thái.");
            }

            return RedirectToAction(nameof(Index));
        }





        // GET: Admin/KhachHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }


    }
}

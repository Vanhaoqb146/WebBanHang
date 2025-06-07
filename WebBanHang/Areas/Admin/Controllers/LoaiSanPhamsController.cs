using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaiSanPhamsController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public LoaiSanPhamsController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/LoaiSanPhams
        public async Task<IActionResult> Index()
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }

            // Lấy danh sách loại sản phẩm và đếm số lượng sản phẩm của từng loại
            var loaiSanPhams = await _context.LoaiSanPhams
                .Include(loai => loai.SanPhams) // Include để lấy danh sách sản phẩm
                .ToListAsync();

            // Gán số lượng sản phẩm cho từng loại
            foreach (var loai in loaiSanPhams)
            {
                loai.SoLuong = loai.SanPhams.Count();
            }

            return View(loaiSanPhams);
        }



        // GET: Admin/LoaiSanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPhams
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return View(loaiSanPham);
        }

        // GET: Admin/LoaiSanPhams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoai,TenLoai,MoTa")] LoaiSanPham loaiSanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiSanPham);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSanPham);
        }

        // GET: Admin/LoaiSanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }
            return View(loaiSanPham);
        }

        // POST: Admin/LoaiSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoai,TenLoai,MoTa")] LoaiSanPham loaiSanPham)
        {
            if (id != loaiSanPham.MaLoai)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSanPham);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSanPhamExists(loaiSanPham.MaLoai))
                    {
                        _notyfService.Warning("Có lỗi xảy ra");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSanPham);
        }

        // GET: Admin/LoaiSanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPhams
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return View(loaiSanPham);
        }

        // POST: Admin/LoaiSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kiểm tra xem có sản phẩm nào thuộc loại này không
            var coSanPham = await _context.SanPhams.AnyAsync(p => p.MaLoai == id);

            if (coSanPham)
            {
                // Nếu có sản phẩm, hiển thị thông báo lỗi cụ thể và không xóa
                _notyfService.Error("Không thể xóa loại sản phẩm này vì vẫn còn sản phẩm thuộc loại này. Vui lòng chuyển các sản phẩm sang loại khác hoặc xóa chúng trước.");
                return RedirectToAction(nameof(Index));
            }

            //Nếu không có sản phẩm nào, tiến hành xóa
            try
            {
                var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);
                if (loaiSanPham != null)
                {
                    _context.LoaiSanPhams.Remove(loaiSanPham);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Xóa loại sản phẩm thành công");
                }
                else
                {
                    _notyfService.Error("Không tìm thấy loại sản phẩm");
                }
            }
            catch (Exception ex)
            {
                // Bắt các lỗi không mong muốn khác
                _notyfService.Error("Đã xảy ra lỗi trong quá trình xóa: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoaiSanPhamExists(int id)
        {
            return _context.LoaiSanPhams.Any(e => e.MaLoai == id);
        }
    }
}

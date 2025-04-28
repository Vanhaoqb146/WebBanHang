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
    public class KhuyenMaisController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public KhuyenMaisController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/KhuyenMais
        public async Task<IActionResult> Index()
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }
            return View(await _context.KhuyenMais.ToListAsync());
        }

        // GET: Admin/KhuyenMais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais
                .FirstOrDefaultAsync(m => m.MaKm == id);
            if (khuyenMai == null)
            {
                return NotFound();
            }

            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhuyenMais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKm,MoTa,MaNhap,SoLuong,GiaTriToiThieu,GiaTriGiam,NgayBatDau,NgayKetThuc")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                if(_context.KhuyenMais.Where(x=>x.MaNhap.ToUpper() == khuyenMai.MaNhap.ToUpper()) != null)
                {
                    _notyfService.Warning("Mã nhập trùng");
                    return View(khuyenMai);
                }

                khuyenMai.MaNhap = khuyenMai.MaNhap.ToUpper();
                _context.Add(khuyenMai);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null)
            {
                return NotFound();
            }
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKm,MoTa,MaNhap,SoLuong,GiaTriToiThieu,GiaTriGiam,NgayBatDau,NgayKetThuc")] KhuyenMai khuyenMai)
        {
            if (id != khuyenMai.MaKm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khuyenMai);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhuyenMaiExists(khuyenMai.MaKm))
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
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais
                .FirstOrDefaultAsync(m => m.MaKm == id);
            if (khuyenMai == null)
            {
                return NotFound();
            }

            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var khuyenMai = await _context.KhuyenMais.FindAsync(id);
                _context.KhuyenMais.Remove(khuyenMai);
                await _context.SaveChangesAsync();
                _notyfService.Success("Xóa thành công");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notyfService.Warning("Xóa thất bại");
                return RedirectToAction(nameof(Index));

            }

        }

        private bool KhuyenMaiExists(int id)
        {
            return _context.KhuyenMais.Any(e => e.MaKm == id);
        }



        public async Task<IActionResult> GiamGia()
        {
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GiamGia(int MaLoai, int PhanTram)
        {
            var loaiSanPham = await _context.LoaiSanPhams
                .FirstOrDefaultAsync(m => m.MaLoai == MaLoai);
            if (loaiSanPham == null)
            {
                return RedirectToAction("GiamGia");
            }

            var lsSP = _context.SanPhams.Where(x => x.MaLoai == MaLoai);
            if (lsSP.Count() == 0)
            {
                _notyfService.Warning("Loại sản phẩm không có sản phẩm nào");
                return RedirectToAction("GiamGia");
            }
            if (PhanTram == 0)
            {
                _notyfService.Warning("Phần trăm giảm lớn hơn 0");
                return RedirectToAction("GiamGia");
            }

            foreach (var item in lsSP)
            {
                item.GiaGiam = item.GiaBan * (PhanTram / 100);
                _context.SanPhams.Update(item);
            }



            await _context.SaveChangesAsync();
            _notyfService.Success("Giảm giá thành công");

            return RedirectToAction("GiamGia");
        }
    }
}

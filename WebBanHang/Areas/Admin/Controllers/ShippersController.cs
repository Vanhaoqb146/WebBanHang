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
using WebBanHang.Extension;
using WebBanHang.Helper;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippersController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public ShippersController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Shippers
        public async Task<IActionResult> Index(int page = 1)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }

            var pageNumber = page;
            var pageSize = 10;

            List<Shipper> lsShip = new List<Shipper>();

            lsShip = _context.Shippers
                .AsNoTracking()
                .OrderByDescending(x => x.MaShipper).ToList();
            //
            PagedList<Shipper> models = new PagedList<Shipper>(lsShip.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: Admin/Shippers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.MaShipper == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // GET: Admin/Shippers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Shippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaShipper,TenShipper,Email,Sdt,MatKhau,LoaiXe,BienSo,Salt")] Shipper shipper)
        {
            if (ModelState.IsValid)
            {
                string salt = Utilities.GetRandomKey();
                //shipper.MatKhau = (shipper.MatKhau + salt.Trim()).ToMD5();
                shipper.MatKhau = shipper.MatKhau;
                shipper.Salt = salt;
                shipper.TenHt = shipper.TenShipper + " - " + shipper.Sdt;
                shipper.Khoa = false;

                _context.Add(shipper);
                await _context.SaveChangesAsync();

                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(shipper);
        }

        // GET: Admin/Shippers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
        }

        // POST: Admin/Shippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaShipper,TenShipper,Email,Sdt,MatKhau,LoaiXe,BienSo,Salt")] Shipper shipper)
        {
            if (id != shipper.MaShipper)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipper);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipperExists(shipper.MaShipper))
                    {
                        _notyfService.Warning("Cập nhật thất bại");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shipper);
        }

        // GET: Admin/Shippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.MaShipper == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // POST: Admin/Shippers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var shipper = await _context.Shippers.FindAsync(id);

                shipper.Khoa = true;
                _context.Update(shipper);
                await _context.SaveChangesAsync();
                _notyfService.Success("Khóa thành công");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notyfService.Warning("Khóa thất bại");
                return RedirectToAction(nameof(Index));
            }
            
        }

        private bool ShipperExists(int id)
        {
            return _context.Shippers.Any(e => e.MaShipper == id);
        }
    }
}

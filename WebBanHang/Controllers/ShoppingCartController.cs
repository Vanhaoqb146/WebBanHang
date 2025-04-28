using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebBanHang.Extension;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public ShoppingCartController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>)){
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = GioHang;
            try
            {
                //thêm
                CartItem item = cart.SingleOrDefault(p => p.product.MaSp == productID);
                if (item != null)
                {
                    SanPham hh = _context.SanPhams.SingleOrDefault(p => p.MaSp == productID);
                    if (amount.Value + item.amount >= hh.SoLuongCo)
                    {
                        item.amount = (int)hh.SoLuongCo;
                    }
                    else
                    {
                        item.amount = item.amount + amount.Value;
                    }

                    //
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    SanPham hh = _context.SanPhams.SingleOrDefault(p => p.MaSp == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    cart.Add(item);//thêm vào giỏ
                }
                //luu sesion
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                _notyfService.Success("Thêm sản phẩm thành công");
                return Json(new {success= true});
            }
            catch
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //lấy giỏ
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.MaSp == productID);
                    //
                    SanPham hh = _context.SanPhams.SingleOrDefault(p => p.MaSp == productID);
                    //
                    if (item!=null && amount.HasValue)
                    {
                        if (amount.Value >= hh.SoLuongCo)
                        {
                            item.amount = (int)hh.SoLuongCo;
                        }
                        else if (amount.Value <= 0)
                        {
                            item.amount = 1;
                        }
                        else
                        {
                            item.amount = amount.Value;
                        }
                    }
                }
                //luu sesion
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                //??giohang or GioHang
                CartItem item = gioHang.SingleOrDefault(p => p.product.MaSp == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //luu sesion

                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }

        [Route("cart", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}

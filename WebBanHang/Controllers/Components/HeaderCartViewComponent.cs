using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebBanHang.Extension;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");            

            if (cart == null || cart.Count == 0)
            {
                
                return View("~/Views/Shared/Components/HeaderCart/Empty.cshtml");
            }         
            return View(cart);
        }
    }
}

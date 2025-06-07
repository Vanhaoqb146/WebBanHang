using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using WebBanHang.ModelViews;


namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbBanHangContext _context;

        public HomeController(ILogger<HomeController> logger, dbBanHangContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? page, [FromQuery] Dictionary<string, int> categoryPages)
        {
            HomeViewVM model = new HomeViewVM();

            // Phân trang cho tab "Tất cả"
            var lsProductsQuery = _context.SanPhams.AsNoTracking()
                .Where(p => p.DaXoa == false)
                .OrderByDescending(x => x.MaSp);
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var pagedProducts = lsProductsQuery.ToPagedList(pageNumber, pageSize);


            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();

            // Lấy danh sách danh mục
            var lsCats = _context.LoaiSanPhams
                .AsNoTracking()
                .OrderBy(x => x.MaLoai)
                .Take(5)
                .ToList();
            _logger.LogInformation($"Số danh mục: {lsCats.Count}");

            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;

                // Lấy số trang cho danh mục từ categoryPages
                int categoryPageNumber = categoryPages.ContainsKey($"page_{item.MaLoai}") ? categoryPages[$"page_{item.MaLoai}"] : 1;

                // Lấy truy vấn sản phẩm theo danh mục
                var lsCategoryProductsQuery = _context.SanPhams.AsNoTracking()
                    .Where(x => x.MaLoai == item.MaLoai && x.DaXoa == false)
                    .OrderByDescending(x => x.MaSp);
                var productCount = lsCategoryProductsQuery.Count();


                // Phân trang cho danh mục
                productHome.lsProducts = lsCategoryProductsQuery.ToPagedList(categoryPageNumber, pageSize);

                lsProductViews.Add(productHome);
            }

            model.Products = lsProductViews;
            ViewBag.AllProducts = pagedProducts;

            return View(model);
        }

        [Route("huong-dan-mua-hang")]
        public IActionResult ShoppingGuide()
        {
            return View();
        }
        [Route("ve-chung-toi")]
        public IActionResult About()
        {
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Collections.Generic;
using WebBanHang.Models;
using PagedList.Core;

namespace WebBanHang.ModelViews
{
    public class ProductHomeVM
    {
        public LoaiSanPham category { get; set; }
        public IPagedList<SanPham> lsProducts { get; set; }
    }
}

using System.Collections.Generic;
using WebBanHang.Models;

namespace WebBanHang.ModelViews
{
    public class XemDonHang
    {
        public DonHang DonHang { get; set; }
        public List<ChiTietDonHang> ChiTietDonHang { get; set; }
    }
}

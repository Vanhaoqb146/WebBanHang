using WebBanHang.Models;

namespace WebBanHang.ModelViews
{
    public class CartItem
    {
        public SanPham product { get; set; }
        public int amount { get; set; }
        public int TotalMoney => amount * product.GiaGiam.Value;
    }
}

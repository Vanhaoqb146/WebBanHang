using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class ChiTietDonHang
    {
        public int MaDh { get; set; }
        public int MaSp { get; set; }
        public int? GiaBan { get; set; }
        public int? GiaGiam { get; set; }
        public int? SoLuong { get; set; }
        public int? TongTien { get; set; }

        public virtual DonHang MaDhNavigation { get; set; }
        public virtual SanPham MaSpNavigation { get; set; }
    }
}

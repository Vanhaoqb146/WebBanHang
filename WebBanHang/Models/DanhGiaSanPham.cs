using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class DanhGiaSanPham
    {
        public int MaDg { get; set; }
        public int? MaKh { get; set; }
        public int? MaSp { get; set; }
        public byte? Diem { get; set; }
        public string NoiDung { get; set; }
        public DateTime? ThoiGian { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual SanPham MaSpNavigation { get; set; }
    }
}

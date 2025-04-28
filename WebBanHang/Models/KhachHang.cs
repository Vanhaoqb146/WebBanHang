using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            DanhGiaSanPhams = new HashSet<DanhGiaSanPham>();
            DonHangs = new HashSet<DonHang>();
        }

        public int MaKh { get; set; }
        public string TenKh { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string MatKhau { get; set; }
        public bool? Khoa { get; set; }
        public string Salt { get; set; }
        public string DiaChi { get; set; }
        public string Matp { get; set; }
        public string Maqh { get; set; }
        public string Maxa { get; set; }

        public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; }
        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

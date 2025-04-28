using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            DanhGiaSanPhams = new HashSet<DanhGiaSanPham>();
        }

        public int MaSp { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập")]
        public string TenSp { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Vui lòng nhập số")]
        public int? GiaBan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Vui lòng nhập số")]
        public int? GiaGiam { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Vui lòng nhập số")]
        public int? SoLuongCo { get; set; }
        public string Anh { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        //public int? CongSuat { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập")]
        //public double? KhoiLuong { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập")]
        public string MoTa { get; set; }
        public string BaoHanh { get; set; }
        public bool? Khoa { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public int? MaLoai { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public int? MaTh { get; set; }

        public virtual LoaiSanPham MaLoaiNavigation { get; set; }
        public virtual ThuongHieu MaThNavigation { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; }
    }
}

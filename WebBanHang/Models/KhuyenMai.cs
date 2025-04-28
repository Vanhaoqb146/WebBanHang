using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.Models
{
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            DonHangs = new HashSet<DonHang>();
        }

        [Required(ErrorMessage = "Vui lòng nhập")]
        public int MaKm { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public string MoTa { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public string MaNhap { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public int? SoLuong { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public int? GiaTriToiThieu { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public int? GiaTriGiam { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public DateTime? NgayBatDau { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public DateTime? NgayKetThuc { get; set; }

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

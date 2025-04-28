using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaShipper { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public string TenShipper { get; set; }
        public string TenHt { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Số điện thoại gồm 10 chữ số")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là số")]
        public string Sdt { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập")]
        public string MatKhau { get; set; }
        public string LoaiXe { get; set; }
        public string BienSo { get; set; }
        public string Salt { get; set; }
        public bool? Khoa { get; set; }

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

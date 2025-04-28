using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class QuanTriVien
    {
        public int MaQtv { get; set; }
        public string TenQtv { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string MatKhau { get; set; }
        public string Salt { get; set; }
        public bool? Khoa { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class TrangThaiDonHang
    {
        public TrangThaiDonHang()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaTt { get; set; }
        public string TenTt { get; set; }
        public string MoTa { get; set; }

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

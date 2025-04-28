using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class TinhThanhPho
    {
        public TinhThanhPho()
        {
            QuanHuyens = new HashSet<QuanHuyen>();
        }

        public string Matp { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }

        public virtual ICollection<QuanHuyen> QuanHuyens { get; set; }
    }
}

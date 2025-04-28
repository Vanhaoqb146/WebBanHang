using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class XaPhuongThiTran
    {
        public string Maxa { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Maqh { get; set; }

        public virtual QuanHuyen MaqhNavigation { get; set; }
    }
}

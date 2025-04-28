using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class QuanHuyen
    {
        public QuanHuyen()
        {
            XaPhuongThiTrans = new HashSet<XaPhuongThiTran>();
        }

        public string Maqh { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Matp { get; set; }

        public virtual TinhThanhPho MatpNavigation { get; set; }
        public virtual ICollection<XaPhuongThiTran> XaPhuongThiTrans { get; set; }
    }
}

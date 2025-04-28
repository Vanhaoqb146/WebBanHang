using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ModelViews
{
    public class MuaHangVM
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }
        public string Email { get; set; }

        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Số điện thoại gồm 10 chữ số")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là số")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Tỉnh/ thành")]
        public string TinhThanh { get; set; }

        [Required(ErrorMessage = "Quận/ huyện")]
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Phường/ xã")]
        public string PhuongXa { get; set; }

        public string Note { get; set; }

    }
}

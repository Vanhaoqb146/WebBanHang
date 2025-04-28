using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ModelViews
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage ="Nhập Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Địa chỉ Email")]
        [EmailAddress]
        public string UserName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Nhập mật khẩu")]
        [MinLength(6, ErrorMessage ="Mật khẩu tối thiểu gồm 6 ký tự")]
        public string Password { get; set; }
    }
}

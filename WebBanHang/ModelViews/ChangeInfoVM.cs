using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebBanHang.ModelViews
{
    public class ChangeInfoVM
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string PasswordNow { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }


        [MinLength(6, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 6 ký tự")]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [MaxLength(250)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }

        [MaxLength(10)]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
    }
}

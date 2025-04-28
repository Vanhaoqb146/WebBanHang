using Microsoft.AspNetCore.Http;
using WebBanHang.Models.Payments;

namespace WebBanHang.Services
{
    public interface IVnPayService
    {
        //tạo ra URL thanh toán tại VnPay
        //CreatePaymentUrl nhận vào một object có tên PaymentInformationModel
        //model này này sẽ chứa các thông tin của hóa đơn thanh toán
        //Và một HttpContext để lấy địa chỉ IP Address của client thanh toán đơn hàng đó
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);

        //kiểm tra thông tin giao dịch và lưu lại thông tin đó khi thanh toán thành công
        //PaymentExecute nhận vào 1 IQueryCollection đây là thông tin trên URL
        //mà VnPay trả về trong các parameter sau khi thanh toán thành công hoặc lỗi.
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}

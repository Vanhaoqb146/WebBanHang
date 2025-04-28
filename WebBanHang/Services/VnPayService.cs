using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using WebBanHang.Libraries;
using WebBanHang.Models.Payments;

namespace WebBanHang.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //tạo ra URL để Redirect tới trang thanh toán của VnPay và xác thực giao dịch.
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            //số tiền cần nhân với 100 để khử phần thập phân sau khi thanh toán.
            //Số tiền thanh toán = Số tiền thanh toán x 100
            pay.AddRequestData("vnp_Amount", (model.tongDonHangInput * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.FullName}-{model.Phone}-{model.Address}" +
                $"-{model.TinhThanh}-{model.QuanHuyen}-{model.PhuongXa}" +
                $"-{model.soTienGiamInput}-{model.phiGiaoHangInput}-{model.tongDonHangInput}");
            pay.AddRequestData("vnp_OrderType", $"Type");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        //lấy ra các thông tin sau khi giao dịch tại VnPay
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }
    }
}

namespace WebBanHang.Models.Payments
{
    public class PaymentInformationModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string TinhThanh { get; set; }
        public string QuanHuyen { get; set; }
        public string PhuongXa { get; set; }

        //
        public int soTienGiamInput { get; set; }
        public int phiGiaoHangInput { get; set; }
        public int tongDonHangInput { get; set; }

    }
}

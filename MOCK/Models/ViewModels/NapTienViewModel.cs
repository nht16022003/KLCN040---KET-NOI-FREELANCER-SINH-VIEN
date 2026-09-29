using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class NapTienViewModel
    {
        public Wallet Wallet { get; set; } = new();
        public User User { get; set; } = new();

        // Dữ liệu tạo nạp tiền
        public decimal SoTien { get; set; } = 500000;
        public string PhuongThuc { get; set; } = "VietQR"; // VietQR, VNPAY, MoMo, ChuyenKhoan
        public string NoiDungChuyenKhoan { get; set; } = "";
        public string MaGiaoDichThamChieu { get; set; } = "";

        // Thông tin ngân hàng nhận hệ thống
        public string TenNganHang { get; set; } = "MBBank (Ngân hàng TMCP Quân Đội)";
        public string SoTaiKhoan { get; set; } = "999988886666";
        public string ChuTaiKhoan { get; set; } = "CTCP FREELANCERSTUDENT VIETNAM";
        public string QrCodeImageUrl { get; set; } = "";

        // Lịch sử các lần nạp gần đây
        public List<LichSuGiaoDich> RecentTopups { get; set; } = new();
    }
}

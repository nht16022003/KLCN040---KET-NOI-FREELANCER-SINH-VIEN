using System;

namespace MOCK.Models.Entities
{
    public class YeuCauNapTien
    {
        public int MaYeuCau { get; set; }
        public int MaWallet { get; set; }
        public decimal SoTien { get; set; }
        public string PhuongThuc { get; set; } = "ChuyenKhoanVietQR"; // ChuyenKhoanVietQR, VNPAY, MoMo
        public string? MaGiaoDichNganHang { get; set; } // Mã GD tại app ngân hàng
        public string? AnhBienLai { get; set; } // Đường dẫn ảnh chụp màn hình chuyển khoản
        public string? GhiChu { get; set; } // Lời nhắn hoặc lý do gửi hỗ trợ
        public string TrangThai { get; set; } = "ChoDuyet"; // ChoDuyet, DaDuyet, TuChoi
        public string? LyDoTuChoi { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayDuyet { get; set; }
    }
}

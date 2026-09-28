using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class HoTroNapTienViewModel
    {
        public Wallet Wallet { get; set; } = new();
        public User User { get; set; } = new();

        // Form gửi hỗ trợ
        public decimal SoTien { get; set; }
        public string PhuongThuc { get; set; } = "ChuyenKhoanVietQR";
        public string? MaGiaoDichNganHang { get; set; }
        public string? GhiChu { get; set; }
        public string? AnhBienLai { get; set; }

        // Danh sách các yêu cầu hỗ trợ đã gửi trước đây
        public List<YeuCauNapTien> DanhSachYeuCau { get; set; } = new();
    }
}

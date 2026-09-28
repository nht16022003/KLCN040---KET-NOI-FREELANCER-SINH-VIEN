using System;

namespace MOCK.Models.Entities
{
    public class YeuCauRutTien
    {
        public int MaRutTien { get; set; }
        public int MaWallet { get; set; }
        public decimal SoTienRut { get; set; }
        public string TenNganHang { get; set; } = string.Empty;
        public string SoTaiKhoan { get; set; } = string.Empty;
        public string TenChuTaiKhoan { get; set; } = string.Empty;
        public DateTime NgayYeuCau { get; set; } = DateTime.Now;
        public DateTime? NgayXuLy { get; set; }
        public string TrangThai { get; set; } = "ChoDuyet"; // ChoDuyet, DaChuyenKhoan, TuChoi
        public string? LyDoTuChoi { get; set; }
        public int? MaAdminXuLy { get; set; }

        // Navigation property
        public Wallet? Wallet { get; set; }
    }
}

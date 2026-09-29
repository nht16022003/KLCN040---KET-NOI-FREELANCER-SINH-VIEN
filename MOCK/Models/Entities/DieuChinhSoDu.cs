using System;

namespace MOCK.Models.Entities
{
    public class DieuChinhSoDu
    {
        public int MaDieuChinh { get; set; }
        public int MaWallet { get; set; }
        public int MaAdmin { get; set; }
        public string LoaiDieuChinh { get; set; } = "CongTien"; // CongTien, TruTien
        public decimal SoTienDieuChinh { get; set; }
        public decimal SoDuTruoc { get; set; }
        public decimal SoDuSau { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public DateTime NgayDieuChinh { get; set; } = DateTime.Now;

        // Navigation properties
        public Wallet? Wallet { get; set; }
        public Admin? Admin { get; set; }
    }
}

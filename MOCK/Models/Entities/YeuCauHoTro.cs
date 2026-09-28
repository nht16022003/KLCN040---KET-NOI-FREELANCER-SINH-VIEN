using System;

namespace MOCK.Models.Entities
{
    public class YeuCauHoTro
    {
        public int MaYeuCau { get; set; }
        public int MaUser { get; set; }
        public string? LoaiYeuCau { get; set; } // HoTroNapTien, BaoCaoViPham, LoiHeThong, KhieuNai
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string? FileDinhKem { get; set; }
        public string TrangThai { get; set; } = "DangXuLy"; // DangXuLy, DaXuLy, DaDong
        public string? PhanHoiAdmin { get; set; }
        public int? MaAdmin { get; set; }
        public DateTime NgayGui { get; set; } = DateTime.Now;
        public DateTime? NgayXuLy { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Admin? Admin { get; set; }
    }
}

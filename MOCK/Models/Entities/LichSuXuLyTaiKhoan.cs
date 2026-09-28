using System;

namespace MOCK.Models.Entities
{
    public class LichSuXuLyTaiKhoan
    {
        public int MaLichSu { get; set; }
        public int MaUser { get; set; }
        public int MaAdmin { get; set; }
        public string? HanhDong { get; set; } // KHOA_TAI_KHOAN, MO_KHOA_TAI_KHOAN, CANH_BAO
        public string LyDo { get; set; } = string.Empty;
        public string? TrangThaiTruocKhiXuLy { get; set; }
        public string? TrangThaiSauKhiXuLy { get; set; }
        public DateTime NgayXuLy { get; set; } = DateTime.Now;

        // Navigation properties
        public User? User { get; set; }
        public Admin? Admin { get; set; }
    }
}

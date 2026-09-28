using System;

namespace MOCK.Models.Entities
{
    public class BanGiao_SanPham
    {
        public int MaBanGiao { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string PhienBan { get; set; } = "v1.0";
        public string? FileSanPhamUrl { get; set; }
        public string? LinkDemo { get; set; }
        public string? MotaBanGiao { get; set; }
        public DateTime NgayNop { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "ChoNghiemThu"; // ChoNghiemThu, DaDuyet, YeuCauChinhSua
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }

        // Navigation
        public HopDong? HopDong { get; set; }
    }
}

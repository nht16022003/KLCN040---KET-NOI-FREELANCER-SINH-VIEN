using System;

namespace MOCK.Models.Entities
{
    public class ThongBao
    {
        public int MaThongBao { get; set; }
        public int MaUser { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiThongBao { get; set; } = "HeThong"; // TuyenDung, HopDong, UngTuyen, UngThue, ThanhToan, HeThong
        public string? LinkDieuHuong { get; set; }
        public bool DaDoc { get; set; } = false;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property
        public User? User { get; set; }
    }
}

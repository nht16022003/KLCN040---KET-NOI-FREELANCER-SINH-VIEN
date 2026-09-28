using System;

namespace MOCK.Models.Entities
{
    public class JobPost
    {
        public string MaJob { get; set; } = string.Empty;
        public int MaNhaTuyenDung { get; set; }
        public string Tieude { get; set; } = string.Empty;
        public string? Mota { get; set; }
        public string? Kynangyeucau { get; set; }
        public double? Thulao { get; set; }
        public DateTime Thoigiandangtuyen { get; set; } = DateTime.Now;
        public string Thoigiandukienhoanthanh { get; set; } = string.Empty;
        public string Status { get; set; } = "DangTuyen"; // DangTuyen, DangThucHien, DaHoanThanh, DaHuy
        public int? Soluongtuyen { get; set; }

        // Navigation properties
        public NhaTuyenDung? NhaTuyenDung { get; set; }
        public HopDong? HopDong { get; set; }
    }
}

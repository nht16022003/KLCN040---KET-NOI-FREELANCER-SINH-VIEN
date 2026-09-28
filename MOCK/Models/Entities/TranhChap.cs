using System;
using System.Collections.Generic;

namespace MOCK.Models.Entities
{
    public class TranhChap
    {
        public int MaTranhChap { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public int NguoiTao { get; set; } // MaUser
        public string LyDo { get; set; } = string.Empty;
        public string MoTaChiTiet { get; set; } = string.Empty;
        public decimal SoTienTranhChap { get; set; }
        public string TrangThai { get; set; } = "DangMo"; // DangMo, DangXuLy, DaGiaiQuyet, DaHuy
        public string? KetLuanAdmin { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayGiaiQuyet { get; set; }

        // Navigation
        public HopDong? HopDong { get; set; }
        public User? Creator { get; set; }
        public List<BangChungTranhChap> BangChungs { get; set; } = new();
    }
}

using System;

namespace MOCK.Models.Entities
{
    public class BangChungTranhChap
    {
        public int MaBangChung { get; set; }
        public int MaTranhChap { get; set; }
        public int NguoiTaiLen { get; set; } // MaUser
        public string LoaiBangChung { get; set; } = "AnhChup"; // AnhChup, TaiLieu, DoanChat, HopDong
        public string DuongDanFile { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime NgayTaiLen { get; set; } = DateTime.Now;

        // Navigation
        public User? Uploader { get; set; }
    }
}

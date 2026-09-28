using System;

namespace MOCK.Models.Entities
{
    public class TinNhan
    {
        public int MaTinNhan { get; set; }
        public string MaPhongChat { get; set; } = string.Empty;
        public int NguoiGui { get; set; } // MaUser
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiTinNhan { get; set; } = "VanBan"; // VanBan, HinhAnh, TepDinhKem, ThePortfolio, DeXuatHopDong
        public string? FileDinhKemUrl { get; set; }
        public string? TenFile { get; set; }
        public string? MaDuAnPortfolio { get; set; } // Nếu đính kèm dự án portfolio
        public bool DaDoc { get; set; } = false;
        public DateTime NgayGui { get; set; } = DateTime.Now;

        // Navigation
        public User? Sender { get; set; }
    }
}

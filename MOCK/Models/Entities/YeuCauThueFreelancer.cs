using System;

namespace MOCK.Models.Entities
{
    public class YeuCauThueFreelancer
    {
        public int MaYeuCau { get; set; }
        public int MaNhaTuyenDung { get; set; }
        public int MaFreelancerStudent { get; set; }
        public string TieuDeCongViec { get; set; } = string.Empty;
        public string? MoTaCongViec { get; set; }
        public double? NganSachDeNghi { get; set; }
        public string? ThoiHanDuKien { get; set; }
        public DateTime NgayGui { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "ChoPhanHoi"; // ChoPhanHoi, DongY, TuChoi, DaLapHopDong
        public string? LyDoTuChoi { get; set; }

        // Navigation properties
        public NhaTuyenDung? NhaTuyenDung { get; set; }
        public FreelancerStudent? FreelancerStudent { get; set; }
    }
}

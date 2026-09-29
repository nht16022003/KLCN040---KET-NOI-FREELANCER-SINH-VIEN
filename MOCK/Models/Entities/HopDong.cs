using System;

namespace MOCK.Models.Entities
{
    public class HopDong
    {
        public string MaHD { get; set; } = string.Empty;
        public string MaJob { get; set; } = string.Empty;
        public int MaFreelancerStudent { get; set; }
        public DateTime? NgayBatDau { get; set; } = DateTime.Now;
        public DateTime? NgayKetThuc { get; set; }
        public double? Sotienkyquy { get; set; }
        public string? Hinhthuclamviec { get; set; } // Remote, Onsite, Hybrid
        public string TrangThai { get; set; } = "DangThucHien"; // DangThucHien, DaHoanThanh, DaHuy

        // Navigation properties
        public JobPost? JobPost { get; set; }
        public FreelancerStudent? FreelancerStudent { get; set; }
    }
}

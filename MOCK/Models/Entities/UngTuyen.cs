using System;

namespace MOCK.Models.Entities
{
    public class UngTuyen
    {
        public int MaUngTuyen { get; set; }
        public string MaJob { get; set; } = string.Empty;
        public int MaFreelancerStudent { get; set; }
        public string? ThuGioiThieu { get; set; }
        public double? ThulaoDeXuat { get; set; }
        public string? ThoiGianHoanThanhDeXuat { get; set; }
        public string? FileCV { get; set; }
        public DateTime NgayUngTuyen { get; set; } = DateTime.Now;
        public string TrangThaiUngTuyen { get; set; } = "ChoDuyet"; // ChoDuyet, ChapNhan, TuChoi, DaLapHopDong

        // Navigation properties
        public JobPost? JobPost { get; set; }
        public FreelancerStudent? FreelancerStudent { get; set; }
    }
}

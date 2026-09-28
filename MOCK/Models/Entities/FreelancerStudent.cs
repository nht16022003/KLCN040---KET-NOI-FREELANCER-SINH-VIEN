using System.Collections.Generic;

namespace MOCK.Models.Entities
{
    public class FreelancerStudent
    {
        public int MaFreelancerStudents { get; set; }
        public string MaChuyenNganh { get; set; } = string.Empty;
        public int MaUser { get; set; }
        public string MaTruong { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public string DiaDiemTruong { get; set; } = string.Empty;
        public string DiaDiemFreelancerStudent { get; set; } = string.Empty;
        public string? NgonNgu { get; set; }
        public string? KyNangCoBan { get; set; }
        public int NamThu { get; set; }
        public double GPA { get; set; }
        public string NienKhoa { get; set; } = string.Empty;
        public string? Gioithieu { get; set; }
        public string? Avatar { get; set; }
        public bool TrangthaiNhanViec { get; set; } = true;
        public decimal? ChiPhiTu { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ChuyenNganh? ChuyenNganh { get; set; }
        public List<KynangChuyennganhFreelancerStudent> KyNangChuyenNganhs { get; set; } = new();
        public Portfolio? Portfolio { get; set; }
    }
}

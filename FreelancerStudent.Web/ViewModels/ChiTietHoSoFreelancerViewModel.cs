namespace FreelancerStudent.Web.ViewModels
{
    public class ChiTietHoSoFreelancerViewModel
    {
        public int MaFreelancerStudents { get; set; }
        public int MaUser { get; set; }
        public string TenUser { get; set; } = string.Empty;
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }

        public string MaChuyenNganh { get; set; } = string.Empty;
        public string TenChuyenNganh { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public string DiaDiemFreelancerStudent { get; set; } = string.Empty;
        public int NamThu { get; set; }
        public double GPA { get; set; }
        public string NienKhoa { get; set; } = string.Empty;

        public string? Gioithieu { get; set; }
        public string? KyNangCoBan { get; set; }
        public string? NgonNgu { get; set; }
        public bool TrangthaiNhanViec { get; set; }
        public decimal? ChiPhiTu { get; set; }

        public List<string> DanhSachKyNang { get; set; } = new List<string>();

        public string NewSkillsInput { get; set; } = string.Empty;

    }
}

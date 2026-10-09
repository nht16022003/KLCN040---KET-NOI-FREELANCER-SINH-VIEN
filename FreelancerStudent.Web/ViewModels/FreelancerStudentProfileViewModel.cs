namespace FreelancerStudent.Web.ViewModels
{
    public class FreelancerStudentProfileViewModel
    {
        public ProfileUserViewModel User { get; set; } = new();
        public ProfileFreelancerViewModel FreelancerStudent { get; set; } = new();
        public ProfileMajorViewModel? ChuyenNganh { get; set; }
        public ProfilePortfolioViewModel? Portfolio { get; set; }
        public List<ProfileSkillViewModel> KyNangs { get; set; } = new();
        public List<ProfileProjectViewModel> DuAns { get; set; } = new();
        public ProfileEvidenceViewModel? MinhChung { get; set; }

        public string tenUser => User.HotenUser;
        public string MaChuyenNganh => FreelancerStudent.MaChuyenNganh;
    }

    public class ProfileUserViewModel
    {
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string HotenUser { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    public class ProfileFreelancerViewModel
    {
        public string MaChuyenNganh { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public string MaTruong { get; set; } = string.Empty;
        public string DiaDiemFreelancerStudent { get; set; } = string.Empty;
        public string? NgonNgu { get; set; }
        public string? KyNangCoBan { get; set; }
        public int NamThu { get; set; }
        public double GPA { get; set; }
        public string NienKhoa { get; set; } = string.Empty;
        public string? Gioithieu { get; set; }
        public bool TrangthaiNhanViec { get; set; }
        public decimal? ChiPhiTu { get; set; }
    }

    public class ProfileMajorViewModel
    {
        public string TenChuyenNganh { get; set; } = string.Empty;
    }

    public class ProfilePortfolioViewModel
    {
        public string? MoTaBanThan { get; set; }
        public string? Url_video { get; set; }
    }

    public class ProfileSkillViewModel
    {
        public string TenKyNang { get; set; } = string.Empty;
    }

    public class ProfileProjectViewModel
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string Congnghe { get; set; } = string.Empty;
        public string? LinkGithub { get; set; }
        public string? LinkDemo { get; set; }
    }

    public class ProfileEvidenceViewModel
    {
        public string LoaiMinhChung { get; set; } = string.Empty;
        public string TrangThaiGuiMinhChung { get; set; } = string.Empty;
    }
}
namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class FreelancerStudentProfile_ReponseDTO
    {
        public ProfileUserResponse User { get; set; } = new();
        public ProfileFreelancerResponse FreelancerStudent { get; set; } = new();
        public ProfileMajorResponse? ChuyenNganh { get; set; }
        public ProfilePortfolioResponse? Portfolio { get; set; }
        public List<ProfileSkillResponse> KyNangs { get; set; } = new();
        public List<ProfileProjectResponse> DuAns { get; set; } = new();
        public ProfileEvidenceResponse? MinhChung { get; set; }
    }

    public class ProfileUserResponse
    {
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string HotenUser { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class ProfileFreelancerResponse
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

    public class ProfileMajorResponse
    {
        public string TenChuyenNganh { get; set; } = string.Empty;
    }

    public class ProfilePortfolioResponse
    {
        public string? MoTaBanThan { get; set; }
        public string? Url_video { get; set; }
    }

    public class ProfileSkillResponse
    {
        public string TenKyNang { get; set; } = string.Empty;
    }

    public class ProfileProjectResponse
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string Congnghe { get; set; } = string.Empty;
        public string? LinkGithub { get; set; }
        public string? LinkDemo { get; set; }
    }

    public class ProfileEvidenceResponse
    {
        public string LoaiMinhChung { get; set; } = string.Empty;
        public string TrangThaiGuiMinhChung { get; set; } = string.Empty;
    }
}
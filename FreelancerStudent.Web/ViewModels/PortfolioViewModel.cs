namespace FreelancerStudent.Web.ViewModels
{
    public class PortfolioViewModel
    {
        public string maPortfolio { get; set; } = string.Empty;
        public int maFreelancerStudents { get; set; }
        public string? moTaBanThan { get; set; }
        public string? url_video { get; set; }
        public List<DuAnTrongPortfolioViewModel> projects { get; set; } = new();

        public string NewTenDuAn { get; set; } = string.Empty;
        public string? NewVaiTro { get; set; }
        public string? NewMoTa { get; set; }
        public string? NewCongNghe { get; set; }
        public string? NewLinkGithub { get; set; }
        public string? NewLinkDemo { get; set; }
        public string? NewLinkFile { get; set; }
        public string? EditMaDA { get; set; }
    }

    public class DuAnTrongPortfolioViewModel
    {
        public string maDA { get; set; } = string.Empty;
        public string tenDuAn { get; set; } = string.Empty;
        public string? vaiTro { get; set; }
        public string? moTa { get; set; }
        public string? congnghe { get; set; }
        public string? linkGithub { get; set; }
        public string? linkDemo { get; set; }
        public string? link_file { get; set; }
        public bool laDuAnNoiBat { get; set; }
    }
}
namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class Portfolio_ReponseDTO
    {
        public string maPortfolio { get; set; } = string.Empty;
        public int maFreelancerStudents { get; set; }
        public string? moTaBanThan { get; set; }
        public string? url_video { get; set; }
        public List<DuAnTrongPortfolio_ReponseDTO> projects { get; set; } = new();
    }

    public class DuAnTrongPortfolio_ReponseDTO
    {
        public string maDA { get; set; } = string.Empty;
        public string maPortfolio { get; set; } = string.Empty;
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
namespace MOCK.Models.Entities
{
    public class DuAnTrongPortfolio
    {
        public string MaDA { get; set; } = string.Empty;
        public string MaPortfolio { get; set; } = string.Empty;
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? VaiTro { get; set; }
        public string? Congnghe { get; set; }
        public string? LinkGithub { get; set; }
        public string? LinkDemo { get; set; }
        public string? Link_file { get; set; }

        // Navigation property
        public Portfolio? Portfolio { get; set; }
    }
}

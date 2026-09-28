namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class DangNhap_ReponseDTO
    {
        public int maUser { get; set; }
        public string hovaten { get; set; } = string.Empty;

        public string tentaikhoan { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string? sodienthoai { get; set; }

        public int marole { get; set; }

        public string tenrole { get; set; } = string.Empty;

        public string status { get; set; } = string.Empty;
    }
}
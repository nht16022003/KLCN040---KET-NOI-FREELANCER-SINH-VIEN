namespace FreelancerStudent.Web.ViewModels
{
    public class ThongTinTaiKhoanViewModel
    {
        public int maUser { get; set; }

        public string hovaten { get; set; } = string.Empty;

        public string tentaikhoan { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string? sodienthoai { get; set; }

        public string? avatarUrl { get; set; }

        public string status { get; set; } = string.Empty;

        public DateTime ngaytao { get; set; }

        public string tenrole { get; set; } = string.Empty;
    }
}
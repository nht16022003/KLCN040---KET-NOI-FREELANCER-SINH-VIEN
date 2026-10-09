//Dữ liệu trong đây sẽ nhận trả về từ API, tức là reponse
namespace FreelancerStudent.Web.ViewModels.Account
{
    public class UserSessionViewModel
    {
        public int maUser { get; set; }
        public string hovaten { get; set; } = string.Empty;

        public string tentaikhoan { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string? sodienthoai { get; set; }

        public int marole { get; set; }

        public string tenrole { get; set; } = string.Empty;

        public string status { get; set; } = string.Empty;
        public string? avatarUrl { get; set; }
    }
}
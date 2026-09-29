using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class CapNhatThongTinTaiKhoanViewModel
    {
        public int maUser { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string hovaten { get; set; } = string.Empty;

        public string? sodienthoai { get; set; }
    }
}
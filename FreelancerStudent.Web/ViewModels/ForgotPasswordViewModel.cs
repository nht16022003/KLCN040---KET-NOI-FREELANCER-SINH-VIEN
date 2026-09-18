using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email để khôi phục mật khẩu.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;
    }
}

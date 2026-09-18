using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class VerifyAccountViewModel
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "nguyen.vana123@student.edu.vn";

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ mã OTP 6 chữ số.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 chữ số.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Mã OTP chỉ bao gồm các chữ số.")]
        [Display(Name = "Mã xác thực OTP")]
        public string OtpCode { get; set; } = string.Empty;

        public string? VerifyType { get; set; } = "Register"; // 'Register' hoặc 'ResetPassword'
    }
}

using System.ComponentModel.DataAnnotations;

namespace MOCK.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email hoặc Tên tài khoản")]
        [Display(Name = "Email hoặc Tên tài khoản")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Display(Name = "Mã xác thực OTP (6 chữ số)")]
        public string? OtpCode { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có tối thiểu 6 ký tự")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        public string? ConfirmPassword { get; set; }

        // Bước thực hiện: 1 = Nhập email/tài khoản, 2 = Nhập mã OTP & Đặt mật khẩu mới, 3 = Thành công
        public int Step { get; set; } = 1;

        public string? UserRoleName { get; set; }
    }
}

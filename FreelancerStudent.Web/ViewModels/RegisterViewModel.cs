using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn loại tài khoản.")]
        [Display(Name = "Loại tài khoản")]
        public string LoaiUser { get; set; } = "FreelancerSV"; // 'FreelancerSV' hoặc 'KhachHang'

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn cần đồng ý với Điều khoản và Chính sách bảo mật.")]
        [Display(Name = "Tôi đồng ý với Điều khoản sử dụng và Chính sách bảo mật")]
        public bool AgreeTerms { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập hoặc Email là bắt buộc.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LoaiUser { get; set; } // KhachHang, FreelancerSV, Admin
        public string? Status { get; set; }
        public string? CusID { get; set; }
        public string? FreeID { get; set; }
        public string? AdminRole { get; set; }
        public string? Token { get; set; }
    }
}

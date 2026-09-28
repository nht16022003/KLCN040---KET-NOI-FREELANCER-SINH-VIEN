using Microsoft.AspNetCore.Http;
using MOCK.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MOCK.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; } = new();

        // Các thông tin hiển thị cơ bản
        public int MaUser { get; set; }
        public string HotenUser { get; set; } = string.Empty;
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public DateTime NgayTao { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string RoleTitle { get; set; } = "Thành viên";
        public string TrangThaiXacThuc { get; set; } = "Chưa xác thực";
        public string AvatarUrl { get; set; } = "uploads/avatar/default_student.png";

        // Trạng thái giao diện
        public bool IsEditMode { get; set; } = false;
        public string ActiveTab { get; set; } = "profile"; // "profile" hoặc "security"

        // Các trường phục vụ chỉnh sửa (Edit Mode)
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100, ErrorMessage = "Họ và tên không vượt quá 100 ký tự.")]
        public string EditHotenUser { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không đúng định dạng quy chuẩn (VD: 0901234567).")]
        public string EditSdtUser { get; set; } = string.Empty;

        // Upload avatar
        public IFormFile? AvatarUpload { get; set; }

        // Đối tượng đổi mật khẩu nhúng trong trang Profile (tab Bảo mật)
        public ChangePasswordViewModel ChangePasswordModel { get; set; } = new();

        // Thông báo phản hồi
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

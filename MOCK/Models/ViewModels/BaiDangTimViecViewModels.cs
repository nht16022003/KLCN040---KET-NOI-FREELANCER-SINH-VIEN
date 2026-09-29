using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    // ViewModel cho View Đăng tin tìm việc / Sửa tin tìm việc
    public class DangTinTimViecViewModel
    {
        public string? MaBaiDang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài đăng tìm việc")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        [Display(Name = "Tiêu đề bài đăng")]
        public string Tieude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết năng lực và dịch vụ bạn cung cấp")]
        [Display(Name = "Mô tả chi tiết năng lực & công việc")]
        public string Mota { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập kỹ năng chuyên môn")]
        [Display(Name = "Kỹ năng chuyên môn (cách nhau bằng dấu phẩy)")]
        public string Kynang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mức thù lao mong muốn tối thiểu")]
        [Range(50000, 100000000, ErrorMessage = "Mức giá từ 50,000 VNĐ đến 100,000,000 VNĐ")]
        [Display(Name = "Mức giá từ (VNĐ)")]
        public double MucGiaTu { get; set; } = 500000;

        [Display(Name = "Trạng thái hiển thị")]
        public string Trangthai { get; set; } = "DangHienThi"; // DangHienThi, DaAn, DaNhanViec

        // Thông tin sinh viên đăng bài
        public FreelancerStudent? StudentInfo { get; set; }
        public User? UserInfo { get; set; }
        public bool IsEditMode => !string.IsNullOrEmpty(MaBaiDang);
    }

    // ViewModel cho View Quản lý tìm việc (Danh sách bài đăng tìm việc của Freelancer)
    public class QuanLyTimViecViewModel
    {
        public List<BaiDangTimViecFreelancerStudent> DanhSachBaiDang { get; set; } = new();
        public FreelancerStudent? StudentInfo { get; set; }
        public User? UserInfo { get; set; }

        public string? FilterStatus { get; set; }
        public string? Keyword { get; set; }

        public int TongSoBaiDang => DanhSachBaiDang.Count;
        public int SoDangHienThi { get; set; }
        public int SoDaAn { get; set; }
        public int SoDaNhanViec { get; set; }
    }
}

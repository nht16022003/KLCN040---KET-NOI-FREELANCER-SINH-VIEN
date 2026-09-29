using System;
using System.ComponentModel.DataAnnotations;

namespace MOCK.Models.ViewModels
{
    public class SendDirectOfferViewModel
    {
        [Required]
        public int MaFreelancerStudent { get; set; }

        public int MaNhaTuyenDung { get; set; } = 1;

        // Thông tin hiển thị của Freelancer được mời
        public string TenFreelancer { get; set; } = string.Empty;
        public string? AvatarFreelancer { get; set; }
        public string TenTruong { get; set; } = string.Empty;
        public string TenChuyenNganh { get; set; } = string.Empty;
        public double? ChiPhiThamKhao { get; set; }

        // Các trường theo bảng UngThue trong CSDL
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc / dự án")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề công việc")]
        public string TieuDeCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết yêu cầu công việc")]
        [Display(Name = "Mô tả chi tiết công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngân sách đề nghị")]
        [Range(50000, 500000000, ErrorMessage = "Ngân sách tối thiểu từ 50,000 VNĐ")]
        [Display(Name = "Ngân sách đề nghị (VNĐ)")]
        public double? NganSachDeNghi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập hoặc chọn thời hạn dự kiến")]
        [Display(Name = "Thời hạn dự kiến hoàn thành")]
        public string ThoiHanDuKien { get; set; } = "7 ngày kể từ khi bắt đầu";

        [Display(Name = "Ghi chú thêm hoặc điều khoản đặc biệt")]
        public string? GhiChuThem { get; set; }
    }
}

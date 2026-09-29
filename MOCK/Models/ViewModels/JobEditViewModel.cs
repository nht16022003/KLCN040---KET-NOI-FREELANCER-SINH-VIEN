using System;
using System.ComponentModel.DataAnnotations;

namespace MOCK.Models.ViewModels
{
    public class JobEditViewModel
    {
        [Required]
        public string MaJob { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài đăng.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        [Display(Name = "Tiêu đề bài đăng")]
        public string Tieude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết công việc.")]
        [Display(Name = "Mô tả công việc")]
        public string Mota { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập các kỹ năng yêu cầu.")]
        [Display(Name = "Kỹ năng yêu cầu")]
        public string Kynangyeucau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mức thù lao dự kiến.")]
        [Range(100000, 500000000, ErrorMessage = "Thù lao phải từ 100,000 VNĐ trở lên.")]
        [Display(Name = "Mức thù lao ngân sách (VNĐ)")]
        public double? Thulao { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến hoàn thành.")]
        [Display(Name = "Thời gian hoàn thành dự kiến")]
        public string Thoigiandukienhoanthanh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tuyển.")]
        [Range(1, 20, ErrorMessage = "Số lượng tuyển từ 1 đến 20 bạn.")]
        [Display(Name = "Số lượng tuyển")]
        public int Soluongtuyen { get; set; } = 1;

        [Display(Name = "Trạng thái bài đăng")]
        public string Status { get; set; } = "DangTuyen"; // DangTuyen, DangThucHien, DaHoanThanh, DaHuy

        [Display(Name = "Tệp tài liệu đính kèm (nếu có)")]
        public string? FileDinhKem { get; set; }

        public double PhiDangBai { get; set; } = 0;
        public DateTime Thoigiandangtuyen { get; set; } = DateTime.Now;
        public int MaNhaTuyenDung { get; set; } = 1;
    }
}

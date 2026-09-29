using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class JobPostViewModel
    {
        public string maJob { get; set; } = string.Empty;

        public int maNhaTuyenDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài đăng")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        [Display(Name = "Tiêu đề bài đăng")]
        public string tieude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả công việc")]
        [Display(Name = "Mô tả chi tiết yêu cầu công việc")]
        public string mota { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Kỹ năng yêu cầu không quá 100 ký tự")]
        [Display(Name = "Kỹ năng yêu cầu")]
        public string? kynangyeucau { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mức thù lao")]
        [Range(50000, 100000000,
            ErrorMessage = "Thù lao phải từ 50.000 đến 100.000.000 VNĐ")]
        [Display(Name = "Mức thù lao ngân sách")]
        public decimal? thulao { get; set; }

        public string? fileDinhKem { get; set; }

        public decimal phiDangBai { get; set; }

        public DateTime thoigiandangtuyen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hạn hoàn thành")]
        [Display(Name = "Hạn hoàn thành dự kiến")]
        public DateTime thoigiandukienhoanthanh { get; set; }

        public string? status { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sinh viên cần tuyển")]
        [Range(1, 50, ErrorMessage = "Số lượng tuyển từ 1 đến 50 sinh viên")]
        [Display(Name = "Số lượng sinh viên cần tuyển")]
        public int? soluongtuyen { get; set; }
        public int maUser { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class DangTinTimViecViewModel
    {
        public int maUser { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài đăng tìm việc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề bài đăng")]
        public string tieude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết về năng lực và dịch vụ bạn cung cấp")]
        [Display(Name = "Mô tả chi tiết năng lực & dịch vụ")]
        public string mota { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập kỹ năng chuyên môn của bạn")]
        [Display(Name = "Kỹ năng chuyên môn (cách nhau bởi dấu phẩy)")]
        public string kynang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mức thù lao mong muốn tối thiểu")]
        [Range(10000, 100000000, ErrorMessage = "Mức thù lao từ 10.000 VNĐ đến 100.000.000 VNĐ")]
        [Display(Name = "Mức thù lao mong muốn từ (VNĐ)")]
        public double mucGiaTu { get; set; } = 500000;
    }
}

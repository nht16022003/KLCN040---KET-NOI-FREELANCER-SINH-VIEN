using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class JobPost_RequestDTO
    {


        [Required(ErrorMessage = "Mã nhà tuyển dụng không được để trống")]
        //Lấy từ Session tài khoản đang đăng nhập
        public int maNhaTuyenDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        public string tieude { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết công việc")]
        public string? mota { get; set; }

        [MaxLength(255, ErrorMessage = "Kỹ năng yêu cầu không quá 255 ký tự")]
        public string? kynangyeucau { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mức thù lao")]
        [Range(50000, 100000000, ErrorMessage = "Thù lao phải từ 50,000 VNĐ đến 100,000,000 VNĐ")]
        public decimal? thulao { get; set; }


        [MaxLength(500)]
        public string? fileDinhKem { get; set; } = string.Empty;

        public decimal phiDangBai { get; set; }


        public DateTime thoigiandangtuyen { get; set; } = DateTime.UtcNow;


        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến hoàn thành")]
        public DateTime thoigiandukienhoanthanh { get; set; }


        public string? status { get; set; } = "DangTuyen";
        [Required(ErrorMessage = "Vui lòng nhập số lượng cần tuyển")]
        [Range(1, 50, ErrorMessage = "Số lượng tuyển phải từ 1 đến 50 người")]
        public int? soluongtuyen { get; set; }


    }
}
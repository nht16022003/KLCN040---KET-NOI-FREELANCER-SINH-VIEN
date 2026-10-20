using System;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class CapNhatTaskCongViec_RequestDTO
    {
        [Required]
        public int maTask { get; set; }

        public int? maGiaiDoan { get; set; }

        [Required(ErrorMessage = "Tiêu đề đầu việc không được để trống.")]
        [MaxLength(500, ErrorMessage = "Tiêu đề không được vượt quá 500 ký tự.")]
        public string tenTask { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string moTa { get; set; } = string.Empty;

        public string? linkSanPham { get; set; }

        public DateTime? hanHoanThanh { get; set; }

        [Required(ErrorMessage = "Trạng thái công việc không được để trống.")]
        public string trangThai { get; set; } = "ChuaThucHien";
    }
}

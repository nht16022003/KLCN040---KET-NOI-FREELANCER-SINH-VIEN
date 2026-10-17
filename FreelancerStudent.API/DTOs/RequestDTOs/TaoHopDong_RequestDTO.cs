using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class TaoGiaiDoanHopDong_DTO
    {
        [Required(ErrorMessage = "Tên giai đoạn không được để trống")]
        public string tenGiaiDoan { get; set; } = string.Empty;

        public string? moTa { get; set; }

        public DateTime hanChot { get; set; }

        public decimal? tyLeGiaiNgan { get; set; }

        public decimal? soTien { get; set; }
    }

    public class TaoHopDong_RequestDTO
    {
        [Required(ErrorMessage = "Mã công việc không được để trống")]
        public string maJob { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã Freelancer không được để trống")]
        public int maFreelancerStudent { get; set; }

        [Required(ErrorMessage = "Số tiền thù lao thỏa thuận không được để trống")]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền thù lao phải lớn hơn 0")]
        public decimal sotienkyquy { get; set; }

        public DateTime? ngayBatDau { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Hạn chót hoàn thành không được để trống")]
        public DateTime ngayKetThuc { get; set; }

        public string? hinhthuclamviec { get; set; } = "Remote";

        public string? dieuKhoan { get; set; }

        public int? maUngTuyen { get; set; }

        public List<TaoGiaiDoanHopDong_DTO>? giaiDoans { get; set; } = new List<TaoGiaiDoanHopDong_DTO>();
    }
}

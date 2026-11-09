using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class GuiYeuCauThue_RequestDTO
    {
        [Required(ErrorMessage = "Mã người dùng Nhà tuyển dụng không được để trống")]
        public int maUserNTD { get; set; }

        [Required(ErrorMessage = "Mã Freelancer Student không được để trống")]
        public int maFreelancerStudent { get; set; }

        public string? maJob { get; set; }

        [Required(ErrorMessage = "Tiêu đề công việc không được để trống")]
        [MaxLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        public string tieuDeCongViec { get; set; } = string.Empty;

        public string? moTaCongViec { get; set; }

        public decimal? nganSachDeNghi { get; set; }

        public string? thoiHanDuKien { get; set; }
    }
}


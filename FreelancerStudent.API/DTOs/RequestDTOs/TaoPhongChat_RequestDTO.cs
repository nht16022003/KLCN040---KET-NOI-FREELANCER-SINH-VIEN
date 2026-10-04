using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class TaoPhongChat_RequestDTO
    {
        [Required(ErrorMessage = "Mã tài khoản nhà tuyển dụng không được để trống")]
        public int maUserClient { get; set; }

        [Required(ErrorMessage = "Mã Freelancer Student không được để trống")]
        public int maFreelancerStudent { get; set; }

        public string? maJob { get; set; } // Mã công việc liên quan (nếu có)
    }
}

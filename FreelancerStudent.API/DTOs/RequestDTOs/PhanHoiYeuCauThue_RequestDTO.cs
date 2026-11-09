using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class PhanHoiYeuCauThue_RequestDTO
    {
        [Required(ErrorMessage = "Mã yêu cầu không được để trống")]
        public int maYeuCau { get; set; }

        [Required(ErrorMessage = "Mã người dùng Freelancer không được để trống")]
        public int maUserFreelancer { get; set; }

        public bool chapNhan { get; set; }

        public string? lyDoTuChoi { get; set; }
    }
}


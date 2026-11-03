using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTO
{
    public class LuuFreelancer_RequestDTO
    {
        [Required]
        public int maUser { get; set; }

        [Required]
        public int maFreelancerStudent { get; set; }

        [MaxLength(255)]
        public string? ghiChu { get; set; }
    }
}


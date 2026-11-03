using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTO
{
    public class LuuViec_RequestDTO
    {
        [Required]
        public int maUser { get; set; }

        [Required]
        [MaxLength(20)]
        public string maJob { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ghiChu { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class CapNhatMocBanGiao_RequestDTO
    {
        [Required]
        public string maHD { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int maGiaiDoan { get; set; }

        public string? linkSanPham { get; set; }

        public string? moTa { get; set; }
    }
}


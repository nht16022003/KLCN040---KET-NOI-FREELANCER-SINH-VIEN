
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class TaoBanGiaoRequestDTO
    {
        [Required]
        public string MaHD { get; set; } = string.Empty;

        [Required]
        public string FileSanPhamUrl { get; set; } = string.Empty;

        public string? LinkDemo { get; set; }

        [Required]
        public string MoTaBanGiao { get; set; } = string.Empty;
    }
}

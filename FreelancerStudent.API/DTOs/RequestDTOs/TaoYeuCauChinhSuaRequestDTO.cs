
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class TaoYeuCauChinhSuaRequestDTO
    {
        [Required]
        public int MaBanGiao { get; set; }

        [Required]
        public string NoiDungYeuCau { get; set; } = string.Empty;
    }
}

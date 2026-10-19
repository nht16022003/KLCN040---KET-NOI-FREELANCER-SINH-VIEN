using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class CapNhatTrangThaiTask_RequestDTO
    {
        [Required]
        public int maTask { get; set; }

        [Required(ErrorMessage = "Trạng thái mới không được để trống.")]
        public string trangThai { get; set; } = string.Empty;
    }
}

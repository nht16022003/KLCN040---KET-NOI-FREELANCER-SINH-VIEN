using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class XacNhanHopDong_RequestDTO
    {
        [Required(ErrorMessage = "Mã hợp đồng không được để trống")]
        public string maHD { get; set; } = string.Empty;

        public bool dongY { get; set; } // true: Xác nhận, false: Từ chối

        public string? lyDoTuChoi { get; set; }
    }
}

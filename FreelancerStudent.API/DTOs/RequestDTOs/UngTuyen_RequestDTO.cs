using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class UngTuyen_RequestDTO
    {
        [Required]
        public string maJob { get; set; } = string.Empty;

        public int maUser { get; set; } // Mã User của sinh viên đang đăng nhập

        [Required(ErrorMessage = "Vui lòng nhập thư giới thiệu")]
        public string thuGioiThieu { get; set; } = string.Empty;

        public decimal? thulaoDeXuat { get; set; }

        public string? thoiGianHoanThanhDeXuat { get; set; }

        public string? fileCV { get; set; }
    }
}

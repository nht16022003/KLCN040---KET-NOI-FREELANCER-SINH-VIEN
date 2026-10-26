using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class TaoYeuCauRutTien_RequestDTO
    {
        [Required(ErrorMessage = "Số tiền rút không được để trống.")]
        [Range(50000, 100000000, ErrorMessage = "Số tiền rút tối thiểu là 50.000 VNĐ.")]
        public decimal soTienRut { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng thụ hưởng.")]
        [MaxLength(100)]
        public string tenNganHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng.")]
        [MaxLength(50)]
        public string soTaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản thụ hưởng.")]
        [MaxLength(100)]
        public string tenChuTaiKhoan { get; set; } = string.Empty;
    }
}

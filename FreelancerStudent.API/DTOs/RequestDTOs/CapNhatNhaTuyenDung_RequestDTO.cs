using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class CapNhatThongTinNhaTuyenDung_RequestDTO
    {
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        public int maUser { get; set; }

        [MaxLength(100, ErrorMessage = "Tên công ty/doanh nghiệp tối đa 100 ký tự")]
        public string? tencongty { get; set; }

        [MaxLength(100, ErrorMessage = "Lĩnh vực hoạt động tối đa 100 ký tự")]
        public string? linhvuc { get; set; }

        [MaxLength(200, ErrorMessage = "Địa chỉ trụ sở tối đa 200 ký tự")]
        public string? diachi { get; set; }

        [MaxLength(500, ErrorMessage = "Đường dẫn website tối đa 500 ký tự")]
        public string? link { get; set; }

        [MaxLength(100, ErrorMessage = "Họ tên người đại diện tối đa 100 ký tự")]
        public string? hotenNguoiDaiDien { get; set; }

        [MaxLength(15, ErrorMessage = "Số điện thoại liên hệ tối đa 15 ký tự")]
        public string? sdtLienHe { get; set; }
    }

    public class CapNhatGioiThieuNhaTuyenDung_RequestDTO
    {
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        public int maUser { get; set; }

        public string? gioithieu { get; set; }
    }
}


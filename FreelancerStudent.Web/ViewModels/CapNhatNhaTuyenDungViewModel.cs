using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class CapNhatThongTinNhaTuyenDungViewModel
    {
        public int MaUser { get; set; }

        [Display(Name = "Tên doanh nghiệp / Đơn vị")]
        [Required(ErrorMessage = "Vui lòng nhập tên công ty hoặc đơn vị tuyển dụng.")]
        [StringLength(100, ErrorMessage = "Tên doanh nghiệp tối đa 100 ký tự.")]
        public string? Tencongty { get; set; }

        [Display(Name = "Người đại diện")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên người đại diện.")]
        [StringLength(100, ErrorMessage = "Tên người đại diện tối đa 100 ký tự.")]
        public string? HotenNguoiDaiDien { get; set; }

        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        [Display(Name = "Số điện thoại liên hệ")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự.")]
        public string? SdtLienHe { get; set; }

        [Display(Name = "Lĩnh vực hoạt động")]
        [StringLength(100, ErrorMessage = "Lĩnh vực tối đa 100 ký tự.")]
        public string? Linhvuc { get; set; }

        [Display(Name = "Địa chỉ trụ sở")]
        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự.")]
        public string? Diachi { get; set; }

        [Display(Name = "Website / Link")]
        [StringLength(500, ErrorMessage = "Đường dẫn website tối đa 500 ký tự.")]
        public string? Link { get; set; }
    }

    public class CapNhatGioiThieuNhaTuyenDungViewModel
    {
        public int MaUser { get; set; }

        [Display(Name = "Giới thiệu doanh nghiệp")]
        public string? Gioithieu { get; set; }
    }
}


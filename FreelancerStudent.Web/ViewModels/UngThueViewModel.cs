using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class GuiYeuCauThueRequestModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Freelancer cần thuê")]
        public int maFreelancerStudent { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bài đăng tuyển dụng")]
        public string maJob { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không quá 200 ký tự")]
        public string tieuDeCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả yêu cầu công việc")]
        public string moTaCongViec { get; set; } = string.Empty;

        [Range(1000, 1000000000, ErrorMessage = "Ngân sách đề nghị tối thiểu từ 1,000 VNĐ")]
        public decimal? nganSachDeNghi { get; set; }

        public string? thoiHanDuKien { get; set; }
    }

    public class PhanHoiYeuCauThueModel
    {
        [Required]
        public int maYeuCau { get; set; }

        [Required]
        public bool chapNhan { get; set; }

        public string? lyDoTuChoi { get; set; }
    }

    public class YeuCauThueViewModel
    {
        public int maYeuCau { get; set; }
        public int maNhaTuyenDung { get; set; }
        public int maUserNTD { get; set; }
        public string tenNhaTuyenDung { get; set; } = string.Empty;
        public string? avatarNhaTuyenDung { get; set; }
        public int maFreelancerStudent { get; set; }
        public int maUserFreelancer { get; set; }
        public string tenFreelancer { get; set; } = string.Empty;
        public string? avatarFreelancer { get; set; }
        public string? maJob { get; set; }
        public string? tieuDeJob { get; set; }
        public string tieuDeCongViec { get; set; } = string.Empty;
        public string moTaCongViec { get; set; } = string.Empty;
        public decimal? nganSachDeNghi { get; set; }
        public string? thoiHanDuKien { get; set; }
        public DateTime ngayGui { get; set; }
        public string trangThai { get; set; } = "Chờ phản hồi";
        public string? lyDoTuChoi { get; set; }
    }

    public class ChonJobThueFreelancerViewModel
    {
        public int MaFreelancerStudent { get; set; }
        public string TenFreelancer { get; set; } = string.Empty;
        public string? AvatarFreelancer { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? TruongDaiHoc { get; set; }
        public int NamThu { get; set; } = 1;
        public double GPA { get; set; }
        public double? SoSaoDanhGia { get; set; }
        public int SoLuotDanhGia { get; set; }
        public string? GioiThieu { get; set; }
        public bool TrangthaiNhanViec { get; set; } = true;
        public decimal? ChiPhiTu { get; set; }
        public List<string> DanhSachKyNang { get; set; } = new();
        public FreelacerStudentViewModel? FreelancerCard { get; set; }

        public List<JobPostViewModel> DanhSachJob { get; set; } = new();
        public string? MaJobDuocChon { get; set; }
    }

    public class XacNhanThueFreelancerViewModel
    {
        public int MaFreelancerStudent { get; set; }
        public string TenFreelancer { get; set; } = string.Empty;
        public string? AvatarFreelancer { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? TruongDaiHoc { get; set; }
        public double? SoSaoDanhGia { get; set; }

        public string MaJob { get; set; } = string.Empty;
        public string TieuDeJob { get; set; } = string.Empty;
        public decimal? ThuLaoJob { get; set; }
        public DateTime? ThoiHanHoanThanhJob { get; set; }
        public string? MoTaJob { get; set; }

        public GuiYeuCauThueRequestModel FormModel { get; set; } = new();
    }
}


using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class GiaiDoanHopDongViewModel
    {
        public int maGiaiDoan { get; set; }
        public string maHD { get; set; } = string.Empty;
        public string tenGiaiDoan { get; set; } = string.Empty;
        public string? moTa { get; set; }
        public DateTime hanChot { get; set; }
        public string trangThai { get; set; } = string.Empty;
    }

    public class HopDongViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string maJob { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string? moTaCongViec { get; set; }

        public int maFreelancerStudent { get; set; }
        public string tenFreelancer { get; set; } = string.Empty;
        public string? emailFreelancer { get; set; }
        public string? sdtFreelancer { get; set; }
        public string? truongFreelancer { get; set; }
        public string? chuyenNganhFreelancer { get; set; }
        public int? maUserFreelancer { get; set; }

        public int? maNhaTuyenDung { get; set; }
        public string? tenNhaTuyenDung { get; set; } = string.Empty;
        public string? emailNhaTuyenDung { get; set; }
        public string? sdtNhaTuyenDung { get; set; }
        public int? maUserNhaTuyenDung { get; set; }

        public DateTime? ngayBatDau { get; set; }
        public DateTime? ngayKetThuc { get; set; }
        public DateTime? ngayHoanThanh { get; set; }
        public DateTime? ngayDongHopDong { get; set; }
        public bool thanhToanTuDong { get; set; }
        public decimal? sotienkyquy { get; set; }
        public string? hinhthuclamviec { get; set; }
        public string? dieuKhoan { get; set; }
        public string trangThai { get; set; } = string.Empty;
        public string? trangThaiDanhGia { get; set; }
        public int? maDanhGia { get; set; }
        public bool coTheChinhSuaDanhGia { get; set; }
        public bool coTheDanhGia { get; set; }
        public List<GiaiDoanHopDongViewModel> giaiDoans { get; set; } = new List<GiaiDoanHopDongViewModel>();

        // Thuộc tính tiện ích giao diện
        public bool CoQuyenXacNhan { get; set; } = false;
        public bool LaFreelancerCuaHopDong { get; set; } = false;
        public bool CoQuyenKyQuy { get; set; } = false;
        public bool LaNhaTuyenDungCuaHopDong { get; set; } = false;

        public decimal? SoDuKhaDung { get; set; }
        public bool DaTaiThongTinVi { get; set; } = false;

        // Tiến độ dự án (FRL-UC-09)
        public TienDoDuAnViewModel? TienDoDuAn { get; set; }
    }

    public class HopDongThanhToanViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string tenFreelancer { get; set; } = string.Empty;
        public decimal sotienkyquy { get; set; }
        public decimal soTienHoaHong { get; set; }
        public decimal soTienFreelancerNhan { get; set; }
        public decimal soDuKhaDung { get; set; }
        public decimal soDuDongBang { get; set; }
        public bool daCoDuLieuEscrow { get; set; }
    }

    public class DanhSachHopDongViewModel
    {
        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }

        public List<HopDongViewModel> HopDongs { get; set; }
            = new List<HopDongViewModel>();

        public string? Loi { get; set; }

        public bool LaFreelancer { get; set; } = false;
    }

    public class TaoGiaiDoanHopDongViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên giai đoạn / mốc bàn giao")]
        public string tenGiaiDoan { get; set; } = string.Empty;

        public string? moTa { get; set; }

        public DateTime? hanChot { get; set; }

        public decimal? tyLeGiaiNgan { get; set; }

        public decimal? soTien { get; set; }
    }

    public class TaoHopDongViewModel
    {
        // Thông tin bài tuyển dụng (điền sẵn)
        public string maJob { get; set; } = string.Empty;
        public string tieuDeJob { get; set; } = string.Empty;
        public string? moTaJob { get; set; }
        public decimal? nganSachGoc { get; set; }

        // Thông tin ứng viên Freelancer (điền sẵn)
        public int maFreelancerStudent { get; set; }
        public string tenFreelancer { get; set; } = string.Empty;
        public string? emailFreelancer { get; set; }
        public string? truongFreelancer { get; set; }
        public string? chuyenNganhFreelancer { get; set; }
        public string? avatarFreelancer { get; set; }

        // Thông tin ứng tuyển
        public int? maUngTuyen { get; set; }
        public decimal? thulaoDeXuat { get; set; }
        public string? camKetThoiGian { get; set; }

        // Thông tin hợp đồng do Khách hàng nhập
        [Required(ErrorMessage = "Vui lòng nhập số tiền thù lao thỏa thuận.")]
        [Range(1000, 10000000000, ErrorMessage = "Số tiền thù lao thỏa thuận phải từ 1.000 VNĐ trở lên.")]
        public decimal sotienkyquy { get; set; }

        public DateTime? ngayBatDau { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn hạn chót hoàn thành tổng thể.")]
        public DateTime ngayKetThuc { get; set; } = DateTime.Now.AddDays(14);

        public string hinhthuclamviec { get; set; } = "Remote";

        public string? dieuKhoan { get; set; }

        // Mốc bàn giao (Milestones)
        public List<TaoGiaiDoanHopDongViewModel> giaiDoans { get; set; } = new List<TaoGiaiDoanHopDongViewModel>();
    }

    public class TaskCongViecViewModel
    {
        public int maTask { get; set; }
        public string maHD { get; set; } = string.Empty;
        public int? maGiaiDoan { get; set; }
        public string? tenGiaiDoan { get; set; }
        public string tenTask { get; set; } = string.Empty;
        public string moTa { get; set; } = string.Empty;
        public DateTime? hanHoanThanh { get; set; }
        public string trangThai { get; set; } = "ChuaThucHien";
        public DateTime ngayCapNhat { get; set; } = DateTime.Now;
    }

    public class TienDoDuAnViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string trangThaiHopDong { get; set; } = string.Empty;
        public DateTime? ngayBatDauHopDong { get; set; }
        public DateTime? ngayKetThucHopDong { get; set; }
        public int tongSoTask { get; set; }
        public int soTaskChuaThucHien { get; set; }
        public int soTaskDangThucHien { get; set; }
        public int soTaskHoanThanh { get; set; }
        public decimal tyLeHoanThanh { get; set; }
        public bool coQuyenCapNhat { get; set; }
        public List<TaskCongViecViewModel> danhSachTask { get; set; } = new List<TaskCongViecViewModel>();
    }

    public class TaoTaskCongViecViewModel
    {
        [Required(ErrorMessage = "Mã hợp đồng là bắt buộc.")]
        public string maHD { get; set; } = string.Empty;

        public int? maGiaiDoan { get; set; }

        [Required(ErrorMessage = "Tiêu đề đầu việc không được để trống.")]
        [MaxLength(500, ErrorMessage = "Tiêu đề không được vượt quá 500 ký tự.")]
        public string tenTask { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string moTa { get; set; } = string.Empty;

        public DateTime? hanHoanThanh { get; set; }
    }

    public class CapNhatTaskCongViecViewModel
    {
        [Required]
        public int maTask { get; set; }

        public string maHD { get; set; } = string.Empty;

        public int? maGiaiDoan { get; set; }

        [Required(ErrorMessage = "Tiêu đề đầu việc không được để trống.")]
        [MaxLength(500, ErrorMessage = "Tiêu đề không được vượt quá 500 ký tự.")]
        public string tenTask { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string moTa { get; set; } = string.Empty;

        public DateTime? hanHoanThanh { get; set; }

        [Required(ErrorMessage = "Trạng thái công việc không được để trống.")]
        public string trangThai { get; set; } = "ChuaThucHien";
    }

    public class CapNhatTrangThaiTaskViewModel
    {
        [Required]
        public int maTask { get; set; }

        [Required(ErrorMessage = "Trạng thái mới không được để trống.")]
        public string trangThai { get; set; } = string.Empty;
    }
}
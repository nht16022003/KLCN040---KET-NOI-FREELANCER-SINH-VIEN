using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class MinhChungItemViewModel
    {
        public int MaMinhChung { get; set; }
        public int MaUser { get; set; }
        public string LoaiMinhChung { get; set; } = string.Empty;
        public string FileMinhChung { get; set; } = string.Empty;
        public DateTime NgayNop { get; set; }
        public DateTime? NgayXacMinh { get; set; }
        public string TrangThaiGuiMinhChung { get; set; } = string.Empty;
        public string? LyDoTuChoi { get; set; }
        public int? NguoiXacMinh { get; set; }

        // Thông tin hiện tại của User
        public string? HoTenUserHienTai { get; set; }
        public string? EmailUserHienTai { get; set; }
        public string? TenTruongHienTai { get; set; }
        public string? TenChuyenNganhHienTai { get; set; }

        // Thông tin trích xuất & xác nhận từ Thẻ sinh viên
        public string? MssvXacNhan { get; set; }
        public string? HoTenXacNhan { get; set; }
        public string? TenTruongXacNhan { get; set; }
        public string? MaTruongXacNhan { get; set; }
        public string? MaChuyenNganhXacNhan { get; set; }
        public string? TenChuyenNganhXacNhan { get; set; }
        public string? NienKhoaXacNhan { get; set; }
        public int? NamThuXacNhan { get; set; }
        public DateTime? NgaySinhXacNhan { get; set; }
    }

    public class ThongTinHienTaiViewModel
    {
        public int MaUser { get; set; }
        public int MaFreelancerStudents { get; set; }
        public string HoTenHienTai { get; set; } = string.Empty;
        public DateTime? NgaySinhHienTai { get; set; }
        public string DiaDiemHienTai { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public string ChuyenNganh { get; set; } = string.Empty;
    }

    public class QuetTheSinhVienResultViewModel
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public double DoTinCay { get; set; }
        public string PhuongThucNhanDien { get; set; } = string.Empty;

        public string? Mssv { get; set; }
        public string? HoVaTen { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? KhoaNganh { get; set; }
        public string? MaChuyenNganhGoiY { get; set; }
        public string? TenChuyenNganhGoiY { get; set; }
        public bool ChuyenNganhMoi { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }
        public DateTime? NgaySinh { get; set; }

        public string FileMinhChungUrl { get; set; } = string.Empty;
        public ThongTinHienTaiViewModel HoSoHienTai { get; set; } = new();
        public string? RawOcrText { get; set; }
    }

    public class XacNhanTheSinhVienViewModel
    {
        [Required]
        public int MaUser { get; set; }

        public string? Mssv { get; set; }
        public string? HoVaTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? MaChuyenNganh { get; set; }
        public string? TenChuyenNganh { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }

        [Required]
        public string FileMinhChungUrl { get; set; } = string.Empty;

        public bool DongYCapNhatHoSo { get; set; } = true;
    }

    public class PheDuyetMinhChungRequestViewModel
    {
        public int MaMinhChung { get; set; }
        public bool ChapNhan { get; set; } = true;
        public string? LyDoTuChoi { get; set; }
        public int? NguoiXacMinh { get; set; }

        public string? HoVaTen { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? MaChuyenNganh { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }
        public DateTime? NgaySinh { get; set; }
    }

    public class MinhChungIndexViewModel
    {
        public ThongTinHienTaiViewModel ThongTinSinhVien { get; set; } = new();
        public List<MinhChungItemViewModel> DanhSachMinhChung { get; set; } = new();
        public List<ChuyenNganhOptionViewModel> DanhSachChuyenNganh { get; set; } = new();
        public KiemTraXacThucViewModel TrangThaiXacThuc { get; set; } = new();
        public string? ThongBaoLoi { get; set; }
        public string? ThongBaoThanhCong { get; set; }
    }

    public class ChuyenNganhOptionViewModel
    {
        public string MaChuyenNganh { get; set; } = string.Empty;
        public string TenChuyenNganh { get; set; } = string.Empty;
    }

    public class KiemTraXacThucViewModel
    {
        public bool DaXacMinh { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string ThongBao { get; set; } = string.Empty;
        public DateTime? NgayXacMinh { get; set; }
        public string? LyDoTuChoi { get; set; }
        public int? MaMinhChungMoiNhat { get; set; }

        // Bổ sung kiểm soát thời hạn niên khóa
        public bool HetHanNienKhoa { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamKetThucNienKhoa { get; set; }
    }
}


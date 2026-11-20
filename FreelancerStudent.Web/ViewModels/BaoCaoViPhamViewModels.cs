using Microsoft.AspNetCore.Http;

namespace FreelancerStudent.Web.ViewModels
{
    public class BaoCaoViPhamViewModel
    {
        public int MaBaoCao { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string? TenFreelancer { get; set; }
        public string? TenNhaTuyenDung { get; set; }
        public string LoaiViPham { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? ThoiGianViPham { get; set; }
        public DateTime NgayBaoCao { get; set; }
        public DateTime HanGiaiTrinh { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public bool? FreelancerDongY { get; set; }
        public string? GiaiTrinh { get; set; }
        public DateTime? NgayGiaiTrinh { get; set; }
        public string? PhanHoiNhaTuyenDung { get; set; }
        public DateTime? NgayPhanHoiNhaTuyenDung { get; set; }
        public bool FreelancerDongYGiaHan { get; set; }
        public bool NhaTuyenDungDongYGiaHan { get; set; }
        public string? KetLuan { get; set; }
        public string? MucDoViPham { get; set; }
        public string? GhiChuAdmin { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public int? SoLanViPham { get; set; }
        public decimal? SoDiemTru { get; set; }
        public decimal? DiemTruocXuLy { get; set; }
        public decimal? DiemSauXuLy { get; set; }
        public List<TepMinhChungViPhamViewModel> TepMinhChungs { get; set; } = new();
    }

    public class TepMinhChungViPhamViewModel
    {
        public int MaTep { get; set; }
        public int MaNguoiTai { get; set; }
        public string LoaiTep { get; set; } = string.Empty;
        public string TenTep { get; set; } = string.Empty;
        public string DuongDan { get; set; } = string.Empty;
        public DateTime NgayTai { get; set; }
    }

    public class BaoCaoViPhamChiTietViewModel
    {
        public BaoCaoViPhamViewModel BaoCao { get; set; } = new();
        public KhieuNaiViPhamViewModel? KhieuNai { get; set; }
        public HopDongViPhamViewModel HopDong { get; set; } = new();
        public List<TaskViPhamViewModel> Tasks { get; set; } = new();
        public List<GiaiDoanViPhamViewModel> LoTrinhBanGiao { get; set; } = new();
        public List<BanGiaoViPhamViewModel> SanPhamDaBanGiao { get; set; } = new();
        public List<TinNhanViPhamViewModel> LichSuTraoDoi { get; set; } = new();
    }

    public class HopDongViPhamViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string maJob { get; set; } = string.Empty;
        public string? TenCongViec { get; set; }
        public string? trangThai { get; set; }
        public DateTime? ngayBatDau { get; set; }
        public DateTime? ngayKetThuc { get; set; }
        public string? dieuKhoan { get; set; }
        public decimal? sotienkyquy { get; set; }
        public string? TenFreelancer { get; set; }
        public string? TenNhaTuyenDung { get; set; }
    }

    public class TaskViPhamViewModel
    {
        public int maTask { get; set; }
        public string tenTask { get; set; } = string.Empty;
        public string moTa { get; set; } = string.Empty;
        public DateTime? hanHoanThanh { get; set; }
        public string? trangThai { get; set; }
        public DateTime ngayCapNhat { get; set; }
    }

    public class GiaiDoanViPhamViewModel
    {
        public int maGiaiDoan { get; set; }
        public string tenGiaiDoan { get; set; } = string.Empty;
        public string? moTa { get; set; }
        public DateTime hanChot { get; set; }
        public string? trangThai { get; set; }
    }

    public class BanGiaoViPhamViewModel
    {
        public int maBanGiao { get; set; }
        public string phienBan { get; set; } = string.Empty;
        public string fileSanPhamUrl { get; set; } = string.Empty;
        public string? linkDemo { get; set; }
        public string motaBanGiao { get; set; } = string.Empty;
        public DateTime ngayNop { get; set; }
        public string trangThai { get; set; } = string.Empty;
        public string? phanHoi { get; set; }
    }

    public class TinNhanViPhamViewModel
    {
        public int maNguoiGui { get; set; }
        public string? noiDung { get; set; }
        public string? fileDinhKem { get; set; }
        public string loaiTinNhan { get; set; } = string.Empty;
        public DateTime ngayGui { get; set; }
    }

    public class KhieuNaiViPhamViewModel
    {
        public int MaKhieuNai { get; set; }
        public int MaBaoCao { get; set; }
        public int MaFreelancerStudent { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string? KetQuaAdmin { get; set; }
        public int? MaAdminUser { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public BaoCaoViPhamViewModel? BaoCao { get; set; }
        public FreelancerViewModel? FreelancerStudent { get; set; }
    }

    public class FreelancerViewModel
    {
        public int maFreelancerStudents { get; set; }
        public string? tenUser { get; set; }
    }

    public class LichSuDiemUyTinViewModel
    {
        public decimal diemUyTin { get; set; }
        public int soLanViPham { get; set; }
        public List<MucLichSuDiemUyTinViewModel> LichSu { get; set; } = new();
    }

    public class MucLichSuDiemUyTinViewModel
    {
        public int MaLichSu { get; set; }
        public int? MaBaoCao { get; set; }
        public string LoaiThayDoi { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public decimal SoDiemThayDoi { get; set; }
        public decimal DiemTruoc { get; set; }
        public decimal DiemSau { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class TaoBaoCaoViPhamViewModel
    {
        public string MaHD { get; set; } = string.Empty;
        public string LoaiViPham { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? ThoiGianViPham { get; set; }
        public List<IFormFile> TepMinhChungs { get; set; } = new();
    }

    public class GiaiTrinhViPhamViewModel
    {
        public int MaBaoCao { get; set; }
        public bool DongYNoiDung { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool DongYGiaHan { get; set; }
        public List<IFormFile> TepMinhChungs { get; set; } = new();
    }

    public class PhanHoiViPhamViewModel
    {
        public int MaBaoCao { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool DongYGiaHan { get; set; }
        public List<IFormFile> TepMinhChungs { get; set; } = new();
    }

    public class KetLuanViPhamViewModel
    {
        public int MaBaoCao { get; set; }
        public string KetLuan { get; set; } = string.Empty;
        public string? MucDoViPham { get; set; }
        public string? GhiChu { get; set; }
        public int SoNgayGiaHan { get; set; }
    }

    public class GuiKhieuNaiViPhamViewModel
    {
        public int MaBaoCao { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public List<IFormFile> TepMinhChungs { get; set; } = new();
    }

    public class KetLuanKhieuNaiViewModel
    {
        public int MaKhieuNai { get; set; }
        public bool ChapNhan { get; set; }
        public string KetQua { get; set; } = string.Empty;
    }
}

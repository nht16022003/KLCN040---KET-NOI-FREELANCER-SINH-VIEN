using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    // ==========================================
    // 1. DASHBOARD & TỔNG QUAN HỆ THỐNG
    // ==========================================
    public class AdminDashboardViewModel
    {
        public int TongSoUser { get; set; }
        public int TongSoSinhVien { get; set; }
        public int TongSoNhaTuyenDung { get; set; }
        public int TongSoJob { get; set; }
        public int TongSoHopDong { get; set; }
        public decimal TongTienDangKyQuy { get; set; }
        public decimal TongDoanhThuHeThong { get; set; }
        public List<AdminRecentTransactionViewModel> GiaoDichMoiNhat { get; set; } = new();
    }

    public class AdminRecentTransactionViewModel
    {
        public int MaGiaoDich { get; set; }
        public int MaWallet { get; set; }
        public string LoaiGiaoDich { get; set; } = string.Empty;
        public decimal SoTien { get; set; }
        public string? NoiDung { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
    }

    // ==========================================
    // 2. AD-UC-01: QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG
    // ==========================================
    public class AdminUserManagementViewModel
    {
        public List<AdminUserItemViewModel> Users { get; set; } = new();
        public string? Keyword { get; set; }
        public int? SelectedRole { get; set; }
        public string? SelectedStatus { get; set; }
        public int TongSoNguoiDung => Users.Count;
        public int SoSinhVien { get; set; }
        public int SoNhaTuyenDung { get; set; }
        public int SoBiKhoa { get; set; }
    }

    public class AdminUserItemViewModel
    {
        public int MaUser { get; set; }
        public string HotenUser { get; set; } = string.Empty;
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, LOCKED
        public DateTime NgayTao { get; set; }
        public int MaRole { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? ThongTinPhu { get; set; } // Trường ĐH hoặc Tên công ty
        public decimal SoDuVi { get; set; }
    }

    // ==========================================
    // 3. AD-UC-02: XỬ LÝ TRẠNG THÁI TÀI KHOẢN (KHÓA / MỞ)
    // ==========================================
    public class AdminAccountStatusViewModel
    {
        public List<AdminUserItemViewModel> DanhSachTaiKhoan { get; set; } = new();
        public List<AdminLichSuXuLyViewModel> LichSuXuLys { get; set; } = new();
        public AdminXuLyPostModel XuLyModel { get; set; } = new();
    }

    public class AdminXuLyPostModel
    {
        [Required(ErrorMessage = "Vui lòng chọn tài khoản cần xử lý")]
        public int MaUser { get; set; }

        [Required]
        public string HanhDong { get; set; } = "KHOA_TAI_KHOAN"; // KHOA_TAI_KHOAN, MO_KHOA_TAI_KHOAN

        [Required(ErrorMessage = "Vui lòng nhập lý do xử lý tài khoản")]
        [StringLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string LyDo { get; set; } = string.Empty;
    }

    public class AdminLichSuXuLyViewModel
    {
        public int MaLichSu { get; set; }
        public int MaUser { get; set; }
        public string TenUser { get; set; } = string.Empty;
        public int MaAdmin { get; set; }
        public string TenAdmin { get; set; } = string.Empty;
        public string HanhDong { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public DateTime NgayXuLy { get; set; }
    }

    // ==========================================
    // 4. AD-UC-04: ĐIỀU CHỈNH SỐ DƯ TÀI KHOẢN (VÍ)
    // ==========================================
    public class AdminBalanceAdjustmentViewModel
    {
        public List<AdminWalletItemViewModel> Wallets { get; set; } = new();
        public List<AdminLichSuDieuChinhViViewModel> LichSuDieuChinhs { get; set; } = new();
        public AdminAdjustmentPostModel FormModel { get; set; } = new();
    }

    public class AdminWalletItemViewModel
    {
        public int MaWallet { get; set; }
        public int MaUser { get; set; }
        public string TenChuVi { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public decimal SoDuKhaDung { get; set; }
        public decimal SoDuDongBang { get; set; }
        public decimal TongTaiSan => SoDuKhaDung + SoDuDongBang;
    }

    public class AdminAdjustmentPostModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ví người dùng")]
        public int MaWallet { get; set; }

        [Required]
        public string LoaiDieuChinh { get; set; } = "CongTien"; // CongTien, TruTien

        [Required(ErrorMessage = "Vui lòng nhập số tiền điều chỉnh")]
        [Range(1000, 1000000000, ErrorMessage = "Số tiền điều chỉnh từ 1,000 VNĐ")]
        public decimal SoTien { get; set; } = 100000;

        [Required(ErrorMessage = "Vui lòng nhập lý do điều chỉnh")]
        [StringLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string LyDo { get; set; } = string.Empty;
    }

    public class AdminLichSuDieuChinhViViewModel
    {
        public int MaDieuChinh { get; set; }
        public int MaWallet { get; set; }
        public string TenChuVi { get; set; } = string.Empty;
        public int MaAdmin { get; set; }
        public string TenAdmin { get; set; } = string.Empty;
        public string LoaiDieuChinh { get; set; } = string.Empty;
        public decimal SoTienDieuChinh { get; set; }
        public decimal SoDuTruoc { get; set; }
        public decimal SoDuSau { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public DateTime NgayDieuChinh { get; set; }
    }

    // ==========================================
    // 5. AD-UC-10: QUẢN LÝ HỢP ĐỒNG & MẪU HỢP ĐỒNG
    // ==========================================
    public class AdminContractTemplateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề mẫu hợp đồng không được để trống")]
        public string TenMauHopDong { get; set; } = "Hợp đồng Dịch vụ Freelancer Sinh viên Chuẩn";

        [Required(ErrorMessage = "Điều khoản hợp đồng không được để trống")]
        public string DieuKhoanChung { get; set; } = string.Empty;

        [Required]
        public int ThoiHanNghiemThuNgay { get; set; } = 3;

        [Required]
        public decimal TyLeGiaiNganMacDinh { get; set; } = 100;

        public List<AdminHopDongItemViewModel> DanhSachHopDongHeThong { get; set; } = new();
    }

    public class AdminHopDongItemViewModel
    {
        public string MaHD { get; set; } = string.Empty;
        public string MaJob { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public int MaFreelancer { get; set; }
        public string TenFreelancer { get; set; } = string.Empty;
        public int MaNhaTuyenDung { get; set; }
        public string TenNhaTuyenDung { get; set; } = string.Empty;
        public decimal SoTienKyQuy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string HinhThucLamViec { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string? DieuKhoan { get; set; }
    }

    // ==========================================
    // 6. AD-UC-05: DUYỆT YÊU CẦU RÚT TIỀN
    // ==========================================
    public class AdminYeuCauRutTienItemViewModel
    {
        public int MaRutTien { get; set; }
        public int MaWallet { get; set; }
        public int MaUser { get; set; }
        public string HoTenUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public decimal SoTienRut { get; set; }
        public string TenNganHang { get; set; } = string.Empty;
        public string SoTaiKhoan { get; set; } = string.Empty;
        public string TenChuTaiKhoan { get; set; } = string.Empty;
        public DateTime? NgayYeuCau { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy, HoanThanh, TuChoi
        public string? LyDoTuChoi { get; set; }
        public int? MaAdminXuLy { get; set; }
        public string? TenAdminXuLy { get; set; }
        public decimal SoDuKhaDungVi { get; set; }
        public decimal SoDuDongBangVi { get; set; }
    }

    public class AdminXuLyRutTienPostModel
    {
        [Required]
        public int MaRutTien { get; set; }

        public bool ChapNhan { get; set; }

        [StringLength(255, ErrorMessage = "Lý do tối đa 255 ký tự")]
        public string? LyDoTuChoi { get; set; }
    }

    public class AdminDuyetRutTienViewModel
    {
        public List<AdminYeuCauRutTienItemViewModel> DanhSachYeuCau { get; set; } = new();
        public string CurrentFilter { get; set; } = "TatCa";
        public int TongSoChoXuLy { get; set; }
        public int TongSoHoanThanh { get; set; }
        public int TongSoTuChoi { get; set; }
        public decimal TongTienChoXuLy { get; set; }
    }
}

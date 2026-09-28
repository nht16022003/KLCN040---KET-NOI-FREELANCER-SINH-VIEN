using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    // ==========================================
    // AD-UC-01.01: QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG
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
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, LOCKED, PENDING
        public DateTime NgayTao { get; set; }
        public int MaRole { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? ThongTinPhu { get; set; } // Trường ĐH hoặc Tên công ty
        public decimal SoDuVi { get; set; }
        public bool DaXacMinhMinhChung { get; set; }
    }

    // ==========================================
    // AD-UC-02.01: XỬ LÝ TRẠNG THÁI TÀI KHOẢN
    // ==========================================
    public class AdminAccountStatusViewModel
    {
        public List<AdminUserItemViewModel> DanhSachTaiKhoan { get; set; } = new();
        public List<LichSuXuLyTaiKhoan> LichSuXuLys { get; set; } = new();
        public AdminXuLyPostModel XuLyModel { get; set; } = new();
    }

    public class AdminXuLyPostModel
    {
        [Required]
        public int MaUser { get; set; }

        [Required]
        public string HanhDong { get; set; } = "KHOA_TAI_KHOAN"; // KHOA_TAI_KHOAN, MO_KHOA_TAI_KHOAN

        [Required(ErrorMessage = "Vui lòng nhập lý do xử lý tài khoản")]
        [StringLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string LyDo { get; set; } = string.Empty;
    }

    // ==========================================
    // AD-UC-03.01: XỬ LÝ YÊU CẦU NẠP TIỀN
    // ==========================================
    public class AdminDepositManagementViewModel
    {
        public List<AdminDepositItemViewModel> YeuCauNaps { get; set; } = new();
        public string? FilterTrangThai { get; set; }
        public int SoChoDuyet { get; set; }
        public decimal TongTienChoDuyet { get; set; }
    }

    public class AdminDepositItemViewModel
    {
        public int MaYeuCauNap { get; set; }
        public int MaUser { get; set; }
        public string TenNguoiNap { get; set; } = string.Empty;
        public string EmailNguoiNap { get; set; } = string.Empty;
        public decimal SoTienNap { get; set; }
        public string PhuongThuc { get; set; } = string.Empty;
        public string? MaGDNganHang { get; set; }
        public string? AnhBienLai { get; set; }
        public string TrangThai { get; set; } = "ChoDuyet"; // ChoDuyet, DaDuyet, TuChoi
        public DateTime NgayYeuCau { get; set; }
        public string? GhiChu { get; set; }
        public string? LyDoTuChoi { get; set; }
    }

    // ==========================================
    // AD-UC-04.01: ĐIỀU CHỈNH SỐ DƯ TÀI KHOẢN
    // ==========================================
    public class AdminBalanceAdjustmentViewModel
    {
        public List<AdminWalletItemViewModel> Wallets { get; set; } = new();
        public List<DieuChinhSoDu> LichSuDieuChinhs { get; set; } = new();
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

    // ==========================================
    // AD-UC-05.01: XỬ LÝ THANH TOÁN GIAO DỊCH (ESCROW)
    // ==========================================
    public class AdminTransactionManagementViewModel
    {
        public List<AdminEscrowContractItemViewModel> EscrowContracts { get; set; } = new();
        public List<LichSuGiaoDich> RecentTransactions { get; set; } = new();
        public decimal TongTienDangKyQuy { get; set; }
        public int TongHopDongDangKyQuy { get; set; }
    }

    public class AdminEscrowContractItemViewModel
    {
        public string MaHD { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public string TenNhaTuyenDung { get; set; } = string.Empty;
        public string TenFreelancer { get; set; } = string.Empty;
        public decimal SoTienKyQuy { get; set; }
        public string TrangThai { get; set; } = "DangThucHien";
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }

    // ==========================================
    // AD-UC-06.01: QUẢN LÝ PHÍ, HOA HỒNG & DOANH THU
    // ==========================================
    public class AdminFeeCommissionViewModel
    {
        public decimal PhiDangBaiHienTai { get; set; } = 2000;
        public decimal PhanTramHoaHongHienTai { get; set; } = 5.0m;
        public List<CauHinhPhiHoaHongPhiDangBai> LichSuCauHinhs { get; set; } = new();

        // Báo cáo doanh thu
        public decimal TongDoanhThuPhiDangBai { get; set; }
        public decimal TongDoanhThuHoaHong { get; set; }
        public decimal TongDoanhThuHeThong => TongDoanhThuPhiDangBai + TongDoanhThuHoaHong;
        public List<MonthlyRevenueItemViewModel> DoanhThuTheoThang { get; set; } = new();
    }

    public class MonthlyRevenueItemViewModel
    {
        public string ThangNam { get; set; } = string.Empty;
        public decimal PhiDangBai { get; set; }
        public decimal HoaHong { get; set; }
        public decimal TongDoanhThu => PhiDangBai + HoaHong;
    }

    // ==========================================
    // AD-UC-07.01: XỬ LÝ TRANH CHẤP
    // ==========================================
    public class AdminDisputeResolutionViewModel
    {
        public List<AdminDisputeItemViewModel> TranhChaps { get; set; } = new();
        public int SoTranhChapDangCho { get; set; }
    }

    public class AdminDisputeItemViewModel
    {
        public int MaTranhChap { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public string NguoiKhieuNai { get; set; } = string.Empty;
        public string RoleNguoiKhieuNai { get; set; } = string.Empty;
        public string NguoiBiKhieuNai { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public decimal SoTienTranhChap { get; set; }
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy, DangPhanXu, DaGiaiQuyet, DaHuy
        public string? KetQuaPhanXu { get; set; }
        public DateTime NgayTao { get; set; }
        public List<BangChungTranhChap> BangChungs { get; set; } = new();
    }

    // ==========================================
    // AD-UC-08.01: XỬ LÝ YÊU CẦU HỖ TRỢ
    // ==========================================
    public class AdminSupportTicketViewModel
    {
        public List<YeuCauHoTro> YeuCaus { get; set; } = new();
        public int SoTicketDangXuLy { get; set; }
        public string? FilterTrangThai { get; set; }
    }

    // ==========================================
    // AD-UC-09.01: GIÁM SÁT HOẠT ĐỘNG HỆ THỐNG
    // ==========================================
    public class AdminSystemMonitoringViewModel
    {
        public int TongNguoiDung { get; set; }
        public int TongBaiDangJob { get; set; }
        public int TongHopDong { get; set; }
        public int TongTranhChap { get; set; }
        public decimal TongTienGiaoDichHeThong { get; set; }
        public List<LichSuGiaoDich> RecentSystemLogs { get; set; } = new();
        public List<LichSuXuLyTaiKhoan> RecentAccountLogs { get; set; } = new();
    }

    // ==========================================
    // AD-UC-10: QUẢN LÝ HỢP ĐỒNG (MẪU HỢP ĐỒNG)
    // ==========================================
    public class AdminContractTemplateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề mẫu hợp đồng không được để trống")]
        public string TenMauHopDong { get; set; } = "Hợp đồng Cung cấp Dịch vụ Freelance Sinh viên Chuẩn";

        [Required(ErrorMessage = "Điều khoản hợp đồng không được để trống")]
        public string DieuKhoanChung { get; set; } = string.Empty;

        [Required]
        public int ThoiHanNghiemThuNgay { get; set; } = 3;

        [Required]
        public decimal TyLeGiaiNganMacDinh { get; set; } = 100;

        public List<HopDong> DanhSachHopDongHeThong { get; set; } = new();
        public List<ContractDetailViewModel> DanhSachChiTietHopDongs { get; set; } = new();
    }

    // ==========================================
    // AD-UC-11: QUẢN LÝ PHÍ ĐĂNG BÀI
    // ==========================================
    public class AdminJobPostingFeeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mức phí đăng bài")]
        [Range(0, 1000000, ErrorMessage = "Mức phí từ 0 VNĐ đến 1,000,000 VNĐ")]
        public decimal PhiDangBaiMoi { get; set; } = 2000;

        public decimal PhiHienTai { get; set; } = 2000;
        public string? GhiChuCapNhat { get; set; }
        public List<CauHinhPhiHoaHongPhiDangBai> LichSuPhiDangBais { get; set; } = new();
    }

    // ==========================================
    // ADMIN DASHBOARD OVERVIEW
    // ==========================================
    public class AdminDashboardViewModel
    {
        public int TongSoUser { get; set; }
        public int TongSoSinhVien { get; set; }
        public int TongSoNhaTuyenDung { get; set; }
        public int TongSoJob { get; set; }
        public int TongSoHopDong { get; set; }
        public decimal TongDoanhThuHeThong { get; set; }
        public int SoYeuCauNapChoDuyet { get; set; }
        public int SoTranhChapCanXuLy { get; set; }
        public int SoTicketHoTro { get; set; }
        public List<LichSuGiaoDich> GiaoDichMoiNhat { get; set; } = new();
    }
}

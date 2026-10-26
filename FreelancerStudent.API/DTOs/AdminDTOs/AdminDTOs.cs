using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.AdminDTOs
{
    // ==========================================
    // 1. DASHBOARD & GIÁM SÁT
    // ==========================================
    public class AdminDashboardResponseDTO
    {
        public int TongSoUser { get; set; }
        public int TongSoSinhVien { get; set; }
        public int TongSoNhaTuyenDung { get; set; }
        public int TongSoJob { get; set; }
        public int TongSoHopDong { get; set; }
        public decimal TongTienDangKyQuy { get; set; }
        public decimal TongDoanhThuHeThong { get; set; }
        public List<AdminRecentTransactionDTO> GiaoDichMoiNhat { get; set; } = new();
    }

    public class AdminRecentTransactionDTO
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
    // 2. QUẢN LÝ NGƯỜI DÙNG (AD-UC-01)
    // ==========================================
    public class AdminUserItemDTO
    {
        public int MaUser { get; set; }
        public string HotenUser { get; set; } = string.Empty;
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public string Status { get; set; } = "HoatDong"; // HoatDong, BiKhoa
        public DateTime NgayTao { get; set; }
        public int MaRole { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? ThongTinPhu { get; set; } // Tên trường hoặc Tên công ty
        public decimal SoDuVi { get; set; }
    }

    // ==========================================
    // 3. KHÓA / MỞ KHÓA TÀI KHOẢN (AD-UC-02)
    // ==========================================
    public class AdminKhoaMoUserRequestDTO
    {
        [Required]
        public int MaUser { get; set; }

        [Required]
        public string HanhDong { get; set; } = "KHOA_TAI_KHOAN"; // KHOA_TAI_KHOAN, MO_KHOA_TAI_KHOAN

        [Required(ErrorMessage = "Vui lòng nhập lý do xử lý")]
        [MaxLength(500)]
        public string LyDo { get; set; } = string.Empty;

        public int? MaAdmin { get; set; }
    }

    public class AdminLichSuXuLyUserDTO
    {
        public int MaLichSu { get; set; }
        public int MaUser { get; set; }
        public string TenUser { get; set; } = string.Empty;
        public int MaAdmin { get; set; }
        public string TenAdmin { get; set; } = string.Empty;
        public string? HanhDong { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public string? TrangThaiTruoc { get; set; }
        public string? TrangThaiSau { get; set; }
        public DateTime NgayXuLy { get; set; }
    }

    // ==========================================
    // 4. ĐIỀU CHỈNH SỐ DƯ VÍ (AD-UC-04)
    // ==========================================
    public class AdminWalletItemDTO
    {
        public int MaWallet { get; set; }
        public int MaUser { get; set; }
        public string TenChuVi { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public decimal SoDuKhaDung { get; set; }
        public decimal SoDuDongBang { get; set; }
        public decimal TongTaiSan => SoDuKhaDung + SoDuDongBang;
    }

    public class AdminDieuChinhViRequestDTO
    {
        [Required]
        public int MaWallet { get; set; }

        [Required]
        public string LoaiDieuChinh { get; set; } = "CongTien"; // CongTien, TruTien

        [Required]
        [Range(1000, 1000000000, ErrorMessage = "Số tiền điều chỉnh từ 1,000 VNĐ")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do điều chỉnh")]
        [MaxLength(500)]
        public string LyDo { get; set; } = string.Empty;

        public int? MaAdmin { get; set; }
    }

    public class AdminLichSuDieuChinhViDTO
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
    // 5. QUẢN LÝ HỢP ĐỒNG & MẪU HỢP ĐỒNG (AD-UC-10)
    // ==========================================
    public class AdminMauHopDongDTO
    {
        public string TenMauHopDong { get; set; } = "Hợp đồng Dịch vụ Freelancer Sinh viên Chuẩn";
        public string DieuKhoanChung { get; set; } = string.Empty;
        public int ThoiHanNghiemThuNgay { get; set; } = 3;
        public decimal TyLeGiaiNganMacDinh { get; set; } = 100;
    }

    public class AdminHopDongItemDTO
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
    // 6. QUẢN LÝ & DUYỆT RÚT TIỀN (AD-UC-05)
    // ==========================================
    public class AdminYeuCauRutTienItemDTO
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

    public class AdminXuLyRutTienRequestDTO
    {
        [Required]
        public int MaRutTien { get; set; }

        public bool ChapNhan { get; set; } // true: Duyệt giải ngân, false: Từ chối hoàn tiền

        [MaxLength(255)]
        public string? LyDoTuChoi { get; set; }

        public int? MaAdmin { get; set; }
    }
}

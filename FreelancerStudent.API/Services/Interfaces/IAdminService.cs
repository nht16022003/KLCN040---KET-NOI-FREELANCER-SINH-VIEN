using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerStudent.API.DTOs.AdminDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardResponseDTO> LayDashboardAsync();
        Task<List<AdminUserItemDTO>> LayDanhSachUsersAsync(string? keyword, int? role, string? status);
        Task<bool> XuLyTrangThaiUserAsync(AdminKhoaMoUserRequestDTO request);
        Task<List<AdminLichSuXuLyUserDTO>> LayLichSuXuLyUserAsync(int? maUser);
        Task<List<AdminWalletItemDTO>> LayDanhSachWalletsAsync();
        Task<bool> DieuChinhSoDuViAsync(AdminDieuChinhViRequestDTO request);
        Task<List<AdminLichSuDieuChinhViDTO>> LayLichSuDieuChinhViAsync(int? maWallet);
        Task<List<AdminHopDongItemDTO>> LayDanhSachHopDongAsync();
        Task<AdminHopDongItemDTO?> LayChiTietHopDongAsync(string maHD);
        Task<AdminMauHopDongDTO> LayMauHopDongAsync();
        Task<bool> CapNhatMauHopDongAsync(AdminMauHopDongDTO model);
        Task<List<AdminYeuCauRutTienItemDTO>> LayDanhSachYeuCauRutTienAsync(string? trangThai);
        Task<bool> XuLyYeuCauRutTienAsync(AdminXuLyRutTienRequestDTO request);
    }
}

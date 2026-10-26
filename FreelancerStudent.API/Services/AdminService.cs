using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerStudent.API.DTOs.AdminDTOs;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;

        public AdminService(IAdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public Task<AdminDashboardResponseDTO> LayDashboardAsync()
        {
            return _adminRepo.LayDashboardAsync();
        }

        public Task<List<AdminUserItemDTO>> LayDanhSachUsersAsync(string? keyword, int? role, string? status)
        {
            return _adminRepo.LayDanhSachUsersAsync(keyword, role, status);
        }

        public Task<bool> XuLyTrangThaiUserAsync(AdminKhoaMoUserRequestDTO request)
        {
            return _adminRepo.XuLyTrangThaiUserAsync(request);
        }

        public Task<List<AdminLichSuXuLyUserDTO>> LayLichSuXuLyUserAsync(int? maUser)
        {
            return _adminRepo.LayLichSuXuLyUserAsync(maUser);
        }

        public Task<List<AdminWalletItemDTO>> LayDanhSachWalletsAsync()
        {
            return _adminRepo.LayDanhSachWalletsAsync();
        }

        public Task<bool> DieuChinhSoDuViAsync(AdminDieuChinhViRequestDTO request)
        {
            return _adminRepo.DieuChinhSoDuViAsync(request);
        }

        public Task<List<AdminLichSuDieuChinhViDTO>> LayLichSuDieuChinhViAsync(int? maWallet)
        {
            return _adminRepo.LayLichSuDieuChinhViAsync(maWallet);
        }

        public Task<List<AdminHopDongItemDTO>> LayDanhSachHopDongAsync()
        {
            return _adminRepo.LayDanhSachHopDongAsync();
        }

        public Task<AdminHopDongItemDTO?> LayChiTietHopDongAsync(string maHD)
        {
            return _adminRepo.LayChiTietHopDongAsync(maHD);
        }

        public Task<AdminMauHopDongDTO> LayMauHopDongAsync()
        {
            return _adminRepo.LayMauHopDongAsync();
        }

        public Task<bool> CapNhatMauHopDongAsync(AdminMauHopDongDTO model)
        {
            return _adminRepo.CapNhatMauHopDongAsync(model);
        }

        public Task<List<AdminYeuCauRutTienItemDTO>> LayDanhSachYeuCauRutTienAsync(string? trangThai)
        {
            return _adminRepo.LayDanhSachYeuCauRutTienAsync(trangThai);
        }

        public Task<bool> XuLyYeuCauRutTienAsync(AdminXuLyRutTienRequestDTO request)
        {
            return _adminRepo.XuLyYeuCauRutTienAsync(request);
        }
    }
}

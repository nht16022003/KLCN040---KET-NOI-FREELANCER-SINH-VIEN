using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IDanhGiaRepository
    {
        Task<HopDong?> LayHopDongAsync(string maHD);
        Task<DanhGia_NhanXet?> LayTheoHopDongNguoiDanhGiaAsync(string maHD, int maUser);
        Task<DanhGia_NhanXet?> LayTheoMaAsync(int maDanhGia);
        IQueryable<DanhGia_NhanXet> TruyVanDanhGiaNguoiDuocDanhGia(int maUser);
        Task CapNhatDiemDanhGiaAsync(int maUser);
    }
}

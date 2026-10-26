
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services
{
    public interface IHopDongService
    {
        Task<List<HopDongResponseDTO>> GetDanhSachHopDongAsync(
            int maNhaTuyenDung,
            string? tuKhoa,
            string? trangThai);

        Task<List<HopDongResponseDTO>> GetDanhSachHopDongTheoFreelancerAsync(
            int maFreelancerStudent,
            string? tuKhoa,
            string? trangThai);

        Task<HopDongResponseDTO> TaoDuThaoHopDongAsync(
            TaoHopDong_RequestDTO request,
            int? maUserNTD);

        Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaAsync(string maHD);

        Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaJobAsync(string maJob);

        Task<bool> XacNhanHopDongAsync(
            XacNhanHopDong_RequestDTO request,
            int? maUserFreelancer);

        Task<bool> KyQuyHopDongAsync(
            string maHD,
            int? maUserNhaTuyenDung);

        Task<bool> NghiemThuAsync(string maHD, int? maUserNTD);

        Task<HopDongThanhToan_ReponseDTO?> LayThongTinThanhToanAsync(string maHD, int? maUserNTD);

        Task<bool> ThanhToanAsync(string maHD, int? maUserNTD);

        Task<bool> ThanhToanCoreAsync(string maHD, int? maUserNTD, bool tuDong);

        Task<bool> XoaHopDongAsync(
            string maHD,
            int? maUserNTD);
    }
}
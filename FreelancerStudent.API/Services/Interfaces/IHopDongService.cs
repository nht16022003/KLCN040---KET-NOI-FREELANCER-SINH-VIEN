
using FreelancerStudent.API.DTOs.ReponseDTOs;

namespace FreelancerStudent.API.Services
{
    public interface IHopDongService
    {
        Task<List<HopDongResponseDTO>> GetDanhSachHopDongAsync(
            int maNhaTuyenDung,
            string? tuKhoa,
            string? trangThai);
    }
}

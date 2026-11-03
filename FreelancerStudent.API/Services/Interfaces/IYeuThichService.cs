using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTO;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IYeuThichService
    {
        // Nhóm Freelancer lưu việc
        Task<bool> LuuViecAsync(LuuViec_RequestDTO request);
        Task<bool> BoLuuViecAsync(int maUser, string maJob);
        Task<bool> KiemTraLuuViecAsync(int maUser, string maJob);
        Task<List<CongViecDaLuu_ResponseDTO>> LayDanhSachViecDaLuuAsync(int maUser);

        // Nhóm Nhà tuyển dụng lưu Freelancer
        Task<bool> LuuFreelancerAsync(LuuFreelancer_RequestDTO request);
        Task<bool> BoLuuFreelancerAsync(int maUser, int maFreelancerStudent);
        Task<bool> KiemTraLuuFreelancerAsync(int maUser, int maFreelancerStudent);
        Task<List<FreelancerYeuThich_ResponseDTO>> LayDanhSachFreelancerYeuThichAsync(int maUser);
    }
}


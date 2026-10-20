using System.Threading.Tasks;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface ITaskCongViecService
    {
        Task<TienDoDuAn_ResponseDTO> LayTienDoDuAnAsync(string maHD, int maUser);
        Task<TaskCongViec_ResponseDTO> ThemTaskAsync(TaoTaskCongViec_RequestDTO request, int maUser);
        Task<bool> CapNhatTaskAsync(CapNhatTaskCongViec_RequestDTO request, int maUser);
        Task<bool> CapNhatTrangThaiTaskAsync(int maTask, string trangThaiMoi, int maUser);
        Task<bool> XoaTaskAsync(int maTask, int maUser);
        Task<bool> DanhGiaGiaiDoanAsync(string maHD, int? maGiaiDoan, string trangThaiMoi, int maUser);
    }
}

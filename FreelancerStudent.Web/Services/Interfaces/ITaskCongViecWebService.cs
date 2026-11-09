using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface ITaskCongViecWebService
    {
        Task<ApiReponse<TienDoDuAnViewModel>> LayTienDoDuAnAsync(string maHD, int maUser);
        Task<ApiReponse<TaskCongViecViewModel>> TaoTaskAsync(TaoTaskCongViecViewModel model, int maUser);
        Task<ApiReponse<TaskCongViecViewModel>> CapNhatTaskAsync(CapNhatTaskCongViecViewModel model, int maUser);
        Task<ApiReponse<TaskCongViecViewModel>> CapNhatTrangThaiTaskAsync(CapNhatTrangThaiTaskViewModel model, int maUser);
        Task<ApiReponse<bool>> XoaTaskAsync(int maTask, int maUser);
        Task<ApiReponse<bool>> DanhGiaGiaiDoanAsync(string maHD, int? maGiaiDoan, string trangThai, string? lyDo, int maUser);
        Task<ApiReponse<bool>> CapNhatMocBanGiaoAsync(string maHD, int maGiaiDoan, string? linkSanPham, string? moTa, int maUser);
    }
}

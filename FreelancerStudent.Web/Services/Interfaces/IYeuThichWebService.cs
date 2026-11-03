using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IYeuThichWebService
    {
        Task<(bool success, string message)> LuuViecAsync(int maUser, string maJob, string? ghiChu = null);
        Task<(bool success, string message)> BoLuuViecAsync(int maUser, string maJob);
        Task<bool> KiemTraLuuViecAsync(int maUser, string maJob);
        Task<List<CongViecDaLuuViewModel>> LayDanhSachViecDaLuuAsync(int maUser);

        Task<(bool success, string message)> LuuFreelancerAsync(int maUser, int maFreelancerStudent, string? ghiChu = null);
        Task<(bool success, string message)> BoLuuFreelancerAsync(int maUser, int maFreelancerStudent);
        Task<bool> KiemTraLuuFreelancerAsync(int maUser, int maFreelancerStudent);
        Task<List<FreelancerYeuThichViewModel>> LayDanhSachFreelancerYeuThichAsync(int maUser);
    }
}


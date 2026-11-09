using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IThongBaoWebService
    {
        Task<ApiReponse<ThongBaoDanhSachViewModel>> LayDanhSachAsync(int maUser, int trang);
        Task<ApiReponse<int>> DemChuaDocAsync(int maUser);
        Task<ApiReponse<bool>> DanhDauDaDocAsync(int maThongBao, int maUser);
        Task<ApiReponse<int>> DanhDauTatCaDaDocAsync(int maUser);
    }
}

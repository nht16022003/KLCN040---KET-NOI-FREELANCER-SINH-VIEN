using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IAuthWebService
    {
        //Gửi dữ liệu đăng ký sang API
        Task<ApiReponse<UserSessionViewModel>> DangKyAsync(DangKyViewModel model);

        Task<ApiReponse<UserSessionViewModel>> DangNhapAsync(DangNhapViewModel model);
    }
}
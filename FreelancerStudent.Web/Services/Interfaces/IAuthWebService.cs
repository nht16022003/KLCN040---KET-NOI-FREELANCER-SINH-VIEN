using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IAuthWebService
    {
        //Gửi dữ liệu đăng ký sang API
        Task<ApiReponse<UserSessionViewModel>> DangKyAsync(DangKyViewModel model);

        Task<ApiReponse<UserSessionViewModel>> DangNhapAsync(DangNhapViewModel model);



        //Tuấn Anh
        Task<ApiReponse<ThongTinTaiKhoanViewModel>> LayThongTinTaiKhoanAsync(int maUser);


        //Tuấn Anh
        Task<ApiReponse<ThongTinTaiKhoanViewModel>> CapNhatThongTinTaiKhoanAsync(CapNhatThongTinTaiKhoanViewModel model);


        //Tuấn Anh
        Task<ApiReponse<object>> CapNhatAvatarAsync(int maUser, string avatarUrl);


        //Tuấn Anh

        Task<ApiReponse<object>> DoiMatKhauAsync(int maUser, DoiMatKhauViewModel model);
    }
}
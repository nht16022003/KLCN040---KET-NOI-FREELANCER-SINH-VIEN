using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IBaiDangTimViecWebService
    {

        //Tuấn
        Task<ApiReponse<BaiDangTimViecViewModel>> taoBaiDangAsync(DangTinTimViecViewModel model);

        //Tuấn
        Task<ApiReponse<List<BaiDangTimViecViewModel>>> layDanhSachCuaToiAsync(int maUser);

        //Tuấn
        Task<ApiReponse<List<BaiDangTimViecViewModel>>> layTatCaBaiDangAsync();

        //Tuấn
        Task<ApiReponse<bool>> doiTrangThaiAsync(string maBaiDang, string trangThai);

        //Tuấn
        Task<ApiReponse<bool>> xoaBaiDangAsync(string maBaiDang);

        //Tuấn
        Task<ApiReponse<bool>> capNhatBaiDangAsync(string maBaiDang, DangTinTimViecViewModel model);
    }
}

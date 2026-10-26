using FreelancerStudent.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IWalletWebService
    {
        Task<ApiReponse<WalletViewModel>> layViTheoUser(int maUser);

        Task<ApiReponse<List<LichSuGiaoDichItemViewModel>>> layLichSuGiaoDich(int maUser);

        Task<ApiReponse<YeuCauRutTienItemViewModel>> taoYeuCauRutTien(int maUser, RutTienRequestModel request);

        Task<ApiReponse<List<YeuCauRutTienItemViewModel>>> layDanhSachRutTien(int maUser);

        Task<ApiReponse<bool>> xacNhanRutTienMoPhong(int maRutTien, int maUser, bool chapNhan, string? lyDo = null);
    }
}
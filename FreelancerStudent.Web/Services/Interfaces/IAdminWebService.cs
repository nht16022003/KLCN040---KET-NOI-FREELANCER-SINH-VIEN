using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IAdminWebService
    {
        Task<ApiReponse<AdminDashboardViewModel>> LayDashboardAsync();
        Task<ApiReponse<List<AdminUserItemViewModel>>> LayDanhSachUsersAsync(string? keyword, int? role, string? status);
        Task<ApiReponse<bool>> XuLyTrangThaiUserAsync(AdminXuLyPostModel model, int maAdmin);
        Task<ApiReponse<List<AdminLichSuXuLyViewModel>>> LayLichSuXuLyUserAsync(int? maUser);
        Task<ApiReponse<List<AdminWalletItemViewModel>>> LayDanhSachWalletsAsync();
        Task<ApiReponse<bool>> DieuChinhSoDuViAsync(AdminAdjustmentPostModel model, int maAdmin);
        Task<ApiReponse<List<AdminLichSuDieuChinhViViewModel>>> LayLichSuDieuChinhViAsync(int? maWallet);
        Task<ApiReponse<List<AdminHopDongItemViewModel>>> LayDanhSachHopDongAsync();
        Task<ApiReponse<AdminHopDongItemViewModel>> LayChiTietHopDongAsync(string maHD);
        Task<ApiReponse<AdminContractTemplateViewModel>> LayMauHopDongAsync();
        Task<ApiReponse<bool>> CapNhatMauHopDongAsync(AdminContractTemplateViewModel model);
        Task<ApiReponse<List<AdminYeuCauRutTienItemViewModel>>> LayDanhSachYeuCauRutTienAsync(string? trangThai);
        Task<ApiReponse<bool>> XuLyRutTienAsync(AdminXuLyRutTienPostModel model, int maAdmin);
        Task<ApiReponse<List<AdminCauHinhPhiViewModel>>> LayDanhSachCauHinhPhiAsync();
        Task<ApiReponse<bool>> CapNhatPhiDangBaiAsync(AdminCapNhatPhiDangBaiViewModel model);
    }
}

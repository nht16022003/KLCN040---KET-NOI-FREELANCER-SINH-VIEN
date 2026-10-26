
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IBanGiaoSanPhamWebService
    {
        Task<ApiReponse<BanGiaoResponseViewModel>>
            TaoBanGiaoAsync(TaoBanGiaoViewModel model);

        Task<ApiReponse<List<BanGiaoResponseViewModel>>>
            LayDanhSachAsync(string maHD);

        Task<ApiReponse<YeuCauChinhSuaResponseViewModel>>
            GuiYeuCauAsync(TaoYeuCauChinhSuaViewModel model);

        Task<ApiReponse<List<YeuCauChinhSuaResponseViewModel>>>
            LayYeuCauAsync(string maHD);

        Task<ApiReponse<BanGiaoResponseViewModel>>
            NghiemThuAsync(int maBanGiao);
    }
}

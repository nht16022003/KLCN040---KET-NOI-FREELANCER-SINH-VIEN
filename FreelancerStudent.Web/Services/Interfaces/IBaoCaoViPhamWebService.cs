using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IBaoCaoViPhamWebService
    {
        Task<ApiReponse<List<BaoCaoViPhamViewModel>>> LayDanhSachAsync();
        Task<ApiReponse<BaoCaoViPhamChiTietViewModel>> LayChiTietAsync(int maBaoCao);
        Task<ApiReponse<LichSuDiemUyTinViewModel>> LayLichSuDiemUyTinAsync();
        Task<ApiReponse<bool>> TaoBaoCaoAsync(TaoBaoCaoViPhamViewModel model);
        Task<ApiReponse<bool>> GuiGiaiTrinhAsync(GiaiTrinhViPhamViewModel model);
        Task<ApiReponse<bool>> PhanHoiAsync(PhanHoiViPhamViewModel model);
        Task<ApiReponse<bool>> KetLuanAsync(KetLuanViPhamViewModel model);
        Task<ApiReponse<bool>> GuiKhieuNaiAsync(GuiKhieuNaiViPhamViewModel model);
        Task<ApiReponse<List<KhieuNaiViPhamViewModel>>> LayDanhSachKhieuNaiAsync();
        Task<ApiReponse<bool>> KetLuanKhieuNaiAsync(KetLuanKhieuNaiViewModel model);
    }
}

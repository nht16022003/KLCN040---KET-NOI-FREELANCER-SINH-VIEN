using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IUngThueWebService
    {
        Task<ApiReponse<YeuCauThueViewModel>> GuiYeuCauThueAsync(GuiYeuCauThueRequestModel model, int maUserNTD);
        Task<ApiReponse<List<YeuCauThueViewModel>>> LayDanhSachYeuCauThueTheoNhaTuyenDungAsync(int maUserNTD);
        Task<ApiReponse<List<YeuCauThueViewModel>>> LayDanhSachYeuCauThueTheoFreelancerAsync(int maUserFreelancer);
        Task<ApiReponse<YeuCauThueViewModel>> LayChiTietYeuCauThueAsync(int maYeuCau);
        Task<ApiReponse<YeuCauThueViewModel>> PhanHoiYeuCauThueAsync(int maYeuCau, bool chapNhan, string? lyDoTuChoi, int maUserFreelancer);
    }
}


using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IDanhGiaWebService
    {
        Task<ApiReponse<DanhGiaFormViewModel>> LayFormAsync(string maHD, int maUser);
        Task<ApiReponse<DanhGiaItemViewModel>> GuiAsync(DanhGiaRequestViewModel request, int maUser);
        Task<ApiReponse<DanhGiaItemViewModel>> CapNhatAsync(int maDanhGia, DanhGiaRequestViewModel request, int maUser);
        Task<ApiReponse<List<HopDongDanhGiaViewModel>>> LayCongViecDaThueAsync(int maUser);
        Task<ApiReponse<DanhGiaThongKeViewModel>> LayFreelancerAsync(int maFreelancerStudent, int trang, int kichThuocTrang);
        Task<ApiReponse<DanhGiaThongKeViewModel>> LayNhaTuyenDungAsync(int maNhaTuyenDung, int trang, int kichThuocTrang);
    }
}

using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IMinhChungWebService
    {
        Task<ApiReponse<QuetTheSinhVienResultViewModel>> QuetTheSinhVienAsync(IFormFile file, int maUser);
        Task<ApiReponse<bool>> XacNhanTheSinhVienAsync(XacNhanTheSinhVienViewModel model);
        Task<ApiReponse<List<MinhChungItemViewModel>>> LayLichSuMinhChungAsync(int maUser);
        Task<ApiReponse<List<ChuyenNganhOptionViewModel>>> LayDanhSachChuyenNganhAsync();
        Task<ApiReponse<ThongTinHienTaiViewModel>> LayThongTinHienTaiAsync(int maUser);
        Task<ApiReponse<KiemTraXacThucViewModel>> KiemTraTrangThaiXacThucAsync(int maUser);
        Task<ApiReponse<bool>> PheDuyetMinhChungAsync(int maMinhChung, bool chapNhan, string? lyDoTuChoi);
        Task<ApiReponse<bool>> PheDuyetMinhChungAsync(PheDuyetMinhChungRequestViewModel request);
        Task<ApiReponse<List<MinhChungItemViewModel>>> LayDanhSachChoDuyetAsync();
    }
}

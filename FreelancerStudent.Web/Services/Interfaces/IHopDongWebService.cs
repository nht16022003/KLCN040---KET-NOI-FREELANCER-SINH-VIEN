
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IHopDongWebService
    {
        Task<List<HopDongViewModel>> LayDanhSachAsync(
            int maNhaTuyenDung,
            string? tuKhoa,
            string? trangThai);

        Task<List<HopDongViewModel>> LayDanhSachTheoFreelancerAsync(
            int maFreelancerStudent,
            string? tuKhoa,
            string? trangThai);

        Task<ApiReponse<HopDongViewModel>> TaoDuThaoHopDongAsync(
            TaoHopDongViewModel model,
            int maUserNTD);

        Task<HopDongViewModel?> LayChiTietTheoJobAsync(string maJob);

        Task<HopDongViewModel?> LayChiTietTheoMaAsync(string maHD);

        Task<ApiReponse<bool>> XacNhanHopDongAsync(
            string maHD,
            bool dongY,
            string? lyDoTuChoi,
            int maUserFreelancer);
    }
}
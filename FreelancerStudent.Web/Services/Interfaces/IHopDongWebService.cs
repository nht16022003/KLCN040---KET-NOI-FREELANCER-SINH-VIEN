
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IHopDongWebService
    {
        Task<List<HopDongViewModel>> LayDanhSachAsync(
            int maNhaTuyenDung,
            string? tuKhoa,
            string? trangThai);
    }
}

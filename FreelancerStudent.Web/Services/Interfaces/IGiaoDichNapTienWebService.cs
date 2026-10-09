using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IGiaoDichNapTienWebService
    {
        Task<ApiReponse<GiaoDichNapTienViewModel>> taoGiaoDichNapTien(
            int maUser,
            TaoGiaoDichNapTienRequest request
        );
        Task<ApiReponse<GiaoDichNapTienViewModel>> layTrangThaiGiaoDich(
            string maGiaoDich
        );
    }
}
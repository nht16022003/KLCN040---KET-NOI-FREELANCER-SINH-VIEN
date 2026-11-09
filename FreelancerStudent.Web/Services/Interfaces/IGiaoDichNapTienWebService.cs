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

        // Xác nhận nạp tiền mô phỏng
        Task<ApiReponse<GiaoDichNapTienViewModel>> xacNhanNapTienMoPhong(
            string maGiaoDich,
            int maUser
        );

        // Upload ảnh biên lai minh chứng nạp tiền
        Task<ApiReponse<string>> uploadBienLai(
            string maGiaoDich,
            IFormFile file
        );
    }
}
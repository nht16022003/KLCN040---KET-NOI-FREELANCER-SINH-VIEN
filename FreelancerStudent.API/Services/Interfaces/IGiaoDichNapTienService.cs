using FreelancerStudent.API.DTOs.Reponse;
using FreelancerStudent.API.DTOs.Request;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IGiaoDichNapTienService
    {
        // Tạo giao dịch nạp tiền
        Task<GiaoDichNapTien_ReponseDTO> taoGiaoDichNapTienAsync(
            int maUser,
            TaoGiaoDichNapTien_RequestDTO request
        );

        // Hoàn thành giao dịch và cộng tiền vào ví
        Task<GiaoDichNapTien_ReponseDTO> hoanThanhGiaoDichNapTienAsync(
            string maGiaoDich
        );
        Task<GiaoDichNapTien_ReponseDTO> layTrangThaiGiaoDichAsync(
            string maGiaoDich
        );
        Task xuLyWebhookSePayAsync(string maGiaoDich, decimal soTien, string maGiaoDichNganHang);
    }
}
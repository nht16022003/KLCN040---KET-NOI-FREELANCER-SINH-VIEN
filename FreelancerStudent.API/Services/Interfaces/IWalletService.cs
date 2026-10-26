using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IWalletService
    {
        // Lấy ví theo mã Users
        Task<Wallet_ReponseDTO> layViTheoMaUsersAsync(Wallet_RequestDTO request);

        // Lấy lịch sử giao dịch của người dùng
        Task<List<LichSuGiaoDich_ResponseDTO>> layLichSuGiaoDichAsync(int maUser);

        // Tạo yêu cầu rút tiền mô phỏng
        Task<YeuCauRutTien_ResponseDTO> taoYeuCauRutTienAsync(int maUser, TaoYeuCauRutTien_RequestDTO request);

        // Lấy danh sách yêu cầu rút tiền của người dùng
        Task<List<YeuCauRutTien_ResponseDTO>> layDanhSachRutTienAsync(int maUser);

        // Xác nhận duyệt hoặc hủy yêu cầu rút tiền mô phỏng
        Task<bool> xacNhanRutTienMoPhongAsync(int maRutTien, int maUser, bool chapNhan, string? lyDoTuChoi);
    }
}
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        // Lấy ví theo mã user
        Task<Wallet> layViTheoMaUser(int maUser);

        // Thêm ví cho User vừa được tạo theo mã user
        Task<Wallet> themViChoUserTheoMaUser(int maUser);

        // Cập nhật thông tin ví
        Task capNhatViAsync(Wallet wallet);

        // Lấy ví theo mã wallet
        Task<Wallet> layViTheoMaWallet(int maWallet);

        // Lấy danh sách lịch sử giao dịch theo ví
        Task<List<LichSuGiaoDich>> layLichSuGiaoDichTheoWalletAsync(int maWallet);

        // Tạo yêu cầu rút tiền mới
        Task<YeuCauRutTien> taoYeuCauRutTienAsync(YeuCauRutTien yeuCau);

        // Lấy danh sách yêu cầu rút tiền của ví
        Task<List<YeuCauRutTien>> layDanhSachRutTienTheoWalletAsync(int maWallet);

        // Xác nhận duyệt hoặc từ chối yêu cầu rút tiền mô phỏng
        Task<bool> xacNhanRutTienMoPhongAsync(int maRutTien, int maUser, bool chapNhan, string? lyDoTuChoi);
    }
}
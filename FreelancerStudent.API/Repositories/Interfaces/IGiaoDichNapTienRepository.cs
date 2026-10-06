using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IGiaoDichNapTienRepository
    {
        // Thêm giao dịch nạp tiền mới
        Task<GiaoDichNapTien> themGiaoDichNapTienAsync(
            GiaoDichNapTien giaoDichNapTien
        );

        // Tìm giao dịch theo mã giao dịch NAP_xxx
        Task<GiaoDichNapTien?> layGiaoDichTheoMaAsync(
            string maGiaoDich
        );

        // Cập nhật giao dịch
        Task capNhatGiaoDichAsync(
            GiaoDichNapTien giaoDichNapTien
        );
        Task<bool> xuLyNapTienSePayAsync(string maGiaoDich, decimal soTien, string maGiaoDichNganHang);
    }
}
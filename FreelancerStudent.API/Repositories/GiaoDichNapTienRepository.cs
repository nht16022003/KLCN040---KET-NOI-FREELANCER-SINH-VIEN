using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class GiaoDichNapTienRepository : IGiaoDichNapTienRepository
    {
        private readonly ApplicationDBContext _context;

        public GiaoDichNapTienRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Thêm giao dịch nạp tiền mới
        public async Task<GiaoDichNapTien> themGiaoDichNapTienAsync(
            GiaoDichNapTien giaoDichNapTien)
        {
            await _context.GiaoDichNapTiens.AddAsync(giaoDichNapTien);

            await _context.SaveChangesAsync();

            return giaoDichNapTien;
        }


        // Tìm giao dịch theo mã NAP_xxx
        public async Task<GiaoDichNapTien?> layGiaoDichTheoMaAsync(
            string maGiaoDich)
        {
            return await _context.GiaoDichNapTiens
                .FirstOrDefaultAsync(
                    g => g.maGiaoDich == maGiaoDich
                );
        }


        // Cập nhật giao dịch
        public async Task capNhatGiaoDichAsync(
            GiaoDichNapTien giaoDichNapTien)
        {
            _context.GiaoDichNapTiens.Update(
                giaoDichNapTien
            );

            await _context.SaveChangesAsync();
        }
        // =====================================================
        // XỬ LÝ NẠP TIỀN TỪ SEPAY
        // =====================================================
        public async Task<bool> xuLyNapTienSePayAsync(
            string maGiaoDich,
            decimal soTien,
            string maGiaoDichNganHang)
        {
            // Transaction giúp:
            // hoặc Wallet + GiaoDichNapTien cùng thành công
            // hoặc tất cả rollback
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable
                );

            try
            {
                // 1. Tìm giao dịch
                var giaoDich =
                    await _context.GiaoDichNapTiens
                        .FirstOrDefaultAsync(
                            g => g.maGiaoDich == maGiaoDich
                        );

                if (giaoDich == null)
                {
                    throw new Exception(
                        "Không tìm thấy giao dịch nạp tiền."
                    );
                }


                // 2. Nếu đã hoàn thành
                // Webhook gửi lại thì không cộng lần nữa
                if (giaoDich.trangThai == "HoanThanh")
                {
                    await transaction.CommitAsync();

                    return false;
                }


                // 3. Kiểm tra trạng thái
                if (giaoDich.trangThai != "DangXuLy")
                {
                    throw new Exception(
                        "Giao dịch không ở trạng thái đang xử lý."
                    );
                }


                // 4. Kiểm tra hết hạn
                if (DateTime.Now > giaoDich.ngayHetHan)
                {
                    throw new Exception(
                        "Giao dịch nạp tiền đã hết hạn."
                    );
                }


                // 5. Kiểm tra số tiền
                if (soTien != giaoDich.soTien)
                {
                    throw new Exception(
                        "Số tiền chuyển khoản không khớp."
                    );
                }


                // 6. Kiểm tra mã giao dịch ngân hàng
                if (string.IsNullOrWhiteSpace(
                    maGiaoDichNganHang))
                {
                    throw new Exception(
                        "Không có mã giao dịch ngân hàng."
                    );
                }


                // 7. Kiểm tra mã ngân hàng đã xử lý chưa
                var daXuLy =
                    await _context.GiaoDichNapTiens
                        .AnyAsync(
                            g =>
                                g.maGiaoDichNganHang
                                    == maGiaoDichNganHang
                        );

                if (daXuLy)
                {
                    await transaction.CommitAsync();

                    return false;
                }


                // 8. Tìm Wallet
                var wallet =
                    await _context.Wallets
                        .FirstOrDefaultAsync(
                            w =>
                                w.maWallet
                                    == giaoDich.maWallet
                        );

                if (wallet == null)
                {
                    throw new Exception(
                        "Không tìm thấy ví."
                    );
                }


                // 9. Cộng tiền
                wallet.soDuKhaDung =
                    (wallet.soDuKhaDung ?? 0)
                    + giaoDich.soTien;


                // 10. Hoàn thành giao dịch
                giaoDich.trangThai = "HoanThanh";

                giaoDich.ngayHoanThanh =
                    DateTime.Now;

                giaoDich.maGiaoDichNganHang =
                    maGiaoDichNganHang;


                // 11. Chỉ SaveChanges một lần
                await _context.SaveChangesAsync();


                // 12. Commit transaction
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
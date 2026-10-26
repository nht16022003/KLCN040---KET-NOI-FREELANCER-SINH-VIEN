using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.Reponse;
using FreelancerStudent.API.DTOs.Request;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class GiaoDichNapTienService : IGiaoDichNapTienService
    {
        private readonly IGiaoDichNapTienRepository _giaoDichNapTienRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ApplicationDBContext _context;

        public GiaoDichNapTienService(
            IGiaoDichNapTienRepository giaoDichNapTienRepository,
            IWalletRepository walletRepository,
            ApplicationDBContext context)
        {
            _giaoDichNapTienRepository = giaoDichNapTienRepository;
            _walletRepository = walletRepository;
            _context = context;
        }

        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO> taoGiaoDichNapTienAsync(
            int maUser,
            TaoGiaoDichNapTien_RequestDTO request)
        {
            // Nếu người dùng là Sinh viên (maRole == 1), yêu cầu phải xác thực minh chứng trước khi nạp ví
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.maUser == maUser);
            if (user != null && user.maRole == 1)
            {
                var daXacMinh = await _context.MinhChung_FreelancerStudents
                    .AsNoTracking()
                    .AnyAsync(m => m.maUser == maUser && m.trangThaiGuiMinhChung == "Đã xác minh");

                if (!daXacMinh)
                {
                    throw new InvalidOperationException("Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') trước khi nạp tiền vào ví!");
                }
            }

            if (request.soTien < 100000 || request.soTien % 50000 != 0)
            {
                throw new Exception("Số tiền nạp phải từ 100.000đ trở lên và phải là bội số của 50.000đ.");
            }

            var wallet = await _walletRepository.layViTheoMaUser(maUser);
            if (wallet == null)
            {
                wallet = await _walletRepository.themViChoUserTheoMaUser(maUser);
            }

            string maGiaoDich = "NAP_" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            DateTime ngayTao = DateTime.Now;

            var giaoDichNapTien = new GiaoDichNapTien
            {
                maWallet = wallet.maWallet,
                maGiaoDich = maGiaoDich,
                soTien = request.soTien,
                noiDungChuyenKhoan = maGiaoDich,
                trangThai = "DangXuLy",
                ngayTao = ngayTao,
                ngayHetHan = ngayTao.AddMinutes(30)
            };

            var giaoDichDaTao = await _giaoDichNapTienRepository.themGiaoDichNapTienAsync(giaoDichNapTien);

            return new GiaoDichNapTien_ReponseDTO
            {
                maNapTien = giaoDichDaTao.maNapTien,
                maGiaoDich = giaoDichDaTao.maGiaoDich,
                soTien = giaoDichDaTao.soTien,
                noiDungChuyenKhoan = giaoDichDaTao.noiDungChuyenKhoan,
                trangThai = giaoDichDaTao.trangThai,
                ngayTao = giaoDichDaTao.ngayTao,
                ngayHetHan = giaoDichDaTao.ngayHetHan
            };
        }

        // =====================================================
        // HOÀN THÀNH GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO> hoanThanhGiaoDichNapTienAsync(string maGiaoDich)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var giaoDich = await _context.GiaoDichNapTiens.FirstOrDefaultAsync(g => g.maGiaoDich == maGiaoDich);
                if (giaoDich == null)
                {
                    throw new Exception("Không tìm thấy giao dịch nạp tiền.");
                }

                if (giaoDich.trangThai == "HoanThanh")
                {
                    throw new Exception("Giao dịch này đã được hoàn thành.");
                }

                if (DateTime.Now > giaoDich.ngayHetHan)
                {
                    giaoDich.trangThai = "HetHan";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    throw new Exception("Giao dịch nạp tiền đã hết hạn.");
                }

                if (giaoDich.trangThai != "DangXuLy")
                {
                    throw new Exception("Giao dịch không ở trạng thái đang xử lý.");
                }

                var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.maWallet == giaoDich.maWallet);
                if (wallet == null)
                {
                    throw new Exception("Không tìm thấy ví.");
                }

                decimal soDuTruoc = wallet.soDuKhaDung ?? 0;
                wallet.soDuKhaDung = soDuTruoc + giaoDich.soTien;

                giaoDich.trangThai = "HoanThanh";
                giaoDich.ngayHoanThanh = DateTime.Now;
                if (string.IsNullOrWhiteSpace(giaoDich.maGiaoDichNganHang))
                {
                    giaoDich.maGiaoDichNganHang = "DEMO_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                }

                // Ghi nhận lịch sử giao dịch vào bảng LichSuGiaoDich
                var lichSu = new LichSuGiaoDich
                {
                    maWallet = wallet.maWallet,
                    loaiGiaoDich = "NapTien",
                    soTien = giaoDich.soTien,
                    soDuTruocGD = soDuTruoc,
                    soDuSauGD = wallet.soDuKhaDung.Value,
                    noiDung = $"Nạp tiền vào ví qua giao dịch {giaoDich.maGiaoDich} [Mô phỏng Demo]",
                    phuongThucThanhToan = "MoPhongDemo",
                    maGiaoDichNgoai = giaoDich.maGiaoDichNganHang,
                    trangThai = "ThanhCong",
                    ngayTao = DateTime.Now
                };
                await _context.LichSuGiaoDichs.AddAsync(lichSu);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GiaoDichNapTien_ReponseDTO
                {
                    maNapTien = giaoDich.maNapTien,
                    maGiaoDich = giaoDich.maGiaoDich,
                    soTien = giaoDich.soTien,
                    noiDungChuyenKhoan = giaoDich.noiDungChuyenKhoan,
                    trangThai = giaoDich.trangThai,
                    ngayTao = giaoDich.ngayTao,
                    ngayHetHan = giaoDich.ngayHetHan
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // =====================================================
        // XÁC NHẬN NẠP TIỀN MÔ PHỎNG (CÓ KIỂM TRA QUYỀN SỞ HỮU VÍ)
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO> xacNhanNapTienMoPhongAsync(string maGiaoDich, int maUser)
        {
            var giaoDich = await _context.GiaoDichNapTiens
                .Include(g => g.Wallet)
                .FirstOrDefaultAsync(g => g.maGiaoDich == maGiaoDich);

            if (giaoDich == null)
            {
                throw new KeyNotFoundException("Không tìm thấy giao dịch nạp tiền.");
            }

            if (giaoDich.Wallet == null || giaoDich.Wallet.maUser != maUser)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xác nhận giao dịch nạp tiền của tài khoản khác.");
            }

            return await hoanThanhGiaoDichNapTienAsync(maGiaoDich);
        }

        // =====================================================
        // LẤY TRẠNG THÁI GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO> layTrangThaiGiaoDichAsync(string maGiaoDich)
        {
            var giaoDich = await _giaoDichNapTienRepository.layGiaoDichTheoMaAsync(maGiaoDich);
            if (giaoDich == null)
            {
                throw new Exception("Không tìm thấy giao dịch nạp tiền.");
            }

            if (giaoDich.trangThai == "DangXuLy" && DateTime.Now > giaoDich.ngayHetHan)
            {
                giaoDich.trangThai = "HetHan";
                await _giaoDichNapTienRepository.capNhatGiaoDichAsync(giaoDich);
            }

            return new GiaoDichNapTien_ReponseDTO
            {
                maNapTien = giaoDich.maNapTien,
                maGiaoDich = giaoDich.maGiaoDich,
                soTien = giaoDich.soTien,
                noiDungChuyenKhoan = giaoDich.noiDungChuyenKhoan,
                trangThai = giaoDich.trangThai,
                ngayTao = giaoDich.ngayTao,
                ngayHetHan = giaoDich.ngayHetHan
            };
        }

        // =====================================================
        // XỬ LÝ WEBHOOK SEPAY
        // =====================================================
        public async Task xuLyWebhookSePayAsync(string maGiaoDich, decimal soTien, string maGiaoDichNganHang)
        {
            await _giaoDichNapTienRepository.xuLyNapTienSePayAsync(maGiaoDich, soTien, maGiaoDichNganHang);
        }
    }
}
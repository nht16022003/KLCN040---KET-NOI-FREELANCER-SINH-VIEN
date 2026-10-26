using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDBContext _context;

        public WalletRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Lấy ví theo mã user
        public async Task<Wallet> layViTheoMaUser(int maUser)
        {
            var ketqua = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.maUser == maUser);

            return ketqua!;
        }

        // Thêm ví cho User vừa được tạo theo mã user
        public async Task<Wallet> themViChoUserTheoMaUser(int maUser)
        {
            var kiemTraVi = await _context.Wallets
                .FirstOrDefaultAsync(w => w.maUser == maUser);

            if (kiemTraVi != null)
            {
                return kiemTraVi;
            }

            var viMoi = new Wallet
            {
                maUser = maUser,
                soDuKhaDung = 0,
                soDuDongBang = 0
            };

            await _context.Wallets.AddAsync(viMoi);
            await _context.SaveChangesAsync();

            return viMoi;
        }

        // Cập nhật ví
        public async Task capNhatViAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet> layViTheoMaWallet(int maWallet)
        {
            var ketqua = await _context.Wallets
                .FirstOrDefaultAsync(w => w.maWallet == maWallet);

            return ketqua!;
        }

        // Lấy danh sách lịch sử giao dịch theo ví
        public async Task<List<LichSuGiaoDich>> layLichSuGiaoDichTheoWalletAsync(int maWallet)
        {
            return await _context.LichSuGiaoDichs
                .AsNoTracking()
                .Where(l => l.maWallet == maWallet)
                .OrderByDescending(l => l.ngayTao)
                .ThenByDescending(l => l.maGiaoDich)
                .ToListAsync();
        }

        // Tạo yêu cầu rút tiền mới (bảo chứng đóng băng số dư)
        public async Task<YeuCauRutTien> taoYeuCauRutTienAsync(YeuCauRutTien yeuCau)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.maWallet == yeuCau.maWallet);

                if (wallet == null)
                {
                    throw new InvalidOperationException("Không tìm thấy ví tương ứng.");
                }

                decimal soDuTruoc = wallet.soDuKhaDung ?? 0;
                if (soDuTruoc < yeuCau.soTienRut)
                {
                    throw new InvalidOperationException("Số dư khả dụng trong ví không đủ để rút tiền.");
                }

                // Chuyển tiền từ khả dụng sang đóng băng chờ duyệt
                wallet.soDuKhaDung = soDuTruoc - yeuCau.soTienRut;
                wallet.soDuDongBang = (wallet.soDuDongBang ?? 0) + yeuCau.soTienRut;
                _context.Wallets.Update(wallet);

                yeuCau.trangThai = "ChoXuLy";
                yeuCau.ngayYeuCau = DateTime.Now;
                await _context.YeuCauRutTiens.AddAsync(yeuCau);
                await _context.SaveChangesAsync();

                // Ghi sổ cái lịch sử giao dịch giữ tiền rút
                var lichSu = new LichSuGiaoDich
                {
                    maWallet = wallet.maWallet,
                    loaiGiaoDich = "GiuTienRut",
                    soTien = yeuCau.soTienRut,
                    soDuTruocGD = soDuTruoc,
                    soDuSauGD = wallet.soDuKhaDung.Value,
                    noiDung = $"Yêu cầu rút tiền về {yeuCau.tenNganHang} ({yeuCau.soTaiKhoan}) - Chờ xử lý [Mô phỏng]",
                    phuongThucThanhToan = "ViNoiBo",
                    maGiaoDichNgoai = $"RUT_{yeuCau.maRutTien}",
                    trangThai = "ChoXuLy",
                    ngayTao = DateTime.Now
                };
                await _context.LichSuGiaoDichs.AddAsync(lichSu);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return yeuCau;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Lấy danh sách yêu cầu rút tiền của ví
        public async Task<List<YeuCauRutTien>> layDanhSachRutTienTheoWalletAsync(int maWallet)
        {
            return await _context.YeuCauRutTiens
                .AsNoTracking()
                .Where(r => r.maWallet == maWallet)
                .OrderByDescending(r => r.ngayYeuCau)
                .ThenByDescending(r => r.maRutTien)
                .ToListAsync();
        }

        // Xác nhận duyệt hoặc từ chối yêu cầu rút tiền mô phỏng
        public async Task<bool> xacNhanRutTienMoPhongAsync(int maRutTien, int maUser, bool chapNhan, string? lyDoTuChoi)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var yeuCau = await _context.YeuCauRutTiens
                    .Include(r => r.Wallet)
                    .FirstOrDefaultAsync(r => r.maRutTien == maRutTien);

                if (yeuCau == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy yêu cầu rút tiền.");
                }

                if (yeuCau.Wallet == null)
                {
                    throw new InvalidOperationException("Không tìm thấy ví tương ứng với yêu cầu rút.");
                }

                // Kiểm tra quyền sở hữu
                if (yeuCau.Wallet.maUser != maUser)
                {
                    // Cho phép nếu là Admin
                    var isAdmin = await _context.Admins.AnyAsync(a => a.maUser == maUser) || 
                                  await _context.Users.AnyAsync(u => u.maUser == maUser && u.maRole == 3);
                    if (!isAdmin)
                    {
                        throw new UnauthorizedAccessException("Bạn không có quyền thao tác trên yêu cầu rút tiền này.");
                    }
                }

                if (yeuCau.trangThai != "ChoXuLy")
                {
                    throw new InvalidOperationException($"Yêu cầu rút tiền đang ở trạng thái '{yeuCau.trangThai}', không thể xử lý lại.");
                }

                var wallet = yeuCau.Wallet;
                decimal soDuKhaDungHienTai = wallet.soDuKhaDung ?? 0;

                if (chapNhan)
                {
                    // DUYỆT RÚT TIỀN: Trừ số dư đóng băng
                    wallet.soDuDongBang = Math.Max(0, (wallet.soDuDongBang ?? 0) - yeuCau.soTienRut);
                    yeuCau.trangThai = "HoanThanh";
                    yeuCau.ngayXuLy = DateTime.Now;

                    _context.Wallets.Update(wallet);
                    _context.YeuCauRutTiens.Update(yeuCau);

                    var lichSu = new LichSuGiaoDich
                    {
                        maWallet = wallet.maWallet,
                        loaiGiaoDich = "RutTien",
                        soTien = yeuCau.soTienRut,
                        soDuTruocGD = soDuKhaDungHienTai,
                        soDuSauGD = soDuKhaDungHienTai,
                        noiDung = $"Rút tiền thành công về {yeuCau.tenNganHang} ({yeuCau.soTaiKhoan}) [Mô phỏng Demo]",
                        phuongThucThanhToan = "ChuyenKhoan",
                        maGiaoDichNgoai = $"RUT_{yeuCau.maRutTien}",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    };
                    await _context.LichSuGiaoDichs.AddAsync(lichSu);
                }
                else
                {
                    // TỪ CHỐI / HỦY RÚT TIỀN: Hoàn lại tiền từ đóng băng về khả dụng
                    wallet.soDuDongBang = Math.Max(0, (wallet.soDuDongBang ?? 0) - yeuCau.soTienRut);
                    wallet.soDuKhaDung = soDuKhaDungHienTai + yeuCau.soTienRut;

                    yeuCau.trangThai = "TuChoi";
                    yeuCau.ngayXuLy = DateTime.Now;
                    yeuCau.lyDoTuChoi = string.IsNullOrWhiteSpace(lyDoTuChoi) ? "Yêu cầu rút tiền bị từ chối hoặc người dùng hủy." : lyDoTuChoi.Trim();

                    _context.Wallets.Update(wallet);
                    _context.YeuCauRutTiens.Update(yeuCau);

                    var lichSu = new LichSuGiaoDich
                    {
                        maWallet = wallet.maWallet,
                        loaiGiaoDich = "HoanTienRut",
                        soTien = yeuCau.soTienRut,
                        soDuTruocGD = soDuKhaDungHienTai,
                        soDuSauGD = wallet.soDuKhaDung.Value,
                        noiDung = $"Hoàn tiền yêu cầu rút #{yeuCau.maRutTien} về số dư khả dụng: {yeuCau.lyDoTuChoi}",
                        phuongThucThanhToan = "ViNoiBo",
                        maGiaoDichNgoai = $"RUT_{yeuCau.maRutTien}",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    };
                    await _context.LichSuGiaoDichs.AddAsync(lichSu);
                }

                await _context.SaveChangesAsync();
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
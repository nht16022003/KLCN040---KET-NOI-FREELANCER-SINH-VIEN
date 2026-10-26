using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMinhChungService _minhChungService;

        public WalletService(IWalletRepository walletRepository, IMinhChungService minhChungService)
        {
            _walletRepository = walletRepository;
            _minhChungService = minhChungService;
        }

        public async Task<Wallet_ReponseDTO> layViTheoMaUsersAsync(Wallet_RequestDTO request)
        {
            var ketqua = await _walletRepository.layViTheoMaUser(request.maUser);

            if (ketqua == null)
            {
                // Nếu chưa có ví thì tự động tạo ví rỗng cho user
                ketqua = await _walletRepository.themViChoUserTheoMaUser(request.maUser);
            }

            var ketqua_Moi = new Wallet_ReponseDTO
            {
                maWallet = ketqua.maWallet,
                tenUsers = ketqua.User?.tenTaiKhoanUser ?? ketqua.User?.hotenUser,
                soDuKhaDung = ketqua.soDuKhaDung ?? 0,
                soDuDongBang = ketqua.soDuDongBang ?? 0
            };
            return ketqua_Moi;
        }

        public async Task<List<LichSuGiaoDich_ResponseDTO>> layLichSuGiaoDichAsync(int maUser)
        {
            var wallet = await _walletRepository.layViTheoMaUser(maUser);
            if (wallet == null)
            {
                return new List<LichSuGiaoDich_ResponseDTO>();
            }

            var list = await _walletRepository.layLichSuGiaoDichTheoWalletAsync(wallet.maWallet);
            return list.Select(l => new LichSuGiaoDich_ResponseDTO
            {
                maGiaoDich = l.maGiaoDich,
                maWallet = l.maWallet,
                loaiGiaoDich = l.loaiGiaoDich,
                soTien = l.soTien,
                soDuTruocGD = l.soDuTruocGD,
                soDuSauGD = l.soDuSauGD,
                noiDung = l.noiDung,
                phuongThucThanhToan = l.phuongThucThanhToan,
                maGiaoDichNgoai = l.maGiaoDichNgoai,
                trangThai = l.trangThai,
                ngayTao = l.ngayTao
            }).ToList();
        }

        public async Task<YeuCauRutTien_ResponseDTO> taoYeuCauRutTienAsync(int maUser, TaoYeuCauRutTien_RequestDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Dữ liệu yêu cầu rút tiền không hợp lệ.");
            }

            var checkXacThuc = await _minhChungService.KiemTraTrangThaiXacThucAsync(maUser);
            if (!checkXacThuc.DaXacMinh)
            {
                throw new InvalidOperationException("Tài khoản sinh viên cần hoàn tất xác thực minh chứng (trạng thái 'Đã xác minh') trước khi rút tiền từ ví!");
            }

            if (request.soTienRut < 50000)
            {
                throw new ArgumentException("Số tiền rút tối thiểu là 50.000 VNĐ.");
            }

            if (string.IsNullOrWhiteSpace(request.tenNganHang) ||
                string.IsNullOrWhiteSpace(request.soTaiKhoan) ||
                string.IsNullOrWhiteSpace(request.tenChuTaiKhoan))
            {
                throw new ArgumentException("Vui lòng điền đầy đủ thông tin tài khoản ngân hàng nhận tiền.");
            }

            var wallet = await _walletRepository.layViTheoMaUser(maUser);
            if (wallet == null)
            {
                throw new InvalidOperationException("Không tìm thấy ví của người dùng.");
            }

            if ((wallet.soDuKhaDung ?? 0) < request.soTienRut)
            {
                throw new InvalidOperationException($"Số dư khả dụng ({wallet.soDuKhaDung:N0} đ) không đủ để rút {request.soTienRut:N0} đ.");
            }

            var entity = new YeuCauRutTien
            {
                maWallet = wallet.maWallet,
                soTienRut = request.soTienRut,
                tenNganHang = request.tenNganHang.Trim(),
                soTaiKhoan = request.soTaiKhoan.Trim(),
                tenChuTaiKhoan = request.tenChuTaiKhoan.Trim().ToUpper(),
                trangThai = "ChoXuLy",
                ngayYeuCau = DateTime.Now
            };

            var ketQua = await _walletRepository.taoYeuCauRutTienAsync(entity);

            return new YeuCauRutTien_ResponseDTO
            {
                maRutTien = ketQua.maRutTien,
                maWallet = ketQua.maWallet,
                soTienRut = ketQua.soTienRut,
                tenNganHang = ketQua.tenNganHang,
                soTaiKhoan = ketQua.soTaiKhoan,
                tenChuTaiKhoan = ketQua.tenChuTaiKhoan,
                ngayYeuCau = ketQua.ngayYeuCau,
                ngayXuLy = ketQua.ngayXuLy,
                trangThai = ketQua.trangThai,
                lyDoTuChoi = ketQua.lyDoTuChoi
            };
        }

        public async Task<List<YeuCauRutTien_ResponseDTO>> layDanhSachRutTienAsync(int maUser)
        {
            var wallet = await _walletRepository.layViTheoMaUser(maUser);
            if (wallet == null)
            {
                return new List<YeuCauRutTien_ResponseDTO>();
            }

            var list = await _walletRepository.layDanhSachRutTienTheoWalletAsync(wallet.maWallet);
            return list.Select(r => new YeuCauRutTien_ResponseDTO
            {
                maRutTien = r.maRutTien,
                maWallet = r.maWallet,
                soTienRut = r.soTienRut,
                tenNganHang = r.tenNganHang,
                soTaiKhoan = r.soTaiKhoan,
                tenChuTaiKhoan = r.tenChuTaiKhoan,
                ngayYeuCau = r.ngayYeuCau,
                ngayXuLy = r.ngayXuLy,
                trangThai = r.trangThai,
                lyDoTuChoi = r.lyDoTuChoi
            }).ToList();
        }

        public async Task<bool> xacNhanRutTienMoPhongAsync(int maRutTien, int maUser, bool chapNhan, string? lyDoTuChoi)
        {
            return await _walletRepository.xacNhanRutTienMoPhongAsync(maRutTien, maUser, chapNhan, lyDoTuChoi);
        }
    }
}
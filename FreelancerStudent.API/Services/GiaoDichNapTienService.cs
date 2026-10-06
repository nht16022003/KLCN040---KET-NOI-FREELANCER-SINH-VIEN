using FreelancerStudent.API.DTOs.Reponse;
using FreelancerStudent.API.DTOs.Request;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class GiaoDichNapTienService : IGiaoDichNapTienService
    {
        private readonly IGiaoDichNapTienRepository _giaoDichNapTienRepository;
        private readonly IWalletRepository _walletRepository;

        public GiaoDichNapTienService(
            IGiaoDichNapTienRepository giaoDichNapTienRepository,
            IWalletRepository walletRepository)
        {
            _giaoDichNapTienRepository = giaoDichNapTienRepository;
            _walletRepository = walletRepository;
        }


        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO> taoGiaoDichNapTienAsync(
            int maUser,
            TaoGiaoDichNapTien_RequestDTO request)
        {
            // 1. Kiểm tra số tiền nạp
            if (request.soTien < 100000 || request.soTien % 50000 != 0)
            {
                throw new Exception(
                    "Số tiền nạp phải từ 100.000đ trở lên và phải là bội số của 50.000đ."
                );
            }

            // 2. Tìm ví của user
            var wallet =
                await _walletRepository.layViTheoMaUser(maUser);

            if (wallet == null)
            {
                throw new Exception(
                    "Không tìm thấy ví của người dùng."
                );
            }

            // 3. Tạo mã giao dịch
            string maGiaoDich =
                "NAP_" +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 10)
                    .ToUpper();

            // 4. Thời gian tạo
            DateTime ngayTao = DateTime.Now;

            // 5. Tạo giao dịch
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

            // 6. Lưu database
            var giaoDichDaTao =
                await _giaoDichNapTienRepository
                    .themGiaoDichNapTienAsync(
                        giaoDichNapTien
                    );

            // 7. Trả kết quả
            return new GiaoDichNapTien_ReponseDTO
            {
                maNapTien = giaoDichDaTao.maNapTien,
                maGiaoDich = giaoDichDaTao.maGiaoDich,
                soTien = giaoDichDaTao.soTien,
                noiDungChuyenKhoan =
                    giaoDichDaTao.noiDungChuyenKhoan,
                trangThai = giaoDichDaTao.trangThai,
                ngayTao = giaoDichDaTao.ngayTao,
                ngayHetHan = giaoDichDaTao.ngayHetHan
            };
        }


        // =====================================================
        // HOÀN THÀNH GIAO DỊCH NẠP TIỀN
        // =====================================================
        // Hàm này hiện dùng cho endpoint test thủ công.
        public async Task<GiaoDichNapTien_ReponseDTO>
            hoanThanhGiaoDichNapTienAsync(
                string maGiaoDich)
        {
            // 1. Tìm giao dịch
            var giaoDich =
                await _giaoDichNapTienRepository
                    .layGiaoDichTheoMaAsync(
                        maGiaoDich
                    );

            if (giaoDich == null)
            {
                throw new Exception(
                    "Không tìm thấy giao dịch nạp tiền."
                );
            }

            // 2. Kiểm tra đã hoàn thành chưa
            if (giaoDich.trangThai == "HoanThanh")
            {
                throw new Exception(
                    "Giao dịch này đã được hoàn thành."
                );
            }

            // 3. Kiểm tra giao dịch hết hạn
            if (DateTime.Now > giaoDich.ngayHetHan)
            {
                giaoDich.trangThai = "HetHan";

                await _giaoDichNapTienRepository
                    .capNhatGiaoDichAsync(
                        giaoDich
                    );

                throw new Exception(
                    "Giao dịch nạp tiền đã hết hạn."
                );
            }

            // 4. Chỉ giao dịch DangXuLy mới được hoàn thành
            if (giaoDich.trangThai != "DangXuLy")
            {
                throw new Exception(
                    "Giao dịch không ở trạng thái đang xử lý."
                );
            }

            // 5. Tìm ví
            var wallet =
                await _walletRepository
                    .layViTheoMaWallet(
                        giaoDich.maWallet
                    );

            if (wallet == null)
            {
                throw new Exception(
                    "Không tìm thấy ví."
                );
            }

            // 6. Cộng tiền vào ví
            wallet.soDuKhaDung =
                (wallet.soDuKhaDung ?? 0)
                + giaoDich.soTien;

            // 7. Lưu số dư
            await _walletRepository
                .capNhatViAsync(wallet);

            // 8. Hoàn thành giao dịch
            giaoDich.trangThai = "HoanThanh";
            giaoDich.ngayHoanThanh = DateTime.Now;

            // 9. Lưu giao dịch
            await _giaoDichNapTienRepository
                .capNhatGiaoDichAsync(
                    giaoDich
                );

            // 10. Trả kết quả
            return new GiaoDichNapTien_ReponseDTO
            {
                maNapTien = giaoDich.maNapTien,
                maGiaoDich = giaoDich.maGiaoDich,
                soTien = giaoDich.soTien,
                noiDungChuyenKhoan =
                    giaoDich.noiDungChuyenKhoan,
                trangThai = giaoDich.trangThai,
                ngayTao = giaoDich.ngayTao,
                ngayHetHan = giaoDich.ngayHetHan
            };
        }


        // =====================================================
        // LẤY TRẠNG THÁI GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<GiaoDichNapTien_ReponseDTO>
            layTrangThaiGiaoDichAsync(
                string maGiaoDich)
        {
            // 1. Tìm giao dịch
            var giaoDich =
                await _giaoDichNapTienRepository
                    .layGiaoDichTheoMaAsync(
                        maGiaoDich
                    );

            if (giaoDich == null)
            {
                throw new Exception(
                    "Không tìm thấy giao dịch nạp tiền."
                );
            }

            // 2. Nếu quá 30 phút thì hết hạn
            if (giaoDich.trangThai == "DangXuLy"
                && DateTime.Now > giaoDich.ngayHetHan)
            {
                giaoDich.trangThai = "HetHan";

                await _giaoDichNapTienRepository
                    .capNhatGiaoDichAsync(
                        giaoDich
                    );
            }

            // 3. Trả thông tin giao dịch
            return new GiaoDichNapTien_ReponseDTO
            {
                maNapTien = giaoDich.maNapTien,
                maGiaoDich = giaoDich.maGiaoDich,
                soTien = giaoDich.soTien,
                noiDungChuyenKhoan =
                    giaoDich.noiDungChuyenKhoan,
                trangThai = giaoDich.trangThai,
                ngayTao = giaoDich.ngayTao,
                ngayHetHan = giaoDich.ngayHetHan
            };
        }


        // =====================================================
        // XỬ LÝ WEBHOOK SEPAY
        // =====================================================
        public async Task xuLyWebhookSePayAsync(
            string maGiaoDich,
            decimal soTien,
            string maGiaoDichNganHang)
        {
            await _giaoDichNapTienRepository
                .xuLyNapTienSePayAsync(
                    maGiaoDich,
                    soTien,
                    maGiaoDichNganHang
                );
        }
    }
}
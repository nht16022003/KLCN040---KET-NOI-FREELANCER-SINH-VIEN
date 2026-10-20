
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class BanGiaoSanPhamService
    {
        private readonly BanGiaoSanPhamRepository _banGiaoRepo;
        private readonly YeuCauChinhSuaRepository _yeuCauRepo;
        private readonly ApplicationDBContext _context;

        public BanGiaoSanPhamService(
            BanGiaoSanPhamRepository banGiaoRepo,
            YeuCauChinhSuaRepository yeuCauRepo,
            ApplicationDBContext context)
        {
            _banGiaoRepo = banGiaoRepo;
            _yeuCauRepo = yeuCauRepo;
            _context = context;
        }

        // Kiểm tra quyền truy cập hợp đồng
        private async Task<HopDong> KiemTraQuyenAsync(
            string maHD, int maUser, bool laFreelancer)
        {
            var hd = await _context.HopDongs
                .Include(x => x.FreelancerStudent)
                .Include(x => x.JobPost)
                    .ThenInclude(x => x!.NhaTuyenDung)
                .FirstOrDefaultAsync(x => x.maHD == maHD);

            if (hd == null)
                throw new KeyNotFoundException(
                    "Không tìm thấy hợp đồng.");

            bool hopLe = laFreelancer
                ? hd.FreelancerStudent?.maUser == maUser
                : hd.JobPost?.NhaTuyenDung?.maUser == maUser;

            if (!hopLe)
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền thao tác hợp đồng này.");

            return hd;
        }

        // 1. Freelancer bàn giao v1.0 hoặc phiên bản mới
        public async Task<BanGiaoResponseDTO> TaoBanGiaoAsync(
            TaoBanGiaoRequestDTO request, int maUser)
        {
            var hd = await KiemTraQuyenAsync(
                request.MaHD, maUser, true);

            if (hd.trangThai != "DangThucHien")
                throw new InvalidOperationException(
                    "Hợp đồng không ở trạng thái đang thực hiện.");

            if (string.IsNullOrWhiteSpace(request.FileSanPhamUrl))
                throw new ArgumentException(
                    "Vui lòng cung cấp đường dẫn sản phẩm.");

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            var moiNhat = await _banGiaoRepo
                .GetLatestAsync(request.MaHD);

            if (moiNhat != null &&
                moiNhat.trangThai != "YeuCauChinhSua")
                throw new InvalidOperationException(
                    "Phiên bản hiện tại chưa có yêu cầu chỉnh sửa.");

            int soPhienBan = 1;

            if (moiNhat != null)
            {
                var chuoi = moiNhat.phienBan
                    .TrimStart('v', 'V')
                    .Split('.')[0];

                if (!int.TryParse(chuoi, out int soCu))
                    throw new InvalidOperationException(
                        "Phiên bản bàn giao không hợp lệ.");

                soPhienBan = soCu + 1;
            }

            var banGiao = new BanGiao_SanPham
            {
                maHD = request.MaHD,
                phienBan = $"v{soPhienBan}.0",
                fileSanPhamUrl = request.FileSanPhamUrl,
                linkDemo = request.LinkDemo,
                motaBanGiao = request.MoTaBanGiao,
                ngayNop = DateTime.Now,
                trangThai = "ChoNghiemThu"
            };

            await _banGiaoRepo.AddAsync(banGiao);

            // Đánh dấu yêu cầu của phiên bản trước đã được nộp lại
            if (moiNhat != null)
            {
                var danhSach = await _context.YeuCauChinhSuas
                    .Where(x => x.maBanGiao == moiNhat.maBanGiao &&
                                x.trangThai == "ChoChinhSua")
                    .ToListAsync();

                foreach (var yc in danhSach)
                {
                    yc.trangThai = "DaNopLai";
                    yc.ngayHoanThanh = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapBanGiao(banGiao);
        }

        // 2. Lấy lịch sử bàn giao
        public async Task<List<BanGiaoResponseDTO>>
            LayDanhSachAsync(string maHD, int maUser)
        {
            await KiemTraQuyenDocAsync(maHD, maUser);

            var danhSach = await _banGiaoRepo
                .GetByMaHDAsync(maHD);

            return danhSach.Select(MapBanGiao).ToList();
        }

        // 3. Khách hàng gửi yêu cầu chỉnh sửa
        public async Task<YeuCauChinhSuaResponseDTO>
            GuiYeuCauAsync(
                TaoYeuCauChinhSuaRequestDTO request,
                int maUser)
        {
            var banGiao = await _banGiaoRepo
                .GetByIdAsync(request.MaBanGiao);

            if (banGiao == null)
                throw new KeyNotFoundException(
                    "Không tìm thấy sản phẩm bàn giao.");

            await KiemTraQuyenAsync(
                banGiao.maHD, maUser, false);

            if (string.IsNullOrWhiteSpace(request.NoiDungYeuCau))
                throw new ArgumentException(
                    "Nội dung yêu cầu không được để trống.");

            if (banGiao.trangThai != "ChoNghiemThu")
                throw new InvalidOperationException(
                    "Sản phẩm không ở trạng thái chờ nghiệm thu.");

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            var yc = new YeuCauChinhSua
            {
                maBanGiao = request.MaBanGiao,
                noiDungYeuCau = request.NoiDungYeuCau,
                trangThai = "ChoChinhSua",
                ngayYeuCau = DateTime.Now
            };

            banGiao.trangThai = "YeuCauChinhSua";
            banGiao.phanHoi = request.NoiDungYeuCau;
            banGiao.ngayPhanHoi = DateTime.Now;

            await _yeuCauRepo.AddAsync(yc);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapYeuCau(yc);
        }

        // 4. Lấy các yêu cầu chỉnh sửa theo hợp đồng
        public async Task<List<YeuCauChinhSuaResponseDTO>>
            LayYeuCauAsync(string maHD, int maUser)
        {
            await KiemTraQuyenDocAsync(maHD, maUser);

            var danhSach = await _yeuCauRepo
                .GetByMaHDAsync(maHD);

            return danhSach.Select(MapYeuCau).ToList();
        }

        // 5. Khách hàng nghiệm thu sản phẩm
        
        public async Task<BanGiaoResponseDTO> NghiemThuAsync(
            int maBanGiao, int maUser)
        {
            var banGiao = await _banGiaoRepo.GetByIdAsync(maBanGiao);

            if (banGiao == null)
                throw new KeyNotFoundException(
                    "Không tìm thấy sản phẩm bàn giao.");

            var hd = await KiemTraQuyenAsync(
                banGiao.maHD, maUser, false);

            if (hd.trangThai != "DangThucHien")
                throw new InvalidOperationException(
                    "Hợp đồng không ở trạng thái đang thực hiện.");

            if (banGiao.trangThai != "ChoNghiemThu")
                throw new InvalidOperationException(
                    "Sản phẩm không ở trạng thái chờ nghiệm thu.");

            decimal soTien = hd.sotienkyquy ?? 0;

            if (soTien <= 0)
                throw new InvalidOperationException(
                    "Số tiền ký quỹ không hợp lệ.");

            int? maUserKhachHang =
                hd.JobPost?.NhaTuyenDung?.maUser;

            int? maUserFreelancer =
                hd.FreelancerStudent?.maUser;

            if (!maUserKhachHang.HasValue ||
                !maUserFreelancer.HasValue)
                throw new InvalidOperationException(
                    "Không tìm thấy tài khoản của hai bên hợp đồng.");

            if (maUserKhachHang.Value == maUserFreelancer.Value)
                throw new InvalidOperationException(
                    "Tài khoản khách hàng và Freelancer không hợp lệ.");

            string maHD = hd.maHD;

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Khóa quyền hoàn thành hợp đồng.
                // Chỉ một yêu cầu nghiệm thu được phép thành công.
                int soHopDong = await _context.Database
                    .ExecuteSqlInterpolatedAsync($@"
                        UPDATE HopDong
                        SET trangThai = N'DaHoanThanh'
                        WHERE maHD = {maHD}
                        AND trangThai = N'DangThucHien'
                    ");

                if (soHopDong != 1)
                    throw new InvalidOperationException(
                        "Hợp đồng đã hoàn thành hoặc trạng thái đã thay đổi.");

                // 2. Chỉ nghiệm thu phiên bản bàn giao mới nhất.
                int soBanGiao = await _context.Database
                    .ExecuteSqlInterpolatedAsync($@"
                        UPDATE BanGiao_SanPham
                        SET trangThai = N'DaNghiemThu',
                            ngayPhanHoi = GETDATE()
                        WHERE maBanGiao = {maBanGiao}
                        AND trangThai = N'ChoNghiemThu'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM BanGiao_SanPham AS bg2
                            WHERE bg2.maHD = {maHD}
                                AND bg2.maBanGiao > {maBanGiao}
                        )
                    ");

                if (soBanGiao != 1)
                    throw new InvalidOperationException(
                        "Phiên bản bàn giao không còn hợp lệ để nghiệm thu.");

                // 3. Trừ tiền đóng băng của khách hàng.
                int soViKhachHang = await _context.Database
                    .ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallet
                        SET soDuDongBang = soDuDongBang - {soTien}
                        WHERE maUser = {maUserKhachHang.Value}
                        AND soDuDongBang >= {soTien}
                    ");

                if (soViKhachHang != 1)
                    throw new InvalidOperationException(
                        "Ví khách hàng không đủ tiền ký quỹ để giải ngân.");

                // 4. Cộng tiền vào số dư khả dụng Freelancer.
                int soViFreelancer = await _context.Database
                    .ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallet
                        SET soDuKhaDung = soDuKhaDung + {soTien}
                        WHERE maUser = {maUserFreelancer.Value}
                        AND soDuKhaDung IS NOT NULL
                    ");

                if (soViFreelancer != 1)
                    throw new InvalidOperationException(
                        "Không tìm thấy ví Freelancer hợp lệ.");

                // 5. Lấy số dư sau giao dịch.
                var viKhachHang = await _context.Wallets
                    .AsNoTracking()
                    .SingleAsync(x =>
                        x.maUser == maUserKhachHang.Value);

                var viFreelancer = await _context.Wallets
                    .AsNoTracking()
                    .SingleAsync(x =>
                        x.maUser == maUserFreelancer.Value);

                decimal soDuFreelancerSau =
                    viFreelancer.soDuKhaDung ?? 0;

                decimal soDuFreelancerTruoc =
                    soDuFreelancerSau - soTien;

                // 6. Ghi lịch sử giải ngân.
                _context.LichSuGiaoDichs.Add(
                    new LichSuGiaoDich
                    {
                        maWallet = viKhachHang.maWallet,
                        loaiGiaoDich = "GiaiNganHopDong",
                        soTien = soTien,
                        soDuTruocGD =
                            (viKhachHang.soDuDongBang ?? 0) + soTien,
                        soDuSauGD = viKhachHang.soDuDongBang ?? 0,
                        noiDung = $"Giải ngân hợp đồng {maHD}",
                        phuongThucThanhToan = "ViNoiBo",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    });

                _context.LichSuGiaoDichs.Add(
                    new LichSuGiaoDich
                    {
                        maWallet = viFreelancer.maWallet,
                        loaiGiaoDich = "NhanTienHopDong",
                        soTien = soTien,
                        soDuTruocGD = soDuFreelancerTruoc,
                        soDuSauGD = soDuFreelancerSau,
                        noiDung = $"Nhận tiền hợp đồng {maHD}",
                        phuongThucThanhToan = "ViNoiBo",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Đọc lại dữ liệu sau khi cập nhật bằng SQL.
                var ketQua = await _context.BanGiao_SanPhams
                    .AsNoTracking()
                    .SingleAsync(x => x.maBanGiao == maBanGiao);

                return MapBanGiao(ketQua);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // Kiểm tra một trong hai bên hợp đồng
        private async Task KiemTraQuyenDocAsync(
            string maHD, int maUser)
        {
            var hd = await _context.HopDongs
                .Include(x => x.FreelancerStudent)
                .Include(x => x.JobPost)
                    .ThenInclude(x => x!.NhaTuyenDung)
                .FirstOrDefaultAsync(x => x.maHD == maHD);

            if (hd == null)
                throw new KeyNotFoundException(
                    "Không tìm thấy hợp đồng.");

            if (hd.FreelancerStudent?.maUser != maUser &&
                hd.JobPost?.NhaTuyenDung?.maUser != maUser)
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền xem hợp đồng.");
        }

        private static BanGiaoResponseDTO MapBanGiao(
            BanGiao_SanPham x)
        {
            return new BanGiaoResponseDTO
            {
                MaBanGiao = x.maBanGiao,
                MaHD = x.maHD,
                PhienBan = x.phienBan,
                FileSanPhamUrl = x.fileSanPhamUrl,
                LinkDemo = x.linkDemo,
                MoTaBanGiao = x.motaBanGiao,
                NgayNop = x.ngayNop,
                TrangThai = x.trangThai,
                PhanHoi = x.phanHoi,
                NgayPhanHoi = x.ngayPhanHoi
            };
        }

        private static YeuCauChinhSuaResponseDTO MapYeuCau(
            YeuCauChinhSua x)
        {
            return new YeuCauChinhSuaResponseDTO
            {
                MaYeuCauChinhSua = x.maYeuCauChinhSua,
                MaBanGiao = x.maBanGiao,
                NoiDungYeuCau = x.noiDungYeuCau,
                TrangThai = x.trangThai,
                NgayYeuCau = x.ngayYeuCau,
                NgayHoanThanh = x.ngayHoanThanh
            };
        }
    }
}

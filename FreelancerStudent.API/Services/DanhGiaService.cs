using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Options;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;

namespace FreelancerStudent.API.Services
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly ApplicationDBContext _context;
        private readonly IDanhGiaRepository _repository;
        private readonly DanhGiaPolicy _policy;
        private readonly NoiDungValidator _noiDungValidator;
        private readonly NghiepVuOptions _options;

        public DanhGiaService(ApplicationDBContext context, IDanhGiaRepository repository, DanhGiaPolicy policy,
            NoiDungValidator noiDungValidator, IOptions<NghiepVuOptions> options)
        {
            _context = context;
            _repository = repository;
            _policy = policy;
            _noiDungValidator = noiDungValidator;
            _options = options.Value;
        }

        public async Task<DanhGia_FormInfo_ReponseDTO> LayThongTinFormAsync(string maHD, int maUser)
        {
            if (string.IsNullOrWhiteSpace(maHD) || maUser <= 0)
                throw new ArgumentException("Thông tin yêu cầu không hợp lệ.");
            var hopDong = await _repository.LayHopDongAsync(maHD);
            if (hopDong == null)
                throw new ArgumentException("Không tìm thấy hợp đồng.");
            var isEmployer = hopDong.JobPost?.NhaTuyenDung?.maUser == maUser;
            var isFreelancer = hopDong.FreelancerStudent?.maUser == maUser;
            if (!isEmployer && !isFreelancer)
                throw new UnauthorizedAccessException("Bạn không có quyền đánh giá hợp đồng này.");

            var review = await _repository.LayTheoHopDongNguoiDanhGiaAsync(maHD, maUser);
            var canRate = _policy.CoTheDanhGia(hopDong);
            var status = review == null ? "ChoDanhGia" : "DaDanhGia";
            string? error = null;
            if (hopDong.trangThai is not ("DaThanhToan" or "DaHuyHoanTien"))
                error = "Chức năng đánh giá chỉ mở sau khi công việc đã hoàn thành nghiệm thu và thanh toán";
            else if (!canRate)
                error = $"Đã quá {_options.SoNgayChoDanhGia} ngày kể từ khi đóng hợp đồng, bạn không thể gửi đánh giá.";
            return new DanhGia_FormInfo_ReponseDTO
            {
                maHD = maHD,
                tenDoiTac = isEmployer
                    ? hopDong.FreelancerStudent?.User?.hotenUser ?? "Freelancer"
                    : hopDong.JobPost?.NhaTuyenDung?.tencongty ?? hopDong.JobPost?.NhaTuyenDung?.User?.hotenUser ?? "Nhà tuyển dụng",
                tenCongViec = hopDong.JobPost?.tieude ?? string.Empty,
                vaiTroNguoiDanhGia = isEmployer ? "NhaTuyenDung" : "FreelancerStudent",
                trangThaiDanhGia = status,
                coTheDanhGia = canRate && review == null,
                hanDanhGia = _policy.HanDanhGia(hopDong),
                danhGiaHienTai = review == null ? null : MapReview(review, hopDong, maUser),
                thongBaoLoi = error
            };
        }

        public async Task<DanhGia_ReponseDTO> GuiDanhGiaAsync(DanhGia_RequestDTO request, int maUser)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.maHD) || maUser <= 0)
                throw new ArgumentException("Thông tin yêu cầu không hợp lệ.");
            var hopDong = await _repository.LayHopDongAsync(request.maHD);
            if (hopDong == null)
                throw new ArgumentException("Không tìm thấy hợp đồng.");
            var maUserNtd = hopDong.JobPost?.NhaTuyenDung?.maUser;
            var maUserFreelancer = hopDong.FreelancerStudent?.maUser;
            if (maUser != maUserNtd && maUser != maUserFreelancer)
                throw new UnauthorizedAccessException("Bạn không có quyền đánh giá hợp đồng này.");
            if (hopDong.trangThai is not ("DaThanhToan" or "DaHuyHoanTien"))
                throw new InvalidOperationException("Chức năng đánh giá chỉ mở sau khi công việc đã hoàn thành nghiệm thu và thanh toán");
            if (!_policy.CoTheDanhGia(hopDong))
                throw new InvalidOperationException($"Đã quá {_options.SoNgayChoDanhGia} ngày kể từ khi đóng hợp đồng, bạn không thể gửi đánh giá.");
            if (await _repository.LayTheoHopDongNguoiDanhGiaAsync(request.maHD, maUser) != null)
                throw new InvalidOperationException("Hợp đồng này đã được đánh giá.");
            ValidateRating(request);

            var maNguoiDuocDanhGia = maUser == maUserNtd ? maUserFreelancer!.Value : maUserNtd!.Value;
            var review = new DanhGia_NhanXet
            {
                maHD = request.maHD,
                maNguoiDanhGia = maUser,
                maNguoiDuocDanhGia = maNguoiDuocDanhGia,
                soSao = request.soSaoTongThe,
                soSaoChatLuong = request.soSaoChatLuong,
                soSaoDungHan = request.soSaoDungHan,
                soSaoGiaoTiep = request.soSaoGiaoTiep,
                nhanXet = request.nhanXet?.Trim(),
                ngayDanhGia = DateTime.Now
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DanhGia_NhanXets.Add(review);
                await _context.SaveChangesAsync();
                await _repository.CapNhatDiemDanhGiaAsync(maNguoiDuocDanhGia);
                AddNotification(maNguoiDuocDanhGia, "Bạn nhận được đánh giá mới", "DanhGia", GetProfileLink(hopDong, maNguoiDuocDanhGia));
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapReview(review, hopDong, maUser);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && sql.Number is 2601 or 2627)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Hợp đồng này đã được đánh giá.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DanhGia_ReponseDTO> CapNhatDanhGiaAsync(int maDanhGia, DanhGia_RequestDTO request, int maUser)
        {
            if (maDanhGia <= 0 || request == null || string.IsNullOrWhiteSpace(request.maHD) || maUser <= 0)
                throw new ArgumentException("Thông tin yêu cầu không hợp lệ.");
            var review = await _repository.LayTheoMaAsync(maDanhGia);
            if (review == null)
                throw new ArgumentException("Không tìm thấy đánh giá.");
            if (review.maNguoiDanhGia != maUser)
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa đánh giá này.");
            if (!string.Equals(review.maHD, request.maHD, StringComparison.Ordinal))
                throw new ArgumentException("Mã hợp đồng không khớp với đánh giá.");
            if (DateTime.Now > _policy.HanChinhSua(review.ngayDanhGia))
                throw new InvalidOperationException($"Đã quá {_options.SoNgayChinhSuaDanhGia} ngày kể từ khi gửi đánh giá, bạn không thể chỉnh sửa.");
            ValidateRating(request);

            var hopDong = review.HopDong ?? await _repository.LayHopDongAsync(review.maHD)
                ?? throw new ArgumentException("Không tìm thấy hợp đồng.");
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DanhGia_NhanXet_LichSus.Add(new DanhGia_NhanXet_LichSu
                {
                    maDanhGia = review.maDanhGia,
                    soSao = review.soSao,
                    soSaoChatLuong = review.soSaoChatLuong,
                    soSaoDungHan = review.soSaoDungHan,
                    soSaoGiaoTiep = review.soSaoGiaoTiep,
                    nhanXet = review.nhanXet,
                    ngayLuu = DateTime.Now
                });
                review.soSao = request.soSaoTongThe;
                review.soSaoChatLuong = request.soSaoChatLuong;
                review.soSaoDungHan = request.soSaoDungHan;
                review.soSaoGiaoTiep = request.soSaoGiaoTiep;
                review.nhanXet = request.nhanXet?.Trim();
                review.ngayCapNhat = DateTime.Now;
                review.soLanChinhSua++;
                await _context.SaveChangesAsync();
                await _repository.CapNhatDiemDanhGiaAsync(review.maNguoiDuocDanhGia);
                AddNotification(review.maNguoiDuocDanhGia, "Đánh giá của bạn đã được cập nhật", "DanhGia", GetProfileLink(hopDong, review.maNguoiDuocDanhGia));
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapReview(review, hopDong, maUser);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DanhGiaThongKe_ReponseDTO> LayThongKeFreelancerAsync(int maFreelancerStudent, int trang, int kichThuocTrang)
        {
            var maUser = await _context.FreelancerStudents.AsNoTracking()
                .Where(f => f.maFreelancerStudents == maFreelancerStudent).Select(f => (int?)f.maUser).FirstOrDefaultAsync();
            if (!maUser.HasValue)
                throw new ArgumentException("Không tìm thấy hồ sơ Freelancer.");
            return await LayThongKeAsync(maUser.Value, trang, kichThuocTrang);
        }

        public async Task<DanhGiaThongKe_ReponseDTO> LayThongKeNhaTuyenDungAsync(int maNhaTuyenDung, int trang, int kichThuocTrang)
        {
            var maUser = await _context.NhaTuyenDungs.AsNoTracking()
                .Where(n => n.maNhaTuyenDung == maNhaTuyenDung).Select(n => (int?)n.maUser).FirstOrDefaultAsync();
            if (!maUser.HasValue)
                throw new ArgumentException("Không tìm thấy nhà tuyển dụng.");
            return await LayThongKeAsync(maUser.Value, trang, kichThuocTrang);
        }

        public async Task<List<HopDongDanhGia_ReponseDTO>> LayCongViecDaThueAsync(int maUser)
        {
            if (maUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ.");
            var contracts = await _context.HopDongs.AsNoTracking()
                .Include(h => h.JobPost).ThenInclude(j => j!.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(h => h.FreelancerStudent).ThenInclude(f => f!.User)
                .Where(h => h.trangThai == "DaHoanThanh" || h.trangThai == "DaThanhToan" || h.trangThai == "DaHuyHoanTien")
                .Where(h => h.JobPost!.NhaTuyenDung!.maUser == maUser || h.FreelancerStudent!.maUser == maUser)
                .OrderByDescending(h => h.ngayDongHopDong ?? h.ngayHoanThanh)
                .ToListAsync();
            var result = new List<HopDongDanhGia_ReponseDTO>();
            foreach (var contract in contracts)
            {
                var review = await _repository.LayTheoHopDongNguoiDanhGiaAsync(contract.maHD, maUser);
                var editable = review != null && DateTime.Now <= _policy.HanChinhSua(review.ngayDanhGia);
                result.Add(new HopDongDanhGia_ReponseDTO
                {
                    maHD = contract.maHD,
                    tenCongViec = contract.JobPost?.tieude ?? string.Empty,
                    tenDoiTac = contract.JobPost?.NhaTuyenDung?.maUser == maUser
                        ? contract.FreelancerStudent?.User?.hotenUser ?? "Freelancer"
                        : contract.JobPost?.NhaTuyenDung?.tencongty ?? contract.JobPost?.NhaTuyenDung?.User?.hotenUser ?? "Nhà tuyển dụng",
                    trangThaiHopDong = contract.trangThai ?? string.Empty,
                    trangThaiDanhGia = review == null ? "ChoDanhGia" : "DaDanhGia",
                    maDanhGia = review?.maDanhGia,
                    coTheDanhGia = review == null && _policy.CoTheDanhGia(contract),
                    coTheChinhSuaDanhGia = editable,
                    hanDanhGia = _policy.HanDanhGia(contract)
                });
            }
            return result;
        }

        private async Task<DanhGiaThongKe_ReponseDTO> LayThongKeAsync(int maUser, int trang, int kichThuocTrang)
        {
            trang = Math.Max(1, trang);
            kichThuocTrang = Math.Clamp(kichThuocTrang, 1, 100);
            var all = await _repository.TruyVanDanhGiaNguoiDuocDanhGia(maUser)
                .OrderByDescending(d => d.ngayDanhGia).ToListAsync();
            var count = all.Count;
            var response = new DanhGiaThongKe_ReponseDTO
            {
                diemTrungBinh = count == 0 ? 0 : Math.Round(all.Average(d => d.soSao), 2),
                tongLuotDanhGia = count,
                trungBinhChatLuong = AverageOptional(all.Select(d => d.soSaoChatLuong)),
                trungBinhDungHan = AverageOptional(all.Select(d => d.soSaoDungHan)),
                trungBinhGiaoTiep = AverageOptional(all.Select(d => d.soSaoGiaoTiep)),
                phanBoSao = Enumerable.Range(1, 5).Select(star => all.Count(d => d.soSao == star)).ToList(),
                tyLeHaiLong = count == 0 ? 0 : Math.Round(all.Count(d => d.soSao >= 4) * 100d / count, 1),
                trang = trang,
                kichThuocTrang = kichThuocTrang,
                tongSoTrang = count == 0 ? 0 : (int)Math.Ceiling(count / (double)kichThuocTrang),
                danhSach = all.Skip((trang - 1) * kichThuocTrang).Take(kichThuocTrang)
                    .Select(d => MapReview(d, d.HopDong, d.maNguoiDanhGia)).ToList()
            };
            return response;
        }

        private void ValidateRating(DanhGia_RequestDTO request)
        {
            if (request.soSaoTongThe < 1 || request.soSaoTongThe > 5)
                throw new InvalidOperationException("Vui lòng chọn số sao đánh giá tổng thể");
            if (!ValidStar(request.soSaoChatLuong) || !ValidStar(request.soSaoDungHan) || !ValidStar(request.soSaoGiaoTiep))
                throw new InvalidOperationException("Số sao tiêu chí phải nằm trong khoảng từ 1 đến 5.");
            if (request.nhanXet?.Length > 1000)
                throw new InvalidOperationException("Nhận xét không được vượt quá 1000 ký tự.");
            if (!_noiDungValidator.HopLe(request.nhanXet))
                throw new InvalidOperationException("Nội dung nhận xét chứa từ ngữ không phù hợp, vui lòng điều chỉnh lại");
        }

        private static bool ValidStar(byte? star) => !star.HasValue || star.Value is >= 1 and <= 5;

        private static double AverageOptional(IEnumerable<byte?> values)
        {
            var available = values.Where(v => v.HasValue).Select(v => (double)v!.Value).ToList();
            return available.Count == 0 ? 0 : Math.Round(available.Average(), 2);
        }

        private DanhGia_ReponseDTO MapReview(DanhGia_NhanXet review, HopDong? hopDong, int maUserNguoiDanhGia)
        {
            var isEmployer = hopDong?.JobPost?.NhaTuyenDung?.maUser == maUserNguoiDanhGia;
            var name = isEmployer
                ? hopDong?.JobPost?.NhaTuyenDung?.tencongty ?? hopDong?.JobPost?.NhaTuyenDung?.User?.hotenUser
                : hopDong?.FreelancerStudent?.User?.hotenUser;
            var user = isEmployer ? hopDong?.JobPost?.NhaTuyenDung?.User : hopDong?.FreelancerStudent?.User;
            return new DanhGia_ReponseDTO
            {
                maDanhGia = review.maDanhGia,
                maHD = review.maHD,
                tenNguoiDanhGia = name ?? review.NguoiDanhGia?.NhaTuyenDung?.tencongty ?? review.NguoiDanhGia?.hotenUser ?? string.Empty,
                avatar = user?.avatarUrl ?? review.NguoiDanhGia?.avatarUrl ?? review.NguoiDanhGia?.NhaTuyenDung?.logo,
                tenCongViec = hopDong?.JobPost?.tieude ?? string.Empty,
                soSaoTongThe = review.soSao,
                soSaoChatLuong = review.soSaoChatLuong,
                soSaoDungHan = review.soSaoDungHan,
                soSaoGiaoTiep = review.soSaoGiaoTiep,
                nhanXet = review.nhanXet,
                ngayDanhGia = review.ngayDanhGia,
                ngayCapNhat = review.ngayCapNhat,
                daChinhSua = review.soLanChinhSua > 0,
                hanChinhSua = _policy.HanChinhSua(review.ngayDanhGia),
                coTheChinhSua = DateTime.Now <= _policy.HanChinhSua(review.ngayDanhGia)
            };
        }

        private void AddNotification(int maUser, string title, string type, string link)
        {
            _context.ThongBaos.Add(new ThongBao
            {
                maUser = maUser,
                tieuDe = title,
                noiDung = title,
                loaiThongBao = type,
                linkDieuHuong = link,
                ngayTao = DateTime.Now
            });
        }

        private static string GetProfileLink(HopDong hopDong, int maUser)
        {
            var freelancer = hopDong.FreelancerStudent;
            if (freelancer?.maUser == maUser)
                return $"/FreelancerStudent/Profile/{freelancer.maFreelancerStudents}";
            var employer = hopDong.JobPost?.NhaTuyenDung;
            return employer?.maUser == maUser ? $"/NhaTuyenDung/ChiTiet/{employer.maNhaTuyenDung}" : "/";
        }
    }
}

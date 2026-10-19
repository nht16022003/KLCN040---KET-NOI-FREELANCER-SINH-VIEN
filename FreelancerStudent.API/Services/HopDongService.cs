
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class HopDongService : IHopDongService
    {
        private readonly HopDongRepository _repository;
        private readonly ApplicationDBContext _context;

        public HopDongService(
            HopDongRepository repository,
            ApplicationDBContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<HopDongResponseDTO>>
            GetDanhSachHopDongAsync(
                int maNhaTuyenDung,
                string? tuKhoa,
                string? trangThai)
        {
            // Kiểm tra mã nhà tuyển dụng
            if (maNhaTuyenDung <= 0)
            {
                throw new ArgumentException(
                    "Mã nhà tuyển dụng không hợp lệ.");
            }

            // Gọi Repository lấy danh sách hợp đồng
            var danhSach = await _repository
                .GetDanhSachHopDongAsync(
                    maNhaTuyenDung,
                    tuKhoa,
                    trangThai);

            return danhSach.Select(MapToResponseDTO).ToList();
        }

        public async Task<List<HopDongResponseDTO>>
            GetDanhSachHopDongTheoFreelancerAsync(
                int maFreelancerStudent,
                string? tuKhoa,
                string? trangThai)
        {
            if (maFreelancerStudent <= 0)
            {
                throw new ArgumentException(
                    "Mã Freelancer không hợp lệ.");
            }

            var danhSach = await _repository
                .GetDanhSachHopDongTheoFreelancerAsync(
                    maFreelancerStudent,
                    tuKhoa,
                    trangThai);

            return danhSach.Select(MapToResponseDTO).ToList();
        }

        public async Task<HopDongResponseDTO> TaoDuThaoHopDongAsync(
            TaoHopDong_RequestDTO request,
            int? maUserNTD)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Dữ liệu hợp đồng không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.maJob))
            {
                throw new ArgumentException("Mã bài tuyển dụng không được để trống.");
            }

            if (request.maFreelancerStudent <= 0)
            {
                throw new ArgumentException("Freelancer được chọn không hợp lệ.");
            }

            if (request.sotienkyquy <= 0)
            {
                throw new ArgumentException("Số tiền thù lao thỏa thuận phải lớn hơn 0.");
            }

            // Kiểm tra sự tồn tại của JobPost
            var job = await _repository.LayJobPostTheoMaAsync(request.maJob);
            if (job == null)
            {
                throw new ArgumentException("Không tìm thấy bài tuyển dụng tương ứng.");
            }

            // Kiểm tra quyền sở hữu bài đăng nếu có maUserNTD
            if (!maUserNTD.HasValue || maUserNTD.Value <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Bạn cần đăng nhập bằng tài khoản Nhà tuyển dụng.");
            }

            if (job.NhaTuyenDung == null ||
                job.NhaTuyenDung.maUser != maUserNTD.Value)
            {
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền tạo hợp đồng cho bài tuyển dụng này.");
            }

            // Tự động xóa các hợp đồng cũ đã bị từ chối / hủy của Job và Freelancer này khỏi SQL
            await _repository.XoaCacHopDongDaHuyTheoJobVaFreelancerAsync(request.maJob, request.maFreelancerStudent);

            // Kiểm tra xem Job đã có hợp đồng chưa
            var hopDongHienCo = await _repository.LayHopDongMoiNhatTheoJobVaFreelancerAsync(request.maJob, request.maFreelancerStudent);

            if (hopDongHienCo != null)
            {
                throw new InvalidOperationException(
                    $"Freelancer này đã có hợp đồng {hopDongHienCo.maHD} " +
                    $"ở trạng thái '{hopDongHienCo.trangThai}'.");
            }
            UngTuyen? donUngTuyen = null;
            if (request.maUngTuyen.HasValue && request.maUngTuyen.Value > 0)
            {
                donUngTuyen = await _context.UngTuyens
                    .FirstOrDefaultAsync(u => u.maUngTuyen == request.maUngTuyen.Value);
            }
            else
            {
                donUngTuyen = await _context.UngTuyens
                    .FirstOrDefaultAsync(u => u.maJob == request.maJob && u.maFreelancerStudent == request.maFreelancerStudent);
                if (donUngTuyen != null)
                {
                    request.maUngTuyen = donUngTuyen.maUngTuyen;
                }
            }

            if (donUngTuyen == null)
            {
                throw new InvalidOperationException("Không tìm thấy đơn ứng tuyển tương ứng với Freelancer và công việc này.");
            }

            if (donUngTuyen.maJob != request.maJob ||
                donUngTuyen.maFreelancerStudent != request.maFreelancerStudent)
            {
                throw new InvalidOperationException(
                    "Đơn ứng tuyển không thuộc công việc hoặc Freelancer đã chọn.");
            }

            if (donUngTuyen.trangThaiUngTuyen != "ChapNhan")
            {
                // Nếu đơn ứng tuyển từng bị đổi sang TuChoi do code cũ, tự động phục hồi về ChapNhan
                donUngTuyen.trangThaiUngTuyen = "ChapNhan";
                await _context.SaveChangesAsync();
            }

            // Kiểm tra Freelancer
            var freelancer = await _repository.LayFreelancerTheoMaAsync(request.maFreelancerStudent);
            if (freelancer == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin Freelancer.");
            }

            // Sinh mã hợp đồng ngẫu nhiên duy nhất (tối đa 20 ký tự)
            string maHD = $"HD{DateTime.Now:yyMMddHHmmss}{new Random().Next(10, 99)}";

            var hopDong = new HopDong
            {
                maHD = maHD,
                maJob = request.maJob,
                maFreelancerStudent = request.maFreelancerStudent,
                sotienkyquy = request.sotienkyquy,
                ngayBatDau = request.ngayBatDau ?? DateTime.Now,
                ngayKetThuc = request.ngayKetThuc,
                hinhthuclamviec = string.IsNullOrWhiteSpace(request.hinhthuclamviec) ? "Remote" : request.hinhthuclamviec,
                trangThai = "ChoXacNhan", // 2.6 Chuyển trạng thái hợp đồng thành "Chờ Freelancer xác nhận"
                dieuKhoan = request.dieuKhoan
            };

            // Tạo các mốc bàn giao (Milestones) nếu có
            var giaiDoans = new List<GiaiDoan_HopDong>();
            if (request.giaiDoans != null && request.giaiDoans.Any())
            {
                foreach (var gd in request.giaiDoans)
                {
                    if (string.IsNullOrWhiteSpace(gd.tenGiaiDoan)) continue;

                    string moTaGiaiDoan = gd.moTa ?? string.Empty;
                    if (gd.tyLeGiaiNgan.HasValue && gd.tyLeGiaiNgan.Value > 0)
                    {
                        decimal soTien = gd.soTien.HasValue && gd.soTien.Value > 0
                            ? gd.soTien.Value
                            : (request.sotienkyquy * gd.tyLeGiaiNgan.Value / 100);
                        moTaGiaiDoan = $"[Giải ngân: {gd.tyLeGiaiNgan.Value:0.#}% ~ {soTien:N0} VNĐ] {moTaGiaiDoan}".Trim();
                    }

                    giaiDoans.Add(new GiaiDoan_HopDong
                    {
                        maHD = maHD,
                        tenGiaiDoan = gd.tenGiaiDoan.Trim(),
                        moTa = moTaGiaiDoan,
                        hanChot = gd.hanChot > DateTime.MinValue ? gd.hanChot : request.ngayKetThuc,
                        trangThai = "ChuaDat",
                        ngayTao = DateTime.Now
                    });
                }
            }

            // Tạo thông báo cho Freelancer (2.6)
            ThongBao? thongBao = null;
            if (freelancer.User != null)
            {
                thongBao = new ThongBao
                {
                    maUser = freelancer.User.maUser,
                    tieuDe = "Dự thảo hợp đồng mới cần xác nhận",
                    noiDung = $"Bạn nhận được dự thảo hợp đồng cho công việc '{job.tieude}' với mức thù lao {request.sotienkyquy:N0} đ. Vui lòng vào danh sách hợp đồng để kiểm tra và xác nhận.",
                    loaiThongBao = "HopDong",
                    linkDieuHuong = "/HopDong",
                    daDoc = false,
                    ngayTao = DateTime.Now
                };
            }

            var hopDongMoi = await _repository.TaoHopDongAsync(hopDong, giaiDoans, request.maUngTuyen, thongBao);

            return MapToResponseDTO(hopDongMoi);
        }

        public async Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaAsync(string maHD)
        {
            if (string.IsNullOrWhiteSpace(maHD)) return null;

            var hopDong = await _repository.LayChiTietHopDongTheoMaAsync(maHD);
            if (hopDong == null) return null;

            var dto = MapToResponseDTO(hopDong);

            // Nếu hợp đồng đã hủy mà chưa có lý do từ chối trích xuất được từ điều khoản, tìm trong Thông báo gần nhất
            if (hopDong.trangThai == "DaHuy" && string.IsNullOrWhiteSpace(dto.lyDoTuChoi))
            {
                var tb = await _context.ThongBaos
                    .AsNoTracking()
                    .Where(t => t.loaiThongBao == "HopDong" && t.noiDung != null && t.noiDung.Contains(maHD) && t.noiDung.Contains("từ chối"))
                    .OrderByDescending(t => t.ngayTao)
                    .FirstOrDefaultAsync();

                if (tb != null && !string.IsNullOrWhiteSpace(tb.noiDung))
                {
                    int idx = tb.noiDung.IndexOf("Lý do: ", StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        dto.lyDoTuChoi = tb.noiDung.Substring(idx + 7).Trim().TrimEnd('.');
                    }
                }
            }

            var donUngTuyen = await _context.UngTuyens
                .AsNoTracking()
                .Where(u => u.maJob == hopDong.maJob && u.maFreelancerStudent == hopDong.maFreelancerStudent)
                .FirstOrDefaultAsync();
            if (donUngTuyen != null)
            {
                dto.maUngTuyen = donUngTuyen.maUngTuyen;
            }

            return dto;
        }

        public async Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaJobAsync(string maJob)
        {
            if (string.IsNullOrWhiteSpace(maJob)) return null;

            var hopDong = await _repository.LayHopDongTheoMaJobAsync(maJob);
            return hopDong != null ? MapToResponseDTO(hopDong) : null;
        }

        public async Task<bool> XacNhanHopDongAsync(
            XacNhanHopDong_RequestDTO request,
            int? maUserFreelancer)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.maHD))
            {
                throw new ArgumentException("Mã hợp đồng không hợp lệ.");
            }

            var hopDong = await _repository.LayChiTietHopDongTheoMaAsync(request.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng tương ứng.");
            }

            // Kiểm tra quyền của Freelancer nếu có truyền maUserFreelancer
            if (!maUserFreelancer.HasValue || maUserFreelancer.Value <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Bạn cần đăng nhập bằng tài khoản Freelancer.");
            }

            if (hopDong.FreelancerStudent?.User == null ||
                hopDong.FreelancerStudent.User.maUser != maUserFreelancer.Value)
            {
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền xác nhận hợp đồng này.");
            }

            // Kiểm tra trạng thái hiện tại
            if (hopDong.trangThai != "ChoXacNhan")
            {
                throw new InvalidOperationException($"Hợp đồng hiện tại đang ở trạng thái '{hopDong.trangThai}', không thể thực hiện xác nhận hoặc từ chối.");
            }

            // Trạng thái mới: nếu đồng ý thì chuyển sang ChoKyQuy, nếu từ chối thì chuyển sang DaHuy
            string trangThaiMoi = request.dongY ? "ChoKyQuy" : "DaHuy";

            // Tạo thông báo gửi cho Nhà tuyển dụng
            ThongBao? thongBao = null;
            if (hopDong.JobPost?.NhaTuyenDung?.User != null)
            {
                string tenFreelancer = hopDong.FreelancerStudent?.User?.hotenUser ?? "Freelancer";
                string tieuDeCongViec = hopDong.JobPost?.tieude ?? "công việc";
                string tieuDeTB = request.dongY ? "Freelancer đã xác nhận hợp đồng" : "Freelancer đã từ chối hợp đồng";
                string noiDungTB = request.dongY
                    ? $"Freelancer {tenFreelancer} đã đồng ý và xác nhận dự thảo hợp đồng {hopDong.maHD} cho công việc '{tieuDeCongViec}'. Vui lòng tiến hành ký quỹ để bắt đầu dự án."
                    : $"Freelancer {tenFreelancer} đã từ chối dự thảo hợp đồng {hopDong.maHD} cho công việc '{tieuDeCongViec}'" + (!string.IsNullOrWhiteSpace(request.lyDoTuChoi) ? $". Lý do: {request.lyDoTuChoi}" : ".");

                thongBao = new ThongBao
                {
                    maUser = hopDong.JobPost.NhaTuyenDung.User.maUser,
                    tieuDe = tieuDeTB,
                    noiDung = noiDungTB,
                    loaiThongBao = "HopDong",
                    linkDieuHuong = "/HopDong",
                    daDoc = false,
                    ngayTao = DateTime.Now
                };
            }

            return await _repository.CapNhatTrangThaiHopDongAsync(request.maHD, trangThaiMoi, thongBao, request.lyDoTuChoi);
        }

        public async Task<bool> KyQuyHopDongAsync(string maHD, int? maUserNhaTuyenDung)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(maHD))
            {
                throw new ArgumentException("Mã hợp đồng không hợp lệ.");
            }

            if (!maUserNhaTuyenDung.HasValue ||
                maUserNhaTuyenDung.Value <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Không xác định được tài khoản nhà tuyển dụng.");
            }

            // 2. Lấy thông tin hợp đồng
            var hopDong = await _repository
                .LayChiTietHopDongTheoMaAsync(maHD);

            if (hopDong == null)
            {
                throw new ArgumentException(
                    "Không tìm thấy hợp đồng.");
            }

            // 3. Kiểm tra quyền sở hữu hợp đồng
            int? maUserChuHopDong =
                hopDong.JobPost?.NhaTuyenDung?.User?.maUser;

            if (!maUserChuHopDong.HasValue ||
                maUserChuHopDong.Value != maUserNhaTuyenDung.Value)
            {
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền ký quỹ hợp đồng này.");
            }

            // 4. Kiểm tra trạng thái hợp đồng
            if (hopDong.trangThai != "ChoKyQuy")
            {
                throw new InvalidOperationException(
                    $"Hợp đồng đang ở trạng thái '{hopDong.trangThai}', " +
                    "không thể thực hiện ký quỹ.");
            }

            // 5. Kiểm tra số tiền ký quỹ
            decimal soTienKyQuy = hopDong.sotienkyquy ?? 0;

            if (soTienKyQuy <= 0)
            {
                throw new InvalidOperationException(
                    "Số tiền ký quỹ phải lớn hơn 0.");
            }

            // 6. Kiểm tra Freelancer nhận thông báo
            int? maUserFreelancer =
                hopDong.FreelancerStudent?.User?.maUser;

            if (!maUserFreelancer.HasValue)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy tài khoản Freelancer.");
            }

            // 7. Bắt đầu Database Transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 8. Khóa trạng thái hợp đồng bằng cập nhật có điều kiện
                // Chỉ một request được chuyển từ ChoKyQuy sang DangThucHien
                int soHopDongCapNhat =
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE HopDong
                        SET trangThai = N'DangThucHien',
                            ngayBatDau = GETDATE()
                        WHERE maHD = {maHD}
                        AND trangThai = N'ChoKyQuy'
                    ");

                if (soHopDongCapNhat != 1)
                {
                    throw new InvalidOperationException(
                        "Hợp đồng đã được ký quỹ hoặc trạng thái đã thay đổi.");
                }

                // 9. Trừ số dư khả dụng và cộng số dư đóng băng
                // Chỉ cập nhật nếu số dư thực tế vẫn đủ tiền
                int soViCapNhat =
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallet
                        SET soDuKhaDung = soDuKhaDung - {soTienKyQuy},
                            soDuDongBang = soDuDongBang + {soTienKyQuy}
                        WHERE maUser = {maUserNhaTuyenDung.Value}
                        AND soDuKhaDung >= {soTienKyQuy}
                    ");

                if (soViCapNhat != 1)
                {
                    throw new InvalidOperationException(
                        "Số dư ví không đủ để ký quỹ hoặc không tìm thấy ví.");
                }

                // 10. Lấy số dư ví sau khi ký quỹ
                var wallet = await _context.Wallets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        w => w.maUser == maUserNhaTuyenDung.Value);

                if (wallet == null)
                {
                    throw new InvalidOperationException(
                        "Không tìm thấy ví nhà tuyển dụng.");
                }

                decimal soDuSau = wallet.soDuKhaDung ?? 0;
                decimal soDuTruoc = soDuSau + soTienKyQuy;

                // 11. Ghi lịch sử giao dịch ký quỹ
                var lichSu = new LichSuGiaoDich
                {
                    maWallet = wallet.maWallet,
                    loaiGiaoDich = "KyQuyHopDong",
                    soTien = soTienKyQuy,
                    soDuTruocGD = soDuTruoc,
                    soDuSauGD = soDuSau,
                    noiDung = $"Ký quỹ hợp đồng {maHD}",
                    phuongThucThanhToan = "ViNoiBo",
                    trangThai = "ThanhCong",
                    ngayTao = DateTime.Now
                };

                await _context.LichSuGiaoDichs.AddAsync(lichSu);

                // 12. Tạo thông báo cho Freelancer
                string tenNhaTuyenDung =
                    hopDong.JobPost?.NhaTuyenDung?.tencongty
                    ?? hopDong.JobPost?.NhaTuyenDung?.User?.hotenUser
                    ?? "Nhà tuyển dụng";

                var thongBao = new ThongBao
                {
                    maUser = maUserFreelancer.Value,
                    tieuDe = "Hợp đồng đã được ký quỹ & kích hoạt",
                    noiDung =
                        $"Nhà tuyển dụng {tenNhaTuyenDung} đã ký quỹ " +
                        $"{soTienKyQuy:N0} VNĐ cho hợp đồng {maHD}. " +
                        "Bạn có thể bắt đầu thực hiện công việc.",
                    loaiThongBao = "HopDong",
                    linkDieuHuong =
                        $"/HopDong/ChiTiet?maHD={maHD}",
                    daDoc = false,
                    ngayTao = DateTime.Now
                };

                await _context.ThongBaos.AddAsync(thongBao);

                // 13. Lưu lịch sử và thông báo
                await _context.SaveChangesAsync();

                // 14. Xác nhận toàn bộ giao dịch
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                // Nếu có lỗi, hoàn tác toàn bộ thay đổi
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> XoaHopDongAsync(string maHD, int? maUserNTD)
        {
            if (string.IsNullOrWhiteSpace(maHD))
            {
                throw new ArgumentException("Mã hợp đồng không hợp lệ.");
            }

            var hopDong = await _repository.LayChiTietHopDongTheoMaAsync(maHD);
            if (hopDong == null)
            {
                return false;
            }

            if (!maUserNTD.HasValue || maUserNTD.Value <= 0)
            {
                throw new UnauthorizedAccessException("Bạn cần đăng nhập bằng tài khoản Nhà tuyển dụng.");
            }

            if (hopDong.JobPost?.NhaTuyenDung?.User == null ||
                hopDong.JobPost.NhaTuyenDung.User.maUser != maUserNTD.Value)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa hợp đồng này.");
            }

            return await _repository.XoaHopDongAsync(maHD);
        }

        private static HopDongResponseDTO MapToResponseDTO(HopDong h)
        {
            string? lyDoTuChoi = null;
            string? dieuKhoanSach = h.dieuKhoan;

            if (!string.IsNullOrWhiteSpace(h.dieuKhoan))
            {
                const string tag = "[LY_DO_TU_CHOI]:";
                int idx = h.dieuKhoan.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    lyDoTuChoi = h.dieuKhoan.Substring(idx + tag.Length).Trim();
                    dieuKhoanSach = h.dieuKhoan.Substring(0, idx).Trim();
                }
            }

            return new HopDongResponseDTO
            {
                maHD = h.maHD,
                maJob = h.maJob,
                tenCongViec = h.JobPost?.tieude ?? string.Empty,
                moTaCongViec = h.JobPost?.mota,
                maFreelancerStudent = h.maFreelancerStudent,
                tenFreelancer = h.FreelancerStudent?.User?.hotenUser ?? string.Empty,
                emailFreelancer = h.FreelancerStudent?.User?.emailUser,
                sdtFreelancer = h.FreelancerStudent?.User?.sdtUser,
                truongFreelancer = h.FreelancerStudent?.tenTruong,
                chuyenNganhFreelancer = h.FreelancerStudent?.ChuyenNganh?.tenChuyenNganh,
                maUserFreelancer = h.FreelancerStudent?.User?.maUser,
                maNhaTuyenDung = h.JobPost?.NhaTuyenDung?.maNhaTuyenDung,
                tenNhaTuyenDung = !string.IsNullOrWhiteSpace(h.JobPost?.NhaTuyenDung?.tencongty)
                    ? h.JobPost.NhaTuyenDung.tencongty
                    : (h.JobPost?.NhaTuyenDung?.User?.hotenUser ?? string.Empty),
                emailNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.emailUser,
                sdtNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.sdtUser,
                maUserNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.maUser,
                ngayBatDau = h.ngayBatDau,
                ngayKetThuc = h.ngayKetThuc,
                sotienkyquy = h.sotienkyquy,
                hinhthuclamviec = h.hinhthuclamviec,
                dieuKhoan = dieuKhoanSach,
                lyDoTuChoi = lyDoTuChoi,
                trangThai = h.trangThai ?? string.Empty,
                giaiDoans = h.GiaiDoan_HopDongs?.Select(g => new GiaiDoanHopDong_ResponseDTO
                {
                    maGiaiDoan = g.maGiaiDoan,
                    maHD = g.maHD,
                    tenGiaiDoan = g.tenGiaiDoan,
                    moTa = g.moTa,
                    hanChot = g.hanChot,
                    trangThai = g.trangThai ?? "ChuaDat"
                }).ToList() ?? new List<GiaiDoanHopDong_ResponseDTO>()
            };
        }
    }
}
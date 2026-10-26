using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FreelancerStudent.API.Services
{
    public class NghiepVuNenHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NghiepVuNenHostedService> _logger;
        private readonly NghiepVuOptions _options;

        public NghiepVuNenHostedService(IServiceScopeFactory scopeFactory,
            ILogger<NghiepVuNenHostedService> logger, IOptions<NghiepVuOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ProcessCycleAsync(stoppingToken);
            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                    await ProcessCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
        }

        private async Task ProcessCycleAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var now = DateTime.Now;

            try
            {
                await GuiNhacThanhToanAsync(context, now, cancellationToken);
                await GuiNhacDanhGiaAsync(context, now, cancellationToken);
                await TuDongGiaiNganAsync(scope.ServiceProvider, context, now, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý chu kỳ nghiệp vụ nền.");
            }
        }

        private async Task GuiNhacThanhToanAsync(ApplicationDBContext context, DateTime now, CancellationToken token)
        {
            var contracts = await context.HopDongs.AsNoTracking()
                .Include(h => h.JobPost).ThenInclude(j => j!.NhaTuyenDung)
                .Where(h => h.trangThai == "DaHoanThanh" && h.ngayHoanThanh.HasValue)
                .ToListAsync(token);
            foreach (var contract in contracts)
            {
                var nhacLuc = contract.ngayHoanThanh!.Value.AddDays(Math.Max(0, _options.SoNgayTuDongGiaiNgan - 2));
                if (now.Date != nhacLuc.Date || contract.JobPost?.NhaTuyenDung == null)
                    continue;
                var coTranhChap = await context.TranhChaps.AsNoTracking()
                    .AnyAsync(t => t.maHD == contract.maHD && t.ngayGiaiQuyet == null, token);
                if (!coTranhChap)
                    await TaoThongBaoNeuChuaCoAsync(context, contract.JobPost.NhaTuyenDung.maUser,
                        "Nhắc thanh toán hợp đồng", $"Hợp đồng {contract.maHD} đang chờ thanh toán cho Freelancer.",
                        "NhacThanhToan", $"/HopDong/ChiTiet?maHD={Uri.EscapeDataString(contract.maHD)}", nhacLuc, now, token);
            }
        }

        private async Task TuDongGiaiNganAsync(IServiceProvider services, ApplicationDBContext context, DateTime now, CancellationToken token)
        {
            var han = now.AddDays(-_options.SoNgayTuDongGiaiNgan);
            var contracts = await context.HopDongs.AsNoTracking()
                .Where(h => h.trangThai == "DaHoanThanh" && h.ngayHoanThanh <= han)
                .Select(h => h.maHD)
                .ToListAsync(token);
            var service = services.GetRequiredService<IHopDongService>();
            foreach (var maHD in contracts)
            {
                if (await context.TranhChaps.AsNoTracking().AnyAsync(t => t.maHD == maHD && t.ngayGiaiQuyet == null, token))
                    continue;
                try
                {
                    await service.ThanhToanCoreAsync(maHD, null, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Không thể tự động giải ngân hợp đồng {MaHD}.", maHD);
                }
            }
        }

        private async Task GuiNhacDanhGiaAsync(ApplicationDBContext context, DateTime now, CancellationToken token)
        {
            var contracts = await context.HopDongs.AsNoTracking()
                .Include(h => h.JobPost).ThenInclude(j => j!.NhaTuyenDung)
                .Include(h => h.FreelancerStudent)
                .Where(h => (h.trangThai == "DaThanhToan" || h.trangThai == "DaHuyHoanTien") && h.ngayDongHopDong.HasValue)
                .ToListAsync(token);
            foreach (var contract in contracts)
            {
                var employerUser = contract.JobPost?.NhaTuyenDung?.maUser;
                var freelancerUser = contract.FreelancerStudent?.maUser;
                if (!employerUser.HasValue || !freelancerUser.HasValue)
                    continue;

                foreach (var days in _options.NgayNhacDanhGia.Distinct())
                {
                    var reminderDate = contract.ngayDongHopDong!.Value.AddDays(days);
                    if (now.Date != reminderDate.Date || now > contract.ngayDongHopDong.Value.AddDays(_options.SoNgayChoDanhGia))
                        continue;
                    foreach (var userId in new[] { employerUser.Value, freelancerUser.Value })
                    {
                        var hasReview = await context.DanhGia_NhanXets.AsNoTracking()
                            .AnyAsync(d => d.maHD == contract.maHD && d.maNguoiDanhGia == userId, token);
                        if (!hasReview)
                            await TaoThongBaoNeuChuaCoAsync(context, userId, "Nhắc đánh giá hợp đồng",
                                $"Bạn chưa gửi đánh giá cho hợp đồng {contract.maHD}.", "NhacDanhGia",
                                $"/HopDong/ChiTiet?maHD={Uri.EscapeDataString(contract.maHD)}", reminderDate, now, token);
                    }
                }
            }
        }

        private static async Task TaoThongBaoNeuChuaCoAsync(ApplicationDBContext context, int maUser, string tieuDe,
            string noiDung, string loai, string link, DateTime moc, DateTime now, CancellationToken token)
        {
            var ngay = moc.Date;
            var daGui = await context.ThongBaos.AsNoTracking().AnyAsync(t =>
                t.maUser == maUser && t.loaiThongBao == loai && t.linkDieuHuong == link &&
                t.ngayTao >= ngay && t.ngayTao < ngay.AddDays(1), token);
            if (daGui)
                return;
            context.ThongBaos.Add(new ThongBao
            {
                maUser = maUser,
                tieuDe = tieuDe,
                noiDung = noiDung,
                loaiThongBao = loai,
                linkDieuHuong = link,
                daDoc = false,
                ngayTao = now
            });
            await context.SaveChangesAsync(token);
        }
    }
}

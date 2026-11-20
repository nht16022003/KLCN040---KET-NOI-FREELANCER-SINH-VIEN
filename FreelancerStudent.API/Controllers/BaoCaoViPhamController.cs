using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BaoCaoViPhamController : ControllerBase
    {
        private const decimal DiemUyTinBanDau = 5m;
        private static readonly HashSet<string> KetLuans = new(StringComparer.OrdinalIgnoreCase)
        {
            "CHAP_NHAN_GIAI_TRINH", "XAC_DINH_VI_PHAM", "YEU_CAU_KHAC_PHUC", "GIA_HAN_HOP_DONG", "CHUYEN_TRANH_CHAP"
        };
        private static readonly HashSet<string> FileExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".zip", ".rar", ".7z"
        };

        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public BaoCaoViPhamController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> DanhSach()
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            IQueryable<BaoCaoViPhamHopDong> query = _context.BaoCaoViPhamHopDongs
                .Include(x => x.TepMinhChungs)
                .Include(x => x.FreelancerStudent).ThenInclude(x => x!.User)
                .Include(x => x.NhaTuyenDungUser);

            if (role != "Admin")
            {
                if (role == "FreelancerStudent")
                {
                    var freelancerId = await _context.FreelancerStudents.Where(x => x.maUser == maUser)
                        .Select(x => (int?)x.maFreelancerStudents).FirstOrDefaultAsync();
                    if (!freelancerId.HasValue) return Ok(new { success = true, data = Array.Empty<object>() });
                    query = query.Where(x => x.MaFreelancerStudent == freelancerId.Value);
                }
                else
                {
                    query = query.Where(x => x.MaNhaTuyenDungUser == maUser);
                }
            }

            var cases = await query.OrderByDescending(x => x.NgayBaoCao).ToListAsync();
            return Ok(new { success = true, data = cases.Select(MapCase) });
        }

        [HttpGet("lich-su-diem")]
        public async Task<IActionResult> LichSuDiemUyTin()
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            if (role != "FreelancerStudent") return Forbid();
            var freelancer = await _context.FreelancerStudents.FirstOrDefaultAsync(x => x.maUser == maUser);
            if (freelancer == null) return NotFound(new { success = false, message = "Không tìm thấy hồ sơ Freelancer." });
            var history = await _context.LichSuDiemUyTins.AsNoTracking()
                .Where(x => x.MaFreelancerStudent == freelancer.maFreelancerStudents)
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new { x.MaLichSu, x.MaBaoCao, x.LoaiThayDoi, x.LyDo, x.SoDiemThayDoi, x.DiemTruoc, x.DiemSau, x.NgayTao })
                .ToListAsync();
            return Ok(new { success = true, data = new { freelancer.diemUyTin, freelancer.soLanViPham, LichSu = history } });
        }

        [HttpGet("{maBaoCao:int}")]
        public async Task<IActionResult> ChiTiet(int maBaoCao)
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            var report = await LayBaoCaoAsync(maBaoCao);
            if (report == null) return NotFound(new { success = false, message = "Không tìm thấy báo cáo." });
            if (!CoQuyenXem(report, maUser, role)) return Forbid();

            var appeal = await _context.KhieuNaiViPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
            var contract = await _context.HopDongs.AsNoTracking()
                .Include(x => x.JobPost).ThenInclude(x => x!.NhaTuyenDung).ThenInclude(x => x!.User)
                .Include(x => x.FreelancerStudent).ThenInclude(x => x!.User)
                .FirstOrDefaultAsync(x => x.maHD == report.MaHD);
            if (contract == null) return NotFound(new { success = false, message = "Không tìm thấy hợp đồng." });

            var tasks = await _context.Task_CongViecs.AsNoTracking().Where(x => x.maHD == report.MaHD)
                .Select(x => new { x.maTask, x.tenTask, x.moTa, x.hanHoanThanh, x.trangThai, x.ngayCapNhat }).ToListAsync();
            var milestones = await _context.GiaiDoan_HopDongs.AsNoTracking().Where(x => x.maHD == report.MaHD)
                .Select(x => new { x.maGiaiDoan, x.tenGiaiDoan, x.moTa, x.hanChot, x.trangThai }).ToListAsync();
            var handoffs = await _context.BanGiao_SanPhams.AsNoTracking().Where(x => x.maHD == report.MaHD)
                .Select(x => new { x.maBanGiao, x.phienBan, x.fileSanPhamUrl, x.linkDemo, x.motaBanGiao, x.ngayNop, x.trangThai, x.phanHoi }).ToListAsync();
            var roomIds = await _context.PhongChats.AsNoTracking()
                .Where(x => x.maFreelancerStudent == report.MaFreelancerStudent && x.maJob == contract.maJob)
                .Select(x => x.maPhongChat).ToListAsync();
            var messages = await _context.TinNhanChats.AsNoTracking().Where(x => roomIds.Contains(x.maPhongChat))
                .OrderBy(x => x.ngayGui).Select(x => new { x.maNguoiGui, x.noiDung, x.fileDinhKem, x.loaiTinNhan, x.ngayGui }).ToListAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    BaoCao = MapCase(report),
                    KhieuNai = appeal,
                    HopDong = new
                    {
                        contract.maHD,
                        contract.maJob,
                        TenCongViec = contract.JobPost?.tieude,
                        contract.trangThai,
                        contract.ngayBatDau,
                        contract.ngayKetThuc,
                        contract.dieuKhoan,
                        contract.sotienkyquy,
                        TenFreelancer = contract.FreelancerStudent?.User?.hotenUser,
                        TenNhaTuyenDung = contract.JobPost?.NhaTuyenDung?.tencongty ?? contract.JobPost?.NhaTuyenDung?.User?.hotenUser
                    },
                    Tasks = tasks,
                    LoTrinhBanGiao = milestones,
                    SanPhamDaBanGiao = handoffs,
                    LichSuTraoDoi = messages
                }
            });
        }

        [HttpPost("tao")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> TaoBaoCao([FromForm] TaoBaoCaoRequest request)
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            if (role != "NhaTuyenDung") return Forbid();
            if (!ModelState.IsValid) return BadRequest(new { success = false, message = "Thông tin báo cáo không hợp lệ." });
            var contract = await _context.HopDongs.Include(x => x.JobPost).ThenInclude(x => x!.NhaTuyenDung)
                .Include(x => x.FreelancerStudent).FirstOrDefaultAsync(x => x.maHD == request.MaHD);
            if (contract?.JobPost?.NhaTuyenDung?.maUser != maUser || contract.FreelancerStudent == null)
                return NotFound(new { success = false, message = "Không tìm thấy hợp đồng thuộc nhà tuyển dụng này." });
            if (contract.trangThai is "DaHuy" or "DaHuyHoanTien")
                return BadRequest(new { success = false, message = "Không thể báo cáo hợp đồng đã hủy." });
            if (await _context.BaoCaoViPhamHopDongs.AnyAsync(x => x.MaHD == request.MaHD && x.TrangThai != "DA_GIAI_QUYET" && x.TrangThai != "CHUYEN_TRANH_CHAP"))
                return Conflict(new { success = false, message = "Hợp đồng đang có báo cáo vi phạm chưa được xử lý." });
            if (!FilesValid(request.TepMinhChungs)) return BadRequest(new { success = false, message = "Tệp minh chứng không hợp lệ (tối đa 5 tệp, mỗi tệp 20 MB)." });

            var report = new BaoCaoViPhamHopDong
            {
                MaHD = contract.maHD,
                MaNhaTuyenDungUser = maUser,
                MaFreelancerStudent = contract.maFreelancerStudent,
                LoaiViPham = request.LoaiViPham.Trim(),
                MoTa = request.MoTa.Trim(),
                ThoiGianViPham = request.ThoiGianViPham,
                NgayBaoCao = DateTime.UtcNow,
                HanGiaiTrinh = DateTime.UtcNow.AddDays(3),
                TrangThai = "CHO_TIEP_NHAN"
            };
            _context.BaoCaoViPhamHopDongs.Add(report);
            await _context.SaveChangesAsync();
            await SaveEvidenceAsync(report, request.TepMinhChungs, maUser, "BAO_CAO");
            var evidenceText = request.TepMinhChungs.Count == 0 ? "Không có tệp đính kèm." : "Minh chứng: " + string.Join(", ", request.TepMinhChungs.Select(x => Path.GetFileName(x.FileName)));
            AddNotification(contract.FreelancerStudent.maUser, "Bạn được báo cáo vi phạm hợp đồng", $"Hợp đồng {contract.maHD}; nội dung: {report.MoTa}; thời gian vi phạm: {report.ThoiGianViPham?.ToString("dd/MM/yyyy HH:mm") ?? "không nêu"}; {evidenceText} Hạn gửi giải trình: {report.HanGiaiTrinh:dd/MM/yyyy HH:mm}.", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã gửi báo cáo vi phạm. Freelancer có 3 ngày để giải trình.", data = new { report.MaBaoCao, report.TrangThai, report.HanGiaiTrinh } });
        }

        [HttpPost("{maBaoCao:int}/giai-trinh")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> GuiGiaiTrinh(int maBaoCao, [FromForm] GiaiTrinhRequest request)
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            var report = await _context.BaoCaoViPhamHopDongs.Include(x => x.FreelancerStudent)
                .FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
            if (report == null) return NotFound(new { success = false, message = "Không tìm thấy báo cáo." });
            if (role != "FreelancerStudent" || report.FreelancerStudent?.maUser != maUser) return Forbid();
            if (report.TrangThai != "CHO_TIEP_NHAN" || DateTime.UtcNow > report.HanGiaiTrinh)
                return BadRequest(new { success = false, message = "Đã hết hạn hoặc báo cáo không còn chờ giải trình." });
            if (!ModelState.IsValid || !FilesValid(request.TepMinhChungs)) return BadRequest(new { success = false, message = "Nội dung hoặc tệp giải trình không hợp lệ." });

            report.FreelancerDongY = request.DongYNoiDung;
            report.GiaiTrinh = request.NoiDung.Trim();
            report.FreelancerDongYGiaHan = request.DongYGiaHan;
            report.NgayGiaiTrinh = DateTime.UtcNow;
            report.TrangThai = "CHO_ADMIN";
            await SaveEvidenceAsync(report, request.TepMinhChungs, maUser, "GIAI_TRINH");
            AddNotification(report.MaNhaTuyenDungUser, "Freelancer đã gửi giải trình", $"Freelancer đã giải trình báo cáo hợp đồng {report.MaHD}. Bạn có thể bổ sung ý kiến hoặc minh chứng.", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã gửi giải trình và chuyển hồ sơ cho Admin.", data = MapCase(report) });
        }

        [HttpPost("{maBaoCao:int}/phan-hoi")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PhanHoiNhaTuyenDung(int maBaoCao, [FromForm] PhanHoiNhaTuyenDungRequest request)
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            var report = await _context.BaoCaoViPhamHopDongs.FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
            if (report == null) return NotFound(new { success = false, message = "Không tìm thấy báo cáo." });
            if (role != "NhaTuyenDung" || report.MaNhaTuyenDungUser != maUser) return Forbid();
            if (report.TrangThai != "CHO_ADMIN" || report.NgayXuLy.HasValue)
                return BadRequest(new { success = false, message = "Hồ sơ không còn nhận phản hồi." });
            if (!ModelState.IsValid || !FilesValid(request.TepMinhChungs)) return BadRequest(new { success = false, message = "Nội dung hoặc tệp phản hồi không hợp lệ." });

            report.PhanHoiNhaTuyenDung = request.NoiDung.Trim();
            report.NhaTuyenDungDongYGiaHan = request.DongYGiaHan;
            report.NgayPhanHoiNhaTuyenDung = DateTime.UtcNow;
            await SaveEvidenceAsync(report, request.TepMinhChungs, maUser, "PHAN_HOI_NTD");
            AddNotification((await _context.FreelancerStudents.Where(x => x.maFreelancerStudents == report.MaFreelancerStudent).Select(x => x.maUser).FirstAsync()), "Nhà tuyển dụng đã phản hồi báo cáo", $"Nhà tuyển dụng đã bổ sung ý kiến cho báo cáo hợp đồng {report.MaHD}.", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã gửi ý kiến bổ sung." });
        }

        [HttpPost("{maBaoCao:int}/ket-luan")]
        public async Task<IActionResult> KetLuanAdmin(int maBaoCao, [FromBody] KetLuanBaoCaoRequest request)
        {
            if (!TryGetUser(out var maAdminUser, out var role)) return Unauthorized();
            if (role != "Admin") return Forbid();
            if (!ModelState.IsValid || !KetLuans.Contains(request.KetLuan)) return BadRequest(new { success = false, message = "Kết luận xử lý không hợp lệ." });
            if (request.KetLuan is "XAC_DINH_VI_PHAM" or "YEU_CAU_KHAC_PHUC" &&
                request.MucDoViPham is not ("NHE" or "TRUNG_BINH" or "NGHIEM_TRONG"))
                return BadRequest(new { success = false, message = "Vui lòng chọn mức độ vi phạm." });
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var report = await _context.BaoCaoViPhamHopDongs.Include(x => x.FreelancerStudent).ThenInclude(x => x!.User)
                    .Include(x => x.TepMinhChungs).FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
                if (report == null) return NotFound(new { success = false, message = "Không tìm thấy báo cáo." });
                if (report.NgayXuLy.HasValue || report.TrangThai == "CHUYEN_TRANH_CHAP") return Conflict(new { success = false, message = "Báo cáo đã được kết luận." });
                if (request.KetLuan == "GIA_HAN_HOP_DONG" && (!report.FreelancerDongYGiaHan || !report.NhaTuyenDungDongYGiaHan || request.SoNgayGiaHan <= 0))
                    return BadRequest(new { success = false, message = "Chỉ được gia hạn khi cả hai bên đồng ý và có số ngày gia hạn hợp lệ." });

                report.KetLuan = request.KetLuan;
                report.MucDoViPham = request.MucDoViPham;
                report.GhiChuAdmin = request.GhiChu?.Trim();
                report.MaAdminUser = maAdminUser;
                report.NgayXuLy = DateTime.UtcNow;
                report.TrangThai = request.KetLuan == "CHUYEN_TRANH_CHAP" ? "CHUYEN_TRANH_CHAP" : "DA_GIAI_QUYET";

                if (request.KetLuan is "XAC_DINH_VI_PHAM" or "YEU_CAU_KHAC_PHUC")
                    await ApplyViolationPenaltyAsync(report, maAdminUser);
                else
                {
                    var scoreBefore = report.FreelancerStudent!.diemUyTin;
                    await CapNhatDiemUyTinAsync(report.FreelancerStudent);
                    if (scoreBefore != report.FreelancerStudent.diemUyTin)
                        AddScoreHistory(report.FreelancerStudent, report, maAdminUser, "CAP_NHAT_DANH_GIA", "Cập nhật điểm từ các đánh giá đủ điều kiện", report.FreelancerStudent.diemUyTin - scoreBefore, scoreBefore, report.FreelancerStudent.diemUyTin);
                }

                var contract = await _context.HopDongs.FirstOrDefaultAsync(x => x.maHD == report.MaHD);
                if (request.KetLuan == "GIA_HAN_HOP_DONG" && contract != null)
                    contract.ngayKetThuc = (contract.ngayKetThuc ?? DateTime.UtcNow).AddDays(request.SoNgayGiaHan);
                if (request.KetLuan == "CHUYEN_TRANH_CHAP" && contract != null)
                {
                    contract.trangThai = "DangTranhChap";
                    if (!await _context.TranhChaps.AnyAsync(x => x.maHD == report.MaHD && x.ngayGiaiQuyet == null))
                        _context.TranhChaps.Add(new TranhChap { maHD = report.MaHD });
                }

                var resultText = request.GhiChu?.Trim() ?? request.KetLuan;
                AddNotification(report.FreelancerStudent!.maUser, "Kết quả xử lý báo cáo vi phạm", $"Báo cáo hợp đồng {report.MaHD}: {request.KetLuan}. {resultText} Điểm uy tín hiện tại: {report.FreelancerStudent.diemUyTin:0.##}.", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
                AddNotification(report.MaNhaTuyenDungUser, "Kết quả xử lý báo cáo vi phạm", $"Báo cáo hợp đồng {report.MaHD} đã được Admin xử lý: {request.KetLuan}. {resultText}", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { success = true, message = "Đã lưu kết luận và cập nhật điểm uy tín.", data = MapCase(report) });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{maBaoCao:int}/khieu-nai")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> GuiKhieuNai(int maBaoCao, [FromForm] GuiKhieuNaiRequest request)
        {
            if (!TryGetUser(out var maUser, out var role)) return Unauthorized();
            if (role != "FreelancerStudent") return Forbid();
            if (!ModelState.IsValid || !FilesValid(request.TepMinhChungs)) return BadRequest(new { success = false, message = "Nội dung hoặc tệp khiếu nại không hợp lệ." });
            var report = await _context.BaoCaoViPhamHopDongs.Include(x => x.FreelancerStudent)
                .FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
            if (report?.FreelancerStudent?.maUser != maUser) return NotFound(new { success = false, message = "Không tìm thấy báo cáo thuộc tài khoản này." });
            if (report.KetLuan is not ("XAC_DINH_VI_PHAM" or "YEU_CAU_KHAC_PHUC") || await _context.KhieuNaiViPhams.AnyAsync(x => x.MaBaoCao == maBaoCao))
                return BadRequest(new { success = false, message = "Báo cáo chưa đủ điều kiện khiếu nại hoặc đã có khiếu nại." });

            var appeal = new KhieuNaiViPham
            {
                MaBaoCao = maBaoCao,
                MaFreelancerStudent = report.MaFreelancerStudent,
                NoiDung = request.NoiDung.Trim(),
                NgayTao = DateTime.UtcNow
            };
            _context.KhieuNaiViPhams.Add(appeal);
            await SaveEvidenceAsync(report, request.TepMinhChungs, maUser, "KHIEU_NAI");
            AddNotification(0, "Khiếu nại vi phạm mới", $"Freelancer đã khiếu nại kết quả xử lý báo cáo hợp đồng {report.MaHD}.", $"/Admin/BaoCaoViPham/ChiTiet/{maBaoCao}", true);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã gửi khiếu nại đến Admin.", data = appeal });
        }

        [HttpGet("khieu-nai")]
        public async Task<IActionResult> DanhSachKhieuNai()
        {
            if (!TryGetUser(out _, out var role)) return Unauthorized();
            if (role != "Admin") return Forbid();
            var appeals = await _context.KhieuNaiViPhams.AsNoTracking()
                .Where(x => x.TrangThai == "CHO_XU_LY").OrderBy(x => x.NgayTao)
                .Select(x => new
                {
                    x.MaKhieuNai,
                    x.MaBaoCao,
                    x.MaFreelancerStudent,
                    x.NoiDung,
                    x.TrangThai,
                    x.KetQuaAdmin,
                    x.MaAdminUser,
                    x.NgayTao,
                    x.NgayXuLy,
                    BaoCao = new { MaHD = x.BaoCao!.MaHD },
                    FreelancerStudent = new { tenUser = x.FreelancerStudent!.User!.hotenUser }
                }).ToListAsync();
            return Ok(new { success = true, data = appeals });
        }

        [HttpPost("khieu-nai/{maKhieuNai:int}/ket-luan")]
        public async Task<IActionResult> KetLuanKhieuNai(int maKhieuNai, [FromBody] KetLuanKhieuNaiRequest request)
        {
            if (!TryGetUser(out var maAdminUser, out var role)) return Unauthorized();
            if (role != "Admin") return Forbid();
            if (!ModelState.IsValid || request.ChapNhan == null) return BadRequest(new { success = false, message = "Kết quả khiếu nại không hợp lệ." });
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var appeal = await _context.KhieuNaiViPhams.Include(x => x.BaoCao).Include(x => x.FreelancerStudent).ThenInclude(x => x!.User)
                .FirstOrDefaultAsync(x => x.MaKhieuNai == maKhieuNai);
            if (appeal == null) return NotFound(new { success = false, message = "Không tìm thấy khiếu nại." });
            if (appeal.TrangThai != "CHO_XU_LY" || appeal.BaoCao == null) return Conflict(new { success = false, message = "Khiếu nại đã được xử lý." });
            try
            {
                appeal.TrangThai = request.ChapNhan.Value ? "CHAP_NHAN" : "TU_CHOI";
                appeal.KetQuaAdmin = request.KetQua.Trim();
                appeal.MaAdminUser = maAdminUser;
                appeal.NgayXuLy = DateTime.UtcNow;
                if (request.ChapNhan.Value)
                {
                    var report = appeal.BaoCao;
                    var freelancer = appeal.FreelancerStudent!;
                    var pointDeduction = report.SoDiemTru ?? 0m;
                    if (pointDeduction > 0)
                    {
                        var before = freelancer.diemUyTin;
                        freelancer.soLanViPham = Math.Max(0, freelancer.soLanViPham - 1);
                        AddScoreHistory(freelancer, report, maAdminUser, "HOAN_DIEM_KHIEU_NAI", "Khiếu nại được chấp nhận: " + request.KetQua.Trim(), -pointDeduction, before, before + pointDeduction);
                        await _context.SaveChangesAsync();
                        await CapNhatDiemUyTinAsync(freelancer);
                    }
                    report.KetLuan = "KHIEU_NAI_CHAP_NHAN";
                    if (freelancer.User?.status == "BiKhoa" && freelancer.soLanViPham < 3)
                        freelancer.User.status = "ACTIVE";
                    AddNotification(freelancer.maUser, "Khiếu nại được chấp nhận", $"Admin đã chấp nhận khiếu nại báo cáo {report.MaHD}. {request.KetQua}", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
                    AddNotification(report.MaNhaTuyenDungUser, "Khiếu nại đã có kết quả", $"Admin đã chấp nhận khiếu nại của Freelancer đối với báo cáo {report.MaHD}.", $"/BaoCaoViPham/ChiTiet/{report.MaBaoCao}");
                }
                else
                {
                    AddNotification(appeal.FreelancerStudent!.maUser, "Khiếu nại bị từ chối", $"Admin đã từ chối khiếu nại báo cáo {appeal.BaoCao.MaHD}. {request.KetQua}", $"/BaoCaoViPham/ChiTiet/{appeal.BaoCao.MaBaoCao}");
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { success = true, message = "Đã lưu kết quả khiếu nại.", data = appeal });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task ApplyViolationPenaltyAsync(BaoCaoViPhamHopDong report, int maAdminUser)
        {
            var freelancer = report.FreelancerStudent!;
            await CapNhatDiemUyTinAsync(freelancer);
            var scoreBefore = freelancer.diemUyTin;
            var strike = freelancer.soLanViPham + 1;
            var deduction = strike switch
            {
                1 => 1m,
                2 => Math.Round(scoreBefore / 2m, 2),
                _ => scoreBefore
            };
            freelancer.soLanViPham = strike;
            freelancer.diemUyTin = strike >= 3 ? 0m : Math.Max(0m, scoreBefore - deduction);
            if (strike >= 3 && freelancer.User != null)
                freelancer.User.status = "BiKhoa";
            report.SoLanViPham = strike;
            report.DiemTruocXuLy = scoreBefore;
            report.SoDiemTru = deduction;
            report.DiemSauXuLy = freelancer.diemUyTin;
            AddScoreHistory(freelancer, report, maAdminUser, "TRU_VI_PHAM", $"Vi phạm lần {strike}: {report.LoaiViPham}", deduction, scoreBefore, freelancer.diemUyTin);
        }

        private async Task CapNhatDiemUyTinAsync(FreelancerStudents freelancer)
        {
            var averageRating = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(x => x.maNguoiDuocDanhGia == freelancer.maUser && x.soSao >= 4)
                .Select(x => (decimal?)x.soSao).AverageAsync() ?? 0m;
            var totalPenalty = await _context.LichSuDiemUyTins.AsNoTracking()
                .Where(x => x.MaFreelancerStudent == freelancer.maFreelancerStudents && (x.LoaiThayDoi == "TRU_VI_PHAM" || x.LoaiThayDoi == "HOAN_DIEM_KHIEU_NAI"))
                .SumAsync(x => (decimal?)x.SoDiemThayDoi) ?? 0m;
            freelancer.diemUyTin = freelancer.soLanViPham >= 3
                ? 0m
                : Math.Max(0m, DiemUyTinBanDau + averageRating - totalPenalty);
        }

        private void AddScoreHistory(FreelancerStudents freelancer, BaoCaoViPhamHopDong? report, int actor, string kind, string reason, decimal change, decimal before, decimal after)
        {
            _context.LichSuDiemUyTins.Add(new LichSuDiemUyTin
            {
                MaFreelancerStudent = freelancer.maFreelancerStudents,
                MaBaoCao = report?.MaBaoCao,
                MaNguoiThucHien = actor,
                LoaiThayDoi = kind,
                LyDo = reason,
                SoDiemThayDoi = change,
                DiemTruoc = before,
                DiemSau = after,
                NgayTao = DateTime.UtcNow
            });
        }

        private async Task SaveEvidenceAsync(BaoCaoViPhamHopDong report, IList<IFormFile> files, int maUser, string type)
        {
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var relativeFolder = Path.Combine("uploads", "baocao-vi-pham", DateTime.UtcNow.ToString("yyyyMM"));
                var apiFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", relativeFolder);
                Directory.CreateDirectory(apiFolder);
                var safeName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
                var apiPath = Path.Combine(apiFolder, safeName);
                await using (var stream = System.IO.File.Create(apiPath)) await file.CopyToAsync(stream);
                try
                {
                    var webFolder = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", relativeFolder));
                    Directory.CreateDirectory(webFolder);
                    System.IO.File.Copy(apiPath, Path.Combine(webFolder, safeName), true);
                }
                catch { }
                _context.TepMinhChungViPhams.Add(new TepMinhChungViPham
                {
                    MaBaoCao = report.MaBaoCao,
                    MaNguoiTai = maUser,
                    LoaiTep = type,
                    TenTep = Path.GetFileName(file.FileName)[..Math.Min(Path.GetFileName(file.FileName).Length, 255)],
                    DuongDan = "/" + Path.Combine(relativeFolder, safeName).Replace('\\', '/'),
                    NgayTai = DateTime.UtcNow
                });
            }
        }

        private bool FilesValid(IList<IFormFile>? files)
        {
            return files == null || (files.Count <= 5 && files.All(file => file.Length > 0 && file.Length <= 20 * 1024 * 1024 && FileExtensions.Contains(Path.GetExtension(file.FileName))));
        }

        private async Task<BaoCaoViPhamHopDong?> LayBaoCaoAsync(int maBaoCao)
        {
            return await _context.BaoCaoViPhamHopDongs.AsNoTracking()
                .Include(x => x.TepMinhChungs)
                .Include(x => x.FreelancerStudent).ThenInclude(x => x!.User)
                .Include(x => x.NhaTuyenDungUser)
                .FirstOrDefaultAsync(x => x.MaBaoCao == maBaoCao);
        }

        private static bool CoQuyenXem(BaoCaoViPhamHopDong report, int maUser, string role)
        {
            return role == "Admin" || (role == "FreelancerStudent" && report.FreelancerStudent?.maUser == maUser) ||
                   (role == "NhaTuyenDung" && report.MaNhaTuyenDungUser == maUser);
        }

        private static object MapCase(BaoCaoViPhamHopDong report) => new
        {
            report.MaBaoCao,
            report.MaHD,
            report.MaNhaTuyenDungUser,
            report.MaFreelancerStudent,
            TenFreelancer = report.FreelancerStudent?.User?.hotenUser,
            TenNhaTuyenDung = report.NhaTuyenDungUser?.hotenUser,
            report.LoaiViPham,
            report.MoTa,
            report.ThoiGianViPham,
            report.NgayBaoCao,
            report.HanGiaiTrinh,
            report.TrangThai,
            report.FreelancerDongY,
            report.GiaiTrinh,
            report.NgayGiaiTrinh,
            report.PhanHoiNhaTuyenDung,
            report.NgayPhanHoiNhaTuyenDung,
            report.FreelancerDongYGiaHan,
            report.NhaTuyenDungDongYGiaHan,
            report.KetLuan,
            report.MucDoViPham,
            report.GhiChuAdmin,
            report.NgayXuLy,
            report.SoLanViPham,
            report.SoDiemTru,
            report.DiemTruocXuLy,
            report.DiemSauXuLy,
            TepMinhChungs = report.TepMinhChungs.Select(x => new { x.MaTep, x.MaNguoiTai, x.LoaiTep, x.TenTep, x.DuongDan, x.NgayTai })
        };

        private void AddNotification(int maUser, string title, string body, string link, bool allAdmins = false)
        {
            var recipientIds = allAdmins
                ? _context.Admins.Select(x => x.maUser).ToList()
                : new List<int> { maUser };
            foreach (var recipient in recipientIds)
                _context.ThongBaos.Add(new ThongBao { maUser = recipient, tieuDe = title, noiDung = body.Length > 1000 ? body[..1000] : body, loaiThongBao = "BaoCaoViPham", linkDieuHuong = link });
        }

        private bool TryGetUser(out int maUser, out string role)
        {
            role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out maUser) && maUser > 0;
        }

        public class TaoBaoCaoRequest
        {
            [Required] public string MaHD { get; set; } = string.Empty;
            [Required, MaxLength(100)] public string LoaiViPham { get; set; } = string.Empty;
            [Required, MaxLength(2000)] public string MoTa { get; set; } = string.Empty;
            public DateTime? ThoiGianViPham { get; set; }
            public List<IFormFile> TepMinhChungs { get; set; } = new();
        }

        public class GiaiTrinhRequest
        {
            public bool DongYNoiDung { get; set; }
            [Required, MaxLength(2000)] public string NoiDung { get; set; } = string.Empty;
            public bool DongYGiaHan { get; set; }
            public List<IFormFile> TepMinhChungs { get; set; } = new();
        }

        public class PhanHoiNhaTuyenDungRequest
        {
            [Required, MaxLength(2000)] public string NoiDung { get; set; } = string.Empty;
            public bool DongYGiaHan { get; set; }
            public List<IFormFile> TepMinhChungs { get; set; } = new();
        }

        public class GuiKhieuNaiRequest
        {
            [Required, MaxLength(2000)] public string NoiDung { get; set; } = string.Empty;
            public List<IFormFile> TepMinhChungs { get; set; } = new();
        }

        public class KetLuanBaoCaoRequest
        {
            [Required] public string KetLuan { get; set; } = string.Empty;
            [MaxLength(30)] public string? MucDoViPham { get; set; }
            [MaxLength(2000)] public string? GhiChu { get; set; }
            [Range(0, 365)] public int SoNgayGiaHan { get; set; }
        }

        public class KetLuanKhieuNaiRequest
        {
            [Required] public bool? ChapNhan { get; set; }
            [Required, MaxLength(2000)] public string KetQua { get; set; } = string.Empty;
        }
    }
}

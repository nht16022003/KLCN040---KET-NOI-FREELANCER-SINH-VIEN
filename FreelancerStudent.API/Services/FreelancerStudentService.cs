using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace FreelancerStudent.API.Services
{
    public class FreelancerStudentService : IFreelancerStudentService
    {
        private readonly IFreelancerStudentRepository _freelancerStudentRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý

        private readonly IPortfolioService _portfolioService;
        private readonly ApplicationDBContext _context;

        public FreelancerStudentService(IFreelancerStudentRepository freelancerStudentRepository, IPortfolioService portfolioService, ApplicationDBContext context)
        {
            _freelancerStudentRepository = freelancerStudentRepository;
            _portfolioService = portfolioService;
            _context = context;
        }


        //
        public async Task<List<FreelancerStudent_ReponseDTO>> layDanhSachFreelancerStudent()
        {
            var ds_layTuSQL = await _freelancerStudentRepository.layTatCaFreelancerStudentsAsync();
            /*
            
             public int maFreelancerStudents { get; set; }


        public string maChuyenNganh { get; set; } = string.Empty;
        public int maUser { get; set; }

        public string maTruong { get; set; } = string.Empty;

        public string tenTruong { get; set; } = string.Empty;

        public string diaDiemTruong { get; set; } = string.Empty;
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;

        public string? ngonNgu { get; set; }

        public string? kyNangCoBan { get; set; }

        public int namThu { get; set; } = 1;

        public float GPA { get; set; } = 0;

        public string nienKhoa { get; set; } = string.Empty;

        public string? gioithieu { get; set; }


        public string? avatar { get; set; }


        public bool trangthaiNhanViec { get; set; } = true;

        public decimal chiPhiTu { get; set; }
            */

            var userIds = ds_layTuSQL.Where(f => f.maUser > 0).Select(f => f.maUser).Distinct().ToList();
            var reviewStats = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => userIds.Contains(d.maNguoiDuocDanhGia))
                .GroupBy(d => d.maNguoiDuocDanhGia)
                .Select(g => new
                {
                    MaUser = g.Key,
                    SoLuot = g.Count(),
                    DiemTB = g.Average(d => (double)d.soSao)
                })
                .ToListAsync();

            var dsMinhChung = await _context.MinhChung_FreelancerStudents.AsNoTracking().Where(m => userIds.Contains(m.maUser)).OrderByDescending(m => m.ngayNop).ToListAsync();

            var dsResult = ds_layTuSQL.Select(free =>
            {
                var stat = reviewStats.FirstOrDefault(s => s.MaUser == free.maUser);
                double sao = (stat != null && stat.SoLuot > 0)
                    ? stat.DiemTB
                    : (double)free.diemDanhGiaTrungBinh;
                int soLuot = (stat != null && stat.SoLuot > 0)
                    ? stat.SoLuot
                    : free.soLuotDanhGia;

                var skills = free.FreelancerStudent_KyNangs != null && free.FreelancerStudent_KyNangs.Any()
                    ? free.FreelancerStudent_KyNangs
                        .Where(k => k.KyNang != null && !string.IsNullOrWhiteSpace(k.KyNang.tenKyNang))
                        .Select(k => k.KyNang!.tenKyNang.Trim())
                        .Distinct()
                        .ToList()
                    : (!string.IsNullOrWhiteSpace(free.kyNangCoBan)
                        ? free.kyNangCoBan.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Distinct()
                            .ToList()
                        : new List<string>());

                return new FreelancerStudent_ReponseDTO
                {
                    maFreelancerStudents = free.maFreelancerStudents,
                    maChuyenNganh = free.maChuyenNganh,
                    tenChuyenNganh = free.ChuyenNganh?.tenChuyenNganh ?? "Công nghệ thông tin",
                    maUser = free.maUser,
                    tenUser = free.User?.hotenUser,
                    maTruong = free.maTruong,
                    tenTruong = free.tenTruong,
                    diaDiemTruong = free.diaDiemTruong,
                    diaDiemFreelancerStudent = free.diaDiemFreelancerStudent,
                    ngonNgu = free.ngonNgu,
                    kyNangCoBan = free.kyNangCoBan,
                    namThu = free.namThu,
                    GPA = free.GPA,
                    nienKhoa = free.nienKhoa,
                    gioithieu = free.gioithieu,
                    avatar = free.User?.avatarUrl,
                    trangthaiNhanViec = free.trangthaiNhanViec,
                    chiPhiTu = free.chiPhiTu,
                    diemDanhGiaTrungBinh = sao,
                    soLuotDanhGia = soLuot,
                    ngayTao = free.User?.ngayTao ?? DateTime.UtcNow,
                    daXacMinhSinhVien = dsMinhChung.FirstOrDefault(m => m.maUser == free.maUser)?.trangThaiGuiMinhChung == "Đã xác minh",
                    trangThaiXacMinh = dsMinhChung.FirstOrDefault(m => m.maUser == free.maUser)?.trangThaiGuiMinhChung ?? "Chưa gửi",
                    danhSachKyNang = skills
                };
            }).ToList();

            return dsResult;
        }

        public async Task<FreelancerStudent_ReponseDTO> layFreelancerStudent_TheoMaUser(int maUser)
        {
            var re = await _freelancerStudentRepository.layFreelancerStudent_TheoMaUser(maUser);

            if (re == null) return null!;



            return new FreelancerStudent_ReponseDTO
            {
                maUser = re.maUser,
                tenUser = re.User!.hotenUser
            };
        }

        //Tuấn
        public async Task<ChiTietHoSoFreelancer_ReponseDTO?> layChiTietHoSoAsync(int maFreelancerStudents)
        {
            var entity = await _freelancerStudentRepository.layChiTietTheoIdAsync(maFreelancerStudents);
            if (entity == null) return null;

            return new ChiTietHoSoFreelancer_ReponseDTO
            {
                maFreelancerStudents = entity.maFreelancerStudents,
                maUser = entity.maUser,
                tenUser = entity.User?.hotenUser ?? string.Empty,
                sdt = entity.User?.sdtUser,
                email = entity.User?.emailUser,
                avatar = entity.User?.avatarUrl,
                maChuyenNganh = entity.maChuyenNganh,
                tenChuyenNganh = entity.ChuyenNganh?.tenChuyenNganh ?? entity.maChuyenNganh,
                tenTruong = entity.tenTruong,
                diaDiemFreelancerStudent = entity.diaDiemFreelancerStudent,
                namThu = entity.namThu,
                GPA = entity.GPA,
                nienKhoa = entity.nienKhoa,
                gioithieu = entity.gioithieu,
                kyNangCoBan = entity.kyNangCoBan,
                ngonNgu = entity.ngonNgu,
                trangthaiNhanViec = entity.trangthaiNhanViec,
                chiPhiTu = entity.chiPhiTu,
                danhSachKyNang = entity.FreelancerStudent_KyNangs
                    .Where(k => k.KyNang != null)
                    .Select(k => k.KyNang!.tenKyNang)
                    .ToList()
            };
        }


        //Tuấn
        public async Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO dto)
        {
            // thực thi việc lưu
            return await _freelancerStudentRepository.capNhatHoSoAsync(dto);
        }

        public async Task<bool> capNhatThongTinCoBanAsync(DTOs.RequestDTOs.CapNhatThongTinCoBan_RequestDTO dto)
        {
            return await _freelancerStudentRepository.capNhatThongTinCoBanAsync(dto);
        }

        public async Task<bool> capNhatGPAAsync(DTOs.RequestDTOs.CapNhatGPA_RequestDTO dto)
        {
            return await _freelancerStudentRepository.capNhatGPAAsync(dto);
        }

        public async Task<FreelancerStudentProfile_ReponseDTO?> layProfileFreelancerStudent(int maFreelancerStudents)
        {
            if (maFreelancerStudents <= 0)
            {
                return null;
            }

            var freelancer = (await _freelancerStudentRepository.layTheoMaFreelancerStudents(maFreelancerStudents));

            if (freelancer == null)
            {
                return null;
            }

            var portfolio = await _portfolioService.layPortfolioAsync(maFreelancerStudents);
            var tatCaKyNang = await _freelancerStudentRepository.layTatCaKyNangAsync();

            var reviewsQuery = _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == freelancer.maUser);
            var tongDanhGia = await reviewsQuery.CountAsync();
            var danhGiaTot = await reviewsQuery.CountAsync(d => d.soSao >= 4);
            var soDuAnHoanThanh = await _context.HopDongs.AsNoTracking()
                .CountAsync(h => h.maFreelancerStudent == maFreelancerStudents && (h.trangThai == "DaThanhToan" || h.trangThai == "DaHoanThanh"));
            var soKhachHang = await _context.HopDongs.AsNoTracking()
                .Where(h => h.maFreelancerStudent == maFreelancerStudents && (h.trangThai == "DaThanhToan" || h.trangThai == "DaHoanThanh"))
                .Select(h => h.JobPost!.maNhaTuyenDung).Distinct().CountAsync();
            var danhGiaGanDay = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == freelancer.maUser)
                .Include(d => d.NguoiDanhGia).ThenInclude(u => u!.NhaTuyenDung)
                .Include(d => d.HopDong).ThenInclude(h => h!.JobPost)
                .OrderByDescending(d => d.ngayDanhGia).Take(5).ToListAsync();

                        var latestMC = await _context.MinhChung_FreelancerStudents.AsNoTracking()
                .Where(m => m.maUser == freelancer.maUser)
                .OrderByDescending(m => m.ngayNop)
                .FirstOrDefaultAsync();

            ProfileEvidenceResponse? evidence = null;
            if (latestMC != null)
            {
                string? mssv = null;
                try
                {
                    string cleanUrl = latestMC.fileMinhChung.TrimStart('/', '\\');
                    string baseDir = Directory.GetCurrentDirectory();
                    string metaPath = Path.Combine(baseDir, "wwwroot", cleanUrl);
                    if (!File.Exists(metaPath))
                    {
                        metaPath = Path.Combine(baseDir, "..", "FreelancerStudent.Web", "wwwroot", cleanUrl);
                    }
                    metaPath = Path.ChangeExtension(metaPath, ".meta.json");
                    if (File.Exists(metaPath))
                    {
                        string json = await File.ReadAllTextAsync(metaPath);
                        using var doc = System.Text.Json.JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("Mssv", out var mssvProp) || doc.RootElement.TryGetProperty("mssv", out mssvProp))
                        {
                            mssv = mssvProp.GetString();
                        }
                    }
                }
                catch { }

                var kqNk = NienKhoaHelper.KiemTraNienKhoa(freelancer.nienKhoa);

                evidence = new ProfileEvidenceResponse
                {
                    LoaiMinhChung = latestMC.loaiMinhChung,
                    TrangThaiGuiMinhChung = latestMC.trangThaiGuiMinhChung,
                    FileMinhChung = latestMC.fileMinhChung,
                    NgayNop = latestMC.ngayNop,
                    NgayXacMinh = latestMC.ngayXacMinh,
                    LyDoTuChoi = latestMC.lydoTuChoi,
                    Mssv = mssv,
                    NienKhoa = freelancer.nienKhoa,
                    HetHanNienKhoa = kqNk.HetHan,
                    NamKetThucNienKhoa = kqNk.NamKetThuc,
                    ThongBao = kqNk.HetHan
                        ? NienKhoaHelper.ThongBaoHetHan
                        : (latestMC.trangThaiGuiMinhChung == "Đã xác minh"
                            ? "Đã xác minh thành công."
                            : (latestMC.trangThaiGuiMinhChung == "Đang gửi"
                                ? "Đang chờ Ban quản trị phê duyệt."
                                : (latestMC.lydoTuChoi ?? "Minh chứng bị từ chối.")))
                };
            }

            return new FreelancerStudentProfile_ReponseDTO
            {
                User = new ProfileUserResponse
                {
                    MaUser = freelancer.maUser,
                    TenTaiKhoanUser = freelancer.User?.tenTaiKhoanUser ?? string.Empty,
                    HotenUser = freelancer.User?.hotenUser ?? string.Empty,
                    AvatarUrl = freelancer.User?.avatarUrl,
                    Ngaysinh = freelancer.User?.ngaysinh,
                    NgayTao = freelancer.User?.ngayTao ?? DateTime.UtcNow
                },
                FreelancerStudent = new ProfileFreelancerResponse
                {
                    MaFreelancerStudents = freelancer.maFreelancerStudents,
                    MaUser = freelancer.maUser,
                    MaChuyenNganh = freelancer.maChuyenNganh,
                    TenTruong = freelancer.tenTruong,
                    MaTruong = freelancer.maTruong,
                    DiaDiemFreelancerStudent = freelancer.diaDiemFreelancerStudent,
                    NgonNgu = freelancer.ngonNgu,
                    KyNangCoBan = freelancer.kyNangCoBan,
                    NamThu = freelancer.namThu,
                    GPA = freelancer.GPA,
                    NienKhoa = freelancer.nienKhoa,
                    Gioithieu = freelancer.gioithieu,
                    TrangthaiNhanViec = freelancer.trangthaiNhanViec,
                    ChiPhiTu = freelancer.chiPhiTu,
                    DiemDanhGiaTrungBinh = freelancer.diemDanhGiaTrungBinh,
                    SoLuotDanhGia = freelancer.soLuotDanhGia,
                    TyLeHaiLong = tongDanhGia == 0 ? 0 : Math.Round(danhGiaTot * 100d / tongDanhGia, 1),
                    SoDuAnHoanThanh = soDuAnHoanThanh,
                    SoKhachHang = soKhachHang,
                    DanhGiaGanDay = danhGiaGanDay.Select(d => new DanhGia_ReponseDTO
                    {
                        maDanhGia = d.maDanhGia,
                        maHD = d.maHD,
                        tenNguoiDanhGia = d.NguoiDanhGia?.NhaTuyenDung?.tencongty ?? d.NguoiDanhGia?.hotenUser ?? "Nhà tuyển dụng",
                        avatar = d.NguoiDanhGia?.avatarUrl,
                        tenCongViec = d.HopDong?.JobPost?.tieude ?? string.Empty,
                        soSaoTongThe = d.soSao,
                        soSaoChatLuong = d.soSaoChatLuong,
                        soSaoDungHan = d.soSaoDungHan,
                        soSaoGiaoTiep = d.soSaoGiaoTiep,
                        nhanXet = d.nhanXet,
                        ngayDanhGia = d.ngayDanhGia,
                        hanChinhSua = d.ngayDanhGia.AddDays(7),
                        daChinhSua = d.soLanChinhSua > 0,
                        ngayCapNhat = d.ngayCapNhat
                    }).ToList()
                },
                ChuyenNganh = freelancer.ChuyenNganh == null ? null : new ProfileMajorResponse
                {
                    TenChuyenNganh = freelancer.ChuyenNganh.tenChuyenNganh
                },
                Portfolio = portfolio == null ? null : new ProfilePortfolioResponse
                {
                    MoTaBanThan = portfolio.moTaBanThan,
                    Url_video = portfolio.url_video
                },
                KyNangs = freelancer.FreelancerStudent_KyNangs?
                    .Where(k => k.KyNang != null)
                    .Select(k => new ProfileSkillResponse
                    {
                        TenKyNang = k.KyNang!.tenKyNang
                    }).ToList() ?? new List<ProfileSkillResponse>(),
                DuAns = portfolio?.projects.Select(project => new ProfileProjectResponse
                {
                    MaDA = project.maDA,
                    LaDuAnNoiBat = project.laDuAnNoiBat,
                    TenDuAn = project.tenDuAn,
                    VaiTro = project.vaiTro ?? string.Empty,
                    MoTa = project.moTa ?? string.Empty,
                    Congnghe = project.congnghe ?? string.Empty,
                    LinkGithub = project.linkGithub,
                    LinkDemo = project.linkDemo,
                    Link_file = project.link_file
                }).ToList() ?? new List<ProfileProjectResponse>(),
                TatCaKyNangGoiY = tatCaKyNang,
                MinhChung = evidence
            };
        }


    }



}
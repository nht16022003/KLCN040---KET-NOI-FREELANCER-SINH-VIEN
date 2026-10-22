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

            var dsResult = ds_layTuSQL.Select(free => new FreelancerStudent_ReponseDTO
            {
                maFreelancerStudents = free.maFreelancerStudents,
                maChuyenNganh = free.maChuyenNganh,
                tenChuyenNganh = free.ChuyenNganh?.tenChuyenNganh ?? "Empty",
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
                diemDanhGiaTrungBinh = free.diemDanhGiaTrungBinh,
                soLuotDanhGia = free.soLuotDanhGia
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
                .CountAsync(h => h.maFreelancerStudent == maFreelancerStudents && h.trangThai == "DaThanhToan");
            var soKhachHang = await _context.HopDongs.AsNoTracking()
                .Where(h => h.maFreelancerStudent == maFreelancerStudents && h.trangThai == "DaThanhToan")
                .Select(h => h.JobPost!.maNhaTuyenDung).Distinct().CountAsync();
            var danhGiaGanDay = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == freelancer.maUser)
                .Include(d => d.NguoiDanhGia).ThenInclude(u => u!.NhaTuyenDung)
                .Include(d => d.HopDong).ThenInclude(h => h!.JobPost)
                .OrderByDescending(d => d.ngayDanhGia).Take(5).ToListAsync();

            return new FreelancerStudentProfile_ReponseDTO
            {
                User = new ProfileUserResponse
                {
                    TenTaiKhoanUser = freelancer.User?.tenTaiKhoanUser ?? string.Empty,
                    HotenUser = freelancer.User?.hotenUser ?? string.Empty,
                    AvatarUrl = freelancer.User?.avatarUrl,
                    Ngaysinh = freelancer.User?.ngaysinh,
                    NgayTao = freelancer.User?.ngayTao ?? DateTime.UtcNow
                },
                FreelancerStudent = new ProfileFreelancerResponse
                {
                    MaFreelancerStudents = freelancer.maFreelancerStudents,
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
                        ngayCapNhat = d.ngayCapNhat,
                        daChinhSua = d.soLanChinhSua > 0
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
                TatCaKyNangGoiY = tatCaKyNang
            };
        }


    }



}
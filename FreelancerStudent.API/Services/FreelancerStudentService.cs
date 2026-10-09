using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class FreelancerStudentService : IFreelancerStudentService
    {
        private readonly IFreelancerStudentRepository _freelancerStudentRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý
        private readonly IPortfolioService _portfolioService;

        public FreelancerStudentService(
            IFreelancerStudentRepository freelancerStudentRepository,
            IPortfolioService portfolioService)
        {
            _freelancerStudentRepository = freelancerStudentRepository;
            _portfolioService = portfolioService;
        }


        //
        public async Task<List<FreelancerStudent_ReponseDTO>> layDanhSachFreelancerStudent()
        {
            var ds_layTuSQL = await _freelancerStudentRepository.layTatCaFreelancerStudentsAsync();
            

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

                trangthaiNhanViec = free.trangthaiNhanViec,
                chiPhiTu = free.chiPhiTu
            }).ToList();

            return dsResult;
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

            return new FreelancerStudentProfile_ReponseDTO
            {
                User = new ProfileUserResponse
                {
                    TenTaiKhoanUser = freelancer.User?.tenTaiKhoanUser ?? string.Empty,
                    HotenUser = freelancer.User?.hotenUser ?? string.Empty,
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
                    ChiPhiTu = freelancer.chiPhiTu
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
                }).ToList() ?? new List<ProfileProjectResponse>()
            };
        }

        /*
        

          

        */



    }
}
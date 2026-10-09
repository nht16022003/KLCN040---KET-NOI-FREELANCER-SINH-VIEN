using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.VisualBasic;

namespace FreelancerStudent.API.Services
{
    public class FreelancerStudentService : IFreelancerStudentService
    {
        private readonly IFreelancerStudentRepository _freelancerStudentRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý

        public FreelancerStudentService(IFreelancerStudentRepository freelancerStudentRepository)
        {
            _freelancerStudentRepository = freelancerStudentRepository;
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
                chiPhiTu = free.chiPhiTu
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
                }
            };
        }


    }



}
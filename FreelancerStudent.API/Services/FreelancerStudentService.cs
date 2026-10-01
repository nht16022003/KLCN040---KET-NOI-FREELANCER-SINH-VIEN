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

                trangthaiNhanViec = free.trangthaiNhanViec,
                chiPhiTu = free.chiPhiTu
            }).ToList();

            return dsResult;
        }

        /*
        

          

        */



    }
}
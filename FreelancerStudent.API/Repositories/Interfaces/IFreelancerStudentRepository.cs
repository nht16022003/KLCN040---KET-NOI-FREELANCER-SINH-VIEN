using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IFreelancerStudentRepository
    {
        //Lấy tất cả freelancer students
        Task<List<FreelamcerStudents>> layTatCaFreelancerStudentsAsync();

        //Thêm freelancer students dựa vào maRole của Users
        //Task<FreelamcerStudents> themNhaTuyenDungDuaVaoMaRoleCuaUsers(Users user, int maRole);

        //Tuấn
        Task<FreelamcerStudents?> layFreelancerStudent_TheoMaUser(int maUser);

        //Tuấn

        //Thêm nhà tuyển dụng dựa vào maRole của Users
        Task<FreelamcerStudents> themFreelancerStudent_DuaVaoMaRoleCuaUsers(Users user, int maRole);

        //Tuấn
        Task<FreelamcerStudents?> layChiTietTheoIdAsync(int maFreelancerStudents);

        //Tuấn
        Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO dto);

    }
}
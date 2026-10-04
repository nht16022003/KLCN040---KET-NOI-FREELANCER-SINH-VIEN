using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IFreelancerStudentRepository
    {
        //Lấy tất cả freelancer students
        Task<List<FreelancerStudents>> layTatCaFreelancerStudentsAsync();

        //Thêm freelancer students dựa vào maRole của Users
        //Task<FreelamcerStudents> themNhaTuyenDungDuaVaoMaRoleCuaUsers(Users user, int maRole);

        //Tuấn
        Task<FreelancerStudents?> layFreelancerStudent_TheoMaUser(int maUser);

        //XS
        Task<FreelancerStudents?> layTheoMaFreelancerStudents(int maFreelancerStudents);

    }
}
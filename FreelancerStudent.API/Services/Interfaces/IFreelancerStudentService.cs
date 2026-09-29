using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IFreelancerStudentService
    {
        //Lấy danh sách freelancer students
        Task<List<FreelancerStudent_ReponseDTO>> layDanhSachFreelancerStudent();
    }
}
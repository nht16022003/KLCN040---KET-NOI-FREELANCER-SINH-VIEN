using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Services.Interfaces
{
        public interface IFreelancerStudentService
        {
                //Lấy danh sách freelancer students
                Task<List<FreelancerStudent_ReponseDTO>> layDanhSachFreelancerStudent();

                //
                Task<FreelancerStudent_ReponseDTO> layFreelancerStudent_TheoMaUser(int maUser);


                //Tuấn
                Task<ChiTietHoSoFreelancer_ReponseDTO?> layChiTietHoSoAsync(int maFreelancerStudents);


                //Tuấn
                Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO reponse);


                Task<FreelancerStudentProfile_ReponseDTO?> layProfileFreelancerStudent(int maFreelancerStudents);
        }
}
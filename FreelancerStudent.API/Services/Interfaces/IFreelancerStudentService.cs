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

                // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo
                Task<bool> capNhatThongTinCoBanAsync(DTOs.RequestDTOs.CapNhatThongTinCoBan_RequestDTO dto);

                // Cập nhật điểm GPA học tập
                Task<bool> capNhatGPAAsync(DTOs.RequestDTOs.CapNhatGPA_RequestDTO dto);
        }
}
using FreelancerStudent.API.DTOs.ReponseDTOs;
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

        //Tuấn

        //Thêm nhà tuyển dụng dựa vào maRole của Users
        Task<FreelancerStudents> themFreelancerStudent_DuaVaoMaRoleCuaUsers(Users user, int maRole);

        //Tuấn
        Task<FreelancerStudents?> layChiTietTheoIdAsync(int maFreelancerStudents);

        //Tuấn
        Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO dto);
        //XS
        Task<FreelancerStudents?> layTheoMaFreelancerStudents(int maFreelancerStudents);

        // Lấy tất cả tên kỹ năng từ bảng KyNang trong database
        Task<List<string>> layTatCaKyNangAsync();

        // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo
        Task<bool> capNhatThongTinCoBanAsync(DTOs.RequestDTOs.CapNhatThongTinCoBan_RequestDTO dto);

        // Cập nhật GPA học tập
        Task<bool> capNhatGPAAsync(DTOs.RequestDTOs.CapNhatGPA_RequestDTO dto);
    }
}
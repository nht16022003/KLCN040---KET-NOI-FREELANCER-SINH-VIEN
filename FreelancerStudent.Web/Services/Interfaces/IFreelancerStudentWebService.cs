using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
        public interface IFreelancerStudentWebService
        {
                Task<ApiReponse<List<FreelacerStudentViewModel>>> layDanhSachFreelancerStudentAsync();

                //Tuấn
                Task<ApiReponse<ChiTietHoSoFreelancerViewModel>> layChiTietHoSoAsync(int maFreelancerStudents);

                //Tuấn
                Task<ApiReponse<bool>> capNhatHoSoAsync(ChiTietHoSoFreelancerViewModel model);



                Task<ApiReponse<FreelancerStudentProfileViewModel>> layProfileFreelancerStudentAsync(int maFreelancerStudents);

                Task<ApiReponse<PortfolioViewModel>> layPortfolioAsync(int maFreelancerStudents);
                Task<ApiReponse<PortfolioViewModel>> capNhatPortfolioAsync(PortfolioViewModel portfolio);

                Task<ApiReponse<PortfolioViewModel>> themDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
                Task<ApiReponse<PortfolioViewModel>> suaDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
                Task<ApiReponse<PortfolioViewModel>> xoaDuAnAsync(string maDA, int maFreelancerStudents);
                Task<ApiReponse<PortfolioViewModel>> capNhatDuAnNoiBatAsync(int maFreelancerStudents, List<string> maDAs);

                // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo
                Task<ApiReponse<bool>> capNhatThongTinCoBanAsync(CapNhatThongTinCoBanViewModel model);

                // Cập nhật điểm GPA học tập
                Task<ApiReponse<bool>> capNhatGPAAsync(int maUser, double gpa);
        }
}
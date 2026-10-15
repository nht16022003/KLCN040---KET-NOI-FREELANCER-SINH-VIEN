using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
        public interface IFreelancerStudentWebService
        {
                Task<ApiReponse<List<FreelacerStudentViewModel>>> layDanhSachFreelancerStudentAsync();

<<<<<<< HEAD
                //Tuấn
                Task<ApiReponse<ChiTietHoSoFreelancerViewModel>> layChiTietHoSoAsync(int maFreelancerStudents);

                //Tuấn
                Task<ApiReponse<bool>> capNhatHoSoAsync(ChiTietHoSoFreelancerViewModel model);



                Task<ApiReponse<FreelancerStudentProfileViewModel>> layProfileFreelancerStudentAsync(int maFreelancerStudents);
        }
=======
        Task<ApiReponse<FreelancerStudentProfileViewModel>> layProfileFreelancerStudentAsync(int maFreelancerStudents);

        Task<ApiReponse<PortfolioViewModel>> layPortfolioAsync(int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> capNhatPortfolioAsync(PortfolioViewModel portfolio);

        Task<ApiReponse<PortfolioViewModel>> themDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> suaDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> xoaDuAnAsync(string maDA, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> capNhatDuAnNoiBatAsync(int maFreelancerStudents, List<string> maDAs);
    }
>>>>>>> 461ecfc3a270be4a4d6c239efb1e62de57019d5b
}
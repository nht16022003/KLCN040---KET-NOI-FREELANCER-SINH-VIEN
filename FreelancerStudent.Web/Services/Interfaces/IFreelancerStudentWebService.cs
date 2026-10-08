using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IFreelancerStudentWebService
    {
        Task<ApiReponse<List<FreelacerStudentViewModel>>> layDanhSachFreelancerStudentAsync();

        Task<ApiReponse<FreelancerStudentProfileViewModel>> layProfileFreelancerStudentAsync(int maFreelancerStudents);

        Task<ApiReponse<PortfolioViewModel>> layPortfolioAsync(int maFreelancerStudents);

        Task<ApiReponse<PortfolioViewModel>> themDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> suaDuAnAsync(DuAnTrongPortfolioViewModel project, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> xoaDuAnAsync(string maDA, int maFreelancerStudents);
        Task<ApiReponse<PortfolioViewModel>> capNhatDuAnNoiBatAsync(int maFreelancerStudents, List<string> maDAs);
    }
}
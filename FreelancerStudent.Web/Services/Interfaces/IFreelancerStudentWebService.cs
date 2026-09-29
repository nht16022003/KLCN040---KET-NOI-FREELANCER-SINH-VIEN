using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IFreelancerStudentWebService
    {
        Task<ApiReponse<List<FreelacerStudentViewModel>>> layDanhSachFreelancerStudentAsync();
    }
}
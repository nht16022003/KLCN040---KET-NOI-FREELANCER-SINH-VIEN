using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IJobPostWebService
    {
        Task<ApiReponse<List<JobPostViewModel>>> layDanhSachJobPost();
    }
}
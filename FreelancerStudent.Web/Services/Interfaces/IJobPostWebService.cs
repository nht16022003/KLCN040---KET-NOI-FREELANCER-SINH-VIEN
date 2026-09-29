using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IJobPostWebService
    {
        Task<ApiReponse<List<JobPostViewModel>>> layDanhSachJobPost();

        Task<ApiReponse<JobPostViewModel>> taoJobPost(
            JobPostViewModel model
        );
    }
}
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IJobPostRepository
    {
        //Lấy tất cả JobPost
        Task<List<JobPost>> layTatCaJobPostAsync();

        Task<JobPost> themJobPostAsync(JobPost job);



    }
}
using FreelancerStudent.API.DTOs.ReponsesDTO;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IJobPostService
    {
        //Lấy danh sách JobPost
        Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync();

        Task<JobPost_ReponseDTO> taoJobPostAsync(JobPost_RequestDTO request);
    }
}
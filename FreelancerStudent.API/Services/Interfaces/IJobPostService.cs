using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IJobPostService
    {
        //Lấy danh sách JobPost
        Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync();

        Task<JobPost_ReponseDTO> taoJobPostAsync(JobPost_RequestDTO request);


        //Tuấn
        Task<List<UngTuyen_ReponseDTO>> layDanhSachUngTuyen_JobPost_TheoMaUser(int maUser);

        //Tuấn
        Task<bool> duyetUngTuyenAsync(DuyetUngTuyen_RequestDTO requestDTO);


        //Tuấn
        Task<bool> nopDonUngTuyenAsync(UngTuyen_RequestDTO requets);

        //Tuấn
        Task<List<JobPost_ReponseDTO>> layJobPostTheoMaUserAsync(int maUser);

        //Tuấn
        Task<bool> capNhatJobPostAsync(JobPost_RequestDTO reqeuest);
    }
}
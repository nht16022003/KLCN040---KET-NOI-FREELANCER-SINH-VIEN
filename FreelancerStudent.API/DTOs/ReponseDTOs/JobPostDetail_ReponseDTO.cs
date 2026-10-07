using FreelancerStudent.API.DTOs.ReponsesDTO;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class JobPostDetail_ReponsesDTO
    {
        public JobPost_ReponseDTO Job { get; set; }
        public NhaTuyenDung_ResponseDTO? Employer { get; set; }

        public List<JobPost_ReponseDTO> RelatedJobs { get; set; } = new();
    }
}
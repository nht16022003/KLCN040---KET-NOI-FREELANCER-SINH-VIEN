namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class JobPostDetail_ReponseDTO
    {
        public JobPost_ReponseDTO Job { get; set; } = new();

        public NhaTuyenDung_ResponseDTO? Employer { get; set; }

        public List<JobPost_ReponseDTO> RelatedJobs { get; set; } = new();
    }
}
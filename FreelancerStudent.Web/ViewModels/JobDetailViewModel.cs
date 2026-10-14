namespace FreelancerStudent.Web.ViewModels
{
    public class JobDetailViewModel
    {
        public JobPostViewModel Job { get; set; } = new();

        public NhaTuyenDungViewModel? Employer { get; set; }

        public NhaTuyenDungViewModel? EmployerUser { get; set; }

        public List<JobPostViewModel> RelatedJobs { get; set; } = new();
    }
}

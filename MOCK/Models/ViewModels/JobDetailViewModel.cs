using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class JobDetailViewModel
    {
        public JobPost Job { get; set; } = new();
        public NhaTuyenDung? Employer { get; set; }
        public User? EmployerUser { get; set; }
        public List<JobPost> RelatedJobs { get; set; } = new();
    }
}

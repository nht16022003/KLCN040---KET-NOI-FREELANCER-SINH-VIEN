using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class ApplicantItemViewModel
    {
        public UngTuyen Application { get; set; } = new();
        public FreelancerStudent Student { get; set; } = new();
        public User StudentUser { get; set; } = new();
        public List<KynangChuyennganhFreelancerStudent> Skills { get; set; } = new();
        public MinhChungFreelancerStudent? MinhChung { get; set; }
    }

    public class JobApplicantsViewModel
    {
        public JobPost Job { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public List<ApplicantItemViewModel> Applicants { get; set; } = new();
        public string? SuccessMessage { get; set; }
    }
}

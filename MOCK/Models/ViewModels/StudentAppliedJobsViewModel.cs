using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class StudentAppliedJobItemViewModel
    {
        public UngTuyen Application { get; set; } = new();
        public JobPost Job { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public User EmployerUser { get; set; } = new();
        public HopDong? Contract { get; set; }
    }

    public class StudentAppliedJobsViewModel
    {
        public FreelancerStudent Student { get; set; } = new();
        public User User { get; set; } = new();
        public List<StudentAppliedJobItemViewModel> AppliedJobs { get; set; } = new();
        public string? CurrentStatusFilter { get; set; }
        public int TotalAppliedCount => AppliedJobs.Count;
        public int PendingCount { get; set; }
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
    }
}

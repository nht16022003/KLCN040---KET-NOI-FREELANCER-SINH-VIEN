using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class EmployerJobItemViewModel
    {
        public JobPost Job { get; set; } = new();
        public int ApplicantCount { get; set; } = 0;
        public int PendingApplicantCount { get; set; } = 0;
        public int AcceptedApplicantCount { get; set; } = 0;
        public HopDong? Contract { get; set; }
    }

    public class EmployerManageJobsViewModel
    {
        public NhaTuyenDung Employer { get; set; } = new();
        public User User { get; set; } = new();
        public List<EmployerJobItemViewModel> Jobs { get; set; } = new();
        public string? CurrentStatusFilter { get; set; }
        public int TotalJobsCount => Jobs.Count;
        public int ActiveJobsCount { get; set; }
        public int InProgressJobsCount { get; set; }
        public int ClosedJobsCount { get; set; }
    }
}

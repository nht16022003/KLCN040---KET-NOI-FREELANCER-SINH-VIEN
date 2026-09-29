using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class AdminVerificationItemViewModel
    {
        public MinhChungFreelancerStudent MinhChung { get; set; } = new();
        public User StudentUser { get; set; } = new();
        public FreelancerStudent? StudentProfile { get; set; }
    }

    public class AdminVerificationViewModel
    {
        public List<AdminVerificationItemViewModel> PendingVerifications { get; set; } = new();
        public List<AdminVerificationItemViewModel> VerifiedList { get; set; } = new();
        public List<AdminVerificationItemViewModel> RejectedList { get; set; } = new();
    }
}

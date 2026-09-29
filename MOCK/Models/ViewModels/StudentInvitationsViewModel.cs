using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class StudentInvitationItemViewModel
    {
        public YeuCauThueFreelancer Invitation { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public User EmployerUser { get; set; } = new();
    }

    public class StudentInvitationsViewModel
    {
        public FreelancerStudent Student { get; set; } = new();
        public User User { get; set; } = new();
        public List<StudentInvitationItemViewModel> Invitations { get; set; } = new();
        public int PendingCount { get; set; }
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class ContractDetailViewModel
    {
        public HopDong Contract { get; set; } = new();
        public JobPost Job { get; set; } = new();
        public FreelancerStudent Freelancer { get; set; } = new();
        public User FreelancerUser { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public User EmployerUser { get; set; } = new();
    }
}

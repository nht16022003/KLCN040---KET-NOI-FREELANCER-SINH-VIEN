using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class FreelancerProfileViewModel
    {
        public User User { get; set; } = new();
        public FreelancerStudent FreelancerStudent { get; set; } = new();
        public List<KynangChuyennganhFreelancerStudent> KyNangs { get; set; } = new();
        public Portfolio? Portfolio { get; set; }
        public List<DuAnTrongPortfolio> DuAns { get; set; } = new();
        public List<HopDong> HopDongs { get; set; } = new();
        public Wallet? Wallet { get; set; }
        public MinhChungFreelancerStudent? MinhChung { get; set; }
    }
}

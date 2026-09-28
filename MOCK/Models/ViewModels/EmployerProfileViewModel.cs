using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class EmployerProfileViewModel
    {
        public User User { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public List<JobPost> ActiveJobs { get; set; } = new();
        public List<JobPost> AllJobs { get; set; } = new();
        public Wallet? Wallet { get; set; }
        public bool IsBookmarked { get; set; }
    }
}

using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class EmployerCardItemViewModel
    {
        public NhaTuyenDung Employer { get; set; } = new();
        public User User { get; set; } = new();
        public int TotalJobs { get; set; }
        public int ActiveJobsCount { get; set; }
        public List<JobPost> FeaturedJobs { get; set; } = new();
        public bool IsBookmarked { get; set; }
    }

    public class EmployerSearchViewModel
    {
        public List<EmployerCardItemViewModel> Employers { get; set; } = new();
        public List<string> AllLinhVucs { get; set; } = new();
        public List<string> AllDiaDiems { get; set; } = new();

        public string? Keyword { get; set; }
        public string? SelectedLinhVuc { get; set; }
        public string? SelectedDiaDiem { get; set; }
        public double? SelectedRating { get; set; }
    }
}

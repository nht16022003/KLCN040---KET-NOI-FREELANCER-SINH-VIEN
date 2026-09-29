using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class FreelancerCardItemViewModel
    {
        public User User { get; set; } = new();
        public FreelancerStudent Student { get; set; } = new();
        public ChuyenNganh? ChuyenNganh { get; set; }
        public List<KynangChuyennganhFreelancerStudent> KyNangs { get; set; } = new();
        public string XepLoai
        {
            get
            {
                if (Student.GPA >= 3.6) return "Xuất sắc";
                if (Student.GPA >= 3.2) return "Giỏi";
                if (Student.GPA >= 2.5) return "Khá";
                return "Trung bình";
            }
        }
        public double DanhGia => 4.5; // Mock rating
    }

    public class FreelancerSearchViewModel
    {
        public List<FreelancerCardItemViewModel> Freelancers { get; set; } = new();
        public List<ChuyenNganh> ChuyenNganhs { get; set; } = new();
        public List<string> AllKyNangs { get; set; } = new();
        public List<string> AllKyNangCoBans { get; set; } = new();

        // Filter values
        public string? Keyword { get; set; }
        public string? SelectedChuyenNganh { get; set; }
        public string? SelectedKyNang { get; set; }
        public string? SelectedKyNangCoBan { get; set; }
        public string? SelectedNganSach { get; set; }
        public double? SelectedGPA { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class StudentPortfolioManageViewModel
    {
        public FreelancerStudent Student { get; set; } = new();
        public User User { get; set; } = new();
        public Portfolio Portfolio { get; set; } = new();
        public List<DuAnTrongPortfolio> Projects { get; set; } = new();

        // Form fields để thêm dự án mới vào DuAn_Trong_Portfolio
        [Required(ErrorMessage = "Vui lòng nhập tên dự án.")]
        [StringLength(200, ErrorMessage = "Tên dự án không quá 200 ký tự.")]
        public string NewTenDuAn { get; set; } = string.Empty;

        public string? NewMoTa { get; set; }
        public string? NewVaiTro { get; set; }
        public string? NewCongNghe { get; set; }
        public string? NewLinkGithub { get; set; }
        public string? NewLinkDemo { get; set; }
        public string? NewLinkFile { get; set; }

        public string? SuccessMessage { get; set; }
    }
}

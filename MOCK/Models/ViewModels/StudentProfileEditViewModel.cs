using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class StudentProfileEditViewModel
    {
        public User User { get; set; } = new();
        public FreelancerStudent Student { get; set; } = new();
        public List<KynangChuyennganhFreelancerStudent> KyNangs { get; set; } = new();
        public List<ChuyenNganh> ChuyenNganhs { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string HotenUser { get; set; } = string.Empty;

        public string? SdtUser { get; set; }
        public string? Gioithieu { get; set; }
        public string? NgonNgu { get; set; }
        public string? KyNangCoBan { get; set; }
        public bool TrangthaiNhanViec { get; set; } = true;

        [Range(0, 100000000, ErrorMessage = "Chi phí từ phải lớn hơn hoặc bằng 0.")]
        public decimal? ChiPhiTu { get; set; }

        public string NewSkillsInput { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
    }
}

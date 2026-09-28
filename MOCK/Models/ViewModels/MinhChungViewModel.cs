using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class MinhChungViewModel
    {
        public User User { get; set; } = new();
        public FreelancerStudent FreelancerStudent { get; set; } = new();
        public MinhChungFreelancerStudent MinhChung { get; set; } = new();
        public List<ChuyenNganh> ChuyenNganhs { get; set; } = new();

        // Thuộc tính nhập liệu form
        [Required(ErrorMessage = "Vui lòng chọn loại minh chứng.")]
        public string SelectedLoaiMinhChung { get; set; } = "Thẻ sinh viên";

        [Required(ErrorMessage = "Vui lòng nhập tên trường đại học/cao đẳng.")]
        public string TenTruong { get; set; } = string.Empty;

        public string? MaTruong { get; set; }
        public string? DiaDiemTruong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên ngành đào tạo.")]
        public string MaChuyenNganh { get; set; } = "CNTT";

        [Range(1, 6, ErrorMessage = "Năm thứ phải từ 1 đến 6.")]
        public int NamThu { get; set; } = 1;

        public string NienKhoa { get; set; } = "2023-2027";
        public double? GPA { get; set; }

        public string? FileMinhChungUrl { get; set; }

        // Trạng thái hiển thị
        public bool IsFirstTimeAfterRegister { get; set; } = false;
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

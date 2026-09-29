using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class EmployerFreelancerDetailViewModel
    {
        public FreelancerStudent Freelancer { get; set; } = new();
        public User User { get; set; } = new();
        public string TenChuyenNganh { get; set; } = string.Empty;
        public List<string> KyNangs { get; set; } = new();
        
        // Portfolio & Dự án tiêu biểu
        public Portfolio? Portfolio { get; set; }
        public List<DuAnTrongPortfolio> DuAns { get; set; } = new();
        
        // Đánh giá & Nhận xét từ các Hợp đồng đã hoàn thành
        public List<FreelancerReviewItemViewModel> DanhGias { get; set; } = new();
        public double DiemDanhGiaTrungBinh { get; set; } = 5.0;
        public int TongSoDanhGia { get; set; } = 0;
        public int SoHopDongThanhCong { get; set; } = 0;
        public double TiLeHoanThanhDungHan { get; set; } = 100.0;
        
        // Trạng thái tương tác từ Employer hiện tại
        public bool IsBookmarked { get; set; } = false;
        public int? CurrentEmployerId { get; set; } = 1;
        public string? GhiChuQuanTam { get; set; }
        public bool HasActiveDirectOffer { get; set; } = false;
        
        // Minh chứng xác thực
        public bool DaXacMinhSinhVien { get; set; } = true;
    }

    public class FreelancerReviewItemViewModel
    {
        public int MaDanhGia { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string TenNhaTuyenDung { get; set; } = string.Empty;
        public string? AvatarNhaTuyenDung { get; set; }
        public string TenCongViec { get; set; } = string.Empty;
        public int SoSao { get; set; }
        public string? NhanXet { get; set; }
        public DateTime NgayDanhGia { get; set; }
    }
}

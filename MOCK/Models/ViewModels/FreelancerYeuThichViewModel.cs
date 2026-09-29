using System;
using System.Collections.Generic;

namespace MOCK.Models.ViewModels
{
    public class FreelancerYeuThichItemViewModel
    {
        public int MaBookMark { get; set; }
        public int MaNhaTuyenDung { get; set; }
        public int MaFreelancerStudent { get; set; }
        public int MaUser { get; set; }
        public string HotenUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public string? Avatar { get; set; }
        public string MaChuyenNganh { get; set; } = string.Empty;
        public string TenChuyenNganh { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public int NamThu { get; set; }
        public double GPA { get; set; }
        public string NienKhoa { get; set; } = string.Empty;
        public string? Gioithieu { get; set; }
        public decimal? ChiPhiTu { get; set; }
        public bool TrangthaiNhanViec { get; set; }
        public List<string> KyNangChuyenNganh { get; set; } = new();
        public string? GhiChu { get; set; }
        public DateTime NgayLuu { get; set; }
        public double DiemDanhGia { get; set; } = 5.0;
        public int SoHopDongHoanThanh { get; set; } = 0;
        public int SoDuAnPortfolio { get; set; } = 0;
    }

    public class FreelancerYeuThichViewModel
    {
        public List<FreelancerYeuThichItemViewModel> DanhSachYeuThich { get; set; } = new();
        public List<string> AllChuyenNganhs { get; set; } = new();
        public string? Keyword { get; set; }
        public string? SelectedChuyenNganh { get; set; }
        public string? SortBy { get; set; }
        public int TotalCount => DanhSachYeuThich.Count;
    }
}

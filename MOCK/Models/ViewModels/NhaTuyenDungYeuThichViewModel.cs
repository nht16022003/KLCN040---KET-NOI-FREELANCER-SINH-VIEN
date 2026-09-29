using System;
using System.Collections.Generic;

namespace MOCK.Models.ViewModels
{
    public class NhaTuyenDungYeuThichItemViewModel
    {
        public int MaBookMark { get; set; }
        public int MaFreelancerStudent { get; set; }
        public int MaNhaTuyenDung { get; set; }
        public int MaUser { get; set; }
        public string Tencongty { get; set; } = string.Empty;
        public string? Linhvuc { get; set; }
        public string? Diachi { get; set; }
        public string? Logo { get; set; }
        public string? Link { get; set; }
        public string? Gioithieu { get; set; }
        public double? Sosaodanhgia { get; set; }
        public string HotenNguoiDaiDien { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobsCount { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayLuu { get; set; }
    }

    public class NhaTuyenDungYeuThichViewModel
    {
        public List<NhaTuyenDungYeuThichItemViewModel> DanhSachYeuThich { get; set; } = new();
        public List<string> AllLinhVucs { get; set; } = new();
        public string? Keyword { get; set; }
        public string? SelectedLinhVuc { get; set; }
        public string? SortBy { get; set; }
        public int TotalCount => DanhSachYeuThich.Count;
    }
}

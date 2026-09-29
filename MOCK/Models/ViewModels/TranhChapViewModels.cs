using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class TranhChapItemViewModel
    {
        public TranhChap TranhChap { get; set; } = new();
        public HopDong HopDong { get; set; } = new();
        public JobPost Job { get; set; } = new();
        public User CreatorUser { get; set; } = new();
        public User OpponentUser { get; set; } = new();
        public int SoLuongBangChung { get; set; }
    }

    public class TranhChapListViewModel
    {
        public List<TranhChapItemViewModel> TranhChaps { get; set; } = new();
        public int TotalCount => TranhChaps.Count;
        public int OpenCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }

        public string? Keyword { get; set; }
        public string? SelectedStatus { get; set; }
    }

    public class TranhChapCreateViewModel
    {
        public string? MaHD { get; set; }
        public HopDong? SelectedContract { get; set; }
        public JobPost? SelectedJob { get; set; }
        public List<HopDong> EligibleContracts { get; set; } = new();

        public string LyDo { get; set; } = string.Empty;
        public string MoTaChiTiet { get; set; } = string.Empty;
        public decimal SoTienTranhChap { get; set; }

        // Bằng chứng ban đầu
        public string LoaiBangChung { get; set; } = "AnhChup";
        public string? DuongDanFile { get; set; }
        public string? MoTaBangChung { get; set; }
    }

    public class TranhChapDetailViewModel
    {
        public TranhChap TranhChap { get; set; } = new();
        public HopDong HopDong { get; set; } = new();
        public JobPost Job { get; set; } = new();
        public User CreatorUser { get; set; } = new();
        public User OpponentUser { get; set; } = new();
        public List<BangChungTranhChap> BangChungs { get; set; } = new();
    }
}

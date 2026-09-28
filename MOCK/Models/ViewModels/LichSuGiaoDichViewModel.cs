using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class LichSuGiaoDichViewModel
    {
        public Wallet Wallet { get; set; } = new();
        public User User { get; set; } = new();
        public List<LichSuGiaoDich> Transactions { get; set; } = new();

        // Bộ lọc tìm kiếm
        public string? Keyword { get; set; }
        public string? LoaiGiaoDich { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        // Thống kê nhanh theo bộ lọc
        public decimal TongTienVao { get; set; }
        public decimal TongTienRa { get; set; }
        public int TongSoGiaoDich => Transactions.Count;
    }
}

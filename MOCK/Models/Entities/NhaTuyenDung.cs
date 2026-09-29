using System;
using System.Collections.Generic;

namespace MOCK.Models.Entities
{
    public class NhaTuyenDung
    {
        public int MaNhaTuyenDung { get; set; }
        public int MaUser { get; set; }
        public string? Avatar { get; set; }
        public string? Gioithieu { get; set; }
        public string? Tencongty { get; set; }
        public string? Linhvuc { get; set; }
        public string? Diachi { get; set; }
        public string? Link { get; set; }
        public string? Logo { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public string Trangthai { get; set; } = "Active";
        public double? Sosaodanhgia { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public List<JobPost> JobPosts { get; set; } = new();
    }
}

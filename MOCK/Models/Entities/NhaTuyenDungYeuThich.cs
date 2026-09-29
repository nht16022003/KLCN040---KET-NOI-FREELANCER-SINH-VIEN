using System;

namespace MOCK.Models.Entities
{
    public class NhaTuyenDungYeuThich
    {
        public int MaBookMark { get; set; }
        public int MaFreelancerStudent { get; set; }
        public int MaNhaTuyenDung { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayLuu { get; set; } = DateTime.Now;
    }
}

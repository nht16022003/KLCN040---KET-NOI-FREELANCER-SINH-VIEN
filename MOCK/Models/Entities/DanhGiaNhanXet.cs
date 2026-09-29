using System;

namespace MOCK.Models.Entities
{
    public class DanhGiaNhanXet
    {
        public int MaDanhGia { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public int MaNguoiDanhGia { get; set; }
        public int MaNguoiDuocDanhGia { get; set; }
        public int SoSao { get; set; } = 5;
        public string? NhanXet { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Navigation properties
        public HopDong? HopDong { get; set; }
        public User? NguoiDanhGia { get; set; }
        public User? NguoiDuocDanhGia { get; set; }
    }
}

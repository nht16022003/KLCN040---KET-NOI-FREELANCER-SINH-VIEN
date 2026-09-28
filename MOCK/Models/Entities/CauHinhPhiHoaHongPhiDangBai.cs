using System;

namespace MOCK.Models.Entities
{
    public class CauHinhPhiHoaHongPhiDangBai
    {
        public int MaCauHinh { get; set; }
        public string TenCauHinh { get; set; } = string.Empty; // PhiDangBai, HoaHongDuAn
        public decimal GiaTri { get; set; }
        public string LoaiCauHinh { get; set; } = "TienMat"; // TienMat, PhanTram
        public string? MoTa { get; set; }
        public int MaAdmin { get; set; }
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public bool HieuLuc { get; set; } = true;

        // Navigation property
        public Admin? Admin { get; set; }
    }
}

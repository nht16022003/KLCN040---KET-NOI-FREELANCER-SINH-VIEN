using System;

namespace MOCK.Models.Entities
{
    public class TaiKhoanNganHang
    {
        public int MaTKNH { get; set; }
        public int MaUser { get; set; }
        public string TenNganHang { get; set; } = string.Empty;
        public string? ChiNhanh { get; set; }
        public string SoTaiKhoan { get; set; } = string.Empty;
        public string TenChuTaiKhoan { get; set; } = string.Empty;
        public bool LaMacDinh { get; set; } = true;
        public DateTime NgayThem { get; set; } = DateTime.Now;

        // Navigation property
        public User? User { get; set; }
    }
}

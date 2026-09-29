using System;

namespace MOCK.Models.Entities
{
    public class User
    {
        public int MaUser { get; set; }
        public string HotenUser { get; set; } = string.Empty;
        public string TenTaiKhoanUser { get; set; } = string.Empty;
        public string PashwordHash { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string? SdtUser { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int MaRole { get; set; }

        // Navigation property
        public Role? Role { get; set; }
    }
}

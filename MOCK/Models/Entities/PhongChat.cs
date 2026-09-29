using System;
using System.Collections.Generic;

namespace MOCK.Models.Entities
{
    public class PhongChat
    {
        public string MaPhongChat { get; set; } = string.Empty;
        public int MaUserClient { get; set; }
        public int MaUserFreelancer { get; set; }
        public string? MaJob { get; set; }
        public string? MaHD { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public string? TinNhanCuoi { get; set; }
        public DateTime? ThoiGianTinNhanCuoi { get; set; }
        public int SoTinNhanChuaDoc { get; set; }

        // Navigation
        public User? UserClient { get; set; }
        public User? UserFreelancer { get; set; }
        public JobPost? JobPost { get; set; }
        public List<TinNhan> TinNhans { get; set; } = new();
    }
}

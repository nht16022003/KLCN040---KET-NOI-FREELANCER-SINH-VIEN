using System;

namespace MOCK.Models.Entities
{
    // Bảng BaiDangTimViec_FreelancerStudent trong KLCN040_FREELANCERSTUDENT.sql
    public class BaiDangTimViecFreelancerStudent
    {
        public string MaBaiDang { get; set; } = string.Empty; // VARCHAR(20) PRIMARY KEY
        public int MaFreelancerStudent { get; set; } // INT FOREIGN KEY -> FreelancerStudents(maFreelancerStudents)
        public string Tieude { get; set; } = string.Empty; // NVARCHAR(200) NOT NULL
        public string? Mota { get; set; } // NVARCHAR(MAX)
        public string? Kynang { get; set; } // NVARCHAR(100)
        public double? MucGiaTu { get; set; } // FLOAT
        public DateTime Thoigiandang { get; set; } = DateTime.Now; // DATETIME NOT NULL DEFAULT GETDATE()
        public string Trangthai { get; set; } = "DangHienThi"; // NVARCHAR(30) DEFAULT N'DangHienThi' ('DangHienThi', 'DaAn', 'DaNhanViec')

        // Navigation property
        public FreelancerStudent? FreelancerStudent { get; set; }
    }
}

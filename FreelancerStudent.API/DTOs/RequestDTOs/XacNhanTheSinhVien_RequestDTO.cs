using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class XacNhanTheSinhVien_RequestDTO
    {
        [Required]
        public int MaUser { get; set; }

        [MaxLength(30)]
        public string? Mssv { get; set; }

        [MaxLength(100)]
        public string? HoVaTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        [MaxLength(150)]
        public string? TenTruong { get; set; }

        [MaxLength(10)]
        public string? MaTruong { get; set; }

        [MaxLength(20)]
        public string? MaChuyenNganh { get; set; }

        [MaxLength(100)]
        public string? TenChuyenNganh { get; set; }

        [MaxLength(20)]
        public string? NienKhoa { get; set; }

        [Range(1, 6)]
        public int? NamThu { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileMinhChungUrl { get; set; } = string.Empty;

        public bool DongYCapNhatHoSo { get; set; } = true;
    }
}

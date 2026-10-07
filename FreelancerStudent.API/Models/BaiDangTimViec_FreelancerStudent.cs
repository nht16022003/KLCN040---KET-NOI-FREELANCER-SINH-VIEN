//Tuấn
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("BaiDangTimViec_FreelancerStudent")]
    public class BaiDangTimViecFreelancerStudent
    {
        [Key]
        [MaxLength(20)]
        public string maBaiDang { get; set; } = string.Empty;

        [Required]
        public int maFreelancerStudent { get; set; }

        [Required]
        [MaxLength(200)]
        public string tieude { get; set; } = string.Empty;

        public string? mota { get; set; }

        [MaxLength(100)]
        public string? kynang { get; set; }

        public double? mucGiaTu { get; set; }

        public DateTime thoigiandang { get; set; } = DateTime.UtcNow;

        [MaxLength(30)]
        public string trangthai { get; set; } = "DangHienThi"; // DangHienThi, DaAn, DaNhanViec

        [ForeignKey("maFreelancerStudent")]
        public virtual FreelancerStudents? FreelamcerStudents { get; set; }
    }
}

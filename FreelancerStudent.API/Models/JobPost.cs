using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("JobPost")]
    public class JobPost
    {
        [Key]
        [MaxLength(20)]
        public string maJob { get; set; } = string.Empty;

        public int maNhaTuyenDung { get; set; }

        [Required]
        [MaxLength]
        public string tieude { get; set; } = string.Empty;

        public string? mota { get; set; }

        [MaxLength(100)]
        public string? kynangyeucau { get; set; }

        public decimal? thulao { get; set; }

        [MaxLength(500)]
        public string? fileDinhKem { get; set; } = string.Empty;

        [Required]
        public decimal phiDangBai { get; set; }

        [Required]
        public DateTime thoigiandangtuyen { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime thoigiandukienhoanthanh { get; set; }

        [MaxLength(30)]
        public string? status { get; set; } = "DangTuyen";

        public int? soluongtuyen { get; set; }

        [ForeignKey("maNhaTuyenDung")]
        public virtual NhaTuyenDung? NhaTuyenDung { get; set; } //1 JobPost thuộc về 1 nhà tuyển dụng
    }
}
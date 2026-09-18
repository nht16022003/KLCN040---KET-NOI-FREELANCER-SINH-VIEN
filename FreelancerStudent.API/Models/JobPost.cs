using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("JobPost")]
    public class JobPost
    {
        [Key]
        [Column("jobID")]
        [StringLength(20)]
        public string JobID { get; set; } = string.Empty;

        [Column("cusID")]
        [Required]
        [StringLength(20)]
        public string CusID { get; set; } = string.Empty;

        [Column("title")]
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("descr")]
        public string? Descr { get; set; }

        [Column("thulao", TypeName = "decimal(18,2)")]
        public decimal? Thulao { get; set; }

        [Column("thoigianthuchien")]
        public DateTime? Thoigianthuchien { get; set; }

        [Column("status")]
        [StringLength(30)]
        public string Status { get; set; } = "DangTuyen";

        [Column("soluongtuyen")]
        public int? Soluongtuyen { get; set; } = 1;

        [ForeignKey("CusID")]
        public virtual KhachHang? KhachHang { get; set; }
    }
}

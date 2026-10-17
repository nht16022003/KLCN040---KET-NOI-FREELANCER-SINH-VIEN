using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("GiaiDoan_HopDong")]
    public class GiaiDoan_HopDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maGiaiDoan { get; set; }

        [Required]
        [MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string tenGiaiDoan { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? moTa { get; set; }

        public DateTime hanChot { get; set; }

        [MaxLength(30)]
        public string? trangThai { get; set; } = "ChuaDat";

        public DateTime ngayTao { get; set; } = DateTime.Now;

        [ForeignKey("maHD")]
        public virtual HopDong? HopDong { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("ThongBao")]
    public class ThongBao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maThongBao { get; set; }

        public int maUser { get; set; }

        [Required]
        [MaxLength(255)]
        public string tieuDe { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string noiDung { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string loaiThongBao { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? linkDieuHuong { get; set; }

        public bool daDoc { get; set; } = false;

        public DateTime ngayTao { get; set; } = DateTime.Now;

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }
    }
}

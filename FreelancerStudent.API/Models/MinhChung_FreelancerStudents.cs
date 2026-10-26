using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("MinhChung_FreelancerStudents")]
    public class MinhChung_FreelancerStudents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maMinhChung { get; set; }

        public int maUser { get; set; }

        [Required]
        [MaxLength(50)]
        public string loaiMinhChung { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string fileMinhChung { get; set; } = string.Empty;

        public DateTime ngayNop { get; set; } = DateTime.Now;

        public DateTime? ngayXacMinh { get; set; }

        [Required]
        [MaxLength(20)]
        public string trangThaiGuiMinhChung { get; set; } = "Đang gửi";

        [MaxLength(500)]
        public string? lydoTuChoi { get; set; }

        public int? nguoiXacMinh { get; set; }

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }

        [ForeignKey("nguoiXacMinh")]
        public virtual Admin? Admin { get; set; }
    }
}

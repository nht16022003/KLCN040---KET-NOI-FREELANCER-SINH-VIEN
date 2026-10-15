using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("DuAn_Trong_Portfolio")]
    public class DuAnTrongPortfolio
    {
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string maDA { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        public string maPortfolio { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string tenDuAn { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? vaiTro { get; set; }

        public string? moTa { get; set; }

        [MaxLength(50)]
        public string? congnghe { get; set; }

        [MaxLength(255)]
        public string? linkGithub { get; set; }

        [MaxLength(255)]
        public string? linkDemo { get; set; }

        [MaxLength(255)]
        public string? link_file { get; set; }

        public bool laDuAnNoiBat { get; set; }

        [ForeignKey(nameof(maPortfolio))]
        public virtual Portfolio? Portfolio { get; set; }
    }
}

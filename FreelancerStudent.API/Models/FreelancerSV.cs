using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("FreelancerSV")]
    public class FreelancerSV
    {
        [Key]
        [Column("free_ID")]
        [StringLength(20)]
        public string FreeID { get; set; } = string.Empty;

        [Column("userID")]
        [Required]
        [StringLength(20)]
        public string UserID { get; set; } = string.Empty;

        [Column("university")]
        [StringLength(150)]
        public string? University { get; set; }

        [Column("major")]
        [StringLength(100)]
        public string? Major { get; set; }

        [Column("studentCardID")]
        [StringLength(20)]
        public string? StudentCardID { get; set; }

        [Column("GPA")]
        public double? GPA { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}

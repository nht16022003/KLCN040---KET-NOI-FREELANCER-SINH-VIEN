using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        [Column("userID")]
        [StringLength(20)]
        public string UserID { get; set; } = string.Empty;

        [Column("adminRole")]
        [Required]
        [StringLength(30)]
        public string AdminRole { get; set; } = "ADMIN";

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maAdmin { get; set; }

        [Required]
        [MaxLength(30)]
        public string hotenAdmin { get; set; } = string.Empty;

        public int maUser { get; set; }

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }

    }
}
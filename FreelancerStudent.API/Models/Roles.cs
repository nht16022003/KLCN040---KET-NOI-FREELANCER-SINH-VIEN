using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maRole { get; set; }

        [Required]
        [MaxLength(30)]
        public string tenRole { get; set; } = string.Empty;

        public virtual ICollection<Users> Users { get; set; } = new List<Users>(); //1 Role có nhiều Users
    }
}
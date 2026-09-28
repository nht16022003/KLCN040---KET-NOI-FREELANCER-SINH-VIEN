using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Models
{
    [Table("KyNang")]
    public class KyNang
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maKyNang { get; set; }

        [Required]
        [MaxLength(50)]
        public string tenKyNang { get; set; } = string.Empty;

        public virtual ICollection<FreelancerStudent_KyNang> FreelancerStudent_KyNangs { get; set; } = new List<FreelancerStudent_KyNang>();
    }
}
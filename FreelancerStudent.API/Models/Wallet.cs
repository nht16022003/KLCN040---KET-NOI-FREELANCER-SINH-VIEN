using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Wallet")]
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maWallet { get; set; }

        public int maUser { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? soDuKhaDung { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? soDuDongBang { get; set; } = 0;

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Wallet")]
    public class Wallet
    {
        [Key]
        [Column("walletID")]
        [StringLength(20)]
        public string WalletID { get; set; } = string.Empty;

        [Column("userID")]
        [Required]
        [StringLength(20)]
        public string UserID { get; set; } = string.Empty;

        [Column("soDuKhadung", TypeName = "decimal(18,2)")]
        public decimal SoDuKhadung { get; set; } = 0;

        [Column("soDuDongBang", TypeName = "decimal(18,2)")]
        public decimal SoDuDongBang { get; set; } = 0;

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}

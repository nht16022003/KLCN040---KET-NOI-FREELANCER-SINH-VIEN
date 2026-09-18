using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [Column("cusID")]
        [StringLength(20)]
        public string CusID { get; set; } = string.Empty;

        [Required]
        [Column("userID")]
        [StringLength(20)]
        public string UserID { get; set; } = string.Empty;

        [Column("rating")]
        public double? Rating { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }
    }
}

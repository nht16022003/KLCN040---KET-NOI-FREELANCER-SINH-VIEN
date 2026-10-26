using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("TranhChap")]
    public class TranhChap
    {
        [Key]
        public int maTranhChap { get; set; }

        [Required, MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        public DateTime? ngayGiaiQuyet { get; set; }
    }
}

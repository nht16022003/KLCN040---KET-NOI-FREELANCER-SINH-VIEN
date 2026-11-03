using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("LuuViec_ChoFreelancer")]
    public class LuuViec_ChoFreelancer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maLuuViec { get; set; }

        public int maFreelancerStudent { get; set; }

        [MaxLength(20)]
        public string maJob { get; set; } = string.Empty;

        public DateTime ngayLuu { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? ghiChu { get; set; }

        [ForeignKey("maFreelancerStudent")]
        public virtual FreelancerStudents? FreelancerStudent { get; set; }

        [ForeignKey("maJob")]
        public virtual JobPost? JobPost { get; set; }
    }
}


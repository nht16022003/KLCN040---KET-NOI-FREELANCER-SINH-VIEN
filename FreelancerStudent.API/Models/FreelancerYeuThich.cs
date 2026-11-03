using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("FreelancerYeuThich")]
    public class FreelancerYeuThich
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maBookMark { get; set; }

        public int maNhaTuyenDung { get; set; }

        public int maFreelancerStudent { get; set; }

        [MaxLength(255)]
        public string? ghiChu { get; set; }

        public DateTime ngayLuu { get; set; } = DateTime.Now;

        [ForeignKey("maNhaTuyenDung")]
        public virtual NhaTuyenDung? NhaTuyenDung { get; set; }

        [ForeignKey("maFreelancerStudent")]
        public virtual FreelancerStudents? FreelancerStudent { get; set; }
    }
}


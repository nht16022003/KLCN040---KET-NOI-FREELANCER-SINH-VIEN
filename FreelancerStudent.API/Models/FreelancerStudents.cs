using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("FreelancerStudents")]
    public class FreelancerStudents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maFreelancerStudents { get; set; }

        [Required]
        [MaxLength(20)]
        public string maChuyenNganh { get; set; } = string.Empty;

        public int maUser { get; set; }

        [Required]
        [MaxLength(10)]
        public string maTruong { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string tenTruong { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string diaDiemTruong { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;



        [MaxLength(100)]
        public string? ngonNgu { get; set; }

        [MaxLength(150)]
        public string? kyNangCoBan { get; set; }

        [Required]
        public int namThu { get; set; } = 1;

        [Required]
        public double GPA { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        public string nienKhoa { get; set; } = string.Empty;

        public string? gioithieu { get; set; }


        [Required]
        public bool trangthaiNhanViec { get; set; } = true;

        public decimal? chiPhiTu { get; set; }

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }

        [ForeignKey("maChuyenNganh")]
        public virtual ChuyenNganh? ChuyenNganh { get; set; }

        public virtual ICollection<KyNang> KyNang { get; set; } = new List<KyNang>();

        public virtual ICollection<FreelancerStudent_KyNang> FreelancerStudent_KyNangs { get; set; } = new List<FreelancerStudent_KyNang>();

        public virtual ICollection<UngTuyen> UngTuyens { get; set; } = new List<UngTuyen>();
    }
}
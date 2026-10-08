using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Portfolio")]
    public class Portfolio
    {
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string maPortfolio { get; set; } = string.Empty;

        public int maFreelancerStudents { get; set; }

        public string? moTaBanThan { get; set; }

        [Column("url_video", TypeName = "varchar(255)")]
        public string? url_video { get; set; }

        [ForeignKey(nameof(maFreelancerStudents))]
        public virtual FreelancerStudents? FreelancerStudent { get; set; }

    }
}
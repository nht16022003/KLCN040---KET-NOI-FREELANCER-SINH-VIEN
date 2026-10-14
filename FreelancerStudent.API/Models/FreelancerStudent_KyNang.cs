using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("FreelancerStudent_KyNang")]
    public class FreelancerStudent_KyNang
    {
        [Key]
        [Column(Order = 0)]
        public int maFreelancerStudents { get; set; }

        [Key]
        [Column(Order = 1)]
        public int maKyNang { get; set; }


        // Navigation
        public virtual FreelancerStudents? FreelancerStudent { get; set; }
        public virtual KyNang? KyNang { get; set; }
    }
}
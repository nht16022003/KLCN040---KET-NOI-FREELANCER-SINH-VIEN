using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("ChuyenNganh")]
    public class ChuyenNganh
    {
        [Key]
        [Required]
        [MaxLength(20)]
        public string maChuyenNganh { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string tenChuyenNganh { get; set; } = string.Empty;

        public virtual ICollection<FreelancerStudents> FreelancerStudents { get; set; } = new List<FreelancerStudents>();
        //1 chuyên ngành có thể có nhiều freelancerstudent
    }
}
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.Web.Models.Entities
{
    public class FreelancerSV
    {
        [Key]
        public string free_ID { get; set; } = string.Empty;
        public string userID { get; set; } = string.Empty;
        public DateTime? birth { get; set; }
        public string? gender { get; set; }
        public string? address { get; set; }
        public string? university { get; set; }
        public int? AcademicYearStart { get; set; }
        public int? AcademicYearEnd { get; set; }
        public string? major { get; set; }
        public string? studentCardID { get; set; }
        public double? GPA { get; set; }

        [ForeignKey("userID")]
        public virtual Users Users { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("UngTuyen")]
    public class UngTuyen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maUngTuyen { get; set; }

        [MaxLength(20)]
        public string maJob { get; set; } = string.Empty; //Khóa ngoại

        public int maFreelancerStudent { get; set; } //Khóa ngoại

        public string thuGioiThieu { get; set; } = string.Empty;

        public decimal? thulaoDeXuat { get; set; }

        public string? thoiGianHoanThanhDeXuat { get; set; }

        public string? fileCV { get; set; }

        public DateTime ngayUngTuyen { get; set; } = DateTime.UtcNow;

        public string trangThaiUngTuyen { get; set; } = "ChoDuyet";


        //Khóa ngoại
        [ForeignKey("maJob")]
        public virtual JobPost? JobPosts { get; set; }

        [ForeignKey("maFreelancerStudent")]

        public virtual FreelancerStudents? FreelamcerStudents { get; set; }
    }
}
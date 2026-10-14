using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("NhaTuyenDung")]
    public class NhaTuyenDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maNhaTuyenDung { get; set; }

        public int maUser { get; set; }


        public string? gioithieu { get; set; }

        [MaxLength(100)]
        public string? tencongty { get; set; }

        [MaxLength(100)]
        public string? linhvuc { get; set; }
        [MaxLength(200)]
        public string? diachi { get; set; }

        [MaxLength(500)]
        public string? link { get; set; }

        [MaxLength(500)]
        public string? logo { get; set; }

        public DateTime ngayDangKy { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string trangthai { get; set; } = "Active";

        public double? sosaodanhgia { get; set; }

        [ForeignKey("maUser")]
        public Users? User { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>(); //Mỗi nhà tuyển dụng có nhiều JobPost
    }
}
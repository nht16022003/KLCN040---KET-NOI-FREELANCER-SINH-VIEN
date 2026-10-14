using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string hotenUser { get; set; } = string.Empty;
        [Required]
        [MaxLength(30)]
        public string tenTaiKhoanUser { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string pashWordHash { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string emailUser { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? sdtUser { get; set; }

        [MaxLength(500)]
        public string? avatarUrl { get; set; }

        public DateTime? ngaysinh { get; set; }

        [Required]
        [MaxLength(20)]
        public string status { get; set; } = "ACTIVE";


        public DateTime ngayTao { get; set; } = DateTime.UtcNow;

        public int maRole { get; set; }

        //Navigation Propertites
        [ForeignKey("maRole")] //Cột chứa id của bảng khác
        public virtual Roles? Roles { get; set; }

        //Navigation Properties không phải là Foreign Key
        public virtual FreelancerStudents? FreelancerStudents { get; set; } //cho phép đi từ User -> FreelancerStudents

        public virtual NhaTuyenDung? NhaTuyenDung { get; set; }

        public virtual Admin? Admin { get; set; }

        public virtual Wallet? Wallet { get; set; }

    }
}
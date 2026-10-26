using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("LichSu_XuLyTaiKhoan")]
    public class LichSu_XuLyTaiKhoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maLichSu { get; set; }

        [Required]
        public int maUser { get; set; }

        [Required]
        public int maAdmin { get; set; }

        [MaxLength(50)]
        public string? hanhDong { get; set; }

        [Required]
        [MaxLength(500)]
        public string lyDo { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? trangThaiTruocKhiXuLy { get; set; }

        [MaxLength(30)]
        public string? trangThaiSauKhiXuLy { get; set; }

        public DateTime ngayXuLy { get; set; } = DateTime.Now;

        [ForeignKey("maUser")]
        public virtual Users? User { get; set; }

        [ForeignKey("maAdmin")]
        public virtual Admin? Admin { get; set; }
    }
}

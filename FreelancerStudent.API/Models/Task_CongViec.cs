using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Task_CongViec")]
    public class Task_CongViec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maTask { get; set; }

        [Required]
        [MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        public int? maGiaiDoan { get; set; }

        [Required]
        [MaxLength(500)]
        public string tenTask { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string moTa { get; set; } = string.Empty;

        public DateTime? hanHoanThanh { get; set; }

        [Required]
        [MaxLength(30)]
        public string trangThai { get; set; } = "ChuaThucHien";

        public DateTime ngayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("maHD")]
        public virtual HopDong? HopDong { get; set; }

        [ForeignKey("maGiaiDoan")]
        public virtual GiaiDoan_HopDong? GiaiDoan_HopDong { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("CauHinhPhiHoaHong_PhiDangBai")]
    public class CauHinhPhiHoaHong
    {
        [Key]
        public int maCauHinh { get; set; }

        [MaxLength(100)]
        public string? tenCauHinh { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal giaTri { get; set; }

        [Required, MaxLength(20)]
        public string loaiCauHinh { get; set; } = string.Empty;

        public bool hieuLuc { get; set; }
    }
}

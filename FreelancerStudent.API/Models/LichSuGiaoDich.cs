using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("LichSuGiaoDich")]
    public class LichSuGiaoDich
    {
        [Key]
        public int maGiaoDich { get; set; }

        public int maWallet { get; set; }

        [Required]
        [MaxLength(50)]
        public string loaiGiaoDich { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal soTien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuTruocGD { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuSauGD { get; set; }

        [MaxLength(255)]
        public string? noiDung { get; set; }

        [MaxLength(50)]
        public string? phuongThucThanhToan { get; set; }

        [MaxLength(100)]
        public string? maGiaoDichNgoai { get; set; }

        [MaxLength(30)]
        public string? trangThai { get; set; }

        public DateTime? ngayTao { get; set; }
    }
}
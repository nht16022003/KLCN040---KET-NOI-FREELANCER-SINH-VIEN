using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("YeuCauRutTien")]
    public class YeuCauRutTien
    {
        [Key]
        public int maRutTien { get; set; }

        public int maWallet { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal soTienRut { get; set; }

        [Required]
        [MaxLength(100)]
        public string tenNganHang { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string soTaiKhoan { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string tenChuTaiKhoan { get; set; } = string.Empty;

        public DateTime? ngayYeuCau { get; set; }

        public DateTime? ngayXuLy { get; set; }

        [MaxLength(50)]
        public string? trangThai { get; set; }

        [MaxLength(255)]
        public string? lyDoTuChoi { get; set; }

        public int? maAdminXuLy { get; set; }

        [ForeignKey("maWallet")]
        public virtual Wallet? Wallet { get; set; }

        [ForeignKey("maAdminXuLy")]
        public virtual Admin? Admin { get; set; }
    }
}

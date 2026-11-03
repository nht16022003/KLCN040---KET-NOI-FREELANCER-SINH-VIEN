using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("DieuChinhSoDu")]
    public class DieuChinhSoDu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maDieuChinh { get; set; }

        [Required]
        public int maWallet { get; set; }

        [Required]
        public int maAdmin { get; set; }

        [Required]
        [MaxLength(30)]
        public string loaiDieuChinh { get; set; } = "CongTien"; // CongTien, TruTien

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal soTienDieuChinh { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuTruoc { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuSau { get; set; }

        [Required]
        [MaxLength(500)]
        public string lyDo { get; set; } = string.Empty;

        public DateTime ngayDieuChinh { get; set; } = DateTime.Now;

        [ForeignKey("maWallet")]
        public virtual Wallet? Wallet { get; set; }

        [ForeignKey("maAdmin")]
        public virtual Admin? Admin { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("NhaTuyenDung")]
    public class NhaTuyenDung
    {
        [Key]
        [Column("employerID")]
        [StringLength(20)]
        public string EmployerID { get; set; } = string.Empty;

        [Required]
        [Column("cusID")]
        [StringLength(20)]
        public string CusID { get; set; } = string.Empty;

        [Required]
        [Column("companyName")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Column("companyLogoUrl")]
        [StringLength(500)]
        public string? CompanyLogoUrl { get; set; }

        [Column("industry")]
        [StringLength(150)]
        public string? Industry { get; set; }

        [Column("companySize")]
        [StringLength(30)]
        public string? CompanySize { get; set; }

        [Column("companyWebsite")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [Column("companyAddress")]
        [StringLength(300)]
        public string? CompanyAddress { get; set; }

        [Column("taxCode")]
        [StringLength(30)]
        public string? TaxCode { get; set; }

        [Column("companyVerified")]
        public bool CompanyVerified { get; set; } = false;

        [Column("verifiedAt")]
        public DateTime? VerifiedAt { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(CusID))]
        public virtual KhachHang? KhachHang { get; set; }
    }
}

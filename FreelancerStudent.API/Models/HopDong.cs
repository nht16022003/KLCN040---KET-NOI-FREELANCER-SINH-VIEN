using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("HopDong")]
    public class HopDong
    {
        [Key]
        [Column("maHD")]
        [StringLength(20)]
        public string MaHD { get; set; } = string.Empty;

        [Column("jobID")]
        [Required]
        [StringLength(20)]
        public string JobID { get; set; } = string.Empty;

        [Column("freeID")]
        [Required]
        [StringLength(20)]
        public string FreeID { get; set; } = string.Empty;

        [Column("ngayBatDau")]
        public DateTime? NgayBatDau { get; set; }

        [Column("hanHoanThanh")]
        public DateTime? HanHoanThanh { get; set; }

        [Column("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [Column("sotienkyquy", TypeName = "decimal(18,2)")]
        public decimal? Sotienkyquy { get; set; }

        [Column("trangThai")]
        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; } = "DangThucHien";

        [Column("projectCode")]
        [StringLength(30)]
        public string? ProjectCode { get; set; }

        [Column("contractType")]
        [StringLength(20)]
        public string ContractType { get; set; } = "FIXED_PRICE";

        [Column("tongGiaTri", TypeName = "decimal(18,2)")]
        public decimal? TongGiaTri { get; set; }

        [Column("tienDo")]
        public int TienDo { get; set; } = 0;

        [Column("paymentStatus")]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "UNPAID";

        [Column("paymentType")]
        [StringLength(30)]
        public string PaymentType { get; set; } = "ONE_TIME";

        [Column("workMode")]
        [StringLength(20)]
        public string? WorkMode { get; set; }

        [Column("fieldName")]
        [StringLength(150)]
        public string? FieldName { get; set; }

        [Column("dieuKhoanChiTiet")]
        public string? DieuKhoanChiTiet { get; set; }

        [Column("proposalStatus")]
        [StringLength(20)]
        public string ProposalStatus { get; set; } = "DRAFT";

        [ForeignKey("JobID")]
        public virtual JobPost? JobPost { get; set; }

        [ForeignKey("FreeID")]
        public virtual FreelancerSV? FreelancerSV { get; set; }
    }
}

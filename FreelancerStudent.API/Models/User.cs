using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("userID")]
        [StringLength(20)]
        public string UserID { get; set; } = string.Empty;

        [Required]
        [Column("userName")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("passwordHash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("phoneNumber")]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Required]
        [Column("loaiUser")]
        [StringLength(20)]
        public string LoaiUser { get; set; } = string.Empty; // KhachHang, FreelancerSV, Admin

        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, SUSPENDED, LOCKED, INACTIVE

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual KhachHang? KhachHang { get; set; }
        public virtual FreelancerSV? FreelancerSV { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}

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

        [Column("userName")]
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Column("passwordHash")]
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("email")]
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("phoneNumber")]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Column("loaiUser")]
        [Required]
        [StringLength(20)]
        public string LoaiUser { get; set; } = string.Empty; // KhachHang, FreelancerSV, Admin

        [Column("status")]
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, SUSPENDED, LOCKED, INACTIVE

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual KhachHang? KhachHang { get; set; }
        public virtual FreelancerSV? FreelancerSV { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}

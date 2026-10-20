
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("BanGiao_SanPham")]
    public class BanGiao_SanPham
    {
        [Key]
        public int maBanGiao { get; set; }

        [Required]
        [MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string phienBan { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string fileSanPhamUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? linkDemo { get; set; }

        [Required]
        [MaxLength(1000)]
        public string motaBanGiao { get; set; } = string.Empty;

        public DateTime ngayNop { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        public string trangThai { get; set; } = "ChoNghiemThu";

        [MaxLength(1000)]
        public string? phanHoi { get; set; }

        public DateTime? ngayPhanHoi { get; set; }

        [ForeignKey(nameof(maHD))]
        public virtual HopDong? HopDong { get; set; }

        public virtual ICollection<YeuCauChinhSua> YeuCauChinhSuas
            { get; set; } = new List<YeuCauChinhSua>();
    }
}

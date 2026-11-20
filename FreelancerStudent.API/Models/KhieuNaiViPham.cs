using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    public class KhieuNaiViPham
    {
        [Key]
        public int MaKhieuNai { get; set; }
        public int MaBaoCao { get; set; }
        public int MaFreelancerStudent { get; set; }
        [Required, MaxLength(2000)] public string NoiDung { get; set; } = string.Empty;
        [Required, MaxLength(30)] public string TrangThai { get; set; } = "CHO_XU_LY";
        [MaxLength(2000)] public string? KetQuaAdmin { get; set; }
        public int? MaAdminUser { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public DateTime? NgayXuLy { get; set; }
        [ForeignKey(nameof(MaBaoCao))] public virtual BaoCaoViPhamHopDong? BaoCao { get; set; }
        [ForeignKey(nameof(MaFreelancerStudent))] public virtual FreelancerStudents? FreelancerStudent { get; set; }
        [ForeignKey(nameof(MaAdminUser))] public virtual Users? AdminUser { get; set; }
    }
}

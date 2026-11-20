using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    public class LichSuDiemUyTin
    {
        [Key]
        public int MaLichSu { get; set; }
        public int MaFreelancerStudent { get; set; }
        public int? MaBaoCao { get; set; }
        public int? MaNguoiThucHien { get; set; }
        [Required, MaxLength(30)] public string LoaiThayDoi { get; set; } = string.Empty;
        [Required, MaxLength(500)] public string LyDo { get; set; } = string.Empty;
        [Column(TypeName = "decimal(8,2)")] public decimal SoDiemThayDoi { get; set; }
        [Column(TypeName = "decimal(8,2)")] public decimal DiemTruoc { get; set; }
        [Column(TypeName = "decimal(8,2)")] public decimal DiemSau { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(MaFreelancerStudent))] public virtual FreelancerStudents? FreelancerStudent { get; set; }
        [ForeignKey(nameof(MaBaoCao))] public virtual BaoCaoViPhamHopDong? BaoCao { get; set; }
        [ForeignKey(nameof(MaNguoiThucHien))] public virtual Users? NguoiThucHien { get; set; }
    }
}

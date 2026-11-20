using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    public class BaoCaoViPhamHopDong
    {
        [Key]
        public int MaBaoCao { get; set; }

        [Required, MaxLength(20)]
        public string MaHD { get; set; } = string.Empty;

        public int MaNhaTuyenDungUser { get; set; }
        public int MaFreelancerStudent { get; set; }

        [Required, MaxLength(100)]
        public string LoaiViPham { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string MoTa { get; set; } = string.Empty;

        public DateTime? ThoiGianViPham { get; set; }
        public DateTime NgayBaoCao { get; set; } = DateTime.UtcNow;
        public DateTime HanGiaiTrinh { get; set; }

        [Required, MaxLength(40)]
        public string TrangThai { get; set; } = "CHO_TIEP_NHAN";

        public bool? FreelancerDongY { get; set; }
        [MaxLength(2000)] public string? GiaiTrinh { get; set; }
        public DateTime? NgayGiaiTrinh { get; set; }
        [MaxLength(2000)] public string? PhanHoiNhaTuyenDung { get; set; }
        public DateTime? NgayPhanHoiNhaTuyenDung { get; set; }
        public bool FreelancerDongYGiaHan { get; set; }
        public bool NhaTuyenDungDongYGiaHan { get; set; }
        [MaxLength(50)] public string? KetLuan { get; set; }
        [MaxLength(50)] public string? MucDoViPham { get; set; }
        [MaxLength(2000)] public string? GhiChuAdmin { get; set; }
        public int? MaAdminUser { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public int? SoLanViPham { get; set; }
        [Column(TypeName = "decimal(8,2)")] public decimal? SoDiemTru { get; set; }
        [Column(TypeName = "decimal(8,2)")] public decimal? DiemTruocXuLy { get; set; }
        [Column(TypeName = "decimal(8,2)")] public decimal? DiemSauXuLy { get; set; }

        [ForeignKey(nameof(MaHD))] public virtual HopDong? HopDong { get; set; }
        [ForeignKey(nameof(MaFreelancerStudent))] public virtual FreelancerStudents? FreelancerStudent { get; set; }
        [ForeignKey(nameof(MaNhaTuyenDungUser))] public virtual Users? NhaTuyenDungUser { get; set; }
        [ForeignKey(nameof(MaAdminUser))] public virtual Users? AdminUser { get; set; }
        public virtual ICollection<TepMinhChungViPham> TepMinhChungs { get; set; } = new List<TepMinhChungViPham>();
    }
}

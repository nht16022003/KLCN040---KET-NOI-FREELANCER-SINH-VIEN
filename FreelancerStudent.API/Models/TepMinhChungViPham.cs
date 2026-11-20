using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    public class TepMinhChungViPham
    {
        [Key]
        public int MaTep { get; set; }
        public int MaBaoCao { get; set; }
        public int MaNguoiTai { get; set; }
        [Required, MaxLength(30)] public string LoaiTep { get; set; } = string.Empty;
        [Required, MaxLength(255)] public string TenTep { get; set; } = string.Empty;
        [Required, MaxLength(500)] public string DuongDan { get; set; } = string.Empty;
        public DateTime NgayTai { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(MaBaoCao))] public virtual BaoCaoViPhamHopDong? BaoCao { get; set; }
        [ForeignKey(nameof(MaNguoiTai))] public virtual Users? NguoiTai { get; set; }
    }
}

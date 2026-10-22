using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("DanhGia_NhanXet")]
    public class DanhGia_NhanXet
    {
        [Key]
        public int maDanhGia { get; set; }

        [Required, MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        public int maNguoiDanhGia { get; set; }
        public int maNguoiDuocDanhGia { get; set; }
        public int soSao { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }

        [MaxLength(1000)]
        public string? nhanXet { get; set; }

        public DateTime ngayDanhGia { get; set; } = DateTime.Now;
        public DateTime? ngayCapNhat { get; set; }
        public int soLanChinhSua { get; set; }

        public virtual HopDong? HopDong { get; set; }
        public virtual Users? NguoiDanhGia { get; set; }
        public virtual Users? NguoiDuocDanhGia { get; set; }
    }
}

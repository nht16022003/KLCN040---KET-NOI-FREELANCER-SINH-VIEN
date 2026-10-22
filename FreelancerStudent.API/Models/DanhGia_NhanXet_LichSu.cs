using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("DanhGia_NhanXet_LichSu")]
    public class DanhGia_NhanXet_LichSu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maLichSu { get; set; }

        public int maDanhGia { get; set; }
        public int soSao { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }

        [MaxLength(1000)]
        public string? nhanXet { get; set; }

        public DateTime ngayLuu { get; set; } = DateTime.Now;
        public virtual DanhGia_NhanXet? DanhGia { get; set; }
    }
}

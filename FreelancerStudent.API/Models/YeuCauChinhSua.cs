
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("YeuCauChinhSua")]
    public class YeuCauChinhSua
    {
        [Key]
        public int maYeuCauChinhSua { get; set; }

        [Required]
        public int maBanGiao { get; set; }

        [Required]
        public string noiDungYeuCau { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string trangThai { get; set; } = "ChoChinhSua";

        public DateTime ngayYeuCau { get; set; } = DateTime.Now;

        public DateTime? ngayHoanThanh { get; set; }

        [ForeignKey(nameof(maBanGiao))]
        public virtual BanGiao_SanPham? BanGiaoSanPham { get; set; }
    }
}

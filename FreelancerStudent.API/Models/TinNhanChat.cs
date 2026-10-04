using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("TinNhanChat")]
    public class TinNhanChat
    {
        [Key]
        public int maTinNhan { get; set; }
        public int maPhongChat { get; set; }
        public int maNguoiGui { get; set; }
        public string? noiDung { get; set; }
        public string? fileDinhKem { get; set; }
        public string loaiTinNhan { get; set; } = "Text";
        public bool daDoc { get; set; } = false;
        public DateTime ngayGui { get; set; } = DateTime.Now;

        [ForeignKey("maPhongChat")]
        public virtual PhongChat? PhongChat { get; set; }

        [ForeignKey("maNguoiGui")]
        public virtual Users? NguoiGui { get; set; }
    }
}

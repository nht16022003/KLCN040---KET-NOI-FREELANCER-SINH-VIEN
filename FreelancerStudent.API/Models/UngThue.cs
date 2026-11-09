using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("UngThue")]
    public class UngThue
    {
        [Key]
        public int maYeuCau { get; set; }

        public int maNhaTuyenDung { get; set; }

        public int maFreelancerStudent { get; set; }

        public string? maJob { get; set; }

        [Required]
        [MaxLength(200)]
        public string tieuDeCongViec { get; set; } = string.Empty;

        public string? moTaCongViec { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? nganSachDeNghi { get; set; }

        [MaxLength(50)]
        public string? thoiHanDuKien { get; set; }

        public DateTime ngayGui { get; set; } = DateTime.Now;

        [MaxLength(30)]
        public string trangThai { get; set; } = "ChoPhanHoi"; // ChoPhanHoi, DaChapNhan, DaTuChoi, DaHuy, DaTaoHopDong

        [MaxLength(255)]
        public string? lyDoTuChoi { get; set; }

        [ForeignKey("maNhaTuyenDung")]
        public virtual NhaTuyenDung? NhaTuyenDung { get; set; }

        [ForeignKey("maFreelancerStudent")]
        public virtual FreelancerStudents? FreelancerStudent { get; set; }

        [ForeignKey("maJob")]
        public virtual JobPost? JobPost { get; set; }
    }
}



using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("HopDong")]
    public class HopDong
    {
        [Key]
        [MaxLength(20)]
        public string maHD { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string maJob { get; set; } = string.Empty;

        public int maFreelancerStudent { get; set; }

        public DateTime? ngayBatDau { get; set; }

        public DateTime? ngayKetThuc { get; set; }

        public DateTime? ngayHoanThanh { get; set; }

        public DateTime? ngayDongHopDong { get; set; }

        public bool thanhToanTuDong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? sotienkyquy { get; set; }

        public string? hinhthuclamviec { get; set; }

        [MaxLength(30)]
        public string? trangThai { get; set; }

        public string? dieuKhoan { get; set; }

        // Một hợp đồng thuộc về một bài tuyển dụng
        [ForeignKey("maJob")]
        public virtual JobPost? JobPost { get; set; }

        // Một hợp đồng thuộc về một Freelancer
        [ForeignKey("maFreelancerStudent")]
        public virtual FreelancerStudents? FreelancerStudent
        {
            get;
            set;
        }

        // Danh sách các giai đoạn / mốc bàn giao của hợp đồng
        public virtual ICollection<GiaiDoan_HopDong>? GiaiDoan_HopDongs { get; set; } = new List<GiaiDoan_HopDong>();

        // Danh sách các đầu việc / task tiến độ của hợp đồng
        public virtual ICollection<Task_CongViec>? Tasks { get; set; } = new List<Task_CongViec>();
    }
}
using System;

namespace MOCK.Models.Entities
{
    public class MinhChungFreelancerStudent
    {
        public int MaMinhChung { get; set; }
        public int MaUser { get; set; }
        public string LoaiMinhChung { get; set; } = string.Empty; // Thẻ sinh viên, giấy xác nhận sinh viên
        public string FileMinhChung { get; set; } = string.Empty;
        public DateTime NgayNop { get; set; } = DateTime.Now;
        public DateTime? NgayXacMinh { get; set; }
        public string TrangThaiGuiMinhChung { get; set; } = "Đang gửi"; // Đang gửi, Đã xác minh, Đã từ chối
        public string? LydoTuChoi { get; set; }
        public int? NguoiXacMinh { get; set; } // maUser của Admin xác minh

        // Navigation properties
        public User? User { get; set; }
        public User? NguoiXacMinhUser { get; set; }
    }
}

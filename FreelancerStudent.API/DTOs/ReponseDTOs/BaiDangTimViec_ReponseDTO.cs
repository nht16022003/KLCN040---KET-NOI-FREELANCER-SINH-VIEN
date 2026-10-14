namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class BaiDangTimViec_ReponseDTO
    {
        public string maBaiDang { get; set; } = string.Empty;
        public int maFreelancerStudent { get; set; }
        public string tieude { get; set; } = string.Empty;
        public string? mota { get; set; }
        public string? kynang { get; set; }
        public double? mucGiaTu { get; set; }
        public DateTime thoigiandang { get; set; }
        public string trangthai { get; set; } = string.Empty;

        // Thông tin sinh viên
        public int maUser { get; set; }
        public string? tenUser { get; set; }
        public string? avatar { get; set; }
        public string? tenTruong { get; set; }
        public string? tenChuyenNganh { get; set; }
        public int namThu { get; set; }
        public double GPA { get; set; }
    }
}

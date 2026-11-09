namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class FileDinhKemChat_DTO
    {
        public int maTinNhan { get; set; }
        public int maPhongChat { get; set; }
        public string tenFile { get; set; } = string.Empty;
        public string fileUrl { get; set; } = string.Empty;
        public string? loaiFile { get; set; }
        public int maNguoiGui { get; set; }
        public string tenNguoiGui { get; set; } = string.Empty;
        public string? avatarNguoiGui { get; set; }
        public DateTime ngayGui { get; set; }
    }
}


namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class TinNhan_ReponseDTO
    {
        public int maTinNhan { get; set; }
        public int maPhongChat { get; set; }

        // Thông tin người gửi
        public int maNguoiGui { get; set; }
        public string? tenNguoiGui { get; set; }
        public string? avatarNguoiGui { get; set; }

        // Nội dung tin nhắn
        public string? noiDung { get; set; }
        public string? fileDinhKem { get; set; }
        public string loaiTinNhan { get; set; } = "Text";
        public bool daDoc { get; set; }
        public DateTime ngayGui { get; set; }
    }
}

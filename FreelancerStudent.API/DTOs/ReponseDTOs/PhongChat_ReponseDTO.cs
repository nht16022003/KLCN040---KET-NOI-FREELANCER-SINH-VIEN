namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class PhongChat_ReponseDTO
    {
        public int maPhongChat { get; set; }

        // Thông tin Nhà tuyển dụng (Client)
        public int maUserClient { get; set; }
        public string? tenClient { get; set; }
        public string? avatarClient { get; set; }

        // Thông tin Sinh viên (Freelancer)
        public int maFreelancerStudent { get; set; }
        public int maUserFreelancer { get; set; }
        public string? tenFreelancer { get; set; }
        public string? avatarFreelancer { get; set; }

        // Thông tin công việc trao đổi (nếu có)
        public string? maJob { get; set; }
        public string? tieuDeJob { get; set; }
        public decimal? thulaoJob { get; set; }

        // Thông tin trạng thái & tin nhắn gần nhất
        public string? tinNhanCuoi { get; set; }
        public DateTime? thoiGianTinNhanCuoi { get; set; }
        public int soTinNhanChuaDoc { get; set; }
        public string trangThai { get; set; } = "Active";
        public DateTime ngayTao { get; set; }
    }
}

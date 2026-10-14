namespace FreelancerStudent.Web.ViewModels.Chat
{
    public class PhongChatViewModel
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

        // Thông tin công việc trao đổi
        public string? maJob { get; set; }
        public string? tieuDeJob { get; set; }
        public decimal? thulaoJob { get; set; }

        // Thông tin tin nhắn gần nhất
        public string? tinNhanCuoi { get; set; }
        public DateTime? thoiGianTinNhanCuoi { get; set; }
        public int soTinNhanChuaDoc { get; set; }
        public string trangThai { get; set; } = "Active";
        public DateTime ngayTao { get; set; }
    }

    public class TinNhanViewModel
    {
        public int maTinNhan { get; set; }
        public int maPhongChat { get; set; }

        // Người gửi
        public int maNguoiGui { get; set; }
        public string? tenNguoiGui { get; set; }
        public string? avatarNguoiGui { get; set; }

        // Nội dung
        public string? noiDung { get; set; }
        public string? fileDinhKem { get; set; }
        public string loaiTinNhan { get; set; } = "Text";
        public bool daDoc { get; set; }
        public DateTime ngayGui { get; set; }
    }

    public class ChatIndexViewModel
    {
        public List<PhongChatViewModel> DanhSachPhong { get; set; } = new();
        public PhongChatViewModel? PhongHienTai { get; set; }
        public List<TinNhanViewModel> LichSuTinNhan { get; set; } = new();
        public int CurrentUserId { get; set; }
        public string? CurrentUserRole { get; set; }
    }

    public class GuiTinNhanWebModel
    {
        public int maPhongChat { get; set; }
        public string noiDung { get; set; } = string.Empty;
        public string? fileDinhKem { get; set; }
    }
}

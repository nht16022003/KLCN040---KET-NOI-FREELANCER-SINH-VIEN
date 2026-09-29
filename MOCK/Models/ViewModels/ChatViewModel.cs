using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class ChatConversationItemViewModel
    {
        public PhongChat PhongChat { get; set; } = new();
        public User PartnerUser { get; set; } = new();
        public string PartnerRole { get; set; } = "Doanh nghiệp"; // Doanh nghiệp, Freelancer
        public string PartnerTitle { get; set; } = string.Empty; // Công ty FPT, SV ĐH Bách Khoa...
        public string PartnerAvatar { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = true;
        public JobPost? RelatedJob { get; set; }
        public HopDong? RelatedContract { get; set; }
    }

    public class ChatMessageItemViewModel
    {
        public TinNhan TinNhan { get; set; } = new();
        public User SenderUser { get; set; } = new();
        public bool IsMine { get; set; }
        public DuAnTrongPortfolio? PortfolioProject { get; set; }
    }

    public class ChatViewModel
    {
        public User CurrentUser { get; set; } = new();
        public string CurrentUserRole { get; set; } = "FreelancerStudent";

        // Danh sách các cuộc trò chuyện bên trái
        public List<ChatConversationItemViewModel> Conversations { get; set; } = new();
        
        // Cuộc trò chuyện đang chọn
        public ChatConversationItemViewModel? ActiveConversation { get; set; }
        
        // Tin nhắn của cuộc trò chuyện đang chọn
        public List<ChatMessageItemViewModel> ActiveMessages { get; set; } = new();

        // Danh sách dự án trong Portfolio của sinh viên (để gửi thẻ dự án vào chat)
        public List<DuAnTrongPortfolio> StudentPortfolioProjects { get; set; } = new();
    }
}

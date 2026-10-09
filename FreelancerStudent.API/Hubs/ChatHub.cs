using Microsoft.AspNetCore.SignalR;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IChatRepository _chatRepo;

        public ChatHub(IChatService chatService, IChatRepository chatRepo)
        {
            _chatService = chatService;
            _chatRepo = chatRepo;
        }

        /// <summary>
        /// Khi người dùng mở một cuộc trò chuyện, tham gia vào phòng chat đó
        /// </summary>
        public async Task ThamGiaPhongChat(int maPhongChat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Phong_{maPhongChat}");
        }

        /// <summary>
        /// Khi người dùng chuyển sang phòng chat khác hoặc đóng hộp thoại
        /// </summary>
        public async Task RoiPhongChat(int maPhongChat)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Phong_{maPhongChat}");
        }

        /// <summary>
        /// Gửi tin nhắn mới trong phòng chat và lưu vào CSDL qua ChatService
        /// </summary>
        public async Task GuiTinNhan(int maPhongChat, int maNguoiGui, string noiDung, string? fileDinhKem)
        {
            if (string.IsNullOrWhiteSpace(noiDung) && string.IsNullOrEmpty(fileDinhKem))
            {
                return;
            }

            // 1. Tạo request DTO và lưu qua ChatService
            var request = new GuiTinNhan_RequestDTO
            {
                maPhongChat = maPhongChat,
                maNguoiGui = maNguoiGui,
                noiDung = noiDung,
                fileDinhKem = fileDinhKem,
                loaiTinNhan = string.IsNullOrEmpty(fileDinhKem) ? "Text" : "File"
            };

            var tinNhanDaLuu = await _chatService.GuiTinNhanAsync(request);

            // 2. Bắn tin nhắn trực tiếp đến tất cả thành viên đang mở phòng chat này
            await Clients.Group($"Phong_{maPhongChat}").SendAsync("NhanTinNhanMoi", new
            {
                maTinNhan = tinNhanDaLuu.maTinNhan,
                maPhongChat = tinNhanDaLuu.maPhongChat,
                maNguoiGui = tinNhanDaLuu.maNguoiGui,
                tenNguoiGui = tinNhanDaLuu.tenNguoiGui,
                avatarNguoiGui = tinNhanDaLuu.avatarNguoiGui ?? "",
                noiDung = tinNhanDaLuu.noiDung,
                fileDinhKem = tinNhanDaLuu.fileDinhKem,
                loaiTinNhan = tinNhanDaLuu.loaiTinNhan,
                ngayGui = tinNhanDaLuu.ngayGui.ToString("o")
            });
        }

        /// <summary>
        /// Thông báo trạng thái người dùng đang gõ phím soạn tin nhắn
        /// </summary>
        public async Task DangSoanTinNhan(int maPhongChat, int maNguoiGui, string tenNguoiGui, bool dangSoan)
        {
            await Clients.OthersInGroup($"Phong_{maPhongChat}").SendAsync("TrangThaiDangSoanTin", new
            {
                maPhongChat = maPhongChat,
                maNguoiGui = maNguoiGui,
                tenNguoiGui = tenNguoiGui,
                dangSoan = dangSoan
            });
        }

        /// <summary>
        /// Đánh dấu tất cả tin nhắn trong phòng là đã đọc
        /// </summary>
        public async Task DanhDauDaDoc(int maPhongChat, int maUserDangXem)
        {
            await _chatRepo.DanhDauDaDocAsync(maPhongChat, maUserDangXem);
            await Clients.OthersInGroup($"Phong_{maPhongChat}").SendAsync("DaDocTatCaTinNhan", new
            {
                maPhongChat = maPhongChat,
                maUserDaDoc = maUserDangXem
            });
        }
    }
}

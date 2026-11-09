using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IChatRepository _chatRepo;
        private readonly ApplicationDBContext _context;

        public ChatHub(IChatService chatService, IChatRepository chatRepo, ApplicationDBContext context)
        {
            _chatService = chatService;
            _chatRepo = chatRepo;
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var maUserStr = httpContext?.Request.Query["maUser"].ToString();
            if (int.TryParse(maUserStr, out int maUser) && maUser > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{maUser}");
            }
            await base.OnConnectedAsync();
        }

        /// Đăng ký connection của người dùng vào group cá nhân User_{maUser}
        public async Task DangKyUser(int maUser)
        {
            if (maUser > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{maUser}");
            }
        }

        /// Khi người dùng mở một cuộc trò chuyện, tham gia vào phòng chat đó
        public async Task ThamGiaPhongChat(int maPhongChat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Phong_{maPhongChat}");
        }

        /// Khi người dùng chuyển sang phòng chat khác hoặc đóng hộp thoại
        public async Task RoiPhongChat(int maPhongChat)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Phong_{maPhongChat}");
        }

        /// Gửi tin nhắn mới trong phòng chat và lưu vào CSDL qua ChatService
        public async Task GuiTinNhan(int maPhongChat, int maNguoiGui, string noiDung, string? fileDinhKem, string? loaiTinNhan = null)
        {
            if (string.IsNullOrWhiteSpace(noiDung) && string.IsNullOrEmpty(fileDinhKem))
            {
                return;
            }

            string xacDinhLoai = loaiTinNhan ?? "Text";
            if (string.IsNullOrEmpty(loaiTinNhan) && !string.IsNullOrEmpty(fileDinhKem))
            {
                var ext = Path.GetExtension(fileDinhKem).ToLowerInvariant();
                var imgExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
                xacDinhLoai = imgExts.Contains(ext) ? "Image" : "File";
            }

            // 1. Tạo request DTO và lưu qua ChatService
            var request = new GuiTinNhan_RequestDTO
            {
                maPhongChat = maPhongChat,
                maNguoiGui = maNguoiGui,
                noiDung = string.IsNullOrWhiteSpace(noiDung) ? "[Tệp đính kèm]" : noiDung,
                fileDinhKem = fileDinhKem,
                loaiTinNhan = xacDinhLoai
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

            // 3. Xác định người nhận để phát thông báo toàn cục ra ngoài phòng chat
            try
            {
                var phong = await _chatRepo.LayPhongChatTheoMaAsync(maPhongChat);
                int recipientUserId = 0;
                if (phong != null)
                {
                    if (phong.maUserClient == maNguoiGui)
                    {
                        recipientUserId = phong.FreelancerStudent?.maUser ?? 0;
                    }
                    else
                    {
                        recipientUserId = phong.maUserClient;
                    }
                }

                if (recipientUserId > 0 && recipientUserId != maNguoiGui)
                {
                    var linkChat = $"/Chat?maPhongChat={maPhongChat}";

                    // Phát sự kiện thông báo thời gian thực tới Group cá nhân của người nhận
                    await Clients.Group($"User_{recipientUserId}").SendAsync("ThongBaoTinNhanMoi", new
                    {
                        maTinNhan = tinNhanDaLuu.maTinNhan,
                        maPhongChat = tinNhanDaLuu.maPhongChat,
                        maNguoiGui = tinNhanDaLuu.maNguoiGui,
                        tenNguoiGui = tinNhanDaLuu.tenNguoiGui,
                        avatarNguoiGui = tinNhanDaLuu.avatarNguoiGui ?? "",
                        noiDung = tinNhanDaLuu.noiDung,
                        loaiTinNhan = tinNhanDaLuu.loaiTinNhan,
                        ngayGui = tinNhanDaLuu.ngayGui.ToString("o"),
                        linkDieuHuong = linkChat
                    });

                    // Lưu / Cập nhật bảng ThongBao trong DB KLCN040_DATA
                    var existingNotif = await _context.ThongBaos
                        .FirstOrDefaultAsync(t => t.maUser == recipientUserId && t.loaiThongBao == "TinNhan" && t.linkDieuHuong == linkChat && !t.daDoc);

                    string rawNoiDung = tinNhanDaLuu.noiDung ?? "[Tệp đính kèm]";
                    string noiDungTomTat = rawNoiDung.Length > 100
                        ? rawNoiDung.Substring(0, 97) + "..."
                        : rawNoiDung;

                    if (existingNotif != null)
                    {
                        existingNotif.noiDung = noiDungTomTat;
                        existingNotif.ngayTao = DateTime.Now;
                    }
                    else
                    {
                        _context.ThongBaos.Add(new ThongBao
                        {
                            maUser = recipientUserId,
                            tieuDe = $"Tin nhắn mới từ {tinNhanDaLuu.tenNguoiGui}",
                            noiDung = noiDungTomTat,
                            loaiThongBao = "TinNhan",
                            linkDieuHuong = linkChat,
                            daDoc = false,
                            ngayTao = DateTime.Now
                        });
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatHub] Lỗi xử lý thông báo tin nhắn mới: {ex.Message}");
            }
        }

        /// Thông báo trạng thái người dùng đang gõ phím soạn tin nhắn
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

        /// Đánh dấu tất cả tin nhắn trong phòng là đã đọc
        public async Task DanhDauDaDoc(int maPhongChat, int maUserDangXem)
        {
            await _chatRepo.DanhDauDaDocAsync(maPhongChat, maUserDangXem);

            // Đánh dấu các thông báo loại 'TinNhan' của phòng chat này là đã đọc
            try
            {
                var linkChat = $"/Chat?maPhongChat={maPhongChat}";
                var notifs = await _context.ThongBaos
                    .Where(t => t.maUser == maUserDangXem && t.loaiThongBao == "TinNhan" && t.linkDieuHuong == linkChat && !t.daDoc)
                    .ToListAsync();

                if (notifs.Any())
                {
                    foreach (var n in notifs)
                    {
                        n.daDoc = true;
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatHub] Lỗi đánh dấu đọc ThongBao: {ex.Message}");
            }

            // Bắn sự kiện tới những người khác trong phòng chat
            await Clients.OthersInGroup($"Phong_{maPhongChat}").SendAsync("DaDocTatCaTinNhan", new
            {
                maPhongChat = maPhongChat,
                maUserDaDoc = maUserDangXem
            });

            // Bắn sự kiện tới tất cả các tab của chính người dùng để cập nhật lại badge Header
            await Clients.Group($"User_{maUserDangXem}").SendAsync("CapNhatTinNhanDaDoc", new
            {
                maPhongChat = maPhongChat,
                maUserDaDoc = maUserDangXem
            });
        }
    }
}

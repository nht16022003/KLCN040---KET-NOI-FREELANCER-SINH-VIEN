# 📖 HƯỚNG DẪN TRIỂN KHAI HỆ THỐNG NHẮN TIN REAL-TIME (SIGNALR + CURSOR PAGINATION)

> **Dự án**: Freelancer Student Platform  
> **Mục tiêu**: Xây dựng hệ thống nhắn tin trao đổi giữa **FreelancerStudent** và **NhaTuyenDung** với hiệu năng cao, tốc độ phản hồi tính bằng mili-giây, không bị nghẽn CSDL khi dữ liệu phình to.

---

## 📑 MỤC LỤC
1. [Kiến trúc & Luồng hoạt động](#1-kiến-trúc--luồng-hoạt-động)
2. [Thiết kế Cơ sở dữ liệu (Database Schema)](#2-thiết-kế-cơ-sở-dữ-liệu-database-schema)
3. [Thành phần 1: Giao tiếp thời gian thực với SignalR Hub](#3-thành-phần-1-giao-tiếp-thời-gian-thực-với-signalr-hub)
4. [Thành phần 2: Tải tin nhắn với Cursor Pagination](#4-thành-phần-2-tải-tin-nhắn-với-cursor-pagination)
5. [Thành phần 3: Hiển thị danh sách cuộc trò chuyện](#5-thành-phần-3-hiển-thị-danh-sách-cuộc-trò-chuyện)
6. [Bảo mật & Tối ưu mở rộng (Scale-out)](#6-bảo-mật--tối-ưu-mở-rộng-scale-out)

---

## 1. KIẾN TRÚC & LUỒNG HOẠT ĐỘNG

```
[ Client A (Sender) ] 
       │
       │ (1. Gửi tin qua WebSocket)
       ▼
[ ASP.NET Core SignalR Hub ] 
       │
       ├─► (2. Lưu tin vào SQL Server / Cache)
       │
       └─► (3. Push trực tiếp tới ConnectionId của Client B)
               │
               ▼
       [ Client B (Receiver) ] (Nhận & render ngay tức thì)
```

- **Khi mở hộp chat**: Tải 20 tin nhắn mới nhất thông qua Cursor Pagination.
- **Khi cuộn lên trên (Scroll Up)**: Tải tiếp 20 tin nhắn cũ hơn có `MaTinNhan < LastLoadedId`.
- **Khi có tin nhắn mới**: SignalR tự đẩy vào giao diện, không cần reload hay polling.

---

## 2. CÁC BẢNG CÓ SẴN TRONG CSDL (ĐÃ CÓ TRONG `KLCN040_FREELANCERSTUDENT.sql`)

Trong file SQL của dự án đã được thiết kế sẵn **3 bảng hoàn chỉnh**, bạn **không cần tạo bảng mới**:

### 2.1. Bảng `PhongChat` (Tương đương Cuộc hội thoại / Conversations)
```sql
CREATE TABLE PhongChat
(
    maPhongChat INT IDENTITY(1,1) PRIMARY KEY,
    maUserClient INT NOT NULL,              -- Nhà tuyển dụng (Users)
    maFreelancerStudent INT NOT NULL,       -- Sinh viên Freelancer (FreelancerStudents)
    maJob VARCHAR(20) NULL,                 -- Gắn với Job (JobPost) nếu có
    ngayTao DATETIME DEFAULT GETDATE(),
    trangThai NVARCHAR(20) DEFAULT N'Active',
    CONSTRAINT FK_PhongChat_Client FOREIGN KEY (maUserClient) REFERENCES Users(maUser),
    CONSTRAINT FK_PhongChat_FreelancerStudents FOREIGN KEY (maFreelancerStudent) REFERENCES FreelancerStudents(maFreelancerStudents),
    CONSTRAINT FK_PhongChat_JobPost FOREIGN KEY (maJob) REFERENCES JobPost(maJob)
);
```

### 2.2. Bảng `TinNhanChat` (Lưu từng tin nhắn)
```sql
CREATE TABLE TinNhanChat 
(
    maTinNhan INT IDENTITY(1,1) PRIMARY KEY,
    maPhongChat INT NOT NULL,
    maNguoiGui INT NOT NULL,
    noiDung NVARCHAR(MAX) NULL,
    fileDinhKem VARCHAR(500) NULL,
    loaiTinNhan NVARCHAR(30) DEFAULT N'Text', -- Text, Image, File
    daDoc BIT DEFAULT 0,
    ngayGui DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TinNhan_PhongChat FOREIGN KEY (maPhongChat) REFERENCES PhongChat(maPhongChat) ON DELETE CASCADE,
    CONSTRAINT FK_TinNhan_NguoiGui FOREIGN KEY (maNguoiGui) REFERENCES Users(maUser)
);

-- Khuyên dùng: Đánh thêm Index tối ưu Cursor Pagination:
CREATE INDEX IX_TinNhanChat_Cursor ON TinNhanChat (maPhongChat, maTinNhan DESC);
```

### 2.3. Bảng `FileGhimChat` (Ghim tệp tài liệu trong phòng chat)
```sql
CREATE TABLE FileGhimChat 
(
    maFileGhim INT IDENTITY(1,1) PRIMARY KEY,
    maPhongChat INT NOT NULL,
    maTinNhan INT NOT NULL,
    tenFile NVARCHAR(255) NOT NULL,
    fileUrl VARCHAR(500) NOT NULL,
    nguoiGhim INT NOT NULL,
    ngayGhim DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_FileGhim_PhongChat FOREIGN KEY (maPhongChat) REFERENCES PhongChat(maPhongChat),
    CONSTRAINT FK_FileGhim_TinNhan FOREIGN KEY (maTinNhan) REFERENCES TinNhanChat(maTinNhan),
    CONSTRAINT FK_FileGhim_NguoiGhim FOREIGN KEY (nguoiGhim) REFERENCES Users(maUser)
);
```

---

## 3. THÀNH PHẦN 1: GIAO TIẾP THỜI GIAN THỰC VỚI SIGNALR HUB

### 3.1. Cài đặt NuGet Packages
Trên cả **FreelancerStudent.API** hoặc **FreelancerStudent.Web**:
```bash
dotnet add package Microsoft.AspNetCore.SignalR.Common
```

### 3.2. Tạo `ChatHub.cs` ở Backend (API / Web)
```csharp
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FreelancerStudent.API.Hubs
{
    public class ChatHub : Hub
    {
        // Khi user kết nối, đưa user vào Group theo MaUser để gửi tin nhắn cá nhân
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? Context.GetHttpContext()?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnConnectedAsync();
        }

        // Tham gia vào phòng chat cụ thể (Conversation Room)
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        // Rời phòng chat
        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        // Gửi tin nhắn real-time
        public async Task SendMessage(int conversationId, int receiverUserId, string messageContent)
        {
            var senderUserId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                         ?? Context.GetHttpContext()?.Request.Query["userId"]);

            // 1. Bắn tin nhắn vào phòng chat chung
            await Clients.Group($"Conversation_{conversationId}").SendAsync("ReceiveMessage", new
            {
                ConversationId = conversationId,
                SenderId = senderUserId,
                Content = messageContent,
                SentAt = DateTime.Now
            });

            // 2. Bắn thông báo cập nhật badge hộp thư cho người nhận
            await Clients.Group($"User_{receiverUserId}").SendAsync("UpdateConversationList", new
            {
                ConversationId = conversationId,
                LastMessage = messageContent,
                SentAt = DateTime.Now
            });
        }

        // Báo trạng thái đang soạn tin (Typing Indicator)
        public async Task Typing(int conversationId, bool isTyping)
        {
            var senderUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Clients.OthersInGroup($"Conversation_{conversationId}")
                         .SendAsync("UserTyping", new { UserId = senderUserId, IsTyping = isTyping });
        }
    }
}
```

### 3.3. Đăng ký Hub trong `Program.cs`
```csharp
builder.Services.AddSignalR();

// ...
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map Endpoint
app.MapHub<ChatHub>("/chatHub");
```

### 3.4. Kết nối từ Client JavaScript (Razor View)
Thêm thư viện SignalR vào view:
```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>

<script>
    const currentUserId = @User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    // Khởi tạo kết nối
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`/chatHub?userId=${currentUserId}`)
        .withAutomaticReconnect()
        .build();

    // Lắng nghe sự kiện nhận tin nhắn mới
    connection.on("ReceiveMessage", function (data) {
        appendMessageToUI(data.senderId === currentUserId, data.content, data.sentAt);
    });

    // Lắng nghe cập nhật danh sách cuộc hội thoại bên sidebar
    connection.on("UpdateConversationList", function (data) {
        updateSidebarLastMessage(data.conversationId, data.lastMessage, data.sentAt);
    });

    // Bắt đầu kết nối
    connection.start()
        .then(() => console.log("SignalR Connected!"))
        .catch(err => console.error(err));

    // Gửi tin nhắn
    function sendMessage() {
        const input = document.getElementById("messageInput");
        const content = input.value.trim();
        if (!content) return;

        connection.invoke("SendMessage", currentConversationId, receiverUserId, content)
            .catch(err => console.error(err));

        input.value = "";
    }
</script>
```

---

## 4. THÀNH PHẦN 2: TẢI TIN NHẮN VỚI CURSOR PAGINATION

### 4.1. Tại sao dùng Cursor Pagination thay vì Offset `Skip/Take`?
- **Offset (`Skip 10000 Take 20`)**: CSDL phải duyệt qua 10.000 dòng trước $\rightarrow$ Càng nhiều tin nhắn càng chậm. Khi có tin nhắn mới gửi vào, vị trí offset bị lệch $\rightarrow$ Bị lặp lại hoặc sót tin nhắn.
- **Cursor (`WHERE MaTinNhan < @cursor ORDER BY MaTinNhan DESC LIMIT 20`)**: Nhờ Index `(MaCuocHoiThoai, MaTinNhan DESC)`, SQL Server nhảy thẳng tới bản ghi ngay lập tức $\rightarrow$ **Thời gian thực thi luôn cố định < 1-2ms dù bảng có hàng chục triệu dòng**.

### 4.2. API Controller: `ChatController.cs`
```csharp
[HttpGet("api/chat/messages")]
public async Task<IActionResult> GetMessages(
    [FromQuery] int maPhongChat, 
    [FromQuery] int? cursor = null, // maTinNhan của mẻ trước
    [FromQuery] int limit = 20)
{
    var query = _context.TinNhanChat
        .AsNoTracking()
        .Where(m => m.maPhongChat == maPhongChat);

    // Nếu có cursor -> chỉ lấy những tin cũ hơn cursor (Keyset pagination)
    if (cursor.HasValue && cursor.Value > 0)
    {
        query = query.Where(m => m.maTinNhan < cursor.Value);
    }

    var messages = await query
        .OrderByDescending(m => m.maTinNhan)
        .Take(limit)
        .Select(m => new
        {
            m.maTinNhan,
            m.maPhongChat,
            m.maNguoiGui,
            m.noiDung,
            m.fileDinhKem,
            m.loaiTinNhan,
            m.daDoc,
            m.ngayGui
        })
        .ToListAsync();

    // Đảo ngược lại theo thứ tự thời gian tăng dần để render từ trên xuống dưới
    messages.Reverse();

    int? nextCursor = messages.FirstOrDefault()?.maTinNhan; // ID tin cũ nhất trong batch
    bool hasMore = messages.Count == limit;

    return Ok(new
    {
        Data = messages,
        NextCursor = nextCursor,
        HasMore = hasMore
    });
}
```

### 4.3. Client JavaScript: Xử lý Cuộn Lên (Infinite Scroll Up)
```javascript
let currentCursor = null;
let hasMoreMessages = true;
let isLoadingMessages = false;
const messageContainer = document.getElementById("messageContainer");

async function loadMessages(maPhongChat) {
    if (isLoadingMessages || !hasMoreMessages) return;
    isLoadingMessages = true;

    const url = `/api/chat/messages?maPhongChat=${maPhongChat}&limit=20` + 
                (currentCursor ? `&cursor=${currentCursor}` : '');

    const res = await fetch(url);
    const result = await res.json();

    const previousScrollHeight = messageContainer.scrollHeight;

    // Chèn tin nhắn lên đầu danh sách
    prependMessagesToUI(result.data);

    currentCursor = result.nextCursor;
    hasMoreMessages = result.hasMore;
    isLoadingMessages = false;

    // Giữ nguyên vị trí cuộn chuột sau khi chèn tin nhắn cũ
    messageContainer.scrollTop = messageContainer.scrollHeight - previousScrollHeight;
}

// Bắt sự kiện cuộn chuột lên đỉnh hộp chat
messageContainer.addEventListener("scroll", () => {
    if (messageContainer.scrollTop === 0 && hasMoreMessages) {
        loadMessages(currentMaPhongChat);
    }
});
```

---

## 5. THÀNH PHẦN 3: HIỂN THỊ DANH SÁCH CUỘC TRÒ CHUYỆN

### 5.1. Truy vấn danh sách phòng chat (`PhongChat`)
```csharp
[HttpGet("api/chat/rooms")]
public async Task<IActionResult> GetChatRooms([FromQuery] int maUser)
{
    // Lấy danh sách phòng chat mà User tham gia (Client hoặc Freelancer)
    var list = await _context.PhongChat
        .AsNoTracking()
        .Include(p => p.UsersClient)
        .Include(p => p.FreelancerStudent).ThenInclude(f => f.Users)
        .Include(p => p.JobPost)
        .Where(p => p.maUserClient == maUser || p.FreelancerStudent.maUser == maUser)
        .Select(p => new
        {
            p.maPhongChat,
            p.maJob,
            TieuDeJob = p.JobPost.tieude,
            PartnerName = p.maUserClient == maUser ? p.FreelancerStudent.Users.hotenUser : p.UsersClient.hotenUser,
            PartnerAvatar = p.maUserClient == maUser ? p.FreelancerStudent.Users.avatarUrl : p.UsersClient.avatarUrl,
            TinNhanMoiNhat = p.TinNhanChats.OrderByDescending(t => t.maTinNhan).Select(t => t.noiDung).FirstOrDefault(),
            ThoiGianMoiNhat = p.TinNhanChats.OrderByDescending(t => t.maTinNhan).Select(t => t.ngayGui).FirstOrDefault() ?? p.ngayTao,
            SoTinChuaDoc = p.TinNhanChats.Count(t => !t.daDoc && t.maNguoiGui != maUser)
        })
        .OrderByDescending(x => x.ThoiGianMoiNhat)
        .ToListAsync();

    return Ok(list);
}
```

### 5.2. Cập nhật Sidebar khi có tin nhắn mới (Real-time Re-ordering)
Khi có tin nhắn mới đến, đẩy cuộc hội thoại đó lên vị trí đầu tiên của Sidebar:
```javascript
function updateSidebarLastMessage(conversationId, lastMessage, sentAt) {
    const item = document.querySelector(`.conversation-item[data-id='${conversationId}']`);
    if (item) {
        // Cập nhật nội dung & thời gian
        item.querySelector(".last-message-text").innerText = lastMessage;
        item.querySelector(".last-message-time").innerText = formatTime(sentAt);
        item.classList.add("unread");

        // Đưa item lên đầu danh sách
        const listContainer = document.getElementById("conversationList");
        listContainer.prepend(item);
    }
}
```

---

## 6. BẢO MẬT & TỐI ƯU MỞ RỘNG (SCALE-OUT)

1. **Xác thực JWT trong SignalR**:
   - Do WebSocket không hỗ trợ custom header trong trình duyệt, gửi Access Token qua Query String `?access_token=...`.
   - Cấu hình trong `Program.cs`:
     ```csharp
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context =>
         {
             var accessToken = context.Request.Query["access_token"];
             var path = context.HttpContext.Request.Path;
             if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
             {
                 context.Token = accessToken;
             }
             return Task.CompletedTask;
         }
     };
     ```
2. **Khi mở rộng nhiều máy chủ (Multiple Web Servers / Load Balancer)**:
   - Dùng **Redis Backplane** (`Microsoft.AspNetCore.SignalR.StackExchangeRedis`) để các SignalR Hub trên các máy chủ khác nhau có thể đồng bộ tin nhắn qua lại.

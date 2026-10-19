# Màn hình Nhắn tin Real-time (`lib/views/chat/`)

## 📄 Danh sách màn hình & Nhiệm vụ:
- `chat_list_screen.dart`:
  - Danh sách các cuộc trò chuyện gần đây.
  - Hiển thị avatar đối phương, tên người chat, tin nhắn gần nhất, thời gian và số lượng tin chưa đọc.
- `chat_detail_screen.dart`:
  - Khung giao diện nhắn tin trực tiếp 1-1 giữa Freelancer và Nhà tuyển dụng.
  - Danh sách tin nhắn phân loại bóng chat bên trái (người khác) / bên phải (bản thân).
  - Khung nhập tin nhắn kèm nút gửi và gửi ảnh/file.
  - Tự động cuộn xuống tin nhắn mới nhất khi nhận dữ liệu từ SignalR Hub.

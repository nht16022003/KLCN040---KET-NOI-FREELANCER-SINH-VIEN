# Thư mục Constants (`lib/core/constants/`)

## 📌 Vai trò
Quản lý toàn bộ các hằng số không thay đổi trong ứng dụng, tránh việc "hardcode" chuỗi, màu sắc hoặc URL trực tiếp trong code.

## 📄 Danh sách các file và nhiệm vụ:
- `api_endpoints.dart`: Định nghĩa Base URL (`http://10.0.2.2:5000/api`) và các path endpoint tương ứng với .NET API:
  - `authLogin`, `authRegister` (`/Auth/...`)
  - `jobs`, `jobDetail`, `postJob` (`/JobPost/...`)
  - `studentPosts` (`/BaiDangTimViec/...`)
  - `applications` (`/UngTuyen/...`)
  - `wallet`, `depositTransactions` (`/Wallet/...`, `/GiaoDichNapTien/...`)
  - `chatHubUrl` (`/chatHub` cho SignalR)
- `app_colors.dart`: Định nghĩa bảng màu chuẩn (Primary, Secondary, Background, TextColor, ErrorColor...).
- `app_text_styles.dart`: Định nghĩa Typography, Font size, Font weight chuẩn theo thiết kế.
- `app_constants.dart`: Các hằng số khác (Page size phân trang, timeout request 30s, regex kiểm tra email/SĐT...).

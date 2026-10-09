# Thư mục UI Widgets tái sử dụng (`lib/views/widgets/`)

## 📌 Vai trò
Chứa các thành phần giao diện dùng chung (Reusable Widgets) được tái sử dụng qua nhiều màn hình khác nhau, giúp đồng bộ thiết kế (Design System) và tránh lặp code.

## 📄 Danh sách các Widget phổ biến:
- `custom_button.dart`: Nút bấm chuẩn của ứng dụng hỗ trợ trạng thái `isLoading` (hiện spinner xoay khi đang submit) và `isDisabled`.
- `custom_text_field.dart`: Ô nhập liệu chuẩn hóa có sẵn icon, placeholder, validator và hiển thị lỗi.
- `custom_app_bar.dart`: Thanh AppBar chuẩn với nút Back, Title và Action icons.
- `loading_overlay.dart`: Lớp phủ mờ hiển thị vòng xoay tải dữ liệu toàn màn hình khi đang chờ gọi API.
- `empty_state_view.dart`: Widget hiển thị khi danh sách trống (ví dụ: "Chưa có công việc nào", "Không có tin nhắn").
- `error_view.dart`: Widget hiển thị khi gọi API thất bại có nút "Thử lại" (Retry).
- `avatar_image.dart`: Widget hiển thị ảnh avatar tròn có hỗ trợ cache và fallback placeholder nếu link ảnh bị lỗi.

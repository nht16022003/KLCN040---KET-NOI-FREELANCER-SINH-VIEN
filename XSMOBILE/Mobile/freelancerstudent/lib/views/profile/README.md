# Màn hình Quản lý Hồ sơ (`lib/views/profile/`)

## 📄 Danh sách màn hình & Nhiệm vụ:
- `profile_screen.dart`:
  - Hiển thị thông tin tổng quan của tài khoản hiện tại: Avatar, Họ tên, Email, Số điện thoại, Vai trò (Sinh viên hay Doanh nghiệp/NTD).
  - Menu chức năng: Thông tin cá nhân, Đổi mật khẩu, Quản lý ví, Đổi ngôn ngữ/giao diện, Nút "Đăng xuất".
- `edit_profile_screen.dart`:
  - Cho phép sửa họ tên, số điện thoại, giới thiệu bản thân (Bio), cập nhật trường học/chuyên ngành.
  - Chọn và cập nhật ảnh đại diện (sử dụng `image_picker` upload lên API).
- `change_password_screen.dart`: Form nhập mật khẩu cũ, mật khẩu mới và xác nhận mật khẩu mới.

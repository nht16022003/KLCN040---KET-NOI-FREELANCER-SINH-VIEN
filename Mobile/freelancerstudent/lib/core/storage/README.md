# Thư mục Storage (`lib/core/storage/`)

## 📌 Vai trò
Quản lý việc lưu trữ dữ liệu bền vững (Local Persistence) trên thiết bị Android/iOS.

## 📄 Danh sách các file và nhiệm vụ:
- `secure_storage_service.dart`: Sử dụng `flutter_secure_storage` (sử dụng Android Keystore / iOS Keychain mã hóa) để lưu trữ các dữ liệu nhạy cảm:
  - Access Token (JWT Token).
  - Refresh Token (nếu có).
  - Khóa bí mật phiên đăng nhập.
- `shared_pref_service.dart`: Sử dụng `shared_preferences` để lưu trữ các cấu hình nhẹ không bảo mật:
  - Trạng thái Dark / Light Mode.
  - Cờ `isFirstTimeOpenApp` (Hiển thị màn hình Onboarding/Giới thiệu).
  - Dữ liệu người dùng cơ bản dạng Cache (User ID, Role).
- `pref_keys.dart`: Hằng số chuỗi định danh các key lưu trữ (ví dụ: `KEY_ACCESS_TOKEN`, `KEY_USER_ROLE`).

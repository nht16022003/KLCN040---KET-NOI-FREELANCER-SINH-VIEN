# Thư mục Utils (`lib/core/utils/`)

## 📌 Vai trò
Chứa các hàm trợ giúp (Helper/Utility functions) tái sử dụng cho việc xử lý chuỗi, định dạng, kiểm tra tính hợp lệ của dữ liệu đầu vào.

## 📄 Danh sách các file và nhiệm vụ:
- `formatters.dart`:
  - `formatCurrency(double amount)`: Định dạng tiền tệ theo chuẩn Việt Nam (ví dụ: `500.000 đ`).
  - `formatDateTime(DateTime date)`: Định dạng ngày giờ hiển thị (ví dụ: `15:30 10/10/2026`).
  - `timeAgo(DateTime date)`: Định dạng thời gian tương đối (ví dụ: `5 phút trước`, `2 ngày trước`).
- `validators.dart`:
  - `validateEmail(String? value)`: Kiểm tra cấu trúc Email hợp lệ.
  - `validatePassword(String? value)`: Kiểm tra độ mạnh mật khẩu (ít nhất 6 ký tự...).
  - `validatePhoneNumber(String? value)`: Kiểm tra định dạng số điện thoại Việt Nam (10 chữ số).
  - `validateRequired(String? value, String fieldName)`: Kiểm tra trường không được để trống.
- `dialog_utils.dart`:
  - Hiển thị Alert Dialog xác nhận (Yes/No), Loading Indicator xoay tròn khi đang gọi API, Toast thông báo nhanh.

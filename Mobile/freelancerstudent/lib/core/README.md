# Thư mục Core (`lib/core/`)

## 📌 Vai trò & Mục đích
Thư mục **Core** chứa toàn bộ các thành phần hạ tầng dùng chung xuyên suốt toàn bộ ứng dụng, không phụ thuộc vào bất kỳ màn hình (UI) cụ thể nào.

---

## 📂 Cấu trúc các thư mục con

| Thư mục con | Nhiệm vụ | Các file điển hình cần có |
|---|---|---|
| [`constants/`](./constants/README.md) | Chứa các hằng số tĩnh (API endpoints, màu sắc, font chữ, chuỗi hệ thống). | `api_endpoints.dart`, `app_colors.dart`, `app_strings.dart` |
| [`network/`](./network/README.md) | Cấu hình HTTP Client (Dio), xử lý Interceptor (tự động gắn Token Bearer), bắt lỗi mạng toàn cục. | `api_client.dart`, `dio_interceptor.dart`, `api_response.dart` |
| [`storage/`](./storage/README.md) | Quản lý lưu trữ cục bộ an toàn (JWT Token, User Session, Preferences). | `secure_storage_service.dart`, `shared_pref_service.dart` |
| [`utils/`](./utils/README.md) | Các hàm tiện ích hỗ trợ định dạng ngày tháng, tiền tệ VNĐ, kiểm tra form (validation). | `formatters.dart`, `validators.dart`, `dialog_utils.dart` |

---

## ⚠️ Nguyên tắc khi làm việc trong thư mục `core/`
1. **Tính độc lập:** Các file trong `core/` KHÔNG ĐƯỢC `import` các file từ `views/` hoặc `viewmodels/`.
2. **Tái sử dụng:** Mọi tiện ích, cấu hình dùng chung từ 2 nơi trở lên nên được đưa vào đây.

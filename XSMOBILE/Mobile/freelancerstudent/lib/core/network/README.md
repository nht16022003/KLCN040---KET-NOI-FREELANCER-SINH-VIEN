# Thư mục Network (`lib/core/network/`)

## 📌 Vai trò
Quản lý tầng giao tiếp mạng HTTP/RESTful giữa ứng dụng Flutter và backend ASP.NET Core Web API.

## 📄 Danh sách các file và nhiệm vụ:
- `api_client.dart`: Khởi tạo và cấu hình `Dio` singleton (BaseOptions, ConnectTimeout, ReceiveTimeout, BaseURL). Cung cấp các hàm wrapper tiện lợi: `get()`, `post()`, `put()`, `delete()`, `uploadFile()`.
- `dio_interceptor.dart`: Tự động can thiệp vào vòng đời của Request / Response / Error:
  - **onRequest:** Lấy Token từ Secure Storage và tự động gắn vào Header: `Authorization: Bearer <Token>`.
  - **onResponse:** Logging dữ liệu trả về cho môi trường Debug.
  - **onError:** Bắt các mã lỗi HTTP phổ biến (`401 Unauthorized` -> Chuyển về màn hình Login; `403 Forbidden`; `500 Internal Server Error`).
- `api_response.dart`: Lớp Generic `ApiResponse<T>` chuẩn hóa cấu trúc dữ liệu trả về: `{ isSuccess, data, message, errorCode }`.

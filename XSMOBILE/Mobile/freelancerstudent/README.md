# HƯỚNG DẪN TRIỂN KHAI DỰ ÁN MOBILE (FLUTTER / ANDROID)
> **Dự án:** FreelancerStudent Mobile  
> **Ánh xạ từ:** Backend Web API (.NET) `Improve/FreelancerStudent.API`  
> **Mục tiêu:** Xây dựng ứng dụng di động Android kết nối toàn bộ hệ thống API nghiệp vụ (Tuyển dụng, Tìm việc, Ứng tuyển, Nạp tiền/Ví và Chat Real-time).

---

## 📑 MỤC LỤC
1. [Yêu cầu môi trường & Công cụ](#1-yêu-cầu-môi-trường--công-cụ)
2. [Cấu hình kết nối API từ Android sang .NET Backend](#2-cấu-hình-kết-nối-api-từ-android-sang-net-backend)
3. [Kiến trúc thư mục khuyến nghị (MVVM Pattern)](#3-kiến-trúc-thư-mục-khuyến-nghị-mvvm-pattern)
4. [Các Packages / Thư viện cần thiết](#4-các-packages--thư-viện-cần-thiết)
5. [Lộ trình triển khai theo từng Module (Mapping Roadmap)](#5-lộ-trình-triển-khai-theo-từng-module-mapping-roadmap)
6. [Hướng dẫn cài đặt & Chạy ứng dụng](#6-hướng-dẫn-cài-đặt--chạy-ứng-dụng)

---

## 1. Yêu cầu môi trường & Công cụ

- **Flutter SDK:** `>= 3.13.0` (Khuyến nghị bản mới nhất, kiểm tra bằng `flutter doctor`)
- **Android Studio / VS Code** (đã cài Flutter & Dart plugin)
- **Android SDK & Máy ảo (Emulator)** hoặc **Thiết bị Android thật** (bật USB Debugging)
- **.NET SDK 8.0+** (để khởi chạy Backend API tại `Improve/FreelancerStudent.API`)
- **SQL Server / Database** đang hoạt động

---

## 2. Cấu hình kết nối API từ Android sang .NET Backend

### 2.1. Cấu hình Base URL kết nối
Do Android Emulator chạy trên môi trường sandbox riêng, địa chỉ `localhost` của máy tính sẽ được map khác nhau:

| Môi trường chạy | Địa chỉ Base URL Backend |
|---|---|
| **Android Emulator** | `http://10.0.2.2:5000/api` *(hoặc port backend chạy)* |
| **Thiết bị thật (Cùng mạng Wi-Fi)** | `http://<IP_LAN_CỦA_MÁY_TÍNH>:5000/api` (Ví dụ: `http://192.168.1.15:5000/api`) |
| **Server Production / Ngrok** | `https://your-domain.com/api` |

### 2.2. Cho phép HTTP không mã hóa (Cleartext Traffic) trên Android
Trong file `android/app/src/main/AndroidManifest.xml`, thêm `android:usesCleartextTraffic="true"` vào thẻ `<application>`:
```xml
<application
    android:label="freelancerstudent"
    android:name="${applicationName}"
    android:icon="@mipmap/ic_launcher"
    android:usesCleartextTraffic="true">
    ...
</application>
```

---

## 3. Kiến trúc thư mục khuyến nghị (MVVM Pattern)

Tổ chức thư mục trong `lib/` theo cấu trúc module rõ ràng, dễ mở rộng:

```text
lib/
├── core/
│   ├── constants/            # Màu sắc, font chữ, API Endpoints, App Strings
│   │   ├── api_endpoints.dart
│   │   └── app_colors.dart
│   ├── network/              # Cấu hình Dio Client, Interceptor gắn Bearer Token
│   │   ├── api_client.dart
│   │   └── dio_interceptor.dart
│   ├── storage/              # Lưu trữ cục bộ (Token, User Session)
│   │   └── secure_storage_service.dart
│   └── utils/                # Helper date, format currency, validators
├── models/                   # Map đối tượng DTO từ .NET API
│   ├── auth_model.dart
│   ├── job_post_model.dart
│   ├── candidate_post_model.dart
│   ├── application_model.dart
│   ├── wallet_model.dart
│   └── chat_message_model.dart
├── services/                 # Gọi API trực tiếp qua ApiClient
│   ├── auth_service.dart
│   ├── job_service.dart
│   ├── student_post_service.dart
│   ├── application_service.dart
│   ├── wallet_service.dart
│   └── chat_signalr_service.dart
├── viewmodels/ (providers)   # Quản lý State & Logic nghiệp vụ màn hình
│   ├── auth_viewmodel.dart
│   ├── job_viewmodel.dart
│   ├── wallet_viewmodel.dart
│   └── chat_viewmodel.dart
├── views/                    # Giao diện người dùng
│   ├── auth/                 # Màn hình Login, Register, Forgot Password
│   ├── home/                 # Trang chủ, danh sách việc làm
│   ├── job_detail/           # Chi tiết bài tuyển dụng & Modal nộp đơn
│   ├── student_posts/        # Bài đăng tìm việc của sinh viên
│   ├── my_applications/      # Lịch sử và trạng thái nộp đơn
│   ├── wallet/               # Xem số dư, Nạp tiền, Lịch sử giao dịch
│   ├── chat/                 # Phòng chat, tin nhắn realtime
│   ├── profile/              # Thông tin cá nhân & Quản lý tài khoản
│   └── widgets/              # Các UI Components tái sử dụng (Button, Input, Card...)
└── main.dart                 # Điểm khởi chạy ứng dụng & Thiết lập Provider/Routes
```

---

## 4. Các Packages / Thư viện cần thiết

Cập nhật vào `pubspec.yaml`:

```yaml
dependencies:
  flutter:
    sdk: flutter
  cupertino_icons: ^1.0.8

  # HTTP Client & Network
  dio: ^5.4.3+1

  # Local Storage & Security
  flutter_secure_storage: ^9.0.0
  shared_preferences: ^2.2.3

  # State Management
  provider: ^6.1.2

  # UI Helper & Routing
  intl: ^0.19.0                      # Format ngày tháng, tiền tệ VNĐ
  cached_network_image: ^3.3.1       # Load & cache ảnh đại diện / ảnh bài đăng
  fluttertoast: ^8.2.8               # Hiển thị thông báo Toast nhanh
  image_picker: ^1.1.2               # Chọn ảnh đại diện, tải lên CV/Portfolio

  # Real-time Chat
  signalr_netcore: ^1.4.1            # Kết nối với ASP.NET Core SignalR Hub
```

---

## 5. Lộ trình triển khai theo từng Module (Mapping Roadmap)

### 🔹 Giai đoạn 1: Khung cơ sở & Module Xác thực (Auth)
- [ ] Xây dựng `ApiClient` với `Dio` + `Interceptors` (tự động đính kèm Token `Bearer {JWT}` vào header).
- [ ] Xây dựng màn hình Đăng ký / Đăng nhập (`AuthController.cs`):
  - Phân quyền theo Role: **Freelancer (Sinh viên)** hoặc **Nhà tuyển dụng (NTD)**.
  - Lưu Token & User Info vào `FlutterSecureStorage`.
  - Tự động đăng nhập (Auto-login) khi mở lại ứng dụng nếu Token còn hạn.

### 🔹 Giai đoạn 2: Module Việc làm & Ứng tuyển (Jobs & Applications)
- [ ] **Duyệt bài đăng tuyển dụng** (`JobPostController.cs`):
  - Danh sách việc làm có phân trang, tìm kiếm theo tiêu đề, lọc theo mức lương/kỹ năng.
  - Màn hình chi tiết công việc.
- [ ] **Nộp hồ sơ ứng tuyển** (`UngTuyenController.cs`):
  - Freelancer gửi đơn ứng tuyển (lời giới thiệu, giá đề xuất, ngày cam kết hoàn thành).
  - Nhà tuyển dụng: Xem danh sách ứng viên, duyệt/từ chối ứng viên.
- [ ] **Bài đăng tìm việc của sinh viên** (`BaiDangTimViecController.cs`):
  - Sinh viên tạo bài tìm việc.
  - NTD tìm kiếm và xem hồ sơ năng lực của sinh viên.

### 🔹 Giai đoạn 3: Module Hồ sơ & Ví/Thanh toán (Profile & Wallet)
- [ ] **Hồ sơ cá nhân** (`FreelancerStudentController.cs`, `NhaTuyenDungController.cs`):
  - Cập nhật thông tin cá nhân, avatar, kỹ năng, trường/ngành học.
- [ ] **Ví điện tử & Nạp tiền** (`WalletController.cs`, `GiaoDichNapTienController.cs`):
  - Hiển thị số dư hiện tại của ví.
  - Tạo yêu cầu nạp tiền (kết nối cổng thanh toán hoặc quét mã QR ngân hàng).
  - Danh sách lịch sử biến động số dư.

### 🔹 Giai đoạn 4: Module Nhắn tin Thời gian thực (Real-time Chat)
- [ ] **Chat SignalR** (`ChatController.cs` & `ChatHub.cs`):
  - Danh sách các cuộc trò chuyện gần đây.
  - Khung chat nhắn tin 1-1 giữa Freelancer và Nhà tuyển dụng.
  - Lắng nghe sự kiện `ReceiveMessage` để cập nhật tin nhắn tức thì không cần reload.

---

## 6. Hướng dẫn cài đặt & Chạy ứng dụng

### Bước 1: Khởi động Backend API (.NET)
Mở terminal tại thư mục backend và chạy:
```bash
cd Improve/FreelancerStudent.API
dotnet run
```
*(Kiểm tra Swagger mở tại `http://localhost:5000/swagger` để đảm bảo API đã sẵn sàng).*

### Bước 2: Cài đặt Dependencies cho Mobile
Mở terminal tại thư mục Flutter app:
```bash
cd Mobile/freelancerstudent
flutter pub get
```

### Bước 3: Khởi chạy ứng dụng trên Android
- Mở Android Emulator hoặc kết nối điện thoại thật với máy tính.
- Chạy lệnh:
```bash
flutter run
```

---

> 💡 **Mẹo phát triển:**  
> - Sử dụng phím `r` trên console để **Hot Reload** giao diện nhanh chóng.  
> - Sử dụng phím `R` để **Hot Restart** lại ứng dụng.

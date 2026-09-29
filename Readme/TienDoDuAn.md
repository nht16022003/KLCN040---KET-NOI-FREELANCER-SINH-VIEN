# 📋 TỔNG HỢP TIẾN ĐỘ VÀ CÁC TÍNH NĂNG ĐÃ HOÀN THÀNH
**Dự án:** FreelancerStudent - Nền tảng kết nối Sinh viên Freelancer và Nhà tuyển dụng  
**Cập nhật ngày:** 28/09/2026  

---

## 🏗️ 1. Tổng quan Kiến trúc Hệ thống

Dự án được xây dựng theo kiến trúc tách biệt **Backend API** và **Frontend Web MVC**:

```
[ Frontend: FreelancerStudent.Web (ASP.NET Core MVC) ]
                         │
                         │ HTTP Requests (HttpClient / JSON)
                         ▼
[ Backend: FreelancerStudent.API (ASP.NET Core Web API) ]
                         │
                         ├─ Controllers (Điều hướng API Endpoints)
                         ├─ Services (Xử lý nghiệp vụ & Logic chuyển đổi DTO)
                         ├─ Repositories (Truy vấn dữ liệu với Entity Framework Core)
                         ▼
[ Cơ sở dữ liệu: SQL Server (KETNOI_FREELANCERSV) ]
```

---

## ✅ 2. Chi tiết các Module & Tính năng đã hoàn thành

### 🔐 Module 1: Xác thực & Phân quyền (Authentication & Authorization)
* **Đăng ký tài khoản (`DangKy`)**:
  * Mã hóa mật khẩu bằng thuật toán an toàn (BCrypt / Hash).
  * Kiểm tra trùng lặp email và tài khoản.
  * Tự động khởi tạo dữ liệu mở rộng tương ứng với Role (nếu là `NhaTuyenDung` (Role 2) thì tự động tạo bản ghi Nhà tuyển dụng, đồng thời tự động cấp ví tiền ban đầu).
* **Đăng nhập (`DangNhap`)**:
  * Xác thực qua API, trả về thông tin người dùng và Role.
  * Phía Web kết hợp lưu **Session** (hiển thị UI nhanh) và **Cookie Claims Authentication** (`ClaimTypes.NameIdentifier`, `ClaimTypes.Role`) để bảo mật và phân quyền.
  * Tự động điều hướng theo Role sau đăng nhập:
    * `Admin` ➡️ `/Admin/Index`
    * `NhaTuyenDung` ➡️ `/NhaTuyenDung/Index`
    * `FreelancerStudent` ➡️ `/JobPost/Index`
* **Đăng xuất (`DangXuat`)**:
  * Xóa sạch Cookie xác thực và Session, đưa người dùng về trang Đăng nhập an toàn.

---

### 🏢 Module 2: Quản lý Nhà Tuyển Dụng (`NhaTuyenDung`)
* **Backend API**:
  * `NhaTuyenDungRepository`: Truy vấn dữ liệu kết hợp `.Include(n => n.User)` lấy đầy đủ họ tên, email, SĐT.
  * `NhaTuyenDungService`: Chuyển đổi sang `NhaTuyenDung_ResponseDTO`.
  * `NhaTuyenDungController`: Cung cấp Endpoint `GET api/NhaTuyenDung/DanhSachNhaTuyenDung`.
* **Frontend Web**:
  * `NhaTuyenDungWebService`: Gọi API lấy danh sách.
  * `NhaTuyenDungController` & View `Views/NhaTuyenDung/Search.cshtml`: Hiển thị danh sách dạng Card trực quan, chuyên nghiệp.

---

### 💼 Module 3: Đăng tin & Quản lý Công việc (`JobPost`)
* **Cấu hình Model & Database**:
  * Model `JobPost.cs` với khóa chính chuỗi `maJob`, khóa ngoại liên kết `NhaTuyenDung`, cùng các trường số dạng nullable (`decimal?`, `DateTime?`, `int?`) chống lỗi `SqlNullValueException`.
* **DTO & Xử lý nghiệp vụ**:
  * `JobPost_RequestDTO.cs` & `JobPost_ReponseDTO.cs`.
  * Cơ chế tự động sinh mã công việc theo định dạng chuẩn (ví dụ: `JOB_yyyyMMdd_...`).
  * API Endpoint: `POST api/JobPost/TaoJob` và `GET api/JobPost/DanhSachJobPost`.

---

### 💰 Module 4: Quản lý Ví Tiền (`Wallet`)
* **Thiết kế quan hệ Database**:
  * Thiết lập quan hệ **1 - 1** giữa `Users` và `Wallet` (mỗi User có duy nhất 1 ví).
* **Backend API**:
  * `WalletRepository`: Hàm `layViTheoMaUser` và `themViChoUserTheoMaUser` (tự động khởi tạo ví có số dư khả dụng = 0 và số dư đóng băng = 0).
  * `WalletService`: Xử lý logic và map dữ liệu sang `Wallet_ReponseDTO`.
  * `WalletController`: Cung cấp API `GET api/Wallet/LayViTheoUser/{maUser}`.
* **Frontend Web**:
  * Thiết kế lấy `maUser` an toàn từ Claims/Session người dùng đăng nhập để xem thông tin ví cá nhân.

---

### 🎓 Module 5: Quản lý Sinh viên Freelancer (`FreelancerStudents`)
* **Backend API**:
  * Model `FreelancerStudents` liên kết với `Users`, `ChuyenNganh`, `KyNang`.
  * `FreelancerStudentRepository`: Truy vấn danh sách sinh viên kết hợp thông tin trường lớp, GPA, chuyên ngành.
  * `FreelancerStudentService` & `FreelancerStudentController`: Cung cấp API `GET api/FreelancerStudent/DanhSachFreelancerStudent`.
* **Frontend Web**:
  * Thiết lập `IFreelancerStudentWebService` và `FreelancerStudentWebService` để đồng bộ dữ liệu lên Web.

---

## 🛠️ 3. Các vấn đề kỹ thuật quan trọng đã giải quyết

| STT | Vấn đề / Lỗi gặp phải | Nguyên nhân | Giải pháp đã khắc phục |
|:---:|:---|:---|:---|
| 1 | `Serialization Task<T>` | Quên từ khóa `await` trong Controller | Thêm `await` trước các lời gọi Service bất đồng bộ. |
| 2 | `Unable to cast Double to Single` | Cột `GPA FLOAT` trong SQL Server (64-bit) lệch kiểu với `float` trong C# (32-bit) | Đổi kiểu `GPA` sang `double` (hoặc `double?`) trong C# Model & DTO. |
| 3 | `Invalid object name FreelamcerStudents` | Sai chính tả chữ cái `m` thay vì `n` ở attribute `[Table("...")]` | Sửa lại thành `[Table("FreelancerStudents")]`. |
| 4 | `Ambiguity between _guiRequest` | Copy file dịch vụ mà chưa đổi tên class `FreelancerStudentWebService` | Đổi tên class, constructor và interface tương ứng. |
| 5 | `Unable to resolve service for type` | Chưa khai báo DI trong `Program.cs` | Thêm đầy đủ `builder.Services.AddScoped<IService, Service>()`. |
| 6 | Lỗi 400 Bad Request khi Đăng xuất | Đặt `[ValidateAntiForgeryToken]` trên phương thức nhận GET request | Bỏ `[ValidateAntiForgeryToken]` ở Action `[HttpGet] DangXuat`. |

---

## 🎯 4. Kế hoạch các bước tiếp theo

1. **Giao diện Quản lý Ví tiền trên Web (`/Wallet/Index`)**:
   - Hiển thị card Số dư khả dụng & Số dư đóng băng.
   - Xây dựng chức năng Nạp tiền và Rút tiền.
2. **Giao diện Đăng tin tuyển dụng cho Nhà tuyển dụng (`/JobPost/Create`)**:
   - Form nhập tiêu đề, mô tả, yêu cầu kỹ năng, mức thù lao, hạn nộp.
   - Gửi dữ liệu về API `POST api/JobPost/TaoJob`.
3. **Chức năng Ứng tuyển công việc cho Sinh viên (`ApplyJob`)**:
   - Sinh viên xem chi tiết công việc và bấm nộp hồ sơ ứng tuyển.
   - Nhà tuyển dụng duyệt/từ chối hồ sơ ứng viên.

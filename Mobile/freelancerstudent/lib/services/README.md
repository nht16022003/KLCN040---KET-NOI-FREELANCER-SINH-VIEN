# Thư mục Services (`lib/services/`)

## 📌 Vai trò & Mục đích
Thư mục **Services** chịu trách nhiệm thực thi các cuộc gọi API thực tế tới Backend thông qua `ApiClient`.

Tầng Service không chứa logic hiển thị UI và không quản lý State trực tiếp; nó nhận dữ liệu đầu vào, gọi API, parse kết quả thành Model và trả về cho `ViewModel` / `Provider`.

---

## 📄 Danh sách các file Service chính:

| Tên File | API Controller tương ứng | Nhiệm vụ |
|---|---|---|
| `auth_service.dart` | `AuthController.cs` | Các phương thức: `login(email, pass)`, `registerStudent(...)`, `registerEmployer(...)`, `forgotPassword(...)`. |
| `job_service.dart` | `JobPostController.cs` | Các phương thức: `getJobs(page, keyword, filter)`, `getJobDetail(id)`, `createJob(dto)`, `updateJob(id, dto)`, `deleteJob(id)`. |
| `student_post_service.dart` | `BaiDangTimViecController.cs` | Các phương thức: `getStudentPosts()`, `createStudentPost(dto)`, `getMyStudentPosts()`. |
| `application_service.dart` | `UngTuyenController.cs` | Các phương thức: `applyJob(jobId, dto)`, `getMyApplications()`, `getApplicantsByJob(jobId)`, `updateApplicationStatus(id, status)`. |
| `user_service.dart` | `FreelancerStudentController.cs`, `NhaTuyenDungController.cs` | Các phương thức: `getProfile()`, `updateProfile(dto)`, `uploadAvatar(imagePath)`. |
| `wallet_service.dart` | `WalletController.cs`, `GiaoDichNapTienController.cs` | Các phương thức: `getWalletBalance()`, `createDepositTransaction(amount, paymentMethod)`, `getTransactionHistory()`. |
| `chat_signalr_service.dart` | `ChatController.cs` & `ChatHub.cs` | Quản lý kết nối WebSocket với SignalR Hub: `connect()`, `disconnect()`, `sendMessage(receiverId, content)`, `onReceiveMessage(callback)`. |

---

## 💡 Quy ước lập trình Service:
1. Mỗi hàm trong Service nên trả về `Future<ApiResponse<T>>` hoặc ném ra Exception có ý nghĩa nếu gặp lỗi.
2. Không thực hiện `setState()` hoặc điều hướng `Navigator` bên trong tầng Service.

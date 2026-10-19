# Thư mục ViewModels (`lib/viewmodels/`)

## 📌 Vai trò & Mục đích
Thư mục **ViewModels** (hoặc `Providers`) là cầu nối trung gian giữa Giao diện người dùng (`Views`) và Tầng dữ liệu (`Services`).

Các ViewModel kế thừa từ `ChangeNotifier` (khi dùng Provider) để quản lý trạng thái (State), biến `isLoading`, thông báo lỗi `errorMessage` và gọi hàm `notifyListeners()` để cập nhật UI khi dữ liệu thay đổi.

---

## 📄 Danh sách các ViewModel chính:

| Tên File | Màn hình sử dụng | Trạng thái & Nghiệp vụ quản lý |
|---|---|---|
| `auth_viewmodel.dart` | `LoginScreen`, `RegisterScreen`, `SplashScreen` | Quản lý trạng thái đăng nhập, lưu thông tin User hiện tại, kiểm tra Token còn hạn khi mở app, hàm `login()`, `register()`, `logout()`. |
| `job_viewmodel.dart` | `HomeScreen`, `JobDetailScreen`, `PostJobScreen` | Quản lý danh sách việc làm `List<JobPostModel>`, phân trang `currentPage`, `hasMore`, tìm kiếm/lọc, chi tiết bài đăng đang chọn, hàm `fetchJobs()`, `loadMore()`. |
| `student_post_viewmodel.dart` | `StudentPostListScreen`, `CreateStudentPostScreen` | Quản lý danh sách bài đăng tìm việc của sinh viên, thao tác đăng bài mới. |
| `application_viewmodel.dart` | `MyApplicationsScreen`, `ApplicantListScreen` | Quản lý lịch sử nộp đơn của Freelancer, danh sách ứng viên ứng tuyển vào bài đăng của Nhà tuyển dụng, xử lý Duyệt/Từ chối đơn. |
| `wallet_viewmodel.dart` | `WalletScreen`, `DepositScreen` | Quản lý số dư ví, danh sách giao dịch, xử lý tạo đơn nạp tiền VNPAY/MoMo. |
| `chat_viewmodel.dart` | `ChatListScreen`, `ChatDetailScreen` | Quản lý trạng thái kết nối SignalR, danh sách tin nhắn hiện tại `List<ChatMessageModel>`, gửi tin nhắn và cập nhật danh sách realtime khi có tin nhắn mới tới. |

---

## 💡 Quy tắc luồng dữ liệu (Data Flow):
```text
[User Interaction trên View] 
       ⬇ Gọi hàm
[ViewModel xử lý logic & gọi Service] 
       ⬇ Gọi API
[Service trả về DTO/Model] 
       ⬇ Cập nhật biến State trong ViewModel
[notifyListeners() thông báo] 
       ⬇ Rebuild UI
[View hiển thị dữ liệu mới]
```

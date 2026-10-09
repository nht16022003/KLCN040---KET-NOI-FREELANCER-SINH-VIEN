# Thư mục Models (`lib/models/`)

## 📌 Vai trò & Mục đích
Thư mục **Models** định nghĩa cấu trúc dữ liệu (Data Classes) tương ứng với các DTOs (Data Transfer Objects) và Entity từ Backend ASP.NET Core (`FreelancerStudent.API/DTOs`).

Mỗi model chịu trách nhiệm parse dữ liệu JSON từ API thành đối tượng Dart (`fromJson`) và chuyển đổi đối tượng Dart thành JSON (`toJson`) để gửi lên server.

---

## 📄 Danh sách các file Model chính và nhiệm vụ:

| Tên File | Ánh xạ từ Backend DTO | Nhiệm vụ |
|---|---|---|
| `auth_response_model.dart` | `LoginDto`, `RegisterDto`, Token Result | Chứa Token JWT, thông tin tài khoản vừa đăng nhập, Role, Hạn sử dụng token. |
| `user_model.dart` | `FreelancerStudentDto`, `NhaTuyenDungDto` | Chứa thông tin hồ sơ: Tên, Email, SĐT, Kỹ năng, Trường học, Avatar, Giới thiệu bản thân. |
| `job_post_model.dart` | `JobPostDto` | Chứa dữ liệu bài tuyển dụng: Tiêu đề, Mô tả công việc, Ngân sách (Budget), Hạn nộp hồ sơ, Danh sách kỹ năng yêu cầu. |
| `student_post_model.dart` | `BaiDangTimViecDto` | Chứa bài đăng tìm việc của sinh viên: Kỹ năng chào mời, Mức giá mong muốn, Lĩnh vực chuyên môn. |
| `application_model.dart` | `UngTuyenDto` | Chứa dữ liệu hồ sơ ứng tuyển: Lời giới thiệu (Cover Letter), Giá đề xuất, Ngày cam kết hoàn thành, Trạng thái duyệt (`Pending`, `Accepted`, `Rejected`). |
| `wallet_model.dart` | `WalletDto`, `GiaoDichNapTienDto` | Chứa số dư ví hiện tại, mã giao dịch nạp tiền, phương thức nạp (VNPAY/MoMo/Bank), trạng thái giao dịch. |
| `chat_message_model.dart` | `ChatMessageDto`, `ChatRoomDto` | Chứa mã phòng chat, Người gửi, Người nhận, Nội dung tin nhắn, Thời gian gửi, Trạng thái đã xem (`isRead`). |

---

## 💡 Quy ước lập trình Model:
- Khuyến nghị sử dụng Factory Constructor `factory ModelName.fromJson(Map<String, dynamic> json)`.
- Viết phương thức `Map<String, dynamic> toJson()` cho các model cần gửi Request Body lên server.
- Sử dụng thuộc tính `final` và constructor `const` nếu có thể để tối ưu hiệu năng render.

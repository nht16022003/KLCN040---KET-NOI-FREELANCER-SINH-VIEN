# TIẾN ĐỘ TRIỂN KHAI VÀ MAPPING SOURCE CODE MOBILE (FLUTTER)
**Dự án:** Kết nối Freelancer Sinh viên (`FreelancerStudent`)  
**Thành phần khảo sát & đối chiếu:** `FreelancerStudent.API`, `FreelancerStudent.Web`, `Mobile/freelancerstudent`  
**Cập nhật lần cuối:** 18/10/2026  

---

## 1. TỔNG QUAN TIẾN ĐỘ THỰC HIỆN

- **Mục tiêu ưu tiên:** Hoàn thiện 13 chức năng bắt buộc của **Chức năng 4 — Tìm việc và ứng tuyển** theo đúng nghiệp vụ Backend, kết nối API thật, bảo toàn schema CSDL và không sử dụng dữ liệu giả lập.
- **Tình trạng phân tích & build code:**
  - `FreelancerStudent.API`: `dotnet build` **0 Error(s)**, build thành công.
  - `FreelancerStudent.Web`: `dotnet build` **0 Error(s)**, build thành công.
  - `Mobile (freelancerstudent)`: `flutter analyze` **No issues found!**
  - `Mobile Unit Tests`: `flutter test` **11/11 tests passed!**

---

## 2. BẢNG THEO DÕI 13 CHỨC NĂNG BẮT BUỘC (CHỨC NĂNG 4)

| STT | Chức năng | Web tương ứng | API Backend | Mobile (Files & Widgets) | Trạng thái |
|:---:|:---|:---|:---|:---|:---:|
| **1** | **Hồ sơ năng lực & Kỹ năng** | `FreelancerStudent/HoSo`, `CapNhatHoSo` | `GET api/FreelancerStudent/Profile/{id}`, `GET ChiTiet/{id}`, `PUT CapNhatHoSo`, `POST Portfolio/projects` | `ProfileScreen`, `EditProfileScreen`, `FreelancerProfileModel`, `FreelancerProfileEditModel`, `FreelancerProfileService`, `FreelancerProfileViewModel` |  **Hoàn thành & Đã kiểm thử** |
| **2** | **Tìm kiếm công việc** | `JobPost/Index` | `GET api/JobPost/DanhSachJobPost`, `GET api/JobPost/ChiTietJobPost/{maJob}` | `HomeScreen` (Tab Việc làm), `JobDetailScreen`, `JobService`, `JobViewModel`, `JobPostModel` |  **Hoàn thành & Đã kiểm thử** |
| **3** | **Gợi ý công việc phù hợp** | Tự động match theo hồ sơ sinh viên | `GET api/JobPost/GoiYChoFreelancer/{maUser}` | `HomeScreen` (Khối Gợi ý dành riêng cho bạn, Badge % phù hợp, Lý do gợi ý & Kỹ năng trùng khớp), `JobService`, `JobViewModel`, `JobPostModel` |  **Hoàn thành & Đã kiểm thử** |
| **4** | **Lưu công việc yêu thích** | Chưa có tính năng lưu yêu thích | Chưa có bảng `JobYeuThich` & API lưu | Không lưu ảo local để tránh sai lệch dữ liệu | ⚠️ **Chưa có API hỗ trợ** |
| **5** | **Ứng tuyển công việc** | `JobPost/Index` (Modal nộp đơn) | `POST api/JobPost/UngTuyen` (`UngTuyen_RequestDTO`) | `ApplyJobBottomSheet`, `JobDetailScreen`, `HomeScreen`, `ApplicationService`, `ApplicationModel` |  **Hoàn thành & Đã kiểm thử** |
| **6** | **Theo dõi trạng thái ứng tuyển** | `JobPost/DanhSachUngTuyen` | `GET api/JobPost/DanhSachUngTuyen/{maUser}` | `MyApplicationsScreen`, `ApplicationViewModel`, `ApplicationService`, `ApplicationModel` |  **Hoàn thành & Đã kiểm thử** |
| **7** | **Chat với Nhà tuyển dụng** | Chat SignalR / Web Chat | `POST api/Chat/TaoHoacLayPhong`, `GET api/Chat/DanhSachPhong/{maUser}`, `GET api/Chat/TinNhan/{maPhongChat}?maUser=`, `POST api/Chat/GuiTinNhan`, Hub SignalR `/chatHub` | `ChatListScreen`, `ChatDetailScreen`, `ChatService`, `ChatSignalrService`, `ChatViewModel`, `PhongChatModel`, `ChatMessageModel` |  **Hoàn thành & Đã kiểm thử** |
| **8** | **Xem hợp đồng** | `HopDong/Index`, `HopDong/ChiTiet` | `GET api/HopDong/danh-sach-freelancer/{maFS}`, `GET api/HopDong/chi-tiet/{maHD}`, `POST api/HopDong/xac-nhan` | `HopDongListScreen`, `HopDongDetailScreen`, `HopDongService`, `HopDongViewModel`, `HopDongModel` |  **Hoàn thành & Đã kiểm thử** |
| **9** | **Cập nhật tiến độ dự án** | `HopDong/ChiTiet` (Tab đầu việc) | `GET api/TaskCongViec/hop-dong/{maHD}`, `POST api/TaskCongViec`, `PUT api/TaskCongViec/{maTask}`, `PATCH api/TaskCongViec/{maTask}/trang-thai`, `DELETE api/TaskCongViec/{maTask}` | `TienDoScreen`, `TaskCongViecService`, `TaskViewModel`, `TaskCongViecModel`, `TienDoDuAnModel` |  **Hoàn thành & Đã kiểm thử** |
| **10** | **Nộp sản phẩm bàn giao** | Chưa có luồng bàn giao sản phẩm | Chưa có entity & API bàn giao | Không tạo mock bàn giao | ⚠️ **Chưa có API hỗ trợ** |
| **11** | **Theo dõi thanh toán** | `Wallet/Index` | `GET api/Wallet/LayViTheoUser/{maUser}`, `POST api/GiaoDichNapTien/TaoGiaoDich/{maUser}`, `GET api/GiaoDichNapTien/TrangThai/{maGiaoDich}` | `WalletScreen` (số dư khả dụng, số dư đóng băng thật), `DepositScreen` (tạo đơn nạp tiền VietQR SePay thật), `TransactionHistoryScreen` (hiển thị trạng thái chưa có API lịch sử) |  **Hoàn thành ví & nạp tiền** (Lịch sử: thiếu API GET) |
| **12** | **Gửi phản ánh / khiếu nại** | Chưa có module giải quyết tranh chấp | Chưa có bảng `TranhChap` & API | Không giả lập giao diện ảo | ⚠️ **Chưa có API hỗ trợ** |
| **13** | **Đánh giá Nhà tuyển dụng** | Chỉ hiển thị số sao trung bình | Chưa có bảng lưu đánh giá chi tiết | Không giả lập đánh giá | ⚠️ **Chưa có API hỗ trợ** |

---

## 3. CHI TIẾT CÁC FILE ĐÃ TẠO MỚI VÀ CHỈNH SỬA TRÊN MOBILE

1. **`lib/core/constants/api_endpoints.dart`**:
   - Thêm `static const String taskCongViec = '$baseUrl/TaskCongViec';`
   - Chuẩn hóa `chatHubUrl = '$apiBaseUrl/chatHub'` để map chính xác với route `app.MapHub<ChatHub>("/chatHub")` của Backend.
2. **`lib/core/network/api_client.dart`**:
   - Bổ sung phương thức `patch<T>()` phục vụ endpoint `PATCH api/TaskCongViec/{maTask}/trang-thai`.
3. **`lib/core/utils/api_error_utils.dart`**:
   - Hàm tiện ích `layThongBaoLoiApi` bóc tách thông báo lỗi từ Backend `DioException`.
4. **`lib/models/phong_chat_model.dart`**:
   - Ánh xạ đầy đủ `PhongChat_ReponseDTO` của Backend, các helper `tenDoiPhuong`, `avatarDoiPhuong`.
5. **`lib/models/chat_message_model.dart`**:
   - Đồng bộ hoàn chỉnh với `TinNhan_ReponseDTO`, bảo đảm backward compatibility.
6. **`lib/models/task_cong_viec_model.dart`**:
   - Ánh xạ `TaskCongViec_ResponseDTO` và `TienDoDuAn_ResponseDTO`.
7. **`lib/services/chat_service.dart`**:
   - Triển khai gọi `TaoHoacLayPhong`, `DanhSachPhong/{maUser}`, `TinNhan/{maPhongChat}`, `GuiTinNhan`.
8. **`lib/services/chat_signalr_service.dart`**:
   - Kết nối SignalR qua `package:signalr_netcore/signalr_client.dart`, xử lý lắng nghe sự kiện `NhanTinNhanMoi`, gọi `ThamGiaPhongChat` và `RoiPhongChat`.
9. **`lib/services/task_cong_viec_service.dart`**:
   - Gọi đầy đủ các phương thức GET, POST, PUT, PATCH, DELETE kèm `maUser` để Backend kiểm tra quyền Freelancer.
10. **`lib/viewmodels/chat_viewmodel.dart`**:
    - Quản lý danh sách phòng, lịch sử tin nhắn, tự động cập nhật realtime khi có tin nhắn từ SignalR hoặc REST.
11. **`lib/viewmodels/task_viewmodel.dart`**:
    - Quản lý trạng thái tải, tạo, sửa, đổi trạng thái và xóa đầu việc tiến độ.
12. **`lib/views/chat/chat_list_screen.dart`**:
    - Danh sách phòng chat thật, avatar đối phương, tin nhắn cuối, số tin nhắn chưa đọc, điều hướng chi tiết.
13. **`lib/views/chat/chat_detail_screen.dart`**:
    - Màn hình chat trực tiếp, bong bóng tin nhắn phân biệt người gửi, gửi tin nhắn REST + SignalR, tự động cuộn xuống cuối.
14. **`lib/views/hop_dong/hop_dong_list_screen.dart`**:
    - Danh sách hợp đồng của Freelancer lấy từ `GET api/HopDong/danh-sach-freelancer/{maFS}`.
15. **`lib/views/hop_dong/hop_dong_detail_screen.dart`**:
    - Chi tiết hợp đồng thật, nút Xác nhận / Từ chối (nhập lý do), nút mở Cập nhật tiến độ dự án, nút mở Chat với Nhà tuyển dụng.
16. **`lib/views/tien_do/tien_do_screen.dart`**:
    - Tiến độ dự án: % hoàn thành, thanh tiến trình, danh sách task, dialog thêm/sửa, dropdown đổi trạng thái theo quyền `coQuyenCapNhat`.
17. **`lib/views/job_detail/job_detail_screen.dart`**:
    - Tải dữ liệu thật từ `GET api/JobPost/ChiTietJobPost/{maJob}`, hiển thị thù lao, mô tả, kỹ năng, hạn chót, nút ứng tuyển liên kết `ApplyJobBottomSheet`.
18. **`lib/views/wallet/wallet_screen.dart`**:
    - Hiển thị số dư khả dụng và số dư đóng băng ký quỹ thật từ `GET api/Wallet/LayViTheoUser`.
19. **`lib/views/wallet/deposit_screen.dart`**:
    - Tạo đơn nạp tiền VietQR SePay qua `POST api/GiaoDichNapTien/TaoGiaoDich`, kiểm tra ràng buộc số tiền tối thiểu 100k và bội số 50k của Backend.
20. **`lib/views/wallet/transaction_history_screen.dart`**:
    - Trạng thái rõ ràng, không sử dụng dữ liệu giả lập.
21. **`test/chat_and_task_models_test.dart`**:
    - 4 bài test unit cho `PhongChatModel`, `ChatMessageModel`, `TienDoDuAnModel`, `TaskCongViecModel`, `JobPostModel` (Gợi ý việc làm). Toàn bộ 11/11 tests pass.
22. **`FreelancerStudent.API` (Backend Service & API Gợi ý việc làm)**:
    - `DTOs/ReponseDTOs/JobPost_ReponseDTO.cs`: Bổ sung `doPhuHop`, `lyDoGoiY`, `kyNangTrungKhop`.
    - `Services/Interfaces/IJobPostService.cs` & `JobPostService.cs`: Triển khai thuật toán tính điểm độ phù hợp dựa trên kỹ năng (`FreelancerStudent_KyNangs`, `kyNangCoBan`), chuyên ngành (`ChuyenNganh`) và yêu cầu công việc.
    - `Controllers/JobPostController.cs`: Cung cấp endpoint `GET api/JobPost/GoiYChoFreelancer/{maUser}`.
23. **`Mobile/freelancerstudent` (Mô-đun Gợi ý việc làm)**:
    - `lib/core/constants/api_endpoints.dart`: Bổ sung endpoint `jobGoiY`.
    - `lib/models/job_post_model.dart`: Hỗ trợ đầy đủ trường `doPhuHop`, `lyDoGoiY`, `kyNangTrungKhop`.
    - `lib/services/job_service.dart`: Phương thức `layDanhSachJobGoiY(int maUser)`.
    - `lib/viewmodels/job_viewmodel.dart`: Quản lý state gợi ý `danhSachJobGoiY`, `dangTaiGoiY`, `taiDanhSachJobGoiY`.
    - `lib/views/home/home_screen.dart`: Tự động tải gợi ý khi mở màn hình, hiển thị khối "Gợi ý dành riêng cho bạn" với icon nổi bật, badge % phù hợp, lý do gợi ý và các chip kỹ năng trùng khớp.

# Thư mục Views (`lib/views/`)

## 📌 Vai trò & Mục đích
Thư mục **Views** chứa toàn bộ các màn hình (Screens / Pages) và các thành phần giao diện người dùng (UI Components) của ứng dụng Mobile.

---

## 📂 Danh mục các Module màn hình con

| Thư mục con | Chức năng & Các màn hình chính |
|---|---|
| [`auth/`](./auth/README.md) | Đăng nhập (`LoginScreen`), Đăng ký (`RegisterScreen`), Quên mật khẩu (`ForgotPasswordScreen`), Splash (`SplashScreen`). |
| [`home/`](./home/README.md) | Trang chủ tìm việc, banner giới thiệu, danh sách việc làm mới nhất, thanh tìm kiếm & bộ lọc nhanh. |
| [`job_detail/`](./job_detail/README.md) | Màn hình chi tiết công việc (`JobDetailScreen`), Modal điền đơn ứng tuyển (`ApplyJobBottomSheet`). |
| [`student_posts/`](./student_posts/README.md) | Danh sách bài tìm việc của sinh viên (`StudentPostListScreen`), Màn hình tạo bài tìm việc (`CreateStudentPostScreen`). |
| [`applications/`](./applications/README.md) | Dành cho Freelancer: Lịch sử nộp đơn (`MyApplicationsScreen`). Dành cho NTD: Danh sách ứng viên (`ApplicantListScreen`). |
| [`wallet/`](./wallet/README.md) | Quản lý ví cá nhân (`WalletScreen`), Nạp tiền vào ví (`DepositScreen`), Lịch sử nạp/rút (`TransactionHistoryScreen`). |
| [`chat/`](./chat/README.md) | Danh sách phòng chat (`ChatListScreen`), Màn hình chat trực tiếp realtime (`ChatDetailScreen`). |
| [`profile/`](./profile/README.md) | Xem và chỉnh sửa hồ sơ Freelancer / Nhà tuyển dụng (`ProfileScreen`, `EditProfileScreen`). |
| [`widgets/`](./widgets/README.md) | Các widget UI tái sử dụng (Nút bấm, Ô nhập liệu, Card công việc, Loading, Empty State...). |

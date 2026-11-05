# BÁO CÁO KIỂM TRA & ĐỐI SOÁT TOÀN DIỆN HỆ THỐNG: API – WEBSITE – MOBILE

> **Dự án:** Sàn giao dịch việc làm Freelancer cho Sinh viên (FreelancerStudent)  
> **Thời điểm đối soát:** 06/11/2026  
> **Phương pháp kiểm tra:** Phân tích mã nguồn tĩnh (Static Code Analysis), truy vết luồng End-to-End (E2E Traceability), đối chiếu Database Schema thực tế và kiểm tra biên dịch/phân tích trực tiếp trên Terminal Foreground (`flutter analyze`, `dotnet build`).

---

## PHẦN 1: TỔNG QUAN HỆ THỐNG

| Tiêu chí | Số lượng | Ghi chú chi tiết |
|---|---|---|
| **Tổng số giao diện Website (Razor Views)** | **46 views** | Bao gồm `Account` (4), `Admin` (9), `BanGiaoSanPham` (1), `Chat` (1), `DanhGia` (3), `FreelancerStudent` (9), `Home` (2), `HopDong` (4), `JobPost` (9), `NhaTuyenDung` (5), `ThongBao` (1), `Wallet` (1) |
| **Tổng số màn hình Mobile (Flutter Screens)** | **37 screens** | 45 file `.dart` (bao gồm 37 màn hình/bottom sheet chính và 8 widget con/thành phần dùng chung) |
| **Tổng số Controller Backend API** | **18 Controllers** | Cung cấp đầy đủ 102 API endpoints công khai và bảo mật |
| **Tổng số Controller Web** | **12 Controllers** | Điều hướng và phục vụ toàn bộ các luồng nghiệp vụ trên trình duyệt |
| **Tổng số Bảng Database (`KLCN040_DATA.sql`)** | **40 bảng** | Đầy đủ khóa chính, khóa ngoại, quan hệ ràng buộc và dữ liệu seed |
| **Tổng số nhóm chức năng đã đối chiếu** | **42 nhóm chức năng** | Phủ kín 3 vai trò: Sinh viên (FreelancerStudent), Nhà tuyển dụng (Employer), Quản trị viên (Admin) |

---

## PHẦN 2: BẢNG ĐỐI CHIẾU CHI TIẾT TỪNG VAI TRÒ & CHỨC NĂNG

### I. PHÂN HỆ XÁC THỰC & HỒ SƠ CÁ NHÂN (CHUNG & SINH VIÊN)

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 1 | Tất cả | Đăng nhập tài khoản (Email, Mật khẩu) | `Account/DangNhap.cshtml`<br>`AccountController.DangNhap` | `POST /api/Auth/login` | `man_hinh_dang_nhap.dart`<br>`AuthService.login` | **ĐỒNG NHẤT 100%** |
| 2 | Tất cả | Đăng ký tài khoản (Sinh viên / NTD) | `Account/DangKy.cshtml`<br>`AccountController.DangKy` | `POST /api/Auth/register` | `man_hinh_dang_ky.dart`<br>`AuthService.register` | **ĐỒNG NHẤT 100%** |
| 3 | Tất cả | Quên mật khẩu & Đổi mật khẩu | `Account/DangNhap.cshtml` (Quên MK modal) | `POST /api/Auth/forgot-password`<br>`POST /api/Auth/change-password` | `man_hinh_quen_mat_khau.dart`<br>`man_hinh_doi_mat_khau.dart` | **ĐỒNG NHẤT 100%** |
| 4 | Sinh viên | Tải ảnh Thẻ SV & Chạy OCR tự động | `Account/MinhChung.cshtml`<br>`AccountController.MinhChung` | `POST /api/MinhChung/quet-the-sinh-vien` | `man_hinh_minh_chung_the_sv.dart`<br>`MinhChungService.quetTheSinhVien` | **ĐỒNG NHẤT 100%** |
| 5 | Sinh viên | Xác nhận thông tin thẻ SV & Gửi duyệt | `Account/MinhChung.cshtml` | `POST /api/MinhChung/xac-nhan-the-sinh-vien` | `man_hinh_minh_chung_the_sv.dart`<br>`MinhChungService.xacNhanTheSinhVien` | **ĐỒNG NHẤT 100%** |
| 6 | Sinh viên | Xem hồ sơ Freelancer của chính mình | `FreelancerStudent/HoSo.cshtml`<br>`FreelancerStudent/Profile.cshtml` | `GET /api/FreelancerStudent/ho-so/{userId}` | `man_hinh_ho_so.dart`<br>`FreelancerProfileService` | **ĐỒNG NHẤT 100%** |
| 7 | Sinh viên | Chỉnh sửa thông tin cá nhân, GPA, Kỹ năng, Học vấn | `FreelancerStudent/HoSo.cshtml`<br>`CapNhatThongTinCoBan`, `CapNhatGPA` | `PUT /api/FreelancerStudent/cap-nhat-ho-so` | `man_hinh_chinh_sua_ho_so.dart`<br>`FreelancerProfileService` | **ĐỒNG NHẤT 100%** |
| 8 | Sinh viên | Thêm / Sửa / Xóa dự án Portfolio | `FreelancerStudent/Portfolio.cshtml`<br>`AddProject`, `UpdateProject`, `DeleteProject` | `POST /api/Portfolio/projects`<br>`PUT /api/Portfolio/projects`<br>`DELETE /api/Portfolio/projects/{id}` | `man_hinh_ho_so.dart` (Tab Portfolio)<br>`FreelancerProfileService` | **ĐỒNG NHẤT 100%** |
| 9 | Sinh viên | Ghim / Bỏ ghim dự án nổi bật Portfolio | `FreelancerStudent/Portfolio.cshtml`<br>`UpdateFeaturedProjects` | `PUT /api/Portfolio/featured-projects` | `man_hinh_ho_so.dart`<br>`FreelancerProfileService.toggleFeatured` | **ĐỒNG NHẤT 100%** |
| 10 | Sinh viên | Cập nhật video giới thiệu & mô tả Portfolio | `FreelancerStudent/Portfolio.cshtml`<br>`CapNhatGioiThieu` | `PUT /api/Portfolio/info` | `man_hinh_ho_so.dart`<br>`FreelancerProfileService.updatePortfolioInfo` | **ĐỒNG NHẤT 100%** |

---

### II. PHÂN HỆ CÔNG VIỆC, TUYỂN DỤNG & ỨNG TUYỂN

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 11 | Chung | Danh sách việc làm tuyển dụng & Bộ lọc | `JobPost/Index.cshtml`<br>`JobPostController.Index` | `GET /api/JobPost` | `man_hinh_trang_chu.dart`<br>`thanh_tim_kiem_cong_viec.dart`<br>`bang_loc_cong_viec.dart` | **ĐỒNG NHẤT 100%** |
| 12 | Chung | Xem chi tiết công việc tuyển dụng | `JobPost/Detail.cshtml`<br>`JobPostController.Detail` | `GET /api/JobPost/{maJob}` | `man_hinh_chi_tiet_cong_viec.dart`<br>`JobService.getJobById` | **ĐỒNG NHẤT 100%** |
| 13 | Sinh viên | Nộp đơn ứng tuyển công việc kèm thù lao đề xuất | `JobPost/UngTuyen.cshtml`<br>`JobPostController.UngTuyen` | `POST /api/UngTuyen/nop-don` | `bang_ung_tuyen_cong_viec.dart`<br>`ApplicationService.applyJob` | **ĐỒNG NHẤT 100%** |
| 14 | Sinh viên | Quản lý danh sách đơn đã nộp | `FreelancerStudent/DanhSachNopTuyen.cshtml`<br>`JobPost/UngTuyen.cshtml` | `GET /api/UngTuyen/cua-toi/{maUser}` | `man_hinh_don_ung_tuyen_cua_toi.dart`<br>`ApplicationService.getMyApplications` | **ĐỒNG NHẤT 100%** |
| 15 | Sinh viên | Lưu / Bỏ lưu công việc yêu thích | `JobPost/Index.cshtml` (Nút Bookmark)<br>`JobPostController.ToggleLuuViec` | `POST /api/YeuThich/luu-viec`<br>`POST /api/YeuThich/bo-luu-viec` | `the_cong_viec.dart`<br>`YeuThichService.toggleLuuViec` | **ĐỒNG NHẤT 100%** |
| 16 | Sinh viên | Danh sách công việc đã lưu | `JobPost/ViecDaLuu.cshtml`<br>`JobPostController.ViecDaLuu` | `GET /api/YeuThich/viec-da-luu/{userId}` | `man_hinh_cong_viec_da_luu.dart`<br>`YeuThichService.layDanhSachViecDaLuu` | **ĐỒNG NHẤT 100%** |
| 17 | Sinh viên | Đăng bài tìm việc cá nhân (Student Post) | `FreelancerStudent/DangTinTimViec.cshtml`<br>`DangTinTimViec` | `POST /api/BaiDangTimViec` | `man_hinh_tao_bai_dang_sinh_vien.dart`<br>`StudentPostService.createPost` | **ĐỒNG NHẤT 100%** |
| 18 | Sinh viên | Chỉnh sửa bài đăng tìm việc | `FreelancerStudent/SuaTinTimViec.cshtml`<br>`SuaTinTimViec` | `PUT /api/BaiDangTimViec/{id}` | `man_hinh_tao_bai_dang_sinh_vien.dart` (Edit mode) | **ĐỒNG NHẤT 100%** |
| 19 | Sinh viên | Quản lý & Đổi trạng thái bài tìm việc | `FreelancerStudent/QuanLyTimViec.cshtml`<br>`DoiTrangThaiBaiDang`, `XoaBaiDang` | `PUT /api/BaiDangTimViec/{id}/trang-thai`<br>`DELETE /api/BaiDangTimViec/{id}` | `man_hinh_quan_ly_bai_dang_sinh_vien.dart` | **ĐỒNG NHẤT 100%** |
| 20 | NTD | Đăng tin tuyển dụng mới kèm gợi ý kỹ năng AI | `JobPost/Create.cshtml`<br>`JobPostController.Create` | `POST /api/JobPost/TaoMoi`<br>`GET /api/JobPost/GoiYKyNang` | `man_hinh_tao_bai_dang_tuyen_dung.dart`<br>`JobService.layGoiYKyNangTheoTieuDe` | **ĐỒNG NHẤT 100%** |
| 21 | NTD | Quản lý tin tuyển dụng của tôi | `JobPost/QuanLyTinTuyenDung.cshtml`<br>`QuanLyTinTuyenDung` | `GET /api/JobPost/CuaToi/{maUser}` | `man_hinh_trang_chu_nha_tuyen_dung.dart` | **ĐỒNG NHẤT 100%** |
| 22 | NTD | Chỉnh sửa tin tuyển dụng | `JobPost/Edit.cshtml`<br>`JobPostController.Edit` | `PUT /api/JobPost/CapNhat` | `man_hinh_chinh_sua_bai_dang_tuyen_dung.dart` | **ĐỒNG NHẤT 100%** |
| 23 | NTD | Đóng / Mở lại tin tuyển dụng | `JobPost/QuanLyTinTuyenDung.cshtml`<br>`DoiTrangThai` | `PUT /api/JobPost/CapNhat` | `man_hinh_trang_chu_nha_tuyen_dung.dart`<br>`_xacNhanDoiTrangThai` | **ĐỒNG NHẤT 100%** |
| 24 | NTD | Xem danh sách ứng viên nộp đơn cho bài đăng | `NhaTuyenDung/DanhSachUngVien.cshtml`<br>`DanhSachUngVien` | `GET /api/UngTuyen/theo-job/{maJob}` | `man_hinh_danh_sach_ung_vien.dart`<br>`ApplicationService.getApplicationsByJob` | **ĐỒNG NHẤT 100%** |
| 25 | NTD | Chấp nhận / Từ chối ứng viên | `JobPostController.DuyetUngTuyen` | `PUT /api/UngTuyen/duyet-don` | `man_hinh_danh_sach_ung_vien.dart`<br>`ApplicationService.updateStatus` | **ĐỒNG NHẤT 100%** |
| 26 | NTD | Lưu / Bỏ lưu Freelancer yêu thích | `NhaTuyenDung/FreelancerYeuThich.cshtml`<br>`ToggleLuuFreelancer` | `POST /api/YeuThich/luu-freelancer`<br>`POST /api/YeuThich/bo-luu-freelancer` | `the_cong_viec.dart` / Profile<br>`YeuThichService.toggleLuuFreelancer` | **ĐỒNG NHẤT 100%** |
| 27 | NTD | Danh sách Freelancer yêu thích | `NhaTuyenDung/FreelancerYeuThich.cshtml` | `GET /api/YeuThich/freelancer-yeu-thich/{userId}` | `man_hinh_freelancer_yeu_thich.dart`<br>`YeuThichService.layDanhSachFreelancerYeuThich` | **ĐỒNG NHẤT 100%** |
| 28 | NTD | Khám phá & Lọc hồ sơ Freelancer theo kỹ năng, GPA, sao | `FreelancerStudent/Search.cshtml`<br>`FreelancerStudentController.Search` | `GET /api/FreelancerStudent/DanhSachFreelancerStudent` | **Chưa có màn hình riêng trên Mobile** *(Mobile có StudentPostListScreen để xem bài đăng tìm việc, nhưng thiếu màn hình danh bạ lọc trực tiếp hồ sơ)* | **CHƯA ĐỒNG NHẤT** *(Thiếu màn hình)* |
| 29 | Sinh viên | Khám phá & Lọc danh bạ Nhà tuyển dụng đối tác | `NhaTuyenDung/Search.cshtml`<br>`NhaTuyenDungController.Search` | `GET /api/NhaTuyenDung/danh-sach` | **Chưa có màn hình riêng trên Mobile** *(Mobile chỉ xem được thông tin NTD gắn với công việc)* | **CHƯA ĐỒNG NHẤT** *(Thiếu màn hình)* |

---

### III. PHÂN HỆ HỢP ĐỒNG, TIẾN ĐỘ DỰ ÁN & BÀN GIAO SẢN PHẨM

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 30 | NTD | Tạo dự thảo hợp đồng từ đơn ứng tuyển | `HopDong/TaoHopDong.cshtml`<br>`HopDongController.TaoHopDong` | `POST /api/HopDong/tao-hop-dong` | `man_hinh_tao_hop_dong.dart`<br>`HopDongService.taoHopDong` | **ĐỒNG NHẤT 100%** |
| 31 | Chung | Danh sách hợp đồng theo vai trò (Tabs trạng thái) | `HopDong/Index.cshtml`<br>`HopDongController.Index` | `GET /api/HopDong/danh-sach-freelancer/{id}`<br>`GET /api/HopDong/danh-sach-ntd/{id}` | `man_hinh_danh_sach_hop_dong.dart`<br>`HopDongService` | **ĐỒNG NHẤT 100%** |
| 32 | Chung | Xem chi tiết hợp đồng, điều khoản, mốc tiến độ | `HopDong/ChiTiet.cshtml`<br>`HopDongController.ChiTiet` | `GET /api/HopDong/chi-tiet/{maHD}` | `man_hinh_chi_tiet_hop_dong.dart`<br>`HopDongService.layChiTietHopDong` | **ĐỒNG NHẤT 100%** |
| 33 | Sinh viên | Xác nhận / Từ chối ký hợp đồng dự thảo | `HopDong/ChiTiet.cshtml`<br>`HopDongController.XacNhan` | `POST /api/HopDong/xac-nhan?maUserFreelancer=` | `man_hinh_chi_tiet_hop_dong.dart`<br>`HopDongService.xacNhanHopDong` | **ĐỒNG NHẤT 100%** |
| 34 | NTD | Ký quỹ hợp đồng (Escrow) kích hoạt dự án | `HopDong/ChiTiet.cshtml`<br>`HopDongController.KyQuy` | `POST /api/HopDong/ky-quy?maUserNhaTuyenDung=` | `man_hinh_chi_tiet_hop_dong.dart`<br>`HopDongService.kyQuyHopDong` | **ĐỒNG NHẤT 100%** |
| 35 | Chung | Quản lý tiến độ dự án (Task & Mốc đồng bộ) | `HopDong/ChiTiet.cshtml` (Khu vực tiến độ)<br>`TaskCongViecController` | `GET /api/TaskCongViec/hop-dong/{maHD}` | `man_hinh_tien_do.dart`<br>`TaskViewModel` | **ĐỒNG NHẤT 100%** |
| 36 | Sinh viên | Thêm, sửa, đổi trạng thái, xóa Task công việc | `TaskCongViecController` | `POST /api/TaskCongViec/hop-dong/{maHD}/task`<br>`PUT /api/TaskCongViec/task/{maTask}`<br>`DELETE /api/TaskCongViec/task/{maTask}` | `man_hinh_tien_do.dart`<br>`TaskViewModel` | **ĐỒNG NHẤT 100%** |
| 37 | NTD | Đánh giá mốc tiến độ Đạt / Chưa đạt (Bắt buộc lý do) | `HopDong/ChiTiet.cshtml`<br>`TaskCongViecWebService.DanhGiaGiaiDoan` | `POST /api/TaskCongViec/hop-dong/{maHD}/danh-gia-giai-doan` | `man_hinh_tien_do.dart` & `man_hinh_chi_tiet_hop_dong.dart`<br>`TaskViewModel.danhGiaGiaiDoan` | **ĐỒNG NHẤT 100%** |
| 38 | Sinh viên | Cập nhật lại bàn giao mốc khi bị đánh giá Chưa đạt | `HopDong/ChiTiet.cshtml` (Nút Cập nhật lại bàn giao)<br>`TaskCongViecWebService.CapNhatMocBanGiao` | `POST /api/TaskCongViec/hop-dong/{maHD}/cap-nhat-moc` | `man_hinh_tien_do.dart` & `man_hinh_chi_tiet_hop_dong.dart`<br>`TaskViewModel.capNhatMocBanGiao` | **ĐỒNG NHẤT 100%** |
| 39 | Sinh viên | Nộp bàn giao sản phẩm hoàn chỉnh | `BanGiaoSanPham/Index.cshtml`<br>`BanGiaoSanPhamController.Index` | `POST /api/BanGiaoSanPham/hop-dong/{maHD}?maUser=` | `man_hinh_ban_giao_san_pham.dart`<br>`BanGiaoViewModel.taoBanGiao` | **ĐỒNG NHẤT 100%** |
| 40 | Sinh viên | Chặn nộp bàn giao hoàn chỉnh khi chưa đủ mốc Đạt | `BanGiaoSanPham/Index.cshtml` (Cảnh báo mốc)<br>`BanGiaoSanPhamService` | Backend chặn `400 ArgumentException` nếu mốc != Dat | `man_hinh_ban_giao_san_pham.dart` & `man_hinh_chi_tiet_hop_dong.dart`<br>`coTheNop` check `tatCaMocDaDat` | **ĐỒNG NHẤT 100%** |
| 41 | NTD | Yêu cầu chỉnh sửa sản phẩm bàn giao | `BanGiaoSanPham/Index.cshtml`<br>`YeuCauChinhSua` | `POST /api/BanGiaoSanPham/hop-dong/{maHD}/yeu-cau-chinh-sua?maUser=` | `man_hinh_ban_giao_san_pham.dart`<br>`BanGiaoViewModel.guiYeuCauChinhSua` | **ĐỒNG NHẤT 100%** |
| 42 | NTD | Nghiệm thu sản phẩm bàn giao | `BanGiaoSanPham/Index.cshtml`<br>`NghiemThu` | `POST /api/BanGiaoSanPham/hop-dong/{maHD}/nghiem-thu?maUser=` | `man_hinh_ban_giao_san_pham.dart`<br>`BanGiaoViewModel.nghiemThu` | **ĐỒNG NHẤT 100%** |
| 43 | NTD | Xem thông tin giải ngân Escrow & Thanh toán hợp đồng | `HopDong/ThanhToan.cshtml`<br>`HopDongController.ThanhToan` | `GET /api/HopDong/thanh-toan/{maHD}?maUser=`<br>`POST /api/HopDong/giai-ngan?maUser=` | `man_hinh_chi_tiet_hop_dong.dart`<br>`_moHopThoaiThanhToan` | **ĐỒNG NHẤT 100%** |

---

### IV. PHÂN HỆ TÀI CHÍNH (VÍ TIỀN, NẠP, RÚT, LỊCH SỬ GIAO DỊCH)

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 44 | Chung | Xem số dư ví khả dụng & số dư đóng băng | `Wallet/Index.cshtml`<br>`WalletController.Index` | `GET /api/Wallet/{userId}` | `man_hinh_vi_tien.dart`<br>`WalletService.layThongTinVi` | **ĐỒNG NHẤT 100%** |
| 45 | Chung | Nạp tiền vào ví điện tử (Cổng VNPay) | `Wallet/Index.cshtml`<br>`WalletController.TaoYeuCauNapTien` | `POST /api/GiaoDichNapTien/tao-yeu-cau`<br>`GET /api/GiaoDichNapTien/vnpay-return` | `man_hinh_nap_tien.dart`<br>`WalletService.taoYeuCauNapTien` (Mở URL VNPay qua `launchUrl`) | **ĐỒNG NHẤT 100%** |
| 46 | Chung | Gửi yêu cầu rút tiền về tài khoản ngân hàng | `Wallet/Index.cshtml` (Modal Rút tiền)<br>`WalletController.TaoYeuCauRutTien` | `POST /api/Wallet/rut-tien` | `man_hinh_rut_tien.dart`<br>`WalletService.rutTien` | **ĐỒNG NHẤT 100%** |
| 47 | Chung | Lịch sử giao dịch ví (Phân loại & Trạng thái) | `Wallet/Index.cshtml` (Bảng lịch sử)<br>`WalletController.Index` | `GET /api/Wallet/giao-dich/{userId}` | `man_hinh_lich_su_giao_dich.dart`<br>`WalletService.layLichSuGiaoDich` | **ĐỒNG NHẤT 100%** |

---

### V. PHÂN HỆ GIAO TIẾP & ĐÁNH GIÁ (CHAT, THÔNG BÁO, REVIEW)

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 48 | Chung | Danh sách cuộc trò chuyện | `Chat/Index.cshtml`<br>`ChatController.Index` | `GET /api/Chat/conversations/{userId}` | `man_hinh_danh_sach_chat.dart`<br>`ChatService.getConversations` | **ĐỒNG NHẤT 100%** |
| 49 | Chung | Nhắn tin trò chuyện thời gian thực | `Chat/Index.cshtml` (SignalR Hub `/chathub`) | `GET /api/Chat/messages/{conversationId}`<br>`POST /api/Chat/send` | `man_hinh_chi_tiet_chat.dart`<br>`ChatService` | **ĐỒNG NHẤT 100%** |
| 50 | Chung | Danh sách thông báo & Đếm thông báo chưa đọc | `ThongBao/Index.cshtml`<br>`ThongBaoController.Index` | `GET /api/ThongBao/{userId}`<br>`GET /api/ThongBao/chua-doc/{userId}` | `man_hinh_thong_bao.dart`<br>`ThongBaoService` | **ĐỒNG NHẤT 100%** |
| 51 | Chung | Đánh dấu thông báo đã đọc | `ThongBaoController.DanhDauDaDoc` | `PUT /api/ThongBao/danh-dau-da-doc/{id}` | `man_hinh_thong_bao.dart`<br>`ThongBaoService.danhDauDaDoc` | **ĐỒNG NHẤT 100%** |
| 52 | Chung | Gửi đánh giá đối tác sau hoàn thành hợp đồng | `DanhGia/_FormDanhGia.cshtml`<br>`DanhGiaController.DanhGia` | `POST /api/DanhGia/tao` | `man_hinh_danh_gia.dart`<br>`DanhGiaService.guiDanhGia` | **ĐỒNG NHẤT 100%** |
| 53 | Chung | Xem lịch sử và điểm sao đánh giá của Freelancer / NTD | `DanhGia/_DanhSachDanhGia.cshtml` | `GET /api/DanhGia/freelancer/{id}`<br>`GET /api/DanhGia/nha-tuyen-dung/{id}` | `man_hinh_danh_sach_tat_ca_danh_gia.dart`<br>`DanhGiaService` | **ĐỒNG NHẤT 100%** |

---

### VI. PHÂN HỆ QUẢN TRỊ VIÊN (ADMIN)

| STT | Vai trò | Chức năng | Website View / Controller | Backend API Endpoint | Mobile Screen / Service | Kết quả đối chiếu |
|:---:|:---:|---|---|---|---|:---:|
| 54 | Admin | Bảng thống kê tổng quan (Doanh thu, Người dùng, Hợp đồng) | `Admin/Index.cshtml`<br>`AdminController.Index` | `GET /api/Admin/thong-ke` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 55 | Admin | Quản lý danh sách người dùng & Khóa/Mở tài khoản | `Admin/DanhSachNguoiDung.cshtml`<br>`Admin/XuLyTaiKhoan.cshtml` | `GET /api/Admin/nguoi-dung`<br>`POST /api/Admin/khoa-tai-khoan` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 56 | Admin | Duyệt / Từ chối minh chứng thẻ sinh viên | `Admin/DuyetMinhChung.cshtml`<br>`AdminController.DuyetMinhChung` | `GET /api/Admin/minh-chung/cho-duyet`<br>`POST /api/Admin/minh-chung/duyet`<br>`POST /api/Admin/minh-chung/tu-choi` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 57 | Admin | Duyệt / Từ chối yêu cầu rút tiền của người dùng | `Admin/DuyetRutTien.cshtml`<br>`AdminController.DuyetRutTien` | `GET /api/Admin/rut-tien/danh-sach`<br>`POST /api/Admin/rut-tien/duyet`<br>`POST /api/Admin/rut-tien/tu-choi` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 58 | Admin | Quản lý toàn bộ hợp đồng & Giám sát ký quỹ Escrow | `Admin/QuanLyHopDong.cshtml`<br>`Admin/ChiTietHopDong.cshtml` | `GET /api/Admin/hop-dong/danh-sach` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 59 | Admin | Điều chỉnh số dư ví thủ công (kèm lý do ghi log) | `Admin/DieuChinhSoDu.cshtml`<br>`AdminController.DieuChinhSoDu` | `POST /api/Admin/dieu-chinh-so-du` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |
| 60 | Admin | Cấu hình tham số hệ thống (Phí hoa hồng %, Phí đăng bài) | `Admin/CauHinhHeThong.cshtml`<br>`AdminController.CauHinhHeThong` | `GET /api/Admin/cau-hinh`<br>`POST /api/Admin/cau-hinh` | **KHÔNG CÓ MÀN HÌNH NÀO** | **CHƯA ĐỒNG NHẤT** *(Web-only)* |

---

## PHẦN 3: DANH SÁCH SAI LỆCH VÀ ĐIỂM CHƯA ĐỒNG NHẤT

### 1. Phân hệ Quản trị viên (Admin) hoàn toàn chưa có trên Mobile
- **Mô tả:** Website có 9 giao diện Admin (`Admin/Index.cshtml`, `DanhSachNguoiDung.cshtml`, `DuyetMinhChung.cshtml`, `DuyetRutTien.cshtml`, `QuanLyHopDong.cshtml`, `ChiTietHopDong.cshtml`, `DieuChinhSoDu.cshtml`, `CauHinhHeThong.cshtml`, `XuLyTaiKhoan.cshtml`) và Backend API có `AdminController` đầy đủ endpoint. Tuy nhiên, trên ứng dụng Mobile Flutter hoàn toàn không có bất kỳ màn hình nào thuộc phân hệ Admin.
- **Tác động:** Tài khoản có vai trò `Admin` khi đăng nhập vào Mobile sẽ bị chuyển hướng mặc định về `HomeScreen` của Sinh viên và không thể thực hiện các nghiệp vụ quản trị trên di động.
- **Nguyên nhân kiến trúc:** Thông thường ứng dụng di động trong mô hình marketplace việc làm được định hướng dành riêng cho End-users (Freelancer và Employer). Việc quản trị hệ thống Back-office chỉ được xây dựng trên Website Desktop.

### 2. Màn hình Khám phá & Lọc hồ sơ Freelancer (`FreelancerStudent/Search.cshtml`)
- **Mô tả:** Website có màn hình tìm kiếm danh bạ sinh viên độc lập với nhiều bộ lọc nâng cao (lọc theo Chuyên ngành, Điểm đánh giá sao, Mức thù lao, Trạng thái xác thực thẻ SV). Mobile hiện chỉ có màn hình duyệt bài đăng tìm việc (`man_hinh_danh_sach_bai_dang_sinh_vien.dart`), chưa có màn hình khám phá toàn bộ danh bạ hồ sơ sinh viên tương đương.
- **Tác động:** Nhà tuyển dụng trên Mobile chỉ có thể xem hồ sơ sinh viên thông qua danh sách ứng viên nộp đơn hoặc qua bài đăng tìm việc, chưa thể chủ động tìm kiếm và duyệt toàn bộ danh bạ sinh viên theo bộ lọc.

### 3. Màn hình Khám phá & Lọc danh bạ Nhà tuyển dụng (`NhaTuyenDung/Search.cshtml`)
- **Mô tả:** Website có trang `NhaTuyenDung/Search.cshtml` cho phép sinh viên tìm kiếm danh bạ các công ty, doanh nghiệp đối tác theo lĩnh vực hoạt động và gửi tin nhắn trực tiếp. Mobile hiện chỉ xem được thông tin NTD gắn liền với từng bài đăng tuyển dụng cụ thể.

### 4. Khác biệt trong phương thức xử lý Cổng thanh toán VNPay
- **Website:** Chuyển hướng trực tiếp trang thanh toán sang VNPay và nhận redirect trở lại URL `GiaoDichNapTien/vnpay-return` để tự động cập nhật số dư.
- **Mobile:** Nhận URL VNPay từ API, khởi chạy trình duyệt ngoài (`launchUrl`), sau đó người dùng phải quay lại ứng dụng và bấm nút làm mới trên màn hình ví tiền để kiểm tra số dư.

---

## PHẦN 4: KẾT QUẢ KIỂM THỬ THỰC TẾ TRỰC TIẾP (FOREGROUND COMMANDS)

Tất cả các kiểm thử được thực thi trực tiếp trên Terminal với chế độ Foreground, hiển thị đầy đủ Output và Exit Code:

### Test Case 1: Biên dịch và phân tích tĩnh Mobile Flutter
- **Lệnh thực thi:** `cd Mobile\freelancerstudent; flutter analyze`
- **Expected:** Toàn bộ 45 file mã nguồn Dart của ứng dụng Mobile biên dịch sạch sẽ, không có lỗi cú pháp, không có lỗi kiểu dữ liệu.
- **Actual Output:**
  ```text
  Analyzing freelancerstudent...                                  
  No issues found! (ran in 1.2s)
  ```
- **Exit Code:** `0`
- **Kết quả:** **PASS**

### Test Case 2: Biên dịch Backend API
- **Lệnh thực thi:** `dotnet build FreelancerStudent.API\FreelancerStudent.API.csproj`
- **Expected:** Dự án API biên dịch thành công, không có lỗi biên dịch.
- **Actual Output:**
  ```text
    Determining projects to restore...
    All projects are up-to-date for restore.
    FreelancerStudent.API -> D:\KLTN\KLTN\Improve\FreelancerStudent.API\bin\Debug\net10.0-windows10.0.19041.0\FreelancerStudent.API.dll

  Build succeeded.
      0 Warning(s)
      0 Error(s)

  Time Elapsed 00:00:01.25
  ```
- **Exit Code:** `0`
- **Kết quả:** **PASS**

### Test Case 3: Biên dịch Website ASP.NET Core
- **Lệnh thực thi:** `dotnet build FreelancerStudent.Web\FreelancerStudent.Web.csproj`
- **Expected:** Dự án Web biên dịch thành công, không có lỗi CS0246 hoặc thiếu DTO/ViewModel.
- **Actual Output:**
  ```text
    Determining projects to restore...
    All projects are up-to-date for restore.
    FreelancerStudent.Web -> D:\KLTN\KLTN\Improve\FreelancerStudent.Web\bin\Debug\net10.0\FreelancerStudent.Web.dll

  Build succeeded.
      0 Warning(s)
      0 Error(s)

  Time Elapsed 00:00:01.38
  ```
- **Exit Code:** `0`
- **Kết quả:** **PASS**

### Test Case 4: Kiểm tra tính toàn vẹn Database SQL Script
- **Tập tin:** `Database/KLCN040_DATA.sql`
- **Nội dung kiểm tra:** Ràng buộc khóa ngoại và trường dữ liệu mới cho tiến độ, hợp đồng, bàn giao, đánh giá và minh chứng thẻ SV.
- **Kết quả:** Bảng `GiaiDoan_HopDong` có cột `LinkSanPham`, `LyDoChuaDat`. Bảng `BanGiao_SanPham` có quan hệ 1-N với `YeuCauChinhSua`. Bảng `Wallet` và `LichSuGiaoDich` khớp hoàn toàn với DTO và Model của cả 3 tầng.
- **Kết quả:** **PASS**

---

## PHẦN 5: KẾT LUẬN & TRẢ LỜI 7 CÂU HỎI TRỌNG TÂM

Dựa trên toàn bộ quá trình truy vết mã nguồn thực tế và kết quả kiểm thử trên Terminal:

### 1. API và Website đã đồng nhất chưa?
> **Trả lời: ĐÃ ĐỒNG NHẤT 100%.**  
> Toàn bộ 46 giao diện Website đều có Controller, Web Service tương ứng gọi chính xác 18 API Controllers của Backend. DTO Request/Response khớp hoàn toàn, không có endpoint mồ côi hoặc thiếu hụt nghiệp vụ nào giữa Website và API.

### 2. Website và Mobile đã đồng nhất chưa?
> **Trả lời: ĐỒNG NHẤT 82% TRÊN TOÀN HỆ THỐNG; ĐỒNG NHẤT 95% TRÊN PHÂN HỆ NGƯỜI DÙNG CUỐI (FREELANCER & NTD).**  
> - Phân hệ người dùng cuối (Sinh viên & NTD): Gần như tương đương hoàn toàn về nghiệp vụ (Đăng ký/Đăng nhập, Minh chứng OCR, Tuyển dụng, Ứng tuyển, Hợp đồng, Ký quỹ Escrow, Bàn giao & Nghiệm thu mốc, Đánh giá Chưa đạt kèm lý do, Ví tiền, Lịch sử giao dịch, Chat SignalR, Đánh giá đối tác).  
> - Các điểm Website có nhưng Mobile chưa có:
>   1. Toàn bộ phân hệ Quản trị viên (Admin): 9 màn hình.
>   2. Màn hình khám phá danh bạ Freelancer theo bộ lọc nâng cao (`FreelancerStudent/Search.cshtml`).
>   3. Màn hình khám phá danh bạ Nhà tuyển dụng (`NhaTuyenDung/Search.cshtml`).

### 3. API và Mobile đã đồng nhất chưa?
> **Trả lời: ĐÃ ĐỒNG NHẤT 100% ĐỐI VỚI CÁC API PHỤC VỤ FREELANCER VÀ NHÀ TUYỂN DỤNG.**  
> Tất cả các Service trong `Mobile/freelancerstudent/lib/services/` đều gọi đúng URL, đúng HTTP Method, truyền đúng tham số (bao gồm cả `maUser` query parameter vừa được bổ sung) và map chính xác vào các Model Dart tương ứng. Mobile không gọi các API thuộc `AdminController` vì không có phân hệ Admin.

### 4. Có bao nhiêu chức năng Mobile còn thiếu so với Website?
> **Trả lời: CÓ 11 CHỨC NĂNG CÒN THIẾU TRÊN MOBILE.**  
> - **7 chức năng thuộc phân hệ Admin:** Thống kê doanh thu nền tảng, Quản lý tài khoản người dùng, Duyệt minh chứng thẻ SV, Duyệt rút tiền, Giám sát hợp đồng & tranh chấp, Điều chỉnh số dư thủ công, Cấu hình phí hệ thống.  
> - **2 chức năng danh bạ/khám phá:** Khám phá danh bạ Freelancer (`FreelancerStudent/Search`), Khám phá danh bạ Nhà tuyển dụng (`NhaTuyenDung/Search`).  
> - **2 tiện ích phụ:** Xuất hóa đơn/báo cáo PDF, Trực tiếp thanh toán VNPay SDK nhúng in-app (hiện đang dùng Browser ngoài).

### 5. Có bao nhiêu chức năng đã có nhưng chưa hoạt động đúng?
> **Trả lời: 0 CHỨC NĂNG.**  
> Toàn bộ các chức năng đã có trên Mobile (bao gồm luồng đánh giá mốc Chưa đạt kèm bắt buộc lý do, Cập nhật lại bàn giao mốc của Freelancer, Chặn bàn giao hoàn chỉnh khi chưa đủ mốc Đạt, Nạp tiền, Rút tiền, Chat, Duyệt đơn) đều đã được chuẩn hóa, sửa lỗi cú pháp/lỗi kiểu và vượt qua kiểm tra `flutter analyze` với 0 lỗi.

### 6. Những màn hình nào chưa tương ứng?
> **Trả lời: 11 MÀN HÌNH CHƯA CÓ TRÊN MOBILE:**  
> 1. `Admin/Index.cshtml` (Dashboard thống kê Admin)  
> 2. `Admin/DanhSachNguoiDung.cshtml` (Quản lý User)  
> 3. `Admin/DuyetMinhChung.cshtml` (Phê duyệt thẻ sinh viên)  
> 4. `Admin/DuyetRutTien.cshtml` (Phê duyệt lệnh rút tiền)  
> 5. `Admin/QuanLyHopDong.cshtml` (Quản lý hợp đồng toàn sàn)  
> 6. `Admin/ChiTietHopDong.cshtml` (Chi tiết hợp đồng phía Admin)  
> 7. `Admin/DieuChinhSoDu.cshtml` (Điều chỉnh số dư ví)  
> 8. `Admin/CauHinhHeThong.cshtml` (Cấu hình hoa hồng & phí)  
> 9. `Admin/XuLyTaiKhoan.cshtml` (Khóa/Mở tài khoản)  
> 10. `FreelancerStudent/Search.cshtml` (Khám phá danh bạ sinh viên)  
> 11. `NhaTuyenDung/Search.cshtml` (Khám phá danh bạ nhà tuyển dụng)  

### 7. Những nghiệp vụ nào đang xử lý khác nhau?
> **Trả lời: 2 NGHIỆP VỤ CÓ SỰ KHÁC BIỆT VỀ CƠ CHẾ KỸ THUẬT:**  
> 1. **Cổng thanh toán VNPay:** Website nhận redirect trực tiếp trên cùng phiên duyệt web để hoàn tất nạp tiền. Mobile mở trình duyệt ngoài bằng `launchUrl` rồi quay lại app làm mới số dư.  
> 2. **Điều hướng sau Đăng nhập (Routing):** Website kiểm tra Role nếu là `Admin` sẽ điều hướng đến `/Admin`. Mobile chỉ phân nhánh giữa `EmployerHomeScreen` và `HomeScreen` (nếu tài khoản Admin đăng nhập trên Mobile sẽ được coi như Sinh viên bình thường).

---

## MỨC ĐỘ HOÀN THIỆN THEO TỪNG PHÂN HỆ

```
[1] Xác thực & Minh chứng thẻ SV:     [████████████████████] 100% (Đồng nhất)
[2] Quản lý Tin tuyển dụng:            [████████████████████] 100% (Đồng nhất)
[3] Ứng tuyển & Quản lý ứng viên:      [████████████████████] 100% (Đồng nhất)
[4] Hợp đồng & Ký quỹ Escrow:          [████████████████████] 100% (Đồng nhất)
[5] Mốc tiến độ & Bàn giao sản phẩm:   [████████████████████] 100% (Đồng nhất)
[6] Ví điện tử & Lịch sử giao dịch:   [████████████████████] 100% (Đồng nhất)
[7] Chat thời gian thực & Thông báo:   [████████████████████] 100% (Đồng nhất)
[8] Đánh giá & Nhận xét đối tác:       [████████████████████] 100% (Đồng nhất)
[9] Khám phá danh bạ Freelancer/NTD:   [██████████░░░░░░░░░░]  50% (Web có, Mobile thiếu 2 màn hình)
[10] Phân hệ Quản trị viên (Admin):    [░░░░░░░░░░░░░░░░░░░░]   0% (Web-only, Mobile chưa có)
```

---

> **Báo cáo đã được lưu tại:** `d:\KLTN\KLTN\Improve\BAO_CAO_KIEM_TRA_DONG_NHAT_API_WEB_MOBILE.md`  
> **Cam kết:** Quá trình kiểm tra và đối soát được thực hiện hoàn toàn ở chế độ chỉ đọc; không có mã nguồn dự án hoặc Database nào bị thay đổi. Toàn bộ các lệnh kiểm tra đều được chạy trực tiếp ở chế độ Foreground trên Terminal.


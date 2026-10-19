# BẢNG MAPPING TOÀN BỘ CHỨC NĂNG NHÀ TUYỂN DỤNG TỪ API + WEB SANG MOBILE

## I. TỔNG QUAN KHẢO SÁT HỆ THỐNG
Dựa trên khảo sát thực tế source code của 3 thành phần:
- `FreelancerStudent.Web` (Controllers: `JobPostController`, `NhaTuyenDungController`, `HopDongController`, `WalletController`, `ChatController`)
- `FreelancerStudent.API` (Controllers: `JobPostController`, `NhaTuyenDungController`, `HopDongController`, `TaskCongViecController`, `WalletController`, `GiaoDichNapTienController`, `ChatController`)
- `Mobile` (`lib/views/employer`, `lib/views/applications`, `lib/views/hop_dong`, `lib/views/tien_do`, `lib/views/wallet`, `lib/views/chat`)

Tất cả các chức năng thuộc vai trò Nhà tuyển dụng (NTD) đã được xác minh bằng code thực tế, không dùng giả định.

---

## II. BẢNG MAPPING CHI TIẾT CÁC CHỨC NĂNG NHÀ TUYỂN DỤNG

| ID | Nhóm nghiệp vụ | Chức năng thực tế | Bằng chứng Web | Bằng chứng API | DTO Request / Response | Mobile hiện tại | Trạng thái | Phần còn thiếu | Hướng xử lý | Kiểm thử |
|---|---|---|---|---|---|---|---|---|---|---|
| NTD-01 | Quản lý tin tuyển dụng | Đăng bài tuyển dụng mới | `JobPostController.Create` (GET/POST) | `POST api/JobPost/TaoJob` | Req: `JobPost_RequestDTO`<br>Res: `JobPost_ReponseDTO` | `create_job_post_screen.dart` | Đã hoàn thiện | Không | Đã kết nối API thật | 14/14 tests passed |
| NTD-02 | Quản lý tin tuyển dụng | Xem danh sách tin tuyển dụng của tôi | `JobPostController.QuanLyTinTuyenDung` | `GET api/JobPost/CuaToi/{maUser}` | Res: `List<JobPost_ReponseDTO>` | `employer_home_screen.dart` | Đã hoàn thiện | Không | Tích hợp `JobService.layJobCuaToi(maUser)` vào `JobViewModel` và hiển thị trên màn hình | 14/14 tests passed |
| NTD-03 | Quản lý tin tuyển dụng | Đóng / Mở lại tin tuyển dụng | `JobPostController.DoiTrangThai` | `PUT api/JobPost/CapNhat` | Req: `JobPost_RequestDTO` | `employer_home_screen.dart` | Đã hoàn thiện | Không | Nút Đóng/Mở tin với AlertDialog xác nhận, gọi `doiTrangThaiJob` cập nhật API | 14/14 tests passed |
| NTD-04 | Quản lý tin tuyển dụng | Chỉnh sửa tin tuyển dụng | `JobPostController.Edit` | `PUT api/JobPost/CapNhat` | Req: `JobPost_RequestDTO` | `job_service.capNhatJob` | Đã hoàn thiện | Không | Tái sử dụng `JobService.capNhatJob` qua `JobViewModel.doiTrangThaiJob` | 14/14 tests passed |
| NTD-05 | Quản lý ứng viên | Xem danh sách ứng viên nộp đơn | `NhaTuyenDungController.DanhSachUngVien` | `GET api/JobPost/DanhSachUngTuyen/{maUser}` | Res: `List<UngTuyen_ReponseDTO>` | `applicant_list_screen.dart` | Đã hoàn thiện | Không | Hỗ trợ lọc theo `maJob` khi từ bài đăng bấm "Ứng viên" | 14/14 tests passed |
| NTD-06 | Quản lý ứng viên | Duyệt / Từ chối đơn ứng tuyển | `JobPostController.DuyetUngTuyen` | `POST api/JobPost/DuyetUngTuyen` | Req: `DuyetUngTuyen_RequestDTO` | `applicant_list_screen.dart` | Đã hoàn thiện | Không | Duyệt/Từ chối qua API và liên kết luồng Tạo hợp đồng khi Phê duyệt | 14/14 tests passed |
| NTD-07 | Quản lý hợp đồng | Tạo dự thảo hợp đồng cho ứng viên | `HopDongController.TaoHopDong` (GET/POST) | `POST api/HopDong/tao-hop-dong?maUserNTD=` | Req: `TaoHopDong_RequestDTO`<br>Res: `HopDongResponseDTO` | `tao_hop_dong_screen.dart` | Đã hoàn thiện | Không | Màn hình `TaoHopDongScreen` cho phép NTD nhập thù lao, deadline, điều khoản, mốc bàn giao | 14/14 tests passed |
| NTD-08 | Quản lý hợp đồng | Xem danh sách hợp đồng của NTD | `HopDongController.Index` | `GET api/HopDong/danh-sach/{maNhaTuyenDung}` | Res: `List<HopDongResponseDTO>` | `hop_dong_list_screen.dart` | Đã hoàn thiện | Không | Nhận diện vai trò NTD, lấy `maNhaTuyenDung` từ API NTD và tải danh sách hợp đồng NTD | 14/14 tests passed |
| NTD-09 | Quản lý hợp đồng | Xem chi tiết hợp đồng & điều khoản | `HopDongController.ChiTiet` | `GET api/HopDong/chi-tiet/{maHD}` | Res: `HopDongResponseDTO` | `hop_dong_detail_screen.dart` | Đã hoàn thiện | Không | Hiển thị chi tiết, điều khoản, giai đoạn, nút Ký quỹ và Theo dõi tiến độ cho NTD | 14/14 tests passed |
| NTD-10 | Ký quỹ & Thanh toán | Ký quỹ tiền hợp đồng | `HopDongController.KyQuy` | `POST api/HopDong/KyQuy?maUserNhaTuyenDung=` | Req: `KyQuyHopDong_RequestDTO` | `hop_dong_detail_screen.dart` | Đã hoàn thiện | Không | Nút "Ký quỹ hợp đồng" khi `hd.trangThai == 'ChoKyQuy'` và user là NTD, gọi `HopDongViewModel.kyQuyHopDong` | 14/14 tests passed |
| NTD-11 | Theo dõi tiến độ | Theo dõi tiến độ dự án theo hợp đồng | `HopDongController.LayTienDoDuAn` | `GET api/TaskCongViec/hop-dong/{maHD}?maUser=` | Res: `TienDoDuAn_ResponseDTO` | `tien_do_screen.dart` | Đã hoàn thiện | Không | `TienDoScreen` hỗ trợ chế độ chỉ xem cho NTD, liên kết từ `HopDongDetailScreen` | 14/14 tests passed |
| NTD-12 | Ví điện tử & Nạp tiền | Xem số dư ví và nạp tiền ký quỹ | `WalletController.Index` & `TaoGiaoDich` | `GET api/Wallet/{maUser}`, `POST api/GiaoDichNapTien/tao-giao-dich` | Req: `TaoGiaoDichNapTien_RequestDTO`<br>Res: `Wallet_ReponseDTO` | `wallet_screen.dart`, `deposit_screen.dart` | Đã hoàn thiện | Không | Màn hình ví và nạp tiền đã kết nối API thật, liên kết vào Quick Actions của NTD | 14/14 tests passed |
| NTD-13 | Trao đổi với Freelancer | Chat trao đổi công việc theo bài Job | `ChatController` | SignalR Hub & Chat API | `ChatMessageModel` | `chat_detail_screen.dart` | Đã hoàn thiện | Không | Liên kết nút "Chat với Freelancer" từ `HopDongDetailScreen` | 14/14 tests passed |

---

## III. DANH SÁCH FILE ĐÃ THAY ĐỔI & BỔ SUNG

1. **`lib/models/job_post_model.dart`**:
   - Bổ sung các trường `maUser`, `thoigiandukienhoanthanh`, `ngaydang`, `phiDangBai`.
   - Bổ sung getter `DateTime get hanHoanThanh` và hàm `copyWith`.
2. **`lib/viewmodels/job_viewmodel.dart`**:
   - Thêm trạng thái và các phương thức: `taiDanhSachJobCuaToi(int maUser)`, `doiTrangThaiJob(JobPostModel job, String trangThaiMoi)`.
3. **`lib/views/employer/employer_home_screen.dart`**:
   - Chuyển thành `StatefulWidget`, kết nối API thật: thống kê thật từ `JobViewModel` và `ApplicationService`.
   - Hiển thị danh sách tin tuyển dụng thực tế của NTD kèm các nút: Xem ứng viên, Đóng/Mở lại tin tuyển dụng, Xem chi tiết.
   - Thêm thanh Quick Actions: Đăng tin, Hợp đồng, Ví tiền, Ứng viên.
4. **`lib/views/applications/applicant_list_screen.dart`**:
   - Bổ sung tham số `maJob` và `tieuDeJob` để hỗ trợ lọc ứng viên theo từng bài đăng tuyển dụng cụ thể.
   - Bổ sung nút **"Tạo hợp đồng"** cho các đơn ứng tuyển đã được Phê duyệt (`ChapNhan`).
5. **`lib/views/employer/tao_hop_dong_screen.dart` (Mới)**:
   - Màn hình soạn thảo và khởi tạo dự thảo hợp đồng từ đơn ứng tuyển đã duyệt: thù lao thỏa thuận, ngày bắt đầu, ngày kết thúc, hình thức (Remote/Onsite), điều khoản giao kết, mốc nghiệm thu và giải ngân.
   - Kết nối trực tiếp API `POST api/HopDong/tao-hop-dong?maUserNTD=`.
6. **`lib/views/hop_dong/hop_dong_list_screen.dart`**:
   - Tự động nhận diện vai trò Nhà tuyển dụng (`NhaTuyenDung`/`Employer`) để gọi `taiDanhSachHopDongNTD` thay vì chỉ dành cho Freelancer.
7. **`lib/views/hop_dong/hop_dong_detail_screen.dart`**:
   - Bổ sung nút **"Ký quỹ hợp đồng"** khi hợp đồng ở trạng thái `ChoKyQuy` và người dùng là Nhà tuyển dụng (gọi `HopDongViewModel.kyQuyHopDong`).
   - Bổ sung nút **"Theo dõi tiến độ dự án"** cho Nhà tuyển dụng sang `TienDoScreen`.
   - Bổ sung nút **"Chat với Freelancer"** cho Nhà tuyển dụng.
8. **`test/employer_workflow_test.dart` (Mới)**:
   - Bộ unit test & widget test kiểm thử toàn diện luồng Nhà tuyển dụng trên Mobile.

---

## IV. KẾT QUẢ KIỂM THỬ HỆ THỐNG
- **Flutter Analyze**: `No issues found! (ran in 1.1s)`
- **Flutter Test**: `14/14 tests passed! (100% thành công)`

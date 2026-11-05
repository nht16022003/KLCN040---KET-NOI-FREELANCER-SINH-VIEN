# BÁO CÁO KIỂM TRA ĐỐI SOÁT TOÀN DIỆN API – WEBSITE – MOBILE
## ĐỒNG BỘ 100% MOBILE FLUTTER THEO WEBSITE VÀ BACKEND API

> **Dự án:** Hệ thống kết nối việc làm Freelancer Sinh Viên (FreelancerStudent)  
> **Thời điểm đối soát:** Ngày 06/11/2026  
> **Chuẩn đối chiếu:** Website (`FreelancerStudent.Web`) & Backend API (`FreelancerStudent.API`)  
> **Đối tượng đối soát:** Ứng dụng di động Flutter (`Mobile/freelancerstudent`)  

---

## I. TỔNG QUAN HIỆN TRẠNG TOÀN HỆ THỐNG

| Thành phần | Công nghệ | Đường dẫn thư mục | Trạng thái hiện tại |
|---|---|---|---|
| **Backend API** | ASP.NET Core 10 Web API, EF Core, SQL Server | `FreelancerStudent.API` | Hoàn thiện, Build 0 Errors, đầy đủ nghiệp vụ Escrow, Bàn giao, Đánh giá, OCR |
| **Website** | ASP.NET Core 10 MVC, Razor Views, Bootstrap 5 | `FreelancerStudent.Web` | Hoàn thiện, Build 0 Errors, đồng bộ 100% luồng Tiến độ mốc và Bàn giao |
| **Mobile App** | Flutter 3.x, Dart, Provider Architecture | `Mobile/freelancerstudent` | Đã có khung 9 phân hệ; còn thiếu các nghiệp vụ mới cập nhật gần nhất |

---

## II. BẢNG KIỂM KÊ TOÀN DIỆN: WEBSITE VS API VS MOBILE (GIAI ĐOẠN 1, 2 & 3)

| STT | Phân hệ / Chức năng | Website (`Web`) | API Backend (`API`) | Mobile (`Flutter`) | Chi tiết đối chiếu & Thiếu sót trên Mobile | Hướng xử lý trên Mobile |
|:---:|---|:---:|:---:|:---:|---|---|
| **1** | **Đánh giá mốc tiến độ: NTD chọn Đạt** | Hoàn thành | Hoàn thành | Hoàn thành | Mobile đã có nút bấm xác nhận Đạt. | Giữ nguyên |
| **2** | **Đánh giá mốc tiến độ: NTD chọn Chưa đạt kèm lý do** | Hoàn thành | Hoàn thành | **Có lỗi** | Web bắt buộc nhập lý do vào popup. API kiểm tra `lyDo` bắt buộc. Mobile hiện tại hiển thị dialog **KHÔNG có ô nhập lý do**, gọi service không truyền `lyDo` $\rightarrow$ **API trả lỗi 400**. | Bổ sung ô nhập lý do bắt buộc vào Dialog; cập nhật Service & ViewModel truyền `lyDo`. |
| **3** | **Hiển thị lý do Chưa đạt của NTD cho Freelancer** | Hoàn thành | Hoàn thành | **Chưa có** | Web hiển thị khung cảnh báo màu đỏ với lý do NTD từ chối. Mobile model `GiaiDoanHopDongModel` **chưa có trường `lyDoChuaDat`**, UI không hiển thị lý do này. | Thêm trường `lyDoChuaDat` vào model; hiển thị Card cảnh báo lý do từ chối. |
| **4** | **Cập nhật lại mốc bàn giao / tiến độ khi bị Chưa đạt** | Hoàn thành | Hoàn thành | **Chưa có** | Web có nút "Cập nhật lại bàn giao", mở modal nhập link & mô tả mới, gọi `POST /api/TaskCongViec/hop-dong/{maHD}/cap-nhat-moc` để reset về `ChoDanhGia`. Mobile **chưa có nút này, chưa có Service/ViewModel gọi API này**. | Bổ sung hàm `capNhatMocBanGiao` vào Service/VM; thêm nút & BottomSheet nhập link/mô tả nộp lại. |
| **5** | **Thống kê 4 khối mốc tiến độ (Tổng, Chưa đạt, Chờ đánh giá, Đã đạt)** | Hoàn thành | Hoàn thành | **Chưa có** | Web hiển thị 4 Card thống kê mốc tiến độ. Mobile `TienDoDuAnModel` chưa parse `tongSoMoc`, `soMocDat`, `soMocChuaDat`, `soMocChoDanhGia`; UI chỉ hiển thị thống kê Task. | Cập nhật Model và giao diện `man_hinh_tien_do.dart` hiển thị 4 khối thống kê mốc tiến độ. |
| **6** | **Badge trạng thái mốc: Chờ NTD đánh giá (`ChoDanhGia`)** | Hoàn thành | Hoàn thành | **Một phần** | Web hiển thị badge xanh cyan "Chờ NTD đánh giá". Mobile gộp chung `ChoDanhGia` thành "Chưa hoàn thành" hoặc "Chưa đạt". | Thêm trạng thái và badge màu xanh info "Chờ NTD đánh giá" trên Mobile. |
| **7** | **Khóa nút Bàn giao sản phẩm cuối cùng khi mốc chưa Đạt** | Hoàn thành | Hoàn thành | **Sai nghiệp vụ** | Web kiểm tra `TatCaMocDaDat`, nếu chưa đạt 100% thì khóa bàn giao & hiện banner cảnh báo. Mobile `BanGiaoViewModel.coTheNop` **không kiểm tra mốc**, vẫn cho nộp $\rightarrow$ gọi API bị chặn 400. | Cập nhật `coTheNop` kiểm tra `tatCaMocDaDat`; hiển thị Banner cảnh báo màu vàng khi chưa đủ điều kiện. |
| **8** | **Nộp lại sản phẩm bàn giao cuối cùng khi bị Yêu cầu chỉnh sửa / Chưa đạt** | Hoàn thành | Hoàn thành | **Một phần** | Web cho nộp lại khi trạng thái là `YeuCauChinhSua` HOẶC `ChuaDat`. Mobile chỉ kiểm tra `YeuCauChinhSua`, bỏ sót `ChuaDat`. | Bổ sung điều kiện `trangThai == 'ChuaDat'` vào `coTheNop`. |
| **9** | **Ủy quyền tham số `maUser` trong API Bàn giao sản phẩm** | Hoàn thành | Hoàn thành | **Thiếu tham số** | Web truyền `?maUser={maUser}` khi gọi API `BanGiaoSanPham`. Mobile hiện không truyền query parameter này, dễ bị lỗi 401/403 nếu token thiếu Claim. | Thêm tham số `maUser` vào `dich_vu_ban_giao.dart` cho các request GET/POST/PUT. |
| **10** | **Đăng nhập & Đăng ký tài khoản (Sinh viên & NTD)** | Hoàn thành | Hoàn thành | Hoàn thành | Cả 2 nền tảng đều hỗ trợ đăng nhập JWT, phân quyền theo vai trò (`FreelancerStudent`, `NhaTuyenDung`). | Giữ nguyên |
| **11** | **Hồ sơ cá nhân & Chỉnh sửa thông tin cơ bản** | Hoàn thành | Hoàn thành | Hoàn thành | Cập nhật họ tên, SĐT, giới tính, ngày sinh, bio, avatar qua API. | Giữ nguyên |
| **12** | **Tải lên & Quét OCR minh chứng thẻ sinh viên** | Hoàn thành | Hoàn thành | Hoàn thành | Cả 2 nền tảng kết nối API `/MinhChung/quet-the-sinh-vien`, trích xuất MSSV, trường học, chuyên ngành. | Giữ nguyên |
| **13** | **Danh sách công việc & Tìm kiếm, Lọc theo kỹ năng / thù lao** | Hoàn thành | Hoàn thành | Hoàn thành | Đầy đủ bộ lọc từ khóa, hình thức làm việc, mức thù lao, danh mục. | Giữ nguyên |
| **14** | **Chi tiết công việc & Đơn ứng tuyển** | Hoàn thành | Hoàn thành | Hoàn thành | Xem mô tả, kỹ năng yêu cầu, thù lao, hạn nộp; Freelancer nộp đề xuất kèm thư ứng tuyển. | Giữ nguyên |
| **15** | **Quản lý tin tuyển dụng & Duyệt ứng viên (NTD)** | Hoàn thành | Hoàn thành | Hoàn thành | NTD xem danh sách đơn, chấp nhận hoặc từ chối ứng viên, khởi tạo dự thảo hợp đồng. | Giữ nguyên |
| **16** | **Bài đăng tìm việc của sinh viên (Student Post)** | Hoàn thành | Hoàn thành | Hoàn thành | Đăng tin giới thiệu năng lực, quản lý trạng thái ẩn/hiện, sửa/xóa bài đăng. | Giữ nguyên |
| **17** | **Khởi tạo và Duyệt hợp đồng (Ký kết / Từ chối)** | Hoàn thành | Hoàn thành | Hoàn thành | NTD tạo hợp đồng kèm mốc tiến độ; Freelancer xem điều khoản, bấm Xác nhận hoặc Từ chối kèm lý do. | Giữ nguyên |
| **18** | **Ký quỹ bảo chứng hợp đồng (Escrow Deposit)** | Hoàn thành | Hoàn thành | Hoàn thành | NTD ký quỹ từ số dư ví tiền; kiểm tra số dư khả dụng, đóng băng tiền bảo chứng, chuyển trạng thái sang `DangThucHien`. | Giữ nguyên |
| **19** | **Quản lý Task con / Đầu việc checklist trong mốc tiến độ** | Hoàn thành | Hoàn thành | Hoàn thành | Thêm, sửa, đổi trạng thái (Chưa làm, Đang làm, Hoàn thành), xóa task con theo từng mốc. | Giữ nguyên |
| **20** | **Ví tiền: Nạp tiền (Mô phỏng / SePay) & Rút tiền** | Hoàn thành | Hoàn thành | Hoàn thành | Xem số dư khả dụng/đóng băng, tạo yêu cầu nạp/rút tiền, lịch sử biến động số dư. | Giữ nguyên |
| **21** | **Nghiệm thu & Giải ngân thanh toán hợp đồng** | Hoàn thành | Hoàn thành | Hoàn thành | NTD nghiệm thu sản phẩm đạt, hệ thống giải ngân tiền ký quỹ về ví Freelancer (trừ phí sàn). | Giữ nguyên |
| **22** | **Đánh giá đối tác 2 chiều (Số sao & Nhận xét)** | Hoàn thành | Hoàn thành | Hoàn thành | Đánh giá chất lượng, đúng hạn, giao tiếp, nhận xét chung sau khi hợp đồng hoàn thành. | Giữ nguyên |
| **23** | **Tin nhắn Chat thời gian thực (SignalR)** | Hoàn thành | Hoàn thành | Hoàn thành | Nhắn tin trực tiếp giữa Freelancer và NTD theo ngữ cảnh công việc/hợp đồng. | Giữ nguyên |
| **24** | **Hệ thống thông báo đẩy (Notifications)** | Hoàn thành | Hoàn thành | Hoàn thành | Nhận thông báo khi có thay đổi trạng thái hợp đồng, duyệt mốc, nộp bàn giao, biến động ví. | Giữ nguyên |

---

## III. CHI TIẾT CÁC LỖI VÀ THIẾU SÓT CỤ THỂ TRÊN MOBILE CẦN ĐỒNG BỘ

### 1. Phân hệ Tiến độ & Đánh giá mốc (`man_hinh_tien_do.dart`, `man_hinh_chi_tiet_hop_dong.dart`)
* **Lỗi nghiêm trọng (Crash/Fail API 400):** Khi NTD bấm "Chưa đạt", Mobile chỉ hiển thị Dialog xác nhận Yes/No thông thường, **không có ô nhập lý do**. Khi gửi lên API endpoint `/danh-gia-giai-doan` mà thiếu `lyDo`, API lập tức trả lỗi 400 Bad Request.
* **Thiếu hiển thị lý do chưa đạt:** Freelancer trên Mobile không biết vì sao mốc của mình bị đánh giá Chưa đạt do model `GiaiDoanHopDongModel` chưa map trường `lyDoChuaDat` từ API.
* **Thiếu chức năng Nộp lại / Cập nhật lại mốc bàn giao:** Freelancer không có nút "Cập nhật lại bàn giao" trên Mobile khi mốc bị Chưa đạt, không có popup cập nhật link/mô tả và chưa có kết nối API `POST /api/TaskCongViec/hop-dong/{maHD}/cap-nhat-moc`.
* **Thiếu thống kê mốc tiến độ:** Màn hình tiến độ trên Mobile chỉ hiển thị thống kê theo task con, thiếu 4 khối thống kê mốc tiến độ tổng quan: *Tổng số mốc*, *Chưa đạt / Cần sửa*, *Chờ NTD đánh giá*, *Đã đạt tiêu chuẩn*.
* **Thiếu trạng thái `ChoDanhGia`:** Khi mốc vừa được Freelancer nộp lại, trạng thái là `ChoDanhGia` (Chờ NTD đánh giá), Mobile hiện đang hiển thị chưa chính xác.

### 2. Phân hệ Bàn giao sản phẩm cuối cùng (`man_hinh_ban_giao_san_pham.dart`, `viewmodel_ban_giao.dart`)
* **Sai lệch nghiệp vụ tiên quyết:** Web và API chặn hoàn toàn việc nộp sản phẩm hoàn thiện nếu còn mốc tiến độ chưa Đạt. Mobile hiện tại **cho phép Freelancer mở form và bấm gửi bàn giao ngay cả khi các mốc chưa hoàn thành**, dẫn đến khi bấm gửi thì bị API quăng lỗi `InvalidOperationException`.
* **Bỏ sót trường hợp nộp lại:** Khi sản phẩm bàn giao bị NTD từ chối với trạng thái `ChuaDat`, Mobile không cho nộp lại vì hàm `coTheNop` chỉ kiểm tra `YeuCauChinhSua`.
* **Thiếu tham số ủy quyền `maUser`:** Các hàm trong `dich_vu_ban_giao.dart` chưa truyền query parameter `?maUser={maUser}` để Backend xác thực phân quyền chuẩn xác.

---

## IV. DANH SÁCH FILE MOBILE CẦN CẬP NHẬT & PHƯƠNG ÁN TRIỂN KHAI

### 1. Danh sách file Mobile dự kiến chỉnh sửa

| STT | File Mobile | Nội dung chỉnh sửa |
|:---:|---|---|
| 1 | `lib/models/mo_hinh_hop_dong.dart` | Bổ sung trường `lyDoChuaDat` vào `GiaiDoanHopDongModel`; bổ sung `tatCaMocDaDat` vào `HopDongModel`. |
| 2 | `lib/models/mo_hinh_task_cong_viec.dart` | Bổ sung `tongSoMoc`, `soMocDat`, `soMocChuaDat`, `soMocChoDanhGia` vào `TienDoDuAnModel`. |
| 3 | `lib/services/dich_vu_task_cong_viec.dart` | Thêm tham số `String? lyDo` vào `danhGiaGiaiDoan`; Thêm hàm `capNhatMocBanGiao(maHD, maGiaiDoan, linkSanPham, moTa, maUser)`. |
| 4 | `lib/viewmodels/viewmodel_task_cong_viec.dart` | Thêm tham số `lyDo` vào hàm `danhGiaGiaiDoan`; Thêm hàm `capNhatMocBanGiao(...)`. |
| 5 | `lib/services/dich_vu_ban_giao.dart` | Thêm query parameter `maUser` vào `layDanhSachBanGiao`, `layDanhSachYeuCau`, `taoBanGiao`, `guiYeuCauChinhSua`, `nghiemThu`. |
| 6 | `lib/viewmodels/viewmodel_ban_giao.dart` | Cập nhật hàm `coTheNop` kiểm tra điều kiện `tatCaMocDaDat` và hỗ trợ cả trạng thái `ChuaDat`. |
| 7 | `lib/views/tien_do/man_hinh_tien_do.dart` | Bổ sung 4 thẻ thống kê mốc tiến độ; hộp thoại nhập lý do bắt buộc khi NTD đánh giá Chưa đạt; Card hiển thị lý do chưa đạt & nút "Cập nhật lại tiến độ/bàn giao" cho Freelancer kèm BottomSheet nhập link/mô tả mới; badge trạng thái `ChoDanhGia`. |
| 8 | `lib/views/hop_dong/man_hinh_chi_tiet_hop_dong.dart` | Bổ sung hộp thoại nhập lý do khi NTD đánh giá Chưa đạt mốc; hiển thị lý do Chưa đạt & nút "Cập nhật lại bàn giao" cho Freelancer tại danh sách mốc; hiển thị Banner cảnh báo điều kiện bàn giao sản phẩm cuối cùng. |
| 9 | `lib/views/hop_dong/man_hinh_ban_giao_san_pham.dart` | Hiển thị Banner cảnh báo màu vàng khi chưa đủ điều kiện bàn giao (còn mốc chưa Đạt); hiển thị lý do từ chối nếu có. |

### 2. Phương án triển khai chi tiết từng bước

* **Bước 1: Cập nhật Model & Service (Tầng Dữ liệu)**
  * Cập nhật `GiaiDoanHopDongModel` (thêm `lyDoChuaDat`).
  * Cập nhật `HopDongModel` (thêm getter `tatCaMocDaDat`).
  * Cập nhật `TienDoDuAnModel` (parse đầy đủ 4 số liệu thống kê mốc từ backend).
  * Viết thêm hàm `capNhatMocBanGiao` và bổ sung `lyDo` cho `danhGiaGiaiDoan` trong `dich_vu_task_cong_viec.dart` & `viewmodel_task_cong_viec.dart`.
  * Bổ sung `maUser` vào `dich_vu_ban_giao.dart`.
* **Bước 2: Cập nhật Màn hình Tiến độ dự án (`man_hinh_tien_do.dart`)**
  * Hiển thị 4 thẻ thống kê mốc tiến độ trên đầu màn hình.
  * Khi NTD bấm "Chưa đạt", hiển thị Dialog với ô nhập lý do (TextFormField required).
  * Khi mốc bị "Chưa đạt", hiển thị khung viền đỏ nổi bật với lý do của NTD.
  * Với Freelancer sở hữu hợp đồng, hiển thị nút màu cam/vàng **"Cập nhật lại tiến độ/bàn giao"** $\rightarrow$ mở Modal Bottom Sheet để nhập lại link sản phẩm và mô tả $\rightarrow$ gọi API cập nhật lại mốc.
  * Hiển thị badge trạng thái màu xanh info **"Chờ NTD đánh giá"** khi mốc đang ở trạng thái `ChoDanhGia`.
* **Bước 3: Cập nhật Màn hình Chi tiết hợp đồng (`man_hinh_chi_tiet_hop_dong.dart`)**
  * Đồng bộ hiển thị lý do Chưa đạt và nút "Cập nhật lại bàn giao" tại danh sách mốc tiến độ.
  * Tại khu vực "Bàn giao & Nghiệm thu sản phẩm", nếu chưa đủ điều kiện (`!tatCaMocDaDat`), hiển thị Banner cảnh báo giải thích rõ ràng giống Website.
* **Bước 4: Cập nhật Màn hình Bàn giao sản phẩm (`man_hinh_ban_giao_san_pham.dart`)**
  * Khóa form nộp nếu hợp đồng chưa đạt 100% mốc tiến độ.
  * Mở form nộp lại nếu phiên bản trước bị `YeuCauChinhSua` hoặc `ChuaDat`.
* **Bước 5: Kiểm thử tĩnh & build Flutter**
  * Chạy `flutter analyze` để đảm bảo 0 lỗi cú pháp, 0 lỗi kiểu dữ liệu.

---

## V. CAM KẾT & CHỜ XÁC NHẬN (THEO NGUYÊN TẮC BẮT BUỘC)

> [!IMPORTANT]
> **Cam kết:** Tôi **CHƯA** thực hiện bất kỳ thao tác chỉnh sửa, xóa hoặc thay đổi mã nguồn nào trên Mobile Flutter. Toàn bộ nội dung trên là kết quả rà soát, đối chiếu thực tế mã nguồn giữa Website, API và Mobile.
> 
> Xin vui lòng xem qua báo cáo đối soát và kế hoạch triển khai trên. Khi bạn xác nhận đồng ý, tôi sẽ bắt đầu tiến hành triển khai chính xác theo từng bước để đồng bộ 100% Mobile theo Website.


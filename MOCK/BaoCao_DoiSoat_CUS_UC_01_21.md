# BÁO CÁO ĐỐI SOÁT CẤU TRÚC DỰ ÁN & CƠ SỞ DỮ LIỆU
## Khớp nối theo Đặc tả Kỹ thuật Hệ thống (DT_Edit.docx: CUS-UC-01 đến CUS-UC-21)

---

## 📌 1. TỔNG QUAN HỆ THỐNG ĐỐI SOÁT

- **Tài liệu nguồn đặc tả**: `DT_Edit.docx` (Trích xuất từ mục **CUS-UC-01** đến **CUS-UC-21**).
- **Dự án thực tế**: `MOCK.sln` (Nền tảng ASP.NET Core MVC - Freelancer Students).
- **Mục tiêu đối soát**:
  1. Đánh giá mức độ hoàn thiện của các **Luồng xử lý (Controllers, Views, Routing)** so với 21 Use Case đặc tả.
  2. Đánh giá tính đầy đủ và mức độ tương thích của **Cơ sở dữ liệu (Entities, Models, MockDataStore)** so với yêu cầu nghiệp vụ.

---

## 📊 2. MA TRẬN ĐỐI SOÁT 21 USE CASE (CUS-UC-01 ĐẾN CUS-UC-21)

| STT | Mã Use Case | Tên Use Case | Tác nhân chính | Hiện trạng trong Project | Mức độ đáp ứng |
| :---: | :--- | :--- | :--- | :--- | :---: |
| **01** | **CUS-UC-01.01** | **Đăng nhập** | Khách hàng | [AccountController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/AccountController.cs) (`Login`), [Login.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Account/Login.cshtml) | 🟢 **100% (Hoàn thành)** |
| **02** | **CUS-UC-01.02** | **Đăng xuất** | Khách hàng | [AccountController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/AccountController.cs) (`Logout`), xóa Session | 🟢 **100% (Hoàn thành)** |
| **03** | **CUS-UC-01.03** | **Quản lý thông tin cá nhân** *(Xem, Sửa, Đổi MK)* | Khách hàng | [AccountController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/AccountController.cs) (`Profile`, `UpdateProfile`, `ChangePassword`), [Profile.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Account/Profile.cshtml) | 🟢 **100% (Hoàn thành)** |
| **04** | **CUS-UC-02.01** | **Tìm kiếm Freelancer** | Khách hàng | [FreelancerController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/FreelancerController.cs) (`Search`), [Search.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Freelancer/Search.cshtml) | 🟢 **100% (Hoàn thành)** |
| **05** | **CUS-UC-03.01** | **Xem hồ sơ Freelancer** | Khách hàng | [FreelancerController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/FreelancerController.cs) (`Profile`), [Profile.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Freelancer/Profile.cshtml) | 🟢 **100% (Hoàn thành)** |
| **06** | **CUS-UC-03.02** | **Xem chi tiết dự án trong Portfolio** | Khách hàng | Tích hợp trong [Profile.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Freelancer/Profile.cshtml) & [Portfolio.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Freelancer/Portfolio.cshtml) | 🟡 **80% (Khớp cơ bản)** |
| **07** | **CUS-UC-04.01** | **Quản lý Freelancer quan tâm** *(Lưu / Bỏ lưu)* | Khách hàng (Client) | Chưa có Controller/View và chưa có bảng lưu trữ danh sách yêu thích | 🔴 **0% (Chưa có)** |
| **08** | **CUS-UC-06.01** | **Tạo và đăng bài tuyển dụng** | Khách hàng (Client) | [JobPostController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/JobPostController.cs) (`Create`), [Create.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/JobPost/Create.cshtml) | 🟡 **75% (Khớp một phần)** |
| **09** | **CUS-UC-06.02** | **Quản lý bài đăng tuyển dụng** | Khách hàng (Client) | [EmployerController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/EmployerController.cs) (`ManageJobs`, `Applicants`), [ManageJobs.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/Employer/ManageJobs.cshtml) | 🟢 **100% (Hoàn thành)** |
| **10** | **CUS-UC-07.01** | **Xem thông tin tài chính** | Khách hàng (Client) | Đã có Entity `Wallet` và `WalletViewModel`, chưa có Dashboard tài chính hoàn chỉnh | 🟡 **60% (Khớp một phần)** |
| **11** | **CUS-UC-07.02** | **Xem lịch sử giao dịch** | Khách hàng (Client) | Chưa có bảng lịch sử biến động số dư và View tra cứu lịch sử giao dịch | 🔴 **0% (Chưa có)** |
| **12** | **CUS-UC-08.01** | **Nạp tiền vào ví** *(Mã QR động / STK)* | Khách hàng (Client) | Chưa có màn hình tạo yêu cầu nạp tiền và hiển thị QR đối soát nạp | 🔴 **0% (Chưa có)** |
| **13** | **CUS-UC-08.02** | **Gửi yêu cầu hỗ trợ nạp tiền** | Khách hàng (Client) | Chưa có form khiếu nại nạp tiền đính kèm biên lai chuyển khoản | 🔴 **0% (Chưa có)** |
| **14** | **CUS-UC-09.01** | **Thanh toán cho Freelancer** *(Giải ngân Escrow)* | Khách hàng (Client) | [HopDongController.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Controllers/HopDongController.cs) (đã có hợp đồng, ký quỹ, chưa có nút bấm giải ngân ví) | 🟡 **70% (Khớp một phần)** |
| **15** | **CUS-UC-10.01** | **Theo dõi công việc đã thuê** *(Tiến độ & Task)* | Khách hàng (Client) | Có [Details.cshtml](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Views/HopDong/Details.cshtml), nhưng chưa có bảng Task/Milestones chi tiết | 🟡 **65% (Khớp một phần)** |
| **16** | **CUS-UC-10.02** | **Nghiệm thu và phản hồi sản phẩm** | Khách hàng (Client) | Chưa có màn hình nhận file sản phẩm bàn giao, xem version và yêu cầu sửa | 🔴 **0% (Chưa có)** |
| **17** | **CUS-UC-10.03** | **Xác nhận hoàn thành công việc** | Khách hàng (Client) | Có trạng thái `HopDong.TrangThai = "HoanThanh"`, cần liên kết sau nghiệm thu | 🟡 **70% (Khớp một phần)** |
| **18** | **CUS-UC-11.01** | **Tạo yêu cầu hỗ trợ tranh chấp** | Khách hàng (Client) | Chưa có form gửi khiếu nại đính kèm file bằng chứng (<= 10MB) | 🔴 **0% (Chưa có)** |
| **19** | **CUS-UC-11.02** | **Quản lý và theo dõi tranh chấp** | Khách hàng (Client) | Chưa có màn hình xem tiến trình xử lý tranh chấp của Admin | 🔴 **0% (Chưa có)** |
| **20** | **CUS-UC-12.01** | **Quản lý thông báo** *(Xem & Đánh dấu đã đọc)* | Khách hàng (Client) | Icon chuông trên Navbar đang ở dạng tĩnh, chưa có dropdown/trang thông báo | 🟡 **30% (Chỉ có UI tĩnh)** |
| **21** | **CUS-UC-12.02** | **Cấu hình thông báo** | Khách hàng (Client) | Chưa có trang cài đặt bật/tắt nhận thông báo qua Web / Email | 🔴 **0% (Chưa có)** |

---

## 🗄️ 3. ĐỐI SOÁT CƠ SỞ DỮ LIỆU (DATABASE / ENTITIES)

### 3.1. Các bảng ĐÃ CÓ trong Project (Khớp với Đặc tả)

1. **`User`** ([User.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/User.cs)): Lưu trữ thông tin tài khoản người dùng (`MaUser`, `HotenUser`, `TenTaiKhoanUser`, `PashwordHash`, `EmailUser`, `SdtUser`, `Status`, `NgayTao`, `MaRole`).
2. **`Role`** ([Role.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/Role.cs)): Phân quyền tài khoản (`1: FreelancerStudent`, `2: NhaTuyenDung`, `3: Admin`).
3. **`FreelancerStudent`** ([FreelancerStudent.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/FreelancerStudent.cs)): Hồ sơ sinh viên (`MaFreelancerStudents`, `MaUser`, `TenTruong`, `MaChuyenNganh`, `GPA`, `NamThu`, `NienKhoa`, `Avatar`, `TrangthaiNhanViec`, `ChiPhiTu`).
4. **`NhaTuyenDung`** ([NhaTuyenDung.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/NhaTuyenDung.cs)): Hồ sơ doanh nghiệp/nhà tuyển dụng (`MaNhaTuyenDung`, `MaUser`, `Tencongty`, `Linhvuc`, `Diachi`, `Logo`, `Sosaodanhgia`).
5. **`ChuyenNganh`** ([ChuyenNganh.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/ChuyenNganh.cs)) & **`KynangChuyennganh_FreelancerStudent`** ([KynangChuyennganhFreelancerStudent.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/KynangChuyennganhFreelancerStudent.cs)).
6. **`MinhChungFreelancerStudent`** ([MinhChungFreelancerStudent.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/MinhChungFreelancerStudent.cs)): Hồ sơ minh chứng xác thực sinh viên (`LoaiMinhChung`, `FileMinhChung`, `TrangThaiGuiMinhChung`, `NgayXacMinh`).
7. **`Portfolio`** ([Portfolio.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/Portfolio.cs)) & **`DuAnTrongPortfolio`** ([DuAnTrongPortfolio.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/DuAnTrongPortfolio.cs)): Các dự án tiêu biểu (`TenDuAn`, `MoTa`, `VaiTro`, `Congnghe`, `LinkGithub`, `LinkDemo`, `Link_file`).
8. **`JobPost`** ([JobPost.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/JobPost.cs)): Tin đăng tuyển dụng (`MaJob`, `Tieude`, `Mota`, `Kynangyeucau`, `Thulao`, `Thoigiandangtuyen`, `Thoigiandukienhoanthanh`, `Status`).
9. **`UngTuyen`** ([UngTuyen.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/UngTuyen.cs)): Đơn ứng tuyển của sinh viên (`ThuGioiThieu`, `ThulaoDeXuat`, `ThoiGianHoanThanhDeXuat`, `FileCV`, `TrangThaiUngTuyen`).
10. **`YeuCauThueFreelancer`** ([YeuCauThueFreelancer.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/YeuCauThueFreelancer.cs)): Lời mời nhận việc trực tiếp từ nhà tuyển dụng.
11. **`HopDong`** ([HopDong.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/HopDong.cs)): Hợp đồng Escrow bảo đảm (`MaHD`, `MaJob`, `MaFreelancerStudent`, `Sotienkyquy`, `Hinhthuclamviec`, `TrangThai`).
12. **`Wallet`** ([Wallet.cs](file:///d:/KLTN/CHUA_CO_FINAL/MOCK/Models/Entities/Wallet.cs)): Ví tiền (`MaWallet`, `MaUser`, `SoDuKhaDung`, `SoDuDongBang`).

---

### 3.2. Các bảng CÒN THIẾU (Cần bổ sung vào Entity & MockDataStore)

| STT | Bảng CSDL cần thêm | Phục vụ Use Case | Cấu trúc thuộc tính đề xuất |
| :---: | :--- | :--- | :--- |
| **1** | **`GiaoDich`** *(Transaction)* | `CUS-UC-11`, `CUS-UC-12`, `CUS-UC-14` | `MaGD` (PK), `MaWallet` (FK), `LoaiGiaoDich` (NapTien, KyQuy, GiaiNgan, HoanTien), `SoTien`, `TrangThai` (ThanhCong, ChoXuLy, ThatBai), `NgayTao`, `MaThamChieu`, `NoiDung`. |
| **2** | **`FreelancerYeuThich`** *(Bookmark)* | `CUS-UC-07` | `MaBookmark` (PK), `MaUser` (FK - Client), `MaFreelancerStudent` (FK), `NgayLuu`. |
| **3** | **`TaskCongViec`** *(Milestone / Task)* | `CUS-UC-15` | `MaTask` (PK), `MaHD` (FK), `TenTask`, `MoTa`, `HanHoanThanh`, `TrangThai` (ChuaThucHien, DangThucHien, HoanThanh). |
| **4** | **`BanGiaoSanPham`** *(Delivery Version)* | `CUS-UC-16`, `CUS-UC-17` | `MaBanGiao` (PK), `MaHD` (FK), `PhienBan` (v1.0, v2.0...), `FileSanPham`, `GhiChuBanGiao`, `NgayNop`, `TrangThai` (ChoDuyet, YeuCauSua, DaNghiemThu), `PhanHoiClient`. |
| **5** | **`TranhChap`** & **`BangChungTranhChap`** | `CUS-UC-18`, `CUS-UC-19` | `MaTranhChap` (PK), `MaHD` (FK), `NguoiTao` (FK), `LyDo`, `MoTaChiTiet`, `FileBangChung`, `TrangThai` (DangXacMinh, DaGiaiQuyet, DaDong), `KetLuanAdmin`. |
| **6** | **`ThongBao`** & **`CauHinhThongBao`** | `CUS-UC-20`, `CUS-UC-21` | `MaThongBao` (PK), `MaUser` (FK), `TieuDe`, `NoiDung`, `LoaiThongBao` (ViecLam, ViTien, TinNhan, TranhChap), `LinkDieuHuong`, `DaDoc` (bool), `NgayTao`. |

---

## 🎯 4. CHI TIẾT TỪNG NHÓM USE CASE & GIẢI PHÁP TRIỂN KHAI TIẾP THEO

### Nhóm A: Nghiệp vụ Xác thực & Hồ sơ (CUS-UC-01 → CUS-UC-05)
* **Hiện trạng**: Đã hoàn thành 100%. Luồng Đăng nhập, Đăng xuất, Tìm kiếm Freelancer, Xem hồ sơ và CUS-UC-03 (Quản lý thông tin cá nhân & Đổi mật khẩu) hoạt động mượt mà, đầy đủ các ngoại lệ (EXC) và nhánh rẽ (ALT).

### Nhóm B: Tuyển dụng & Đăng tin (CUS-UC-06 → CUS-UC-09)
* **Đã có**: Đăng tin tuyển dụng `JobPost/Create` và Quản lý tin / Ứng viên `Employer/ManageJobs`.
* **Cần bổ sung**: 
  - Thêm chức năng **Lưu Freelancer quan tâm (CUS-UC-07)**: Nút "Thêm vào danh sách yêu thích" tại trang Profile Freelancer và trang danh sách đã lưu `/Employer/Favorites`.
  - Bổ sung **Brief Template (Mẫu yêu cầu tuyển dụng)** khi đăng tin.

### Nhóm C: Ví tiền, Nạp tiền & Thanh toán Escrow (CUS-UC-10 → CUS-UC-14)
* **Đã có**: Entity `Wallet`, số dư ví và hiển thị số tiền ký quỹ hợp đồng.
* **Cần bổ sung**:
  - Trang **Ví tiền & Lịch sử giao dịch** (`/Wallet/Index` hoặc `/NhaTuyenDung/ThanhToan`): Hiển thị số dư khả dụng, số dư đóng băng, danh sách biến động số dư (`CUS-UC-10, CUS-UC-11`).
  - Màn hình **Nạp tiền vào ví qua VietQR động** (`CUS-UC-12`) và form **Hỗ trợ nạp tiền** đính kèm hóa đơn (`CUS-UC-13`).
  - Nút **Giải ngân Escrow cho Freelancer** tại trang chi tiết hợp đồng sau khi nghiệm thu (`CUS-UC-14`).

### Nhóm D: Quản lý tiến độ, Nghiệm thu & Tranh chấp (CUS-UC-15 → CUS-UC-19)
* **Đã có**: Trang chi tiết hợp đồng `HopDong/Details`.
* **Cần bổ sung**:
  - Bảng **Quản lý Task / Tiến độ công việc** theo từng Milestone (`CUS-UC-15`).
  - Khung **Bàn giao sản phẩm & Lịch sử phiên bản (Version 1.0, 2.0...)**: Client có thể tải file, bấm "Yêu cầu chỉnh sửa" hoặc "Nghiệm thu & Hoàn thành" (`CUS-UC-16, CUS-UC-17`).
  - Chức năng **Gửi khiếu nại tranh chấp** đến Admin kèm upload file minh chứng (`CUS-UC-18, CUS-UC-19`).

### Nhóm E: Quản lý & Cấu hình thông báo (CUS-UC-20, CUS-UC-21)
* **Cần bổ sung**:
  - Tích hợp Dropdown chuông thông báo hiển thị danh sách thông báo thời gian thực (`CUS-UC-20`).
  - Màn hình Cài đặt cấu hình nhận thông báo qua Web / Email (`CUS-UC-21`).

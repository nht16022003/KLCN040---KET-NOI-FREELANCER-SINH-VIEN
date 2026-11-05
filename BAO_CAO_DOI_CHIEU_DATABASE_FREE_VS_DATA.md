# BÁO CÁO TOÀN DIỆN ĐỐI CHIẾU DATABASE FREE VÀ DATA
*(Dự án FreelancerStudent: Backend API, Web MVC, Mobile Flutter và Cơ sở dữ liệu)*

---

## 1. TỔNG QUAN HAI DATABASE

| Tiêu chí | Database FREE (`KLCN040_FREELANCERSTUDENT.sql` / `KETNOI_FREELANCERSV`) | Database DATA (`KLCN040_DATA.sql` / `TESTDATA`) |
| :--- | :--- | :--- |
| **Tên Database khai báo trong SQL** | `KETNOI_FREELANCERSV` | `TESTDATA` |
| **Kích thước file SQL** | 49,851 bytes (1,262 dòng) | 50,186 bytes (1,250 dòng) |
| **Số bảng khai báo trong SQL** | 40 bảng | 40 bảng |
| **Số bảng thực tế trong Database Live** | **41 bảng** *(Bao gồm bảng `DanhGia_NhanXet_LichSu`)* | **40 bảng** *(Thiếu bảng `DanhGia_NhanXet_LichSu`)* |
| **Thứ tự tạo bảng (DDL Order)** | **Gặp lỗi phụ thuộc:** `LuuViec_ChoFreelancer` tạo trước `JobPost` $\rightarrow$ sinh lỗi FK nếu chạy tuần tự. | **Đã sửa:** Chuyển `JobPost` lên trước `LuuViec_ChoFreelancer`. |
| **Lệnh INSERT ban đầu trong SQL** | **Lỗi:** Cố gắng INSERT cột `avatar` vào `FreelancerStudents` và `NhaTuyenDung` dù cột này đã bị `DROP COLUMN`. | **Đã sửa:** Đã loại bỏ cột `avatar` trong câu lệnh `INSERT`. |
| **Cột schema được bổ sung sau này** | **Đầy đủ:** Đã chạy 2 Migration (thêm đánh giá đa tiêu chí, điểm TB, số lượt, dự án nổi bật, điều khoản HĐ, v.v.). | **Thiếu hoàn toàn:** Chỉ là schema khởi tạo ban đầu, chưa hề áp dụng các migration bổ sung. |
| **Cơ chế Hash mật khẩu trong Dữ liệu** | **Chuẩn BCrypt:** Mật khẩu người dùng được băm dạng `$2a$11$...` tương thích hoàn toàn với thư viện `BCrypt.Net-Next`. | **Lỗi chuỗi thô (Fake String):** Mật khẩu là `'hash_pass_123'`, `'hash_pass_admin'` $\rightarrow$ Không thể đăng nhập. |
| **Trạng thái vận hành thực tế hệ thống** | **Hoạt động ổn định 100%:** Cả Backend API, Website MVC và Mobile Flutter đều giao tiếp trơn tru. | **Lỗi nghiêm trọng:** API sập/báo lỗi 400 & 500 ngay khi Đăng nhập, Đăng ký, Xem hồ sơ, Đánh giá hoặc Xem dự án. |

---

## 2. BẢNG THỐNG KÊ SỰ KHÁC BIỆT VỀ CẤU TRÚC GIỮA HAI DATABASE

### 2.1. Đối chiếu tổng thể đối tượng CSDL

| Loại đối tượng | Database FREE (`KETNOI_FREELANCERSV`) | Database DATA (`TESTDATA`) | Ghi chú khác biệt |
| :--- | :---: | :---: | :--- |
| **Bảng (Base Tables)** | 41 | 40 | `TESTDATA` thiếu bảng `DanhGia_NhanXet_LichSu`. |
| **Tổng số cột (Columns)** | 277 | 262 | `TESTDATA` thiếu 15 cột nghiệp vụ quan trọng. |
| **Khóa ngoại (Foreign Keys)** | 64 | 62 | `TESTDATA` thiếu FK trên bảng lịch sử đánh giá và FK kỹ năng. |
| **Ràng buộc mặc định (Default Constraints)** | 35 | 28 | `TESTDATA` thiếu 7 ràng buộc mặc định (điểm TB = 0, số lượt = 0, v.v.). |
| **Stored Procedures** | 0 | 0 | Dự án sử dụng hoàn toàn EF Core ORM (LINQ), không dùng Stored Procedure. |
| **Views / Triggers / Functions** | 0 | 0 | Dự án xử lý toàn bộ logic nghiệp vụ tại tầng Service C#. |

### 2.2. Điểm khác biệt trong hai file SQL mã nguồn (`KLCN040_FREELANCERSTUDENT.sql` vs `KLCN040_DATA.sql`)

1. **Khối khai báo Database:**
   - File `FREE`: `CREATE DATABASE KETNOI_FREELANCERSV; USE KETNOI_FREELANCERSV;`
   - File `DATA`: `CREATE DATABASE TESTDATA; USE TESTDATA;`
2. **Thứ tự định nghĩa bảng:**
   - Trong `KLCN040_FREELANCERSTUDENT.sql` (dòng 192), bảng `LuuViec_ChoFreelancer` được tạo trước bảng `JobPost`. Bảng này có khóa ngoại:
     ```sql
     CONSTRAINT FK_LuuViec_JobPost FOREIGN KEY (maJob) REFERENCES JobPost(maJob)
     ```
     Do đó, nếu chạy file SQL này trên một Database mới, SQL Server sẽ báo lỗi: `Foreign key 'FK_LuuViec_JobPost' references invalid table 'JobPost'`.
   - Trong `KLCN040_DATA.sql` (dòng 201), tác giả đã đảo thứ tự, đưa bảng `JobPost` lên trước bảng `LuuViec_ChoFreelancer` để tránh lỗi khóa ngoại này.
3. **Lỗi INSERT cột đã bị xóa (`avatar`):**
   - Cả hai file đều có lệnh:
     ```sql
     ALTER TABLE FreelancerStudents DROP COLUMN avatar;
     ALTER TABLE NhaTuyenDung DROP COLUMN avatar;
     ```
   - Tuy nhiên, trong `KLCN040_FREELANCERSTUDENT.sql` (dòng 740 & 758), câu lệnh INSERT dữ liệu mẫu vẫn cố tình gán giá trị cho cột `avatar`:
     ```sql
     INSERT INTO FreelancerStudents (..., avatar, ...) VALUES (..., 'uploads/avatar/user1.jpg', ...)
     INSERT INTO NhaTuyenDung (maUser, avatar, ...) VALUES (4, 'uploads/avatar/employer1.jpg', ...)
     ```
     Dẫn đến lỗi thực thi SQL: `Invalid column name 'avatar'`.
   - Trong `KLCN040_DATA.sql` (dòng 703 & 721), lỗi này đã được sửa bằng cách loại bỏ cột `avatar` ra khỏi danh sách cột của lệnh INSERT.
4. **Các câu lệnh rác / truy vấn tạm:**
   - `KLCN040_FREELANCERSTUDENT.sql` chứa nhiều câu lệnh truy vấn nháp trong quá trình phát triển như `SELECT * FROM BaiDangTimViec_FreelancerStudent;`, `SELECT * FROM users;`, `DELETE FROM Users Where maUser = 10;`, `SELECT u.maUser, u.hotenUser...`.
   - `KLCN040_DATA.sql` đã dọn dẹp toàn bộ các câu lệnh nháp này.
5. **Khối lệnh DROP TABLE cuối file:**
   - `KLCN040_FREELANCERSTUDENT.sql` sử dụng các lệnh `DROP TABLE ...;` không có kiểm tra tồn tại.
   - `KLCN040_DATA.sql` được viết lại bài bản thành 16 nhóm `DROP TABLE IF EXISTS ...;` sắp xếp chuẩn theo thứ tự phụ thuộc khóa ngoại ngược từ dưới lên trên.

---

## 3. BẢNG THỐNG KÊ CÁC BẢNG VÀ CỘT BỊ THIẾU HOẶC KHÔNG TƯƠNG THÍCH

Dưới đây là bảng đối chiếu chi tiết các bảng và cột **thực tế đang tồn tại trong Database FREE (`KETNOI_FREELANCERSV`) và được C# Backend API (`ApplicationDBContext`) ánh xạ trực tiếp**, nhưng **HOÀN TOÀN BỊ THIẾU trong Database DATA (`TESTDATA`)**:

| STT | Bảng ảnh hưởng | Cột bị thiếu / Bảng bị thiếu | Kiểu dữ liệu | Nullable | Ràng buộc mặc định | Nguồn gốc nghiệp vụ (Migration) |
| :---: | :--- | :--- | :---: | :---: | :---: | :--- |
| **1** | **`DanhGia_NhanXet_LichSu`** | *(Toàn bộ bảng bị thiếu)* | Table | - | PK: `maLichSu` (Identity) | `20261022_HoanThienThanhToanDanhGia.sql` |
| **2** | `FreelancerStudents` | `diemDanhGiaTrungBinh` | `DECIMAL(3,2)` | `NOT NULL` | `DEFAULT (0)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **3** | `FreelancerStudents` | `soLuotDanhGia` | `INT` | `NOT NULL` | `DEFAULT (0)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **4** | `NhaTuyenDung` | `soLuotDanhGia` | `INT` | `NOT NULL` | `DEFAULT (0)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **5** | `DuAn_Trong_Portfolio` | `laDuAnNoiBat` | `BIT` | `NOT NULL` | `DEFAULT (0)` | `20261018_AddPortfolioProjectFeatured.sql` |
| **6** | `DanhGia_NhanXet` | `soSaoChatLuong` | `TINYINT` | `NULL` | `CHECK (1-5)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **7** | `DanhGia_NhanXet` | `soSaoDungHan` | `TINYINT` | `NULL` | `CHECK (1-5)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **8** | `DanhGia_NhanXet` | `soSaoGiaoTiep` | `TINYINT` | `NULL` | `CHECK (1-5)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **9** | `DanhGia_NhanXet` | `ngayCapNhat` | `DATETIME` | `NULL` | - | `20261022_HoanThienThanhToanDanhGia.sql` |
| **10** | `DanhGia_NhanXet` | `soLanChinhSua` | `INT` | `NOT NULL` | `DEFAULT (0)` | `20261022_HoanThienThanhToanDanhGia.sql` |
| **11** | `HopDong` | `ngayHoanThanh` | `DATETIME` | `NULL` | - | `20261022_HoanThienThanhToanDanhGia.sql` |
| **12** | `HopDong` | `ngayDongHopDong` | `DATETIME` | `NULL` | - | `20261022_HoanThienThanhToanDanhGia.sql` |
| **13** | `HopDong` | `thanhToanTuDong` | `BIT` | `NOT NULL` | `DEFAULT (0)` | `20261022_HoanThienThanhToanDanhGia.sql` |

> [!CAUTION]
> Trong Entity Framework Core, nếu một Entity C# khai báo thuộc tính tương ứng với một cột của Database mà cột đó không tồn tại trong CSDL thực tế, EF Core sẽ tạo câu lệnh `SELECT [t].[TenCot] ...` và SQL Server sẽ lập tức ném ra lỗi ngoại lệ nghiêm trọng: **`SqlException: Invalid column name 'TenCot'`**, làm ngắt quãng toàn bộ luồng xử lý và trả về HTTP 400/500 cho giao diện Web/Mobile.

---

## 4. PHÂN TÍCH CÁC QUAN HỆ KHÓA NGOẠI

1. **Khóa ngoại trên bảng bị thiếu (`DanhGia_NhanXet_LichSu`):**
   - Trong Database FREE: Có khóa ngoại `FK_DanhGia_NhanXet_LichSu_DanhGia` nối từ `DanhGia_NhanXet_LichSu(maDanhGia)` tới `DanhGia_NhanXet(maDanhGia)` với quy tắc `ON DELETE CASCADE`.
   - Trong Database DATA: Hoàn toàn không có vì bảng không tồn tại.
2. **Khóa ngoại `FK_FreelancerStudents_KyNang`:**
   - Trong Database FREE: Tồn tại khóa ngoại giữa `FreelancerStudents(maKyNang)` và `KyNang(maKyNang)`.
   - Trong Database DATA: Không có (quan hệ kỹ năng sinh viên được chuẩn hóa thông qua bảng trung gian `FreelancerStudent_KyNang`).
3. **Thứ tự tạo khóa ngoại giữa `LuuViec_ChoFreelancer` và `JobPost`:**
   - Bảng `LuuViec_ChoFreelancer` bắt buộc phải được tạo sau `JobPost` vì có khóa ngoại `FK_LuuViec_JobPost` tham chiếu đến `JobPost(maJob)`. File `KLCN040_DATA.sql` đã khắc phục được lỗi thứ tự này so với `KLCN040_FREELANCERSTUDENT.sql`.

---

## 5. PHÂN TÍCH DỮ LIỆU GIỮA HAI DATABASE

### 5.1. Bảng đối chiếu số lượng bản ghi thực tế (Live Row Counts)

| Bảng dữ liệu | FREE (`KETNOI_FREELANCERSV`) | DATA (`TESTDATA`) | Chênh lệch | Đánh giá mức độ ảnh hưởng |
| :--- | :---: | :---: | :---: | :--- |
| **Users** | **14** | **6** | -8 | **Nghiêm trọng:** Thiếu nhiều tài khoản test, mật khẩu seed bị lỗi băm. |
| **Roles** | 3 | 3 | 0 | Giống nhau (1: Freelancer, 2: Employer, 3: Admin). |
| **FreelancerStudents** | **8** | **2** | -6 | **Nghiêm trọng:** Thiếu 6 hồ sơ sinh viên; trang tìm kiếm Freelancer bị rỗng. |
| **NhaTuyenDung** | **3** | **1** | -2 | **Nghiêm trọng:** Chỉ có 1 nhà tuyển dụng duy nhất (maUser=4). |
| **JobPost** | **12** | **3** | -9 | **Nghiêm trọng:** Danh sách công việc rất ít, không đủ các trạng thái để test. |
| **UngTuyen** | **14** | **3** | -11 | Thiếu dữ liệu ứng tuyển cho các vị trí. |
| **HopDong** | **8** | **1** | -7 | **Nghiêm trọng:** Chỉ có 1 hợp đồng duy nhất, không thể test luồng hợp đồng. |
| **GiaiDoan_HopDong** | **16** | **3** | -13 | Thiếu mốc bàn giao / milestone. |
| **Task_CongViec** | **17** | **4** | -13 | Thiếu đầu việc quản lý tiến độ. |
| **BanGiao_SanPham** | **8** | **2** | -6 | Thiếu dữ liệu nghiệm thu / nộp sản phẩm. |
| **YeuCauChinhSua** | **1** | **0** | -1 | Không có dữ liệu kiểm thử tính năng yêu cầu sửa sản phẩm. |
| **DanhGia_NhanXet** | **9** | **2** | -7 | Thiếu dữ liệu đánh giá và lịch sử sửa đánh giá. |
| **DanhGia_NhanXet_LichSu**| **1** | **0 (Không có bảng)**| -1 | Mất toàn bộ lịch sử chỉnh sửa đánh giá. |
| **Wallet** | **13** | **4** | -9 | Thiếu ví tiền cho các tài khoản sinh viên và nhà tuyển dụng mới. |
| **GiaoDichNapTien** | **11** | **0** | -11 | **Nghiêm trọng:** Không có bất kỳ giao dịch nạp tiền VNPay/ngân hàng nào. |
| **LichSuGiaoDich** | **52** | **4** | -48 | Mất toàn bộ lịch sử dòng tiền ký quỹ, giải ngân, hoàn tiền. |
| **LuuViec_ChoFreelancer** | **1** | **0** | -1 | Không có dữ liệu lưu việc làm yêu thích. |
| **ThongBao** | **58** | **5** | -53 | Không có thông báo tiến độ, hợp đồng, thanh toán. |
| **TinNhanChat / PhongChat**| **19 / 12** | **5 / 2** | -14 / -10 | Thiếu lịch sử hội thoại trao đổi dự án. |

### 5.2. Sự cố Hash mật khẩu người dùng (Root Cause làm mất khả năng Đăng nhập)

Đây là **phát hiện then chốt** giải thích tại sao không thể đăng nhập trên database DATA:

Trong mã nguồn Backend API (`FreelancerStudent.API/Helper/PasswordHasher.cs`):
```csharp
public static bool VerifyPassword(string password, string matKhauDaHash)
{
    try
    {
        return BCrypt.Net.BCrypt.Verify(password, matKhauDaHash);
    }
    catch
    {
        return false;
    }
}
```
Phương thức xác thực sử dụng hàm `BCrypt.Net.BCrypt.Verify`. Hàm này yêu cầu chuỗi `matKhauDaHash` bắt buộc phải tuân theo cấu trúc mã hóa BCrypt hợp lệ (bắt đầu bằng `$2a$`, `$2b$` hoặc `$2y$`). Nếu chuỗi mật khẩu trong DB không đúng định dạng BCrypt, thư viện sẽ bắn ra ngoại lệ `SaltParseException`, khối `catch` bắt lại và trả về **`false`**!

- **Tại Database FREE (`KETNOI_FREELANCERSV`):**
  - Tài khoản Admin (`admin_sys`): Mật khẩu là `$2a$11$yHRnVVaHMgLZBlk1vZO74eTL8Jndh0VwTLR50z5yvP7mJaVHAzx/u` (Chuẩn BCrypt $\rightarrow$ **Đăng nhập thành công**).
  - Tài khoản Admin khác (`lnht123`): Mật khẩu băm `$2a$11$...` $\rightarrow$ **Đăng nhập thành công**.
  - Các tài khoản Employer/Freelancer (`nht123`, `tvt123`, `nln123`, `tval123`, `lnt123`, `freelancer`, `ntd`): Đều có mã băm BCrypt $\rightarrow$ **Đăng nhập thành công**.
- **Tại Database DATA (`TESTDATA`):**
  - Cả 5 tài khoản seed mặc định đều chứa chuỗi giả lập thô:
    - User 1 (`an_freelancer`): `'hash_pass_123'`
    - User 2 (`bich_designer`): `'hash_pass_456'`
    - User 3 (`long_coder`): `'hash_pass_789'`
    - User 4 (`employer_abc`): `'hash_pass_abc'`
    - User 5 (`admin_sys`): `'hash_pass_admin'`
  - **Hậu quả:** Bất kể người dùng nhập mật khẩu là gì (`123456`, `admin`, `password`), `BCrypt.Verify` luôn bắt lỗi định dạng và trả về `false`. **KHÔNG MỘT TÀI KHOẢN NÀO TRONG SỐ 5 TÀI KHOẢN MẪU CỦA DATABASE DATA CÓ THỂ ĐĂNG NHẬP ĐƯỢC.**

---

## 6. ĐỐI CHIẾU VỚI ENTITY, DBCONTEXT VÀ TRUY VẤN SOURCE CODE

Khi rà soát mã nguồn C# tại `FreelancerStudent.API`, phát hiện các xung đột trực tiếp giữa Model và CSDL `TESTDATA`:

### 6.1. Entity `FreelancerStudents` (`Models/FreelancerStudents.cs`)
```csharp
[Column(TypeName = "decimal(3,2)")]
public decimal diemDanhGiaTrungBinh { get; set; }

public int soLuotDanhGia { get; set; }
```
- **Sử dụng trong code:**
  - `FreelancerStudentService.cs`: Gọi khi lấy chi tiết hồ sơ sinh viên, cập nhật điểm đánh giá trung bình.
  - `BaiDangTimViecService.cs` & `JobPostService.cs`: Include `FreelancerStudents` để lấy điểm sao hiển thị lên thẻ bài đăng.
  - `AccountController.cs` / `AuthService.cs`: Khi đăng ký tài khoản sinh viên mới, hàm `themFreelancerStudent_DuaVaoMaRoleCuaUsers` chèn một đối tượng `FreelancerStudents` mới vào DB.
- **Hành vi trên DATA:** Bị chặn với lỗi `Invalid column name 'diemDanhGiaTrungBinh'`, `Invalid column name 'soLuotDanhGia'`.

### 6.2. Entity `NhaTuyenDung` (`Models/NhaTuyenDung.cs`)
```csharp
public int soLuotDanhGia { get; set; }
```
- **Sử dụng trong code:**
  - `NhaTuyenDungService.cs`: Tính toán và hiển thị độ uy tín của nhà tuyển dụng.
  - `AuthService.cs`: Khi đăng ký tài khoản Nhà tuyển dụng mới, hàm `themNhaTuyenDungDuaVaoMaRoleCuaUsers` chèn bản ghi mới.
- **Hành vi trên DATA:** Bị chặn với lỗi `Invalid column name 'soLuotDanhGia'`.

### 6.3. Entity `DanhGia_NhanXet` & `DanhGia_NhanXet_LichSu` (`Models/DanhGia_NhanXet.cs`)
```csharp
public byte? soSaoChatLuong { get; set; }
public byte? soSaoDungHan { get; set; }
public byte? soSaoGiaoTiep { get; set; }
public DateTime? ngayCapNhat { get; set; }
public int soLanChinhSua { get; set; }
```
- **Sử dụng trong code:**
  - `DanhGiaService.cs`: Xử lý đánh giá đa tiêu chí (chất lượng, đúng hạn, giao tiếp), kiểm tra số lần chỉnh sửa tối đa (`soLanChinhSua < 1`), ghi log chỉnh sửa sang bảng `DanhGia_NhanXet_LichSu`.
- **Hành vi trên DATA:** Bị ném lỗi `Invalid column name 'soSaoChatLuong' ...` và `Invalid object name 'DanhGia_NhanXet_LichSu'`.

### 6.4. Entity `DuAnTrongPortfolio` (`Models/DuAnTrongPorfolio.cs`)
```csharp
public bool laDuAnNoiBat { get; set; }
```
- **Sử dụng trong code:**
  - `PortfolioRepository.cs` & `PorfolioService.cs`: Lọc và hiển thị các dự án tiêu biểu lên đầu trang Profile sinh viên.
- **Hành vi trên DATA:** Bị ném lỗi `Invalid column name 'laDuAnNoiBat'`.

---

## 7. KẾT QUẢ KIỂM THỬ THỰC TẾ TRÊN TỪNG DATABASE

| Nội dung kiểm thử | Database FREE (`KETNOI_FREELANCERSV`) | Database DATA (`TESTDATA`) | Kết luận & Chi tiết lỗi thực tế trên DATA |
| :--- | :---: | :---: | :--- |
| **1. Kết nối Database** | **Thành công** | **Thành công** | Cả hai database đều kết nối bình thường qua chuỗi kết nối SQL Server. |
| **2. Khởi động API** | **Thành công** | **Thành công** | API khởi động được (do EF Core khởi tạo mô hình chậm - lazy validation). |
| **3. Đăng nhập tài khoản seed** | **Thành công** | **Thất bại** | `BCrypt.Verify` thất bại do mật khẩu là `'hash_pass_...'` không đúng định dạng. |
| **4. Đăng ký tài khoản Sinh viên**| **Thành công** | **Thất bại** | SQL Server báo lỗi: `Invalid column name 'diemDanhGiaTrungBinh'`. |
| **5. Đăng ký tài khoản Tuyển dụng**| **Thành công** | **Thất bại** | SQL Server báo lỗi: `Invalid column name 'soLuotDanhGia'`. |
| **6. Xem hồ sơ Freelancer** | **Thành công** | **Thất bại** | Lỗi không tìm thấy cột `diemDanhGiaTrungBinh` và `laDuAnNoiBat`. |
| **7. Xem / Tạo đánh giá dự án** | **Thành công** | **Thất bại** | Lỗi thiếu các cột `soSaoChatLuong` và thiếu bảng `DanhGia_NhanXet_LichSu`. |
| **8. Xem danh sách công việc** | **Thành công** | **Thiếu dữ liệu** | Chỉ hiển thị 3 bài việc làm đơn điệu, không có bài đang tuyển mới. |
| **9. Ký quỹ & Dòng tiền ví** | **Thành công** | **Thất bại** | Bảng nạp tiền rỗng (`0 rows`), lịch sử giao dịch thiếu dữ liệu liên kết. |

---

## 8. DANH SÁCH LỖI PHÁT HIỆN ĐƯỢC (ERROR CATALOG)

### Lỗi 1: `SqlException: Invalid column name 'diemDanhGiaTrungBinh'` & `'soLuotDanhGia'`
- **Bằng chứng thực nghiệm:** Thực thi câu lệnh `SELECT maFreelancerStudents, diemDanhGiaTrungBinh, soLuotDanhGia FROM FreelancerStudents` trên `TESTDATA` $\rightarrow$ SQL Server báo mã lỗi `Msg 207, Level 16`.
- **Database bị ảnh hưởng:** `TESTDATA` (`KLCN040_DATA.sql`).
- **Chức năng bị ảnh hưởng:** Đăng ký tài khoản Freelancer Student; Xem chi tiết sinh viên; Danh sách ứng viên; Tìm việc làm.

### Lỗi 2: `SqlException: Invalid column name 'soLuotDanhGia'` trên bảng `NhaTuyenDung`
- **Bằng chứng thực nghiệm:** Thực thi câu lệnh `SELECT maNhaTuyenDung, soLuotDanhGia FROM NhaTuyenDung` trên `TESTDATA` $\rightarrow$ SQL Server báo mã lỗi `Msg 207, Level 16`.
- **Database bị ảnh hưởng:** `TESTDATA`.
- **Chức năng bị ảnh hưởng:** Đăng ký tài khoản Nhà tuyển dụng; Xem hồ sơ công ty; Quản lý bài đăng tuyển dụng.

### Lỗi 3: `SqlException: Invalid object name 'DanhGia_NhanXet_LichSu'`
- **Bằng chứng thực nghiệm:** Thực thi câu lệnh `SELECT * FROM DanhGia_NhanXet_LichSu` trên `TESTDATA` $\rightarrow$ SQL Server báo mã lỗi `Msg 208, Level 16`.
- **Database bị ảnh hưởng:** `TESTDATA`.
- **Chức năng bị ảnh hưởng:** Toàn bộ module Đánh giá & Phản hồi; Lịch sử chỉnh sửa nhận xét giữa Nhà tuyển dụng và Sinh viên.

### Lỗi 4: `SqlException: Invalid column name 'laDuAnNoiBat'`
- **Bằng chứng thực nghiệm:** Thực thi `SELECT maDA, laDuAnNoiBat FROM DuAn_Trong_Portfolio` trên `TESTDATA` $\rightarrow$ Báo lỗi `Msg 207`.
- **Database bị ảnh hưởng:** `TESTDATA`.
- **Chức năng bị ảnh hưởng:** Xem Portfolio; Chỉnh sửa Portfolio sinh viên trên Website và Mobile.

### Lỗi 5: Toàn bộ tài khoản Seed không thể đăng nhập
- **Bằng chứng thực nghiệm:** Kiểm tra bảng `Users` trên `TESTDATA`, cột `pashWordHash` chứa các chuỗi thô `'hash_pass_123'`, `'hash_pass_admin'`. Khi gọi `PasswordHasher.VerifyPassword`, thư viện `BCrypt.Net` quăng lỗi format salt $\rightarrow$ luôn trả về false.
- **Database bị ảnh hưởng:** `TESTDATA`.
- **Chức năng bị ảnh hưởng:** Màn hình Đăng nhập của Quản trị viên, Nhà tuyển dụng và Sinh viên.

### Lỗi 6: Giao diện Đăng ký trên Web bị "đóng băng" (Silent Failure)
- **Bằng chứng thực nghiệm:** Trong `FreelancerStudent.Web/Views/Account/DangKy.cshtml` dòng 102:
  ```html
  <div asp-validation-summary="All" class="alert alert-danger mb-3" style="display:none;"></div>
  ```
  Thẻ thông báo lỗi bị gán cứng thuộc tính `style="display:none;"`. Khi Backend API trả về lỗi (do Lỗi 1 hoặc Lỗi 2 ở trên), Controller gán `ModelState.AddModelError(string.Empty, message)` và render lại View. Vì thẻ bị ẩn, người dùng không nhìn thấy bất kỳ thông báo lỗi nào và lầm tưởng hệ thống không phản hồi khi bấm nút "Đăng ký".
- **Chức năng bị ảnh hưởng:** Màn hình Đăng ký Website MVC.

---

## 9. NGUYÊN NHÂN GỐC RỄ (ROOT CAUSE)

1. **Sự phân kỳ giữa Schema mã nguồn SQL và Schema thực tế đã chạy Migration:**
   - Database `KETNOI_FREELANCERSV` (FREE) là cơ sở dữ liệu đã được áp dụng đầy đủ các tệp migration:
     - `FreelancerStudent.API/Database/Migrations/20261018_AddPortfolioProjectFeatured.sql`
     - `FreelancerStudent.API/Database/Migrations/20261022_HoanThienThanhToanDanhGia.sql`
   - Trong khi đó, cả 2 file `KLCN040_FREELANCERSTUDENT.sql` và `KLCN040_DATA.sql` chỉ là bản xuất cấu trúc gốc ban đầu (Baseline Schema), **chưa từng được cập nhật các câu lệnh của 2 migration trên**.
   - Khi tạo ra database `TESTDATA` từ file `KLCN040_DATA.sql`, database này bị thiếu toàn bộ các cột và bảng mà Backend API phiên bản mới nhất đang yêu cầu.

2. **Dữ liệu mẫu (Seed Data) trong `KLCN040_DATA.sql` sử dụng mật khẩu giả định thay vì BCrypt Hash:**
   - Người tạo file SQL đã điền các chuỗi giữ chỗ (`hash_pass_admin`, `hash_pass_123`) thay vì tạo chuỗi băm BCrypt thực sự bằng Helper `PasswordHasher`, dẫn đến việc không thể xác thực tài khoản qua API.

3. **Thiếu sót dữ liệu liên kết nghiệp vụ:**
   - `TESTDATA` chỉ chứa một lượng rất nhỏ bản ghi mẫu rời rạc (chỉ có 1 hợp đồng, 0 giao dịch nạp tiền, 0 việc đã lưu), không đáp ứng được các luồng xử lý liên hoàn giữa Tuyển dụng $\rightarrow$ Ứng tuyển $\rightarrow$ Ký quỹ hợp đồng $\rightarrow$ Tạo task tiến độ $\rightarrow$ Bàn giao nghiệm thu $\rightarrow$ Đánh giá.

---

## 10. HƯỚNG KHẮC PHỤC CỤ THỂ (REMEDIATION PLAN)

> [!IMPORTANT]
> Tuân thủ tuyệt đối quy định: Không thực hiện bất kỳ thao tác nào làm biến đổi database hoặc mã nguồn khi chưa có sự xác nhận của người dùng. Dưới đây là phương án kỹ thuật đề xuất để người dùng xem xét:

### Phương án 1: Nâng cấp chuẩn hóa Database `TESTDATA` và cập nhật file `KLCN040_DATA.sql` (Khuyến nghị)
1. **Bổ sung Schema còn thiếu vào `TESTDATA`:**
   - Thực thi tệp `20261018_AddPortfolioProjectFeatured.sql` trên `TESTDATA` (thêm cột `laDuAnNoiBat`).
   - Thực thi tệp `20261022_HoanThienThanhToanDanhGia.sql` trên `TESTDATA` (thêm các cột đánh giá đa tiêu chí, tạo bảng `DanhGia_NhanXet_LichSu`, thêm điểm đánh giá trung bình và số lượt đánh giá).
2. **Cập nhật Mật khẩu băm chuẩn BCrypt cho các User Seed trong `TESTDATA`:**
   - Cập nhật lại mật khẩu cho các User 1, 2, 3, 4, 5 với chuỗi băm BCrypt hợp lệ tương ứng với mật khẩu dễ nhớ (ví dụ: `123456` $\rightarrow$ `$2a$11$yHRnVVaHMgLZBlk1vZO74eTL8Jndh0VwTLR50z5yvP7mJaVHAzx/u`).
3. **Đồng bộ hóa vào file `KLCN040_DATA.sql`:**
   - Tích hợp trực tiếp các cột mới và bảng `DanhGia_NhanXet_LichSu` vào file `KLCN040_DATA.sql`.
   - Cập nhật các câu lệnh INSERT trong file với mã hash BCrypt thực tế, giúp file `KLCN040_DATA.sql` trở thành bộ cài đặt độc lập, chạy một lần là hoàn thiện.
4. **Sửa thẻ báo lỗi trên giao diện Web (`DangKy.cshtml`):**
   - Bỏ thuộc tính `style="display:none;"` tại dòng 102 để người dùng luôn thấy rõ thông báo lỗi từ server nếu form không hợp lệ hoặc gặp sự cố.

### Phương án 2: Sử dụng Database `KETNOI_FREELANCERSV` làm Database chuẩn
- Giữ nguyên cấu hình `appsettings.json` trỏ tới `Database=KETNOI_FREELANCERSV`.
- Tiến hành xuất (Export/Backup) toàn bộ cấu trúc và dữ liệu đầy đủ từ `KETNOI_FREELANCERSV` ghi đè sang `KLCN040_FREELANCERSTUDENT.sql` để lưu trữ mã nguồn chuẩn xác nhất của dự án.


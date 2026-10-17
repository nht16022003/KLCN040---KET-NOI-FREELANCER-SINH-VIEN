# README – Cập nhật luồng ứng tuyển, từ chối hợp đồng và ứng tuyển lại

**Dự án:** KLCN040 – Kết nối Freelancer Sinh viên  
**Nguồn đối chiếu:** `KLCN040---KET-NOI-FREELANCER-SINH-VIEN-main(7).zip`  
**Phạm vi:** ASP.NET Core API, ASP.NET Core MVC Web và SQL Server (`KETNOI_FREELANCERSV`).  
**Trạng thái tài liệu:** Ghi nhận mã nguồn có trong ZIP (7), phân biệt với các lưu ý/việc cần xác minh. Chưa chạy ứng dụng, Build hoặc kiểm thử Database thực tế.

## 1. Mục tiêu thay đổi

Cho phép Freelancer ứng tuyển lại một Job khi đơn bị Nhà tuyển dụng từ chối **hoặc** Freelancer từ chối dự thảo hợp đồng. Khi ứng tuyển lại, Nhà tuyển dụng **phải duyệt lại đơn** rồi mới được tạo dự thảo hợp đồng mới. Hợp đồng đã hủy được giữ trong lịch sử, không được coi là hợp đồng đang hoạt động.

**Luồng kỳ vọng:**

1. Freelancer nộp đơn → `UngTuyen.trangThaiUngTuyen = ChoDuyet`.
2. Nhà tuyển dụng duyệt → `ChapNhan`; hoặc từ chối → `TuChoi`.
3. Khi `ChapNhan`, Nhà tuyển dụng tạo dự thảo → `HopDong.trangThai = ChoXacNhan`.
4. Freelancer đồng ý → `ChoKyQuy`; sau khi ký quỹ thành công → `DangThucHien`.
5. Freelancer từ chối dự thảo → hợp đồng `DaHuy`, đơn ứng tuyển `TuChoi` trong cùng transaction.
6. Freelancer bấm **Ứng tuyển lại** → cập nhật lại đơn cũ thành `ChoDuyet`, cập nhật thông tin nộp đơn và ngày ứng tuyển.
7. Nhà tuyển dụng duyệt lại → `ChapNhan`, sau đó mới tạo hợp đồng **mới** (`maHD` mới, `ChoXacNhan`). Hợp đồng `DaHuy` cũ không bị xóa.

## 2. Các file liên quan và ý nghĩa thay đổi

> Đường dẫn bên dưới tính từ thư mục gốc dự án trong ZIP. “Đã có” nghĩa là logic được nhìn thấy trong source (7), **không** có nghĩa đã xác minh được lịch sử diff hay đã chạy thành công.

| File | Nội dung trong source (7) | Ý nghĩa |
|---|---|---|
| `FreelancerStudent.API/Services/JobPostService.cs` | `nopDonUngTuyenAsync()` chặn đơn `ChoDuyet`/`ChapNhan`, cho đơn `TuChoi` cập nhật lại thành `ChoDuyet`; `duyetUngTuyenAsync()` chỉ xử lý đơn `ChoDuyet`; mapping thêm trạng thái hợp đồng | Ngăn nộp trùng, bắt buộc duyệt lại sau khi ứng tuyển lại và cung cấp dữ liệu cho UI |
| `FreelancerStudent.API/Repositories/JobPostRepository.cs` | Các thao tác tìm đơn theo Job–Freelancer, cập nhật đơn, thêm đơn | Lớp truy cập dữ liệu phục vụ kiểm tra trùng và nộp lại |
| `FreelancerStudent.API/Repositories/HopDongRepository.cs` | `CapNhatTrangThaiHopDongAsync()` dùng transaction; khi `DaHuy` chuyển đơn tương ứng sang `TuChoi`; truy vấn kiểm tra hợp đồng chưa hủy; truy vấn theo Job bỏ qua `DaHuy` | Đồng bộ hai bảng khi từ chối; cho phép tạo hợp đồng mới sau khi hợp đồng cũ bị hủy |
| `FreelancerStudent.API/Services/HopDongService.cs` | Kiểm tra Nhà tuyển dụng sở hữu Job, đơn ứng tuyển đúng Job/Freelancer và trạng thái `ChapNhan` trước khi tạo; `XacNhanHopDongAsync()` chọn `ChoKyQuy`/`DaHuy` | Không cho tạo hợp đồng khi chưa được duyệt lại; xử lý xác nhận/từ chối |
| `FreelancerStudent.API/Controllers/HopDongController.cs` | Endpoint `tao-hop-dong`, `xac-nhan`, `KyQuy`, tra cứu hợp đồng | Điểm tiếp nhận yêu cầu Web gửi tới API |
| `FreelancerStudent.API/DTOs/ReponseDTOs/UngTuyen_ReponseDTO.cs` | Có `TrangThaiHopDong`, `MaHopDong` | Đưa thông tin hợp đồng vào phản hồi danh sách ứng viên |
| `FreelancerStudent.Web/ViewModels/UngTuyenViewModel.cs` | Có `TrangThaiHopDong`, `MaHopDong` | Nhận dữ liệu API và hiển thị ở Razor View |
| `FreelancerStudent.Web/Controllers/JobPostController.cs` | Trang danh sách Job xây `ViewBag.TrangThaiUngTuyen`; GET `UngTuyen` kiểm tra đơn đã nộp | Đổi nút “Ứng tuyển”, “Đã nộp đơn”, “Đã được duyệt”, “Ứng tuyển lại”; hạn chế mở biểu mẫu trùng |
| `FreelancerStudent.Web/Views/JobPost/Index.cshtml` | Rẽ nhánh theo `ChoDuyet`, `ChapNhan`, `TuChoi` | Trạng thái nút theo đơn ứng tuyển |
| `FreelancerStudent.Web/Views/NhaTuyenDung/DanhSachUngVien.cshtml` | Hiển thị `ChoDuyet` với nút Duyệt/Từ chối; `ChapNhan` với trạng thái hợp đồng hoặc nút Trao đổi/Tạo hợp đồng; `TuChoi` + `DaHuy` với thông báo đã hủy | Phân biệt đơn chờ duyệt, hợp đồng chờ xác nhận và hợp đồng đã hủy |
| `FreelancerStudent.Web/Controllers/HopDongController.cs` | Action `XacNhan` lấy `maHD`, `dongY`, `lyDoTuChoi` và gọi WebService | Chuyển thao tác đồng ý/từ chối từ giao diện sang API |
| `FreelancerStudent.Web/Services/HopDongWebService.cs` | POST `api/HopDong/xac-nhan?maUserFreelancer=...` với payload `{ maHD, dongY, lyDoTuChoi }` | Đảm bảo lựa chọn từ chối (`dongY=false`) được gửi sang API |
| `Database/KLCN040_FREELANCERSTUDENT.sql` | Có phần bổ sung cột `dieuKhoan` và filtered unique index ở cuối script | Lưu điều khoản và giới hạn số hợp đồng chưa hủy cho một Job; xem lưu ý quan trọng ở mục 5 |

## 3. Chi tiết logic đã có trong source

### 3.1. Chặn nộp đơn trùng và cho phép nộp lại

**API:** `FreelancerStudent.API/Services/JobPostService.cs` → `nopDonUngTuyenAsync()`.

- Nếu chưa có đơn: thêm bản ghi `UngTuyen` mới với trạng thái `ChoDuyet`.
- Nếu đã có đơn `ChoDuyet`: báo đã nộp, chờ duyệt.
- Nếu đã có đơn `ChapNhan`: không cho nộp trùng.
- Nếu đơn là `TuChoi`: cập nhật thư giới thiệu, thù lao đề xuất, thời gian hoàn thành, CV, `ngayUngTuyen`, và chuyển trạng thái về `ChoDuyet`.
- Các trạng thái khác không được nộp lại theo nhánh xử lý này.

**Ý nghĩa:** Freelancer không thể bỏ qua bước duyệt; nộp lại sử dụng bản ghi đơn cũ thay vì tự tạo nhiều đơn cho cùng cặp Job–Freelancer.

### 3.2. Nhà tuyển dụng duyệt lại

**API:** `JobPostService.cs` → `duyetUngTuyenAsync()`.

Chỉ nhận trạng thái đích `ChapNhan` hoặc `TuChoi` và chỉ cho xử lý đơn hiện tại là `ChoDuyet`. Vì vậy, sau khi Freelancer nộp lại, Nhà tuyển dụng phải thực hiện thao tác duyệt lần nữa.

### 3.3. Tạo dự thảo hợp đồng

**API:** `HopDongService.cs` → `TaoDuThaoHopDongAsync()`.

Kiểm tra Job, quyền sở hữu Job, Freelancer, đơn ứng tuyển, trạng thái `ChapNhan` và sự tồn tại của hợp đồng chưa hủy trước khi tạo. Hợp đồng mới được khởi tạo `ChoXacNhan` và có thể lưu `dieuKhoan`.

**Repository:** `LayHopDongMoiNhatTheoJobVaFreelancerAsync()` thực tế đang tìm **một hợp đồng chưa hủy** theo Job–Freelancer; tên hàm chứa “MoiNhat” nhưng truy vấn không sắp xếp theo thời gian.

### 3.4. Freelancer từ chối dự thảo

**Web:** `FreelancerStudent.Web/Controllers/HopDongController.cs` → `XacNhan()` → `HopDongWebService.XacNhanHopDongAsync()`.

**API:** `HopDongController.XacNhanHopDong()` → `HopDongService.XacNhanHopDongAsync()` → `HopDongRepository.CapNhatTrangThaiHopDongAsync()`.

- `dongY = true` → hợp đồng `ChoKyQuy`.
- `dongY = false` → hợp đồng `DaHuy` và đơn `TuChoi`.
- Repository dùng transaction để lưu thay đổi hợp đồng, đơn ứng tuyển và thông báo cùng nhau.
- Repository hiện yêu cầu tìm **đúng một** đơn ứng tuyển cho cặp Job–Freelancer và đơn phải đang `ChapNhan` khi hủy.

**Ý nghĩa:** Không còn tình huống dự kiến `HopDong = DaHuy` nhưng đơn vẫn `ChapNhan` nếu thao tác thực sự đi qua phương thức này và transaction thành công.

### 3.5. Hiển thị trạng thái ở Web

**`Views/JobPost/Index.cshtml`:** `ChoDuyet` → “Đã nộp đơn”; `ChapNhan` → “Đã được duyệt”; `TuChoi` → “Ứng tuyển lại”; chưa có đơn → “Ứng tuyển”.

**`Views/NhaTuyenDung/DanhSachUngVien.cshtml`:** `ChoDuyet` → nút Duyệt/Từ chối; `ChapNhan` chưa có hợp đồng hiện hành → Trao đổi/Tạo hợp đồng; hợp đồng `ChoXacNhan`, `ChoKyQuy`, `DangThucHien` → trạng thái tương ứng; `TuChoi` có lịch sử `DaHuy` → “Đã hủy - Chờ ứng tuyển lại”.

**API mapping:** `JobPostService.cs` trả `DaHuy` cho đơn `TuChoi` nếu tìm thấy hợp đồng hủy; với các trạng thái đơn khác, chỉ lấy hợp đồng **không** `DaHuy`. Nhờ đó hợp đồng cũ không che nút Tạo hợp đồng sau khi duyệt lại.

## 4. Bảng trạng thái kiểm thử

| Tình huống | `UngTuyen` | `HopDong` | Hành vi mong đợi |
|---|---|---|---|
| Chưa ứng tuyển | Chưa có | Chưa có | Ứng tuyển |
| Vừa nộp đơn | `ChoDuyet` | Chưa có | Đã nộp đơn; NTD có nút Duyệt/Từ chối |
| NTD từ chối đơn | `TuChoi` | Chưa có | Ứng tuyển lại |
| NTD duyệt đơn | `ChapNhan` | Chưa có | NTD có nút Tạo hợp đồng |
| Tạo dự thảo | `ChapNhan` | `ChoXacNhan` | Freelancer có thể xác nhận/từ chối |
| Freelancer từ chối dự thảo | `TuChoi` | `DaHuy` | Ứng tuyển lại; hợp đồng cũ được lưu |
| Freelancer ứng tuyển lại | `ChoDuyet` | `DaHuy` (lịch sử) | NTD phải duyệt lại |
| NTD duyệt lại | `ChapNhan` | `DaHuy` (lịch sử) | NTD có nút Tạo hợp đồng mới |
| Tạo hợp đồng mới | `ChapNhan` | `DaHuy` cũ + `ChoXacNhan` mới | Freelancer xác nhận dự thảo mới |
| Freelancer đồng ý | `ChapNhan` | `ChoKyQuy` mới | NTD ký quỹ |
| Ký quỹ thành công | `ChapNhan` | `DangThucHien` mới | Bắt đầu thực hiện công việc |

## 5. Database – phần nên giữ và cảnh báo quan trọng

**File:** `Database/KLCN040_FREELANCERSTUDENT.sql`.

Trong phần cập nhật cuối file ZIP (7) có logic:

```sql
IF COL_LENGTH('dbo.HopDong', 'dieuKhoan') IS NULL
BEGIN
    ALTER TABLE dbo.HopDong
    ADD dieuKhoan NVARCHAR(MAX) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_HopDong_MaJob_Active'
      AND object_id = OBJECT_ID('dbo.HopDong')
)
BEGIN
    CREATE UNIQUE INDEX UX_HopDong_MaJob_Active
    ON dbo.HopDong(maJob)
    WHERE trangThai <> 'DaHuy';
END;
GO
```

**Ý nghĩa:** `dieuKhoan` lưu điều khoản hợp đồng. Filtered unique index cho phép lưu nhiều hợp đồng `DaHuy` lịch sử của cùng Job nhưng tối đa một hợp đồng **chưa hủy** cho mỗi Job.

**CẢNH BÁO KHI KHỞI TẠO DATABASE MỚI:** Ở phần `CREATE TABLE HopDong` đầu file SQL (7), `maJob` vẫn được khai báo `VARCHAR(20) UNIQUE NOT NULL`. Phần cuối file thêm filtered index **không tự xóa** ràng buộc UNIQUE cũ. Nếu chạy script này để dựng DB mới, vẫn không thể tạo hợp đồng thứ hai cùng Job sau khi hợp đồng cũ `DaHuy`. Cần bỏ `UNIQUE` khỏi định nghĩa `maJob` khi tạo bảng mới, hoặc có migration bỏ đúng constraint cũ trước khi tạo filtered index. Không chạy `DROP CONSTRAINT` với tên tự sinh cố định trên mọi môi trường.

**Quy tắc nghiệp vụ hiện hành:** index theo `maJob` cho phép tối đa một hợp đồng chưa hủy cho **toàn Job**. Nếu một Job tuyển nhiều Freelancer đồng thời, cần xem xét index theo `(maJob, maFreelancerStudent)` và cập nhật quy tắc kiểm tra tương ứng.

**Không đưa vào migration chính:** Các câu `SELECT` debug, `SELECT @@TRANCOUNT`, hoặc `UPDATE UngTuyen ... WHERE maUngTuyen = 21` (đó là thao tác sửa dữ liệu kiểm thử, không phải thay đổi cấu trúc).

## 6. Những điểm còn cần xác minh/hoàn thiện

1. **Mã hợp đồng:** `HopDongService.cs` trong ZIP (7) vẫn tạo `maHD` bằng thời gian + `new Random().Next(10,99)`. Cần kiểm tra nguy cơ trùng mã khi tạo đồng thời; có thể dùng GUID rút gọn nếu vẫn bảo đảm cột `maHD VARCHAR(20)`.
2. **Phân quyền API:** API Controller vẫn nhận `maUserNTD` và `maUserFreelancer` từ query string. Kiểm tra ở Service là cần thiết nhưng không đủ nếu ID do client tự truyền. Nên xác thực token/claims và lấy user ID phía server.
3. **Đồng thời:** Kiểm tra hợp đồng chưa hủy trước khi thêm không thay thế được ràng buộc database. Filtered unique index giúp ngăn trùng ở DB; cần xử lý lỗi unique constraint thành thông báo nghiệp vụ.
4. **Lịch sử các lần ứng tuyển:** Hiện việc nộp lại cập nhật cùng một bản ghi `UngTuyen`. Muốn truy vết mỗi lần ứng tuyển độc lập, cần bổ sung bảng lịch sử hoặc liên kết hợp đồng với một phiên/lần ứng tuyển.
5. **Tra cứu hợp đồng:** `LayHopDongTheoMaJobAsync()` đã bỏ qua `DaHuy`, phù hợp cho kiểm tra hợp đồng hiện hành nhưng không dùng để xem lịch sử đã hủy. Muốn xem lịch sử, dùng danh sách hợp đồng hoặc chi tiết theo `maHD`.
6. **Source SQL và DB đang chạy có thể khác nhau:** ZIP là snapshot mã nguồn, không chứng minh các thay đổi SQL đã được áp dụng trên instance SQL Server hiện tại. Cần kiểm tra index/constraint thực tế bằng `sys.indexes` và `sys.key_constraints`.
7. **Kiểm thử:** Chưa có kết quả Build API/Web hoặc kiểm thử đầu-cuối được thực thi trong quá trình soạn README này.

## 7. Cách kiểm tra sau khi cập nhật

### Build

Tại thư mục dự án:

```powershell
cd FreelancerStudent.API
dotnet build

cd ../FreelancerStudent.Web
dotnet build
```

### Kiểm tra trạng thái dữ liệu

```sql
USE [KETNOI_FREELANCERSV];
GO

SELECT u.maUngTuyen, u.maJob, u.maFreelancerStudent,
       u.trangThaiUngTuyen, u.ngayUngTuyen,
       h.maHD, h.trangThai AS trangThaiHopDong
FROM dbo.UngTuyen AS u
LEFT JOIN dbo.HopDong AS h
  ON h.maJob = u.maJob
 AND h.maFreelancerStudent = u.maFreelancerStudent
ORDER BY u.ngayUngTuyen DESC;

SELECT i.name, i.is_unique, i.has_filter, i.filter_definition
FROM sys.indexes AS i
WHERE i.object_id = OBJECT_ID('dbo.HopDong');
```

**Lưu ý:** JOIN có thể trả nhiều dòng cho một đơn khi có nhiều hợp đồng lịch sử; đó là hành vi bình thường. Không tự động xem `ChapNhan` + `DaHuy` là sai nếu Freelancer đã ứng tuyển lại và được duyệt lại.

### Kiểm thử đầu-cuối

1. Nộp đơn mới; thử nộp trùng và xác nhận bị chặn.
2. NTD duyệt đơn; tạo dự thảo hợp đồng.
3. Freelancer từ chối; kiểm tra `UngTuyen = TuChoi`, `HopDong = DaHuy`.
4. Freelancer ứng tuyển lại; kiểm tra `UngTuyen = ChoDuyet`.
5. NTD duyệt lại; kiểm tra nút Tạo hợp đồng xuất hiện.
6. Tạo hợp đồng mới; xác nhận có `maHD` khác và hợp đồng cũ vẫn `DaHuy`.
7. Freelancer đồng ý; NTD ký quỹ; xác nhận trạng thái chuyển đúng.

## 8. Kết luận

Source (7) đã chứa phần lớn logic cần thiết cho luồng **từ chối dự thảo → ứng tuyển lại → duyệt lại → tạo hợp đồng mới** ở API và Web. Việc hoàn thiện cần ưu tiên **đồng bộ script tạo Database mới**, **kiểm thử toàn bộ luồng**, **bảo mật danh tính API**, và **xử lý tạo hợp đồng đồng thời**. README này mô tả những gì đã quan sát được trong ZIP (7), không thay thế kết quả chạy thử.

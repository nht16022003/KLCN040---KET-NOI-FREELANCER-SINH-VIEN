# README – Tổng hợp các bước sửa chức năng ký quỹ hợp đồng (Bước 1–9)

**Dự án:** Freelancer Students (ASP.NET Core Web + ASP.NET Core API + SQL Server)  
**Phạm vi:** Luồng Nhà tuyển dụng ký quỹ hợp đồng bằng ví nội bộ.  
**Database đang dùng khi demo:** `KETNOI_FREELANCERSV`.

> **Lưu ý về mức độ xác nhận:** Tài liệu được tổng hợp từ các đoạn code, kết quả Build, ảnh và quá trình trao đổi. Các bước đã Build thành công **không đồng nghĩa** giao dịch đã được kiểm thử thành công từ đầu đến cuối. Tên một số lớp/hàm được giữ theo code đã trao đổi; nếu source đang dùng có khác biệt nhỏ, hãy đối chiếu trước khi áp dụng.

## 1. Mục tiêu nghiệp vụ theo README ban đầu

Khi Nhà tuyển dụng ký quỹ một hợp đồng ở trạng thái `ChoKyQuy`:

1. Xác nhận đúng Nhà tuyển dụng sở hữu hợp đồng và hợp đồng đủ điều kiện ký quỹ.
2. Trừ `sotienkyquy` khỏi `Wallet.soDuKhaDung`.
3. Cộng cùng số tiền vào `Wallet.soDuDongBang`.
4. Chuyển `HopDong.trangThai` từ `ChoKyQuy` sang `DangThucHien`, cập nhật `ngayBatDau`.
5. Ghi một dòng `LichSuGiaoDich` loại `KyQuyHopDong`, phương thức `ViNoiBo`, trạng thái `ThanhCong`.
6. Tạo `ThongBao` cho Freelancer.
7. Toàn bộ cập nhật phải nằm trong cùng một SQL Transaction: thành công cùng nhau hoặc rollback khi có lỗi.
8. Web hiển thị số dư, cảnh báo khi thiếu tiền và không cho xác nhận nếu không đủ tiền.

**Không tạo thêm bảng mới**; sử dụng `HopDong`, `Wallet`, `LichSuGiaoDich`, `ThongBao`.

---

## Bước 1 – Tạo Entity lịch sử giao dịch

**File tạo mới:** `FreelancerStudent.API/Models/LichSuGiaoDich.cs`

**Lý do:** API cần một Entity để ghi lại mỗi lần tiền bị đóng băng khi ký quỹ.

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("LichSuGiaoDich")]
    public class LichSuGiaoDich
    {
        [Key]
        public int maGiaoDich { get; set; }
        public int maWallet { get; set; }

        [Required]
        [MaxLength(50)]
        public string loaiGiaoDich { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal soTien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuTruocGD { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal soDuSauGD { get; set; }

        [MaxLength(255)]
        public string? noiDung { get; set; }

        [MaxLength(50)]
        public string? phuongThucThanhToan { get; set; }

        [MaxLength(100)]
        public string? maGiaoDichNgoai { get; set; }

        [MaxLength(30)]
        public string? trangThai { get; set; }
        public DateTime? ngayTao { get; set; }
    }
}
```

**Giải thích:** `[Table]` chỉ định bảng SQL; `[Key]` là khóa chính; các cột tiền dùng `decimal(18,2)` để tránh sai số do kiểu số thực. Entity phải khớp cấu trúc bảng thực tế trước khi chạy.

**Kết quả:** Đã thêm và Build thành công.

## Bước 2 – Đăng ký Entity vào DbContext

**File sửa:** `FreelancerStudent.API/Data/ApplicationDBContext.cs`

**Nội dung thêm** vào lớp `ApplicationDBContext`:

```csharp
public DbSet<LichSuGiaoDich> LichSuGiaoDichs { get; set; }
```

**Lý do:** Cho phép Entity Framework Core truy vấn và thêm bản ghi `LichSuGiaoDich` qua `_context.LichSuGiaoDichs`.

**Kết quả:** Đã Build thành công. Việc đăng ký `DbSet` **không tự tạo bảng**; bảng đã có trong SQL Server.

## Bước 3 – Viết nghiệp vụ ký quỹ ở API Service

**File sửa:** `FreelancerStudent.API/Services/HopDongService.cs`

**Các thay đổi đã thực hiện:**

- Thêm `using FreelancerStudent.API.Data;` và `using Microsoft.EntityFrameworkCore;`.
- Inject `ApplicationDBContext` vào constructor, đồng thời giữ `HopDongRepository`.
- Thay logic cũ của `KyQuyHopDongAsync(string maHD, int? maUserNhaTuyenDung)` vốn chỉ chuyển trạng thái hợp đồng bằng logic xử lý ký quỹ và ví.
- Kiểm tra mã Nhà tuyển dụng, chủ sở hữu hợp đồng, trạng thái `ChoKyQuy`, số tiền ký quỹ dương và người nhận thông báo.
- Dùng `BeginTransactionAsync()`; cập nhật hợp đồng, cập nhật ví, ghi lịch sử, ghi thông báo; `CommitAsync()` khi thành công và `RollbackAsync()` nếu có lỗi.

**Hai truy vấn cập nhật trọng tâm:**

```csharp
int soHopDongCapNhat = await _context.Database.ExecuteSqlInterpolatedAsync($@"
    UPDATE HopDong
    SET trangThai = N'DangThucHien', ngayBatDau = GETDATE()
    WHERE maHD = {maHD} AND trangThai = N'ChoKyQuy'");

if (soHopDongCapNhat != 1)
    throw new InvalidOperationException("Hợp đồng không còn ở trạng thái chờ ký quỹ.");

int soViCapNhat = await _context.Database.ExecuteSqlInterpolatedAsync($@"
    UPDATE Wallet
    SET soDuKhaDung = soDuKhaDung - {soTienKyQuy},
        soDuDongBang = soDuDongBang + {soTienKyQuy}
    WHERE maUser = {maUserNhaTuyenDung.Value}
      AND soDuKhaDung >= {soTienKyQuy}");

if (soViCapNhat != 1)
    throw new InvalidOperationException("Ví không tồn tại hoặc số dư khả dụng không đủ.");
```

**Giải thích:**

- `WHERE trangThai = 'ChoKyQuy'` giúp tránh xử lý lại hợp đồng đã đổi trạng thái.
- `WHERE soDuKhaDung >= ...` giúp tránh trừ thành số âm ngay ở câu SQL cập nhật.
- `ExecuteSqlInterpolatedAsync` truyền tham số SQL an toàn hơn nối chuỗi thủ công.
- Transaction giúp hạn chế tình trạng hợp đồng đổi trạng thái nhưng ví chưa được cập nhật.
- Sau cập nhật ví, Service lấy lại số dư để tạo `LichSuGiaoDich` và thêm `ThongBao` cho Freelancer.

**Kết quả:** Đã Build thành công. **Chưa xác nhận** các thao tác INSERT lịch sử/thông báo chạy thành công trên Database thực tế.

## Bước 4 – Bổ sung thông tin ví và Modal ký quỹ trên Web

**Các file sửa:**

1. `FreelancerStudent.Web/ViewModels/HopDongViewModel.cs`
2. `FreelancerStudent.Web/Controllers/HopDongController.cs`
3. `FreelancerStudent.Web/Views/HopDong/ChiTiet.cshtml`

**Trong ViewModel, thêm:**

```csharp
public decimal SoDuKhaDung { get; set; } = 0;
public bool DaTaiThongTinVi { get; set; } = false;
```

**Trong Web Controller:** inject `IWalletWebService` và tại action GET `ChiTiet(string maHD)`, chỉ tải ví khi người dùng đúng là chủ hợp đồng và hợp đồng `ChoKyQuy`:

```csharp
if (hopDong.CoQuyenKyQuy &&
    hopDong.maUserNhaTuyenDung == maUser.Value &&
    hopDong.trangThai == "ChoKyQuy")
{
    var ketQuaVi = await _walletWebService.layViTheoUser(maUser.Value);
    if (ketQuaVi != null && ketQuaVi.success && ketQuaVi.data != null)
    {
        hopDong.SoDuKhaDung = ketQuaVi.data.soDuKhaDung;
        hopDong.DaTaiThongTinVi = true;
    }
}
```

**Trong View `ChiTiet.cshtml`:**

- Chỉ hiển thị nút ký quỹ cho Nhà tuyển dụng sở hữu hợp đồng đang `ChoKyQuy`.
- Modal hiển thị tiền cần ký quỹ, số dư khả dụng, phần tiền còn thiếu.
- Khi không tải được ví hoặc ví thiếu tiền, hiển thị cảnh báo và vô hiệu hóa nút xác nhận.
- Form POST về `HopDong/KyQuy`, có Anti-Forgery Token và `maHD`.

**Điều kiện nút ký quỹ:**

```cshtml
@if (Model.CoQuyenKyQuy &&
     Model.LaNhaTuyenDungCuaHopDong &&
     Model.maUserNhaTuyenDung.HasValue &&
     Model.trangThai == "ChoKyQuy")
{
    // Hiển thị nút mở Modal ký quỹ
}
```

**Điều kiện cho phép xác nhận trong Modal:**

```csharp
var soTienCanKyQuy = Model.sotienkyquy ?? 0;
var soDuHienTai = Model.SoDuKhaDung;
var duTien = Model.DaTaiThongTinVi &&
            soTienCanKyQuy > 0 &&
            soDuHienTai >= soTienCanKyQuy;
```

**Kết quả:** Đã Build thành công; chưa có ảnh xác nhận Modal hoạt động đúng trong tình huống thiếu/đủ tiền.

## Bước 5 – Siết quyền xem chi tiết và quyền ký quỹ

**File sửa:** `FreelancerStudent.Web/Controllers/HopDongController.cs` – action GET `ChiTiet(string maHD)`.

**Vấn đề phát hiện:** Code cũ có nhánh `if (role == "NhaTuyenDung")` tự đánh dấu người dùng là Nhà tuyển dụng của hợp đồng dù chưa kiểm tra `maUser` có trùng chủ sở hữu. Điều này có thể khiến người dùng thấy thao tác không thuộc quyền của mình.

**Thay phần xác định quyền Nhà tuyển dụng bằng:**

```csharp
bool laChuHopDong = hopDong.maUserNhaTuyenDung.HasValue &&
                    hopDong.maUserNhaTuyenDung.Value == maUser.Value;

hopDong.LaNhaTuyenDungCuaHopDong = laChuHopDong;
hopDong.CoQuyenKyQuy = laChuHopDong &&
                         hopDong.trangThai == "ChoKyQuy";
```

**Thêm kiểm tra quyền xem**, sau khi xác định cả hai cờ `LaFreelancerCuaHopDong` và `LaNhaTuyenDungCuaHopDong`, trước đoạn tải ví và `return View(hopDong)`:

```csharp
if (!hopDong.LaFreelancerCuaHopDong &&
    !hopDong.LaNhaTuyenDungCuaHopDong)
{
    TempData["ErrorMessage"] = "Bạn không có quyền xem hợp đồng này.";
    return RedirectToAction("Index");
}
```

**Giải thích:** Quyền phải dựa trên **quan hệ sở hữu hợp đồng**, không chỉ dựa trên tên role trong Session.

**Kết quả:** Người dùng báo Build thành công.

> **Giới hạn bảo mật còn tồn tại:** Kiểm tra ở Web Controller không thay thế xác thực/ủy quyền tại API. API vẫn phải xác minh danh tính người gọi từ cơ chế xác thực đáng tin cậy, không chỉ tin tham số `maUserNhaTuyenDung` được gửi trong URL.

## Bước 6 – Kiểm tra và hoàn thiện POST ký quỹ ở Web

**File liên quan:** `FreelancerStudent.Web/Controllers/HopDongController.cs`.

**Action đã được đối chiếu:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> KyQuy(string maHD)
{
    var maUser = HttpContext.Session.GetInt32("maUser");
    if (maUser == null)
        return RedirectToAction("DangNhap", "Account");

    if (string.IsNullOrWhiteSpace(maHD))
    {
        TempData["ErrorMessage"] = "Mã hợp đồng không hợp lệ.";
        return RedirectToAction("Index");
    }

    var response = await _hopDongService.KyQuyHopDongAsync(maHD, maUser.Value);

    if (response.success)
        TempData["SuccessMessage"] = response.message;
    else
        TempData["ErrorMessage"] = response.message;

    return RedirectToAction("ChiTiet", new { maHD });
}
```

**Giải thích:**

- `[HttpPost]` và `[ValidateAntiForgeryToken]` bảo vệ thao tác thay đổi dữ liệu khỏi yêu cầu GET và hỗ trợ chống CSRF.
- Lấy `maUser` từ Session thay vì từ trường nhập trong form.
- Kiểm tra `maHD` trước khi gọi Web Service.
- Hiển thị thông báo thành công/thất bại qua `TempData`, rồi quay lại chi tiết hợp đồng.

**Kết quả:** Người dùng báo Bước 6 Build thành công.

## Bước 7 – Kiểm tra đường truyền Web Service → API Controller

**Các file liên quan:**

- Web: file chứa `KyQuyHopDongAsync(string maHD, int maUserNhaTuyenDung)` (dịch vụ gọi API hợp đồng, thường là `HopDongWebService.cs`).
- API: `FreelancerStudent.API/Controllers/HopDongController.cs`.

**Web Service gửi request:**

```csharp
var payload = new { maHD = maHD };
var url = $"api/HopDong/KyQuy?maUserNhaTuyenDung={maUserNhaTuyenDung}";
using var response = await _httpClient.PostAsJsonAsync(url, payload);
```

Web Service kiểm tra `response.IsSuccessStatusCode`, chuyển kết quả thành `ApiReponse<bool>` và đọc trường `message` khi API trả lỗi.

**API Controller tiếp nhận:**

```csharp
[HttpPost("KyQuy")]
public async Task<IActionResult> KyQuy(
    [FromBody] KyQuyHopDong_RequestDTO request,
    [FromQuery] int? maUserNhaTuyenDung)
{
    if (!ModelState.IsValid || request == null ||
        string.IsNullOrWhiteSpace(request.maHD))
        return BadRequest(new { message = "Mã hợp đồng không hợp lệ." });

    try
    {
        var ketQua = await _hopDongService.KyQuyHopDongAsync(
            request.maHD, maUserNhaTuyenDung);

        return Ok(new
        {
            success = ketQua,
            message = "Ký quỹ hợp đồng thành công! Hợp đồng đã chuyển sang trạng thái Đang thực hiện."
        });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (UnauthorizedAccessException ex)
    {
        return StatusCode(403, new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Lỗi hệ thống khi ký quỹ hợp đồng: " + ex.Message });
    }
}
```

**Giải thích:** Web gửi `maHD` trong JSON Body và mã Nhà tuyển dụng qua Query String. API gọi Service để xử lý nghiệp vụ và phân loại lỗi HTTP (`400`, `403`, `409`, `500`).

**Kết quả:** Người dùng báo Build thành công.

**Cần hoàn thiện trước khi triển khai thực tế:**

- API phải dùng danh tính được xác thực phía server; **không coi Query String `maUserNhaTuyenDung` là bằng chứng đăng nhập**.
- Không trả `ex.Message` của lỗi hệ thống ra môi trường production vì có thể lộ chi tiết nội bộ.
- Web Service nên đọc `success` trong JSON nếu API có quy ước trả `200` nhưng `success=false`; HTTP `2xx` một mình không luôn đủ để xác nhận thành công nghiệp vụ.

## Bước 8 – Kiểm tra Database và đối chiếu Entity

**Không phải bước sửa file C# trực tiếp.** Đây là bước kiểm tra bằng SQL Server.

Ban đầu truy vấn trả 0 dòng vì cửa sổ SQL đang ở Database `master`. Sau đó thử `USE KLCN040_FREELANCERSTUDENT` nhưng tên này không tồn tại trên SQL Server đang dùng. Cuối cùng xác định Database thực tế là **`KETNOI_FREELANCERSV`**.

**SQL kiểm tra cấu trúc:**

```sql
USE [KETNOI_FREELANCERSV];
GO

SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE,
       CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION,
       NUMERIC_SCALE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN
    ('HopDong', 'Wallet', 'LichSuGiaoDich', 'ThongBao')
ORDER BY TABLE_NAME, ORDINAL_POSITION;
```

**Đã xác nhận từ ảnh:** Bảng `HopDong` có `maHD`, `maJob`, `maFreelancerStudent`, `ngayBatDau`, `ngayKetThuc`, `sotienkyquy`, `hinhthuclamviec`, `trangThai`; `sotienkyquy` là `decimal(18,2)`.

**Chưa xác nhận đầy đủ:** Từng cột bắt buộc và kiểu dữ liệu của `Wallet`, `LichSuGiaoDich`, `ThongBao` đối chiếu với Entity/API Service.

**Lưu ý:** Connection String trong `FreelancerStudent.API/appsettings.json` hoặc `appsettings.Development.json` phải trỏ tới đúng Database thực tế.

## Bước 9 – Chạy demo và xử lý lỗi tải hợp đồng

**Các thao tác đã thực hiện:**

1. Chạy Web và API bằng Visual Studio.
2. Xác nhận Swagger API truy cập được tại `http://localhost:5152/swagger/index.html`.
3. Mở trang `Quản lý hợp đồng` trên Web; ban đầu hiển thị thông báo **“Không thể kết nối đến API hợp đồng.”**
4. Từ log API, phát hiện lỗi SQL **`Invalid column name 'dieuKhoan'`**: Entity Framework đang truy vấn cột `HopDong.dieuKhoan` nhưng bảng SQL thực tế không có.
5. Kiểm tra `INFORMATION_SCHEMA.COLUMNS` xác nhận bảng `HopDong` chỉ có 8 cột ở thời điểm đó.
6. Đã hướng dẫn **hai phương án**: thêm cột `dieuKhoan` nếu nghiệp vụ cần lưu điều khoản, hoặc bỏ ánh xạ Entity nếu không dùng. Có cung cấp câu lệnh `ALTER TABLE` với điều kiện kiểm tra tồn tại cột.
7. Sau đó Web đã hiển thị trang danh sách hợp đồng mà không còn lỗi kết nối, nhưng tại thời điểm ảnh chụp hiển thị **0 hợp đồng**. Điều này chứng minh lỗi tải trang đã được khắc phục hoặc không còn xảy ra trong lần kiểm tra, **không chứng minh chắc chắn phương án SQL nào đã được áp dụng**.

**SQL đã được đề xuất nếu xác nhận cần lưu điều khoản:**

```sql
USE [KETNOI_FREELANCERSV];
GO

IF COL_LENGTH('dbo.HopDong', 'dieuKhoan') IS NULL
BEGIN
    ALTER TABLE dbo.HopDong
    ADD dieuKhoan NVARCHAR(MAX) NULL;
END;
GO
```

> Câu `ALTER TABLE` là **phương án đã hướng dẫn**, không có xác nhận trực tiếp trong trao đổi rằng người dùng đã chạy nó. Sao lưu Database trước khi sửa schema.

**Kết quả Bước 9:** Web/API đã chạy; lỗi truy vấn `dieuKhoan` được khoanh vùng; danh sách hợp đồng có thể mở lại. Chưa kiểm thử xong luồng ký quỹ.

---

## Danh sách file đã tạo, sửa hoặc kiểm tra

| Bước | File | Loại công việc | Mục đích |
|---|---|---|---|
| 1 | `FreelancerStudent.API/Models/LichSuGiaoDich.cs` | Tạo mới | Entity lịch sử giao dịch |
| 2 | `FreelancerStudent.API/Data/ApplicationDBContext.cs` | Sửa | Khai báo `DbSet<LichSuGiaoDich>` |
| 3 | `FreelancerStudent.API/Services/HopDongService.cs` | Sửa | Xử lý transaction ký quỹ |
| 4 | `FreelancerStudent.Web/ViewModels/HopDongViewModel.cs` | Sửa | Bổ sung số dư và trạng thái tải ví |
| 4 | `FreelancerStudent.Web/Controllers/HopDongController.cs` | Sửa | Tải ví khi xem chi tiết |
| 4 | `FreelancerStudent.Web/Views/HopDong/ChiTiet.cshtml` | Sửa | Modal và kiểm tra đủ tiền |
| 5 | `FreelancerStudent.Web/Controllers/HopDongController.cs` | Sửa | Kiểm tra chủ hợp đồng, quyền xem |
| 6 | `FreelancerStudent.Web/Controllers/HopDongController.cs` | Kiểm tra/hoàn thiện | POST ký quỹ và phản hồi |
| 7 | Web Service chứa `KyQuyHopDongAsync` | Kiểm tra/hoàn thiện | Gửi HTTP POST đến API |
| 7 | `FreelancerStudent.API/Controllers/HopDongController.cs` | Kiểm tra/hoàn thiện | Endpoint `POST /api/HopDong/KyQuy` |
| 8 | SQL Server `KETNOI_FREELANCERSV` | Kiểm tra | Đối chiếu cấu trúc 4 bảng |
| 9 | Entity `HopDong` và bảng SQL `HopDong` | Chẩn đoán | Lỗi thiếu cột `dieuKhoan` |

**Ghi chú:** Các thay đổi về **đăng ký tài khoản / WalletRepository** được thực hiện trong cuộc trao đổi sau đó, không nằm trong 9 bước ký quỹ này; vì vậy không trộn vào danh sách sửa Bước 1–9.

## Checklist kiểm thử trước khi xác nhận hoàn thành README nghiệp vụ

- [x] Tạo Entity lịch sử giao dịch và đăng ký DbContext; Build thành công.
- [x] Bổ sung Service ký quỹ có SQL Transaction; Build thành công.
- [x] Bổ sung giao diện kiểm tra số dư và quyền ký quỹ; Build thành công.
- [x] Web và Swagger API đã khởi động được.
- [ ] Kiểm tra đầy đủ schema `Wallet`, `LichSuGiaoDich`, `ThongBao` với Entity.
- [ ] Xác thực danh tính tại API độc lập với `maUserNhaTuyenDung` trên Query String.
- [ ] Kiểm thử ví không đủ tiền: không trừ tiền, không đổi trạng thái.
- [ ] Kiểm thử ví đủ tiền: giảm số dư khả dụng, tăng số dư đóng băng đúng số tiền.
- [ ] Kiểm thử `HopDong` đổi `ChoKyQuy` → `DangThucHien` **chỉ sau khi ký quỹ thành công**.
- [ ] Kiểm tra tạo đúng một dòng `LichSuGiaoDich` và thông báo cho Freelancer.
- [ ] Kiểm thử rollback khi một bước lưu Database thất bại.
- [ ] Kiểm thử chống ký quỹ trùng (gửi yêu cầu hai lần/đồng thời).

## Truy vấn kiểm tra hợp đồng demo

Tại thời điểm trao đổi, ảnh Web cho thấy hợp đồng `HD26101717415092` ở trạng thái `DangThucHien`, nhưng **chưa có bằng chứng rằng trạng thái này đến từ thao tác ký quỹ thành công**.

```sql
USE [KETNOI_FREELANCERSV];
GO

SELECT maHD, sotienkyquy, trangThai, ngayBatDau
FROM dbo.HopDong
WHERE maHD = 'HD26101717415092';
```

Nếu hợp đồng đã `DangThucHien` mà chưa từng có giao dịch đóng băng ví, cần kiểm tra luồng tạo/xác nhận hợp đồng có chuyển trạng thái quá sớm hay không.

---

## Tổng kết

**Bước 1–7:** Hoàn thiện phần code nền tảng ký quỹ và đã Build thành công theo phản hồi trong quá trình làm việc.  
**Bước 8:** Kiểm tra cấu trúc Database thực tế; còn thiếu đối chiếu một số bảng.  
**Bước 9:** Chạy Web/API, xác định lỗi thiếu cột `dieuKhoan` và đưa trang danh sách hợp đồng trở lại trạng thái tải được.  

**Kết luận:** Đã triển khai đáng kể chức năng theo README, nhưng **chưa thể xác nhận hoàn thành toàn bộ yêu cầu nghiệp vụ** cho đến khi kiểm thử đầy đủ thay đổi số dư, đóng băng tiền, lịch sử giao dịch, thông báo và quyền API.

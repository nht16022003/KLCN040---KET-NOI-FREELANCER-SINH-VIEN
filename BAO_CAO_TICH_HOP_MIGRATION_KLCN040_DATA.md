# BÁO CÁO PHÂN TÍCH VÀ ĐỀ XUẤT TÍCH HỢP MIGRATION VÀO KLCN040_DATA.sql THEO SYNTAX GỐC

---

## 1. DANH SÁCH HAI TỆP MIGRATION ĐƯỢC YÊU CẦU

1. **Migration 1:** `FreelancerStudent.API/Database/Migrations/20261018_AddPortfolioProjectFeatured.sql`
   - **Kích thước:** 221 bytes (6 dòng).
   - **Mục tiêu ban đầu:** Bổ sung thuộc tính dự án nổi bật (`laDuAnNoiBat`) vào bảng `DuAn_Trong_Portfolio`.
2. **Migration 2:** `FreelancerStudent.API/Database/Migrations/20261022_HoanThienThanhToanDanhGia.sql`
   - **Kích thước:** 4,888 bytes (88 dòng).
   - **Mục tiêu ban đầu:** Bổ sung điều khoản hợp đồng, tạo bảng lưu lịch sử đánh giá, bổ sung đánh giá sao chi tiết, bổ sung điểm đánh giá trung bình & số lượt đánh giá, bổ sung thời gian hoàn thành hợp đồng và tính điểm rating ban đầu.

---

## 2. PHÂN TÍCH SYNTAX GỐC CỦA TỆP `KLCN040_DATA.sql`

Qua khảo sát toàn bộ 1,250 dòng của file `KLCN040_DATA.sql`, quy ước và phong cách viết SQL gốc được xác định như sau:

| Thành phần SQL | Quy ước và Phong cách trong `KLCN040_DATA.sql` | Ví dụ thực tế từ file |
| :--- | :--- | :--- |
| **Khai báo bảng** | Dùng cú pháp `CREATE TABLE <TenBang> (\n ... \n)\nGO`. Không sử dụng tiền tố schema `dbo.` | `CREATE TABLE DanhGia_NhanXet (...)` (Dòng 532) |
| **Primary Key** | Khai báo trực tiếp trên dòng định nghĩa cột (Inline PK), kết hợp `IDENTITY(1,1)` hoặc `IDENTITY` | `maDanhGia INT IDENTITY(1,1) PRIMARY KEY,` |
| **Foreign Key** | Khai báo ở cuối khối bảng với cú pháp:<br>`CONSTRAINT FK_<TenBang>_<BangThamChieu> FOREIGN KEY (<Cot>) REFERENCES <BangThamChieu>(<CotThamChieu>)` | `CONSTRAINT FK_DanhGia_HopDong FOREIGN KEY (maHD) REFERENCES HopDong(maHD),` |
| **Giá trị DEFAULT** | Khai báo nội dòng (Inline Default) ngay sau kiểu dữ liệu | `ngayDanhGia DATETIME DEFAULT GETDATE(),` |
| **CHECK Constraint**| Khai báo nội dòng hoặc dạng constraint cấp bảng có đặt tên `CK_<TenBang>_<Cot>` | `soSao INT NOT NULL CHECK (soSao BETWEEN 1 AND 5),` |
| **Dấu phân cách lệnh** | Sử dụng lệnh `GO` hoặc `Go` đứng riêng trên một dòng sau mỗi bảng | `GO` xuất hiện sau mỗi định nghĩa bảng |
| **Quy ước đặt tên** | Tên bảng và tên cột dùng chuẩn PascalCase hoặc camelCase tiếng Việt không dấu | `DanhGia_NhanXet`, `soSao`, `nhanXet`, `maDanhGia` |
| **Tạo Index** | Khai báo `CREATE [UNIQUE] INDEX <TenIndex> ON <TenBang>(<CacCot>);` kèm `GO` | `CREATE UNIQUE INDEX UX_HopDong_MaJob_Active ON dbo.HopDong(maJob)...` (Dòng 1095) |

> **Nguyên tắc chuyển đổi:** Tuyệt đối không sao chép nguyên xi cú pháp của Migration (vốn dùng `IF OBJECT_ID(...) IS NULL BEGIN ... END;`, `dbo.`, `CONSTRAINT PK_...`, `CONSTRAINT DF_...`). Mọi nội dung mới phải được viết lại chuẩn 100% theo syntax gốc nêu trên.

---

## 3. NỘI DUNG ĐƯỢC PHÉP TÍCH HỢP (THEO MỤC 3)

Theo quy định tại **Mục 3** (*Phạm vi được phép tích hợp: CREATE TABLE thông thường và CREATE INDEX*):

### 3.1. Bảng mới: `DanhGia_NhanXet_LichSu`
* **Nguồn gốc:** Tách từ Migration 2 (`20261022_HoanThienThanhToanDanhGia.sql`, dòng 31-45).
* **Bản chất:** Là một bảng hoàn toàn mới trong hệ thống, chưa tồn tại trong `KLCN040_DATA.sql`.
* **Cú pháp chuyển đổi sang Syntax gốc:**
```sql
CREATE TABLE DanhGia_NhanXet_LichSu
(
    maLichSu INT IDENTITY(1,1) PRIMARY KEY,
    maDanhGia INT NOT NULL,
    soSao INT NOT NULL,
    soSaoChatLuong TINYINT NULL,
    soSaoDungHan TINYINT NULL,
    soSaoGiaoTiep TINYINT NULL,
    nhanXet NVARCHAR(1000) NULL,
    ngayLuu DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DanhGia_NhanXet_LichSu_DanhGia FOREIGN KEY (maDanhGia) REFERENCES DanhGia_NhanXet(maDanhGia) ON DELETE CASCADE
)
GO
```
* **Đánh giá tính hợp lệ:**
  - Không sửa bất kỳ bảng cũ nào.
  - Khóa ngoại tham chiếu đúng đến `DanhGia_NhanXet(maDanhGia)` (bảng này đã tồn tại trước đó).
  - Sử dụng Inline PK, Inline Default `GETDATE()`, đặt tên FK chuẩn `FK_DanhGia_NhanXet_LichSu_DanhGia`.

### 3.2. Chỉ mục mới (Index): `UX_DanhGia_NhanXet_maHD_maNguoiDanhGia` & `IX_DanhGia_NhanXet_maNguoiDuocDanhGia`
* **Nguồn gốc:** Migration 2, dòng 26-29.
* **Bản chất:** Chỉ mục tăng tốc tìm kiếm và đảm bảo mỗi người chỉ đánh giá 1 lần trên 1 hợp đồng. Các cột tham gia index (`maHD`, `maNguoiDanhGia`, `maNguoiDuocDanhGia`) đều đã có sẵn trong bảng `DanhGia_NhanXet`.
* **Cú pháp chuyển đổi sang Syntax gốc:**
```sql
CREATE UNIQUE INDEX UX_DanhGia_NhanXet_maHD_maNguoiDanhGia ON DanhGia_NhanXet(maHD, maNguoiDanhGia);
CREATE INDEX IX_DanhGia_NhanXet_maNguoiDuocDanhGia ON DanhGia_NhanXet(maNguoiDuocDanhGia);
GO
```

### 3.3. Bổ sung trong khối DROP TABLE cuối file:
Để duy trì tính toàn vẹn khi chạy khối xóa bảng cuối file `KLCN040_DATA.sql`, bổ sung lệnh xóa bảng phụ thuộc trước bảng cha:
```sql
-- Tại nhóm 3 (dòng 1182):
DROP TABLE IF EXISTS DanhGia_NhanXet_LichSu;
DROP TABLE IF EXISTS DanhGia_NhanXet;
```

---

## 4. NỘI DUNG TUYỆT ĐỐI KHÔNG THỂ TÍCH HỢP VÀ LÝ DO (THEO MỤC 4)

Tuân thủ nghiêm ngặt **Mục 4** (*Không ALTER TABLE, không thêm/xóa cột bảng hiện có, không UPDATE dữ liệu hiện có*), các nội dung sau **bắt buộc giữ nguyên ở danh sách chưa tích hợp**:

| STT | Câu lệnh trong Migration | Bảng liên quan | Lý do không tích hợp theo Mục 4 |
| :---: | :--- | :--- | :--- |
| **1** | `ALTER TABLE DuAn_Trong_Portfolio ADD laDuAnNoiBat...` | `DuAn_Trong_Portfolio` | Yêu cầu `ALTER TABLE` và thêm cột vào bảng hiện có. Vi phạm Mục 4. |
| **2** | `ALTER TABLE HopDong ADD dieuKhoan...` | `HopDong` | **Đã có sẵn** tại dòng 1079-1083 của `KLCN040_DATA.sql` (bỏ qua để tránh trùng lặp). |
| **3** | `ALTER TABLE DanhGia_NhanXet ADD soSaoChatLuong, soSaoDungHan, soSaoGiaoTiep, ngayCapNhat, soLanChinhSua` | `DanhGia_NhanXet` | Yêu cầu `ALTER TABLE` và thêm 5 cột vào bảng hiện có. Vi phạm Mục 4. |
| **4** | `ALTER TABLE DanhGia_NhanXet ADD CONSTRAINT CK_...` | `DanhGia_NhanXet` | Yêu cầu `ALTER TABLE` thêm Check constraint vào bảng hiện có. Vi phạm Mục 4. |
| **5** | `ALTER TABLE FreelancerStudents ADD diemDanhGiaTrungBinh, soLuotDanhGia` | `FreelancerStudents` | Yêu cầu `ALTER TABLE` và thêm 2 cột vào bảng hiện có. Vi phạm Mục 4. |
| **6** | `ALTER TABLE NhaTuyenDung ADD soLuotDanhGia` | `NhaTuyenDung` | Yêu cầu `ALTER TABLE` và thêm 1 cột vào bảng hiện có. Vi phạm Mục 4. |
| **7** | `ALTER TABLE HopDong ADD ngayHoanThanh, ngayDongHopDong, thanhToanTuDong` | `HopDong` | Yêu cầu `ALTER TABLE` và thêm 3 cột vào bảng hiện có. Vi phạm Mục 4. |
| **8** | `;WITH FreelancerRating AS (...) UPDATE f SET ...`<br>`;WITH EmployerRating AS (...) UPDATE n SET ...` | `FreelancerStudents`, `NhaTuyenDung` | Yêu cầu `UPDATE` dữ liệu hiện có. Vi phạm Mục 4. |

---

## 5. VỊ TRÍ BỔ SUNG TRONG `KLCN040_DATA.sql`

Để đảm bảo thứ tự quan hệ phụ thuộc hợp lệ (Foreign Key dependency):
1. **Bảng `DanhGia_NhanXet`** kết thúc tại dòng **546** (`GO`).
2. **Bảng `ThongBao`** bắt đầu tại dòng **547**.
3. **Vị trí đề xuất chèn nội dung mới:**
   * Chèn ngay sau dòng **546** (sau `DanhGia_NhanXet`) và trước dòng **547** (trước `ThongBao`).
   * Tại vị trí này, bảng cha `DanhGia_NhanXet` đã được tạo xong, giúp bảng con `DanhGia_NhanXet_LichSu` và 2 Index tham chiếu khóa ngoại hoàn toàn hợp lệ mà không phát sinh bất kỳ lỗi biên dịch nào.

---

## 6. KẾT QUẢ KIỂM TRA RÀNG BUỘC BẢO TOÀN (CHECKLIST)

| Tiêu chuẩn kiểm tra | Trạng thái | Ghi chú cam kết |
| :--- | :---: | :--- |
| **Không dùng ALTER TABLE** | **ĐẠT** | 100% không phát sinh lệnh `ALTER TABLE`. |
| **Không thêm/xóa cột bảng hiện có** | **ĐẠT** | Toàn bộ 40 bảng hiện có giữ nguyên 100% số cột ban đầu. |
| **Không có INSERT / UPDATE / DELETE / TRUNCATE** | **ĐẠT** | Tuyệt đối không thay đổi, cập nhật hay chèn dữ liệu. |
| **Không làm mất nội dung SQL ban đầu** | **ĐẠT** | Toàn bộ 1,250 dòng hiện tại của file được giữ nguyên vẹn. |
| **Giữ nguyên syntax gốc** | **ĐẠT** | Viết theo phong cách chuẩn của file đích (Inline PK, Inline Default, `GO`). |
| **Thứ tự phụ thuộc khóa ngoại** | **ĐẠT** | Bảng con nằm ngay sau bảng cha, đảm bảo chạy tuần tự thành công. |

---

## 7. KẾ HOẠCH BƯỚC TIẾP THEO (GIAI ĐOẠN CHỜ XÁC NHẬN)

Theo đúng quy định tại Bước 5 & Bước 6, tôi **chưa thực hiện chỉnh sửa** vào file `KLCN040_DATA.sql`.

Sau khi bạn xem xét và **xác nhận đồng ý phương án trên**, tôi sẽ thực hiện:
1. Sao lưu file gốc thành `Database/KLCN040_DATA.sql.bak`.
2. Chèn bảng `DanhGia_NhanXet_LichSu` và 2 Index theo đúng syntax gốc vào vị trí dòng 546.
3. Bổ sung `DROP TABLE IF EXISTS DanhGia_NhanXet_LichSu;` vào dòng 1182 trong khối DROP TABLE.
4. Kiểm tra lại toàn bộ file để đảm bảo tính nguyên vẹn.


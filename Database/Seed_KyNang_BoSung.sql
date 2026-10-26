-- ==========================================================================================
-- SCRIPT BỔ SUNG DANH MỤC KỸ NĂNG VÀO DATABASE KETNOI_FREELANCERSV
-- Tuân thủ nghiêm ngặt nguyên tắc:
--   1. Chỉ INSERT vào bảng KyNang hiện có (không tạo bảng mới, không thêm cột).
--   2. Kiểm tra trùng lặp trước khi thêm (so sánh không phân biệt chữ hoa/thường, cắt khoảng trắng).
--   3. Không chỉ định IDENTITY thủ công.
--   4. An toàn khi chạy lại nhiều lần (Idempotent).
-- ==========================================================================================

USE [KETNOI_FREELANCERSV];
GO

PRINT N'Bắt đầu kiểm tra và bổ sung danh mục kỹ năng chuẩn...';

DECLARE @DanhSachKyNang TABLE (
    tenKyNang NVARCHAR(50)
);

INSERT INTO @DanhSachKyNang (tenKyNang) VALUES
-- ==========================================
-- A. THIẾT KẾ ĐỒ HỌA & UI/UX
-- ==========================================
(N'Adobe Illustrator'),
(N'Thiết kế logo'),
(N'Thiết kế banner'),
(N'Thiết kế poster'),
(N'Bộ nhận diện thương hiệu'),
(N'Thiết kế menu'),
(N'CorelDRAW'),
(N'Adobe InDesign'),
(N'UI Design'),
(N'UX Design'),
(N'UI/UX Design'),

-- ==========================================
-- B. LẬP TRÌNH WEB & BACKEND
-- ==========================================
(N'ASP.NET Core'),
(N'C#'),
(N'HTML/CSS'),
(N'JavaScript'),
(N'TypeScript'),
(N'ReactJS'),
(N'Angular'),
(N'Vue.js'),
(N'Node.js'),
(N'PHP'),
(N'Laravel'),
(N'MySQL'),
(N'REST API'),

-- ==========================================
-- C. LẬP TRÌNH MOBILE
-- ==========================================
(N'Dart'),
(N'React Native'),
(N'Kotlin'),
(N'Java Android'),
(N'Swift'),
(N'Firebase'),

-- ==========================================
-- D. DATA & TRÍ TUỆ NHÂN TẠO (AI)
-- ==========================================
(N'Python'),
(N'SQL'),
(N'Power BI'),
(N'Data Analysis'),
(N'Machine Learning'),
(N'Deep Learning'),

-- ==========================================
-- E. MARKETING & NỘI DUNG (CONTENT)
-- ==========================================
(N'Content Writing'),
(N'Copywriting'),
(N'SEO'),
(N'Digital Marketing'),
(N'Social Media Marketing'),
(N'Facebook Ads'),
(N'Google Ads'),
(N'Quản lý Fanpage'),
(N'Biên tập nội dung'),

-- ==========================================
-- F. DỊCH THUẬT & NGÔN NGỮ
-- ==========================================
(N'Dịch thuật Anh - Việt'),
(N'Dịch thuật Việt - Anh'),
(N'Tiếng Anh'),
(N'Tiếng Trung'),
(N'Viết nội dung tiếng Anh'),

-- ==========================================
-- G. VIDEO, HÌNH ẢNH & ĐA PHƯƠNG TIỆN
-- ==========================================
(N'Adobe Premiere Pro'),
(N'Adobe After Effects'),
(N'CapCut'),
(N'Video Editing'),
(N'Motion Graphics'),
(N'Chụp ảnh sản phẩm'),
(N'Chỉnh sửa ảnh'),

-- ==========================================
-- H. VĂN PHÒNG & NGHIỆP VỤ
-- ==========================================
(N'Microsoft Word'),
(N'Microsoft Excel'),
(N'Microsoft PowerPoint'),
(N'Nhập liệu'),
(N'Kiểm thử phần mềm'),
(N'Phân tích nghiệp vụ (BA)');

-- Số lượng kỹ năng trước khi chèn
DECLARE @SoLuongTruoc INT;
SELECT @SoLuongTruoc = COUNT(*) FROM KyNang;

-- Thực hiện chèn an toàn các kỹ năng chưa tồn tại
INSERT INTO KyNang (tenKyNang)
SELECT LTRIM(RTRIM(src.tenKyNang))
FROM @DanhSachKyNang src
WHERE NOT EXISTS (
    SELECT 1 
    FROM KyNang k 
    WHERE LOWER(LTRIM(RTRIM(k.tenKyNang))) = LOWER(LTRIM(RTRIM(src.tenKyNang)))
);

-- Số lượng kỹ năng sau khi chèn
DECLARE @SoLuongSau INT;
SELECT @SoLuongSau = COUNT(*) FROM KyNang;

PRINT N'===========================================================';
PRINT N'Số kỹ năng ban đầu: ' + CAST(@SoLuongTruoc AS NVARCHAR(10));
PRINT N'Số kỹ năng được bổ sung mới: ' + CAST((@SoLuongSau - @SoLuongTruoc) AS NVARCHAR(10));
PRINT N'Tổng số kỹ năng hiện có trong CSDL: ' + CAST(@SoLuongSau AS NVARCHAR(10));
PRINT N'Bổ sung danh mục kỹ năng hoàn tất an toàn!';
PRINT N'===========================================================';
GO

-- Truy vấn kiểm tra danh mục kỹ năng
SELECT maKyNang, tenKyNang 
FROM KyNang 
ORDER BY maKyNang ASC;
GO

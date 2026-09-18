CREATE DATABASE QL_Freelancer;
GO

USE QL_Freelancer;
GO

-- =========================================================
-- 1. USERS
-- =========================================================
CREATE TABLE Users (
    userID VARCHAR(20) PRIMARY KEY,
    userName NVARCHAR(100) NOT NULL,

    -- CODE CŨ:
    -- pashwordHash VARCHAR(255) NOT NULL,
    -- LÝ DO SỬA: Tên cột bị sai chính tả "pashwordHash".
    -- CODE MỚI:
    passwordHash VARCHAR(255) NOT NULL,

    email VARCHAR(100) UNIQUE NOT NULL,
    phoneNumber VARCHAR(15),

    -- CODE CŨ:
    -- loaiUser NVARCHAR(20) CHECK (loaiUser IN (N'KhachHang', N'FreelancerSV', N'Admin'))
    -- LÝ DO SỬA: Mỗi tài khoản bắt buộc phải xác định loại người dùng, tránh NULL.
    -- CODE MỚI:
    loaiUser NVARCHAR(20) NOT NULL
        CHECK (loaiUser IN (N'KhachHang', N'FreelancerSV', N'Admin')),

    -- CODE CŨ:
    -- status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE'
    --     CHECK (status IN ('ACTIVE', 'LOCKED', 'INACTIVE')),
    -- GHI CHÚ SỬA:
    -- Bổ sung SUSPENDED để phân biệt Tạm khóa với LOCKED (Đã khóa) và INACTIVE (Ngừng hoạt động).
    -- CODE MỚI:
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE'
        CHECK (status IN ('ACTIVE', 'SUSPENDED', 'LOCKED', 'INACTIVE')),

    -- CODE MỚI:
    -- LÝ DO THÊM: Theo dõi thời điểm tài khoản được tạo.
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    -- CODE MỚI:
    -- LÝ DO THÊM: Theo dõi lần cập nhật gần nhất khi Admin chỉnh sửa tài khoản.
    updatedAt DATETIME NULL
);
GO

-- =========================================================
-- 2. USER AUDIT LOG
-- =========================================================
-- CODE MỚI:
-- LÝ DO THÊM: AD-UC-01 có thao tác Admin cập nhật thông tin tài khoản.
-- Bảng này lưu ai sửa, sửa tài khoản nào, trường nào, giá trị cũ/mới và lý do.
CREATE TABLE UserAuditLog (
    auditID INT IDENTITY(1,1) PRIMARY KEY,
    adminUserID VARCHAR(20) NOT NULL,
    targetUserID VARCHAR(20) NOT NULL,
    actionType VARCHAR(30) NOT NULL
        CHECK (actionType IN ('UPDATE', 'LOCK', 'UNLOCK')),
    fieldName VARCHAR(100),
    oldValue NVARCHAR(MAX),
    newValue NVARCHAR(MAX),
    reason NVARCHAR(500),
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_UserAudit_Admin
        FOREIGN KEY (adminUserID) REFERENCES Users(userID),

    CONSTRAINT FK_UserAudit_Target
        FOREIGN KEY (targetUserID) REFERENCES Users(userID)
);
GO

-- =========================================================
-- 3. ADMIN
-- =========================================================
CREATE TABLE Admin (
    userID VARCHAR(20) PRIMARY KEY,

    -- CODE CŨ:
    -- adminRole INT DEFAULT 1,
    -- LÝ DO SỬA: Kiểu INT không thể hiện rõ tên quyền và không phù hợp với dữ liệu 'SUPER_ADMIN'.
    -- CODE MỚI:
    adminRole VARCHAR(30) NOT NULL DEFAULT 'ADMIN'
        CHECK (adminRole IN ('ADMIN', 'SUPER_ADMIN')),

    CONSTRAINT FK_Admin_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO

-- =========================================================
-- 4. KHACH HANG
-- =========================================================
CREATE TABLE KhachHang (
    cusID VARCHAR(20) PRIMARY KEY,

    -- CODE CŨ:
    -- userID VARCHAR(20),
    -- LÝ DO SỬA: Một hồ sơ khách hàng phải thuộc đúng một User và mỗi User chỉ có một hồ sơ khách hàng.
    -- CODE MỚI:
    userID VARCHAR(20) UNIQUE NOT NULL,

    rating FLOAT,

    -- CODE CŨ:
    -- walletID VARCHAR(20),
    -- LÝ DO SỬA: Wallet đã liên kết trực tiếp với Users bằng userID.
    -- Nếu giữ thêm walletID trong KhachHang sẽ dư dữ liệu và có nguy cơ không đồng bộ.

    CONSTRAINT FK_KhachHang_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO

-- =========================================================
-- 5. FREELANCER SINH VIEN
-- =========================================================
CREATE TABLE FreelancerSV (
    free_ID VARCHAR(20) PRIMARY KEY NOT NULL,

    -- CODE CŨ:
    -- userID VARCHAR(20) NOT NULL,
    -- LÝ DO SỬA: Mỗi User Freelancer chỉ nên có một hồ sơ FreelancerSV.
    -- CODE MỚI:
    userID VARCHAR(20) UNIQUE NOT NULL,
	birth DATETIME,
	gender NCHAR(10),
	address NVARCHAR(100),

    university NVARCHAR(150),
	AcademicYearStart INT,
	AcademicYearEnd  INT,

    -- CODE CŨ:
    -- major VARCHAR(20),
    -- LÝ DO SỬA: Tên chuyên ngành có thể dài và cần hỗ trợ tiếng Việt.
    -- CODE MỚI:
    major NVARCHAR(100),
	--Thuoc tinh them


    studentCardID VARCHAR(20),
    GPA FLOAT,

    CONSTRAINT FK_Freelancer_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO

-- =========================================================
-- 6. WALLET
-- =========================================================
CREATE TABLE Wallet (
    walletID VARCHAR(20) PRIMARY KEY,
    userID VARCHAR(20) UNIQUE NOT NULL,

    -- CODE CŨ:
    -- soDuKhadung DECIMAL(18,2) DEFAULT 0,
    -- soDuDongBang DECIMAL(18,2) DEFAULT 0,
    -- LÝ DO SỬA: Số dư không nên NULL.
    -- CODE MỚI:
    soDuKhadung DECIMAL(18,2) NOT NULL DEFAULT 0,
    soDuDongBang DECIMAL(18,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_Wallet_User
        FOREIGN KEY (userID) REFERENCES Users(userID)
);
GO

-- =========================================================
-- 7. PORTFOLIO
-- =========================================================
CREATE TABLE Portfolio (
    portfolioID VARCHAR(20) PRIMARY KEY,
    free_ID VARCHAR(20) UNIQUE NOT NULL,
    moTaBanThan NVARCHAR(MAX),
    url_video VARCHAR(255),

    CONSTRAINT FK_Portfolio_Freelancer
        FOREIGN KEY (free_ID) REFERENCES FreelancerSV(free_ID)
);
GO

-- =========================================================
-- 8. DU AN PORTFOLIO
-- =========================================================
CREATE TABLE DuAnPortfolio (
    maDA VARCHAR(20) PRIMARY KEY,
    portfolioID VARCHAR(20) NOT NULL,
    tenDuAn NVARCHAR(200) NOT NULL,
    moTa NVARCHAR(MAX),
    vaiTro NVARCHAR(50),
    congnghe NVARCHAR(50),
    linkGithub VARCHAR(255),
    linkDemo VARCHAR(255),
    link_file VARCHAR(255),

    CONSTRAINT FK_DuAn_Portfolio
        FOREIGN KEY (portfolioID) REFERENCES Portfolio(portfolioID) ON DELETE CASCADE
);
GO

-- =========================================================
-- 9. JOB POST
-- =========================================================
CREATE TABLE JobPost (
    jobID VARCHAR(20) PRIMARY KEY,
    cusID VARCHAR(20) NOT NULL,
    title NVARCHAR(200) NOT NULL,
    descr NVARCHAR(MAX),

    -- CODE CŨ:
    -- thulao FLOAT,
    -- LÝ DO SỬA: Dữ liệu tiền không nên dùng FLOAT vì có thể gây sai số làm tròn.
    -- CODE MỚI:
    thulao DECIMAL(18,2),

    thoigianthuchien DATETIME,
    status NVARCHAR(30) DEFAULT N'DangTuyen',
    soluongtuyen INT,

    CONSTRAINT FK_JobPost_KhachHang
        FOREIGN KEY (cusID) REFERENCES KhachHang(cusID)
);
GO

-- =========================================================
-- 10. HOP DONG
-- =========================================================
CREATE TABLE HopDong (
    maHD VARCHAR(20) PRIMARY KEY,

    -- CODE CŨ:
    -- jobID VARCHAR(20) UNIQUE NOT NULL,
    -- GHI CHÚ SỬA:
    -- JobPost có soluongtuyen nên một Job có thể phát sinh nhiều hợp đồng cho nhiều Freelancer.
    -- CODE MỚI:
    jobID VARCHAR(20) NOT NULL,

    freeID VARCHAR(20) NOT NULL,
    ngayBatDau DATETIME DEFAULT GETDATE(),

    -- CODE CŨ:
    -- ngayKetThuc DATETIME,
    -- GHI CHÚ SỬA:
    -- Tách deadline dự kiến và thời điểm hoàn thành thực tế để tránh nhập nhằng.
    -- CODE MỚI:
    hanHoanThanh DATETIME NULL,
    completedAt DATETIME NULL,

    -- CODE CŨ:
    -- sotienkyquy FLOAT,
    -- GHI CHÚ SỬA: Tiền ký quỹ cần dùng DECIMAL để tránh sai số.
    -- CODE MỚI:
    sotienkyquy DECIMAL(18,2) NULL,

    trangThai NVARCHAR(30) NOT NULL DEFAULT N'DangThucHien',

    -- CODE MỚI: Bổ sung để khớp UI danh sách/chi tiết/form tạo hợp đồng.
    projectCode VARCHAR(30) NULL,
    contractType VARCHAR(20) NOT NULL DEFAULT 'FIXED_PRICE',
    tongGiaTri DECIMAL(18,2) NULL,
    tienDo INT NOT NULL DEFAULT 0,
    paymentStatus VARCHAR(20) NOT NULL DEFAULT 'UNPAID',
    paymentType VARCHAR(30) NOT NULL DEFAULT 'ONE_TIME',
    workMode VARCHAR(20) NULL,
    fieldName NVARCHAR(150) NULL,
    dieuKhoanChiTiet NVARCHAR(MAX) NULL,

    -- CODE MỚI: Quy trình đề xuất hợp đồng từ Chat.
    proposalStatus VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    proposedAt DATETIME NULL,
    acceptedAt DATETIME NULL,
    rejectedAt DATETIME NULL,
    rejectionReason NVARCHAR(500) NULL,

    -- CODE MỚI: Ghi nhận xác nhận điều khoản trên hệ thống.
    termsConfirmed BIT NOT NULL DEFAULT 0,
    termsConfirmedAt DATETIME NULL,

    cancelledAt DATETIME NULL,
    cancelReason NVARCHAR(500) NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL,

    CONSTRAINT FK_HopDong_Job
        FOREIGN KEY (jobID) REFERENCES JobPost(jobID),

    -- CODE CŨ:
    -- CONSTRAINT FK_HopDong_Freelancer FOREIGN KEY (freeID) REFERENCES FreelancerSV(freeID)
    -- GHI CHÚ SỬA: Khóa chính đúng của FreelancerSV là free_ID.
    -- CODE MỚI:
    CONSTRAINT FK_HopDong_Freelancer
        FOREIGN KEY (freeID) REFERENCES FreelancerSV(free_ID),

    -- CODE MỚI: Không cho cùng một Freelancer có hai hợp đồng cho cùng một Job.
    CONSTRAINT UQ_HopDong_Job_Freelancer UNIQUE (jobID, freeID),
    CONSTRAINT CK_HopDong_TongGiaTri CHECK (tongGiaTri IS NULL OR tongGiaTri >= 0),
    CONSTRAINT CK_HopDong_SoTienKyQuy CHECK (sotienkyquy IS NULL OR sotienkyquy >= 0),
    CONSTRAINT CK_HopDong_TienDo CHECK (tienDo BETWEEN 0 AND 100),
    CONSTRAINT CK_HopDong_ContractType CHECK (contractType IN ('FIXED_PRICE', 'HOURLY')),
    CONSTRAINT CK_HopDong_PaymentStatus CHECK (paymentStatus IN ('UNPAID', 'ESCROWED', 'PENDING', 'PAID', 'REFUNDED')),
    CONSTRAINT CK_HopDong_PaymentType CHECK (paymentType IN ('ONE_TIME', 'MILESTONE', 'TIME_BASED')),
    CONSTRAINT CK_HopDong_WorkMode CHECK (workMode IS NULL OR workMode IN ('REMOTE', 'ONSITE', 'HYBRID')),
    CONSTRAINT CK_HopDong_ProposalStatus CHECK (proposalStatus IN ('DRAFT', 'SENT', 'ACCEPTED', 'REJECTED', 'CANCELLED')),
    CONSTRAINT CK_HopDong_TermsConfirmedTime CHECK (
           (termsConfirmed = 0 AND termsConfirmedAt IS NULL)
        OR (termsConfirmed = 1 AND termsConfirmedAt IS NOT NULL)
    )
);
GO

-- =========================================================
-- 11. DANH GIA
-- =========================================================
CREATE TABLE DanhGia (
    maDG VARCHAR(20) PRIMARY KEY,
    -- CODE CŨ:
    -- maHD VARCHAR(20) UNIQUE NOT NULL,
    -- GHI CHÚ SỬA:
    -- Một hợp đồng có thể có đánh giá từ cả hai phía; không UNIQUE riêng maHD.
    -- CODE MỚI:
    maHD VARCHAR(20) NOT NULL,
    nguoiDanhGiaID VARCHAR(20) NOT NULL,
    nguoiDuocDanhGiaID VARCHAR(20) NOT NULL,
    soSao INT CHECK (soSao BETWEEN 1 AND 5),
    nhanXet NVARCHAR(MAX),
    ngayDanhGia DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_DanhGia_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD),

    CONSTRAINT FK_DanhGia_NguoiGui
        FOREIGN KEY (nguoiDanhGiaID) REFERENCES Users(userID),

    CONSTRAINT FK_DanhGia_NguoiNhan
        FOREIGN KEY (nguoiDuocDanhGiaID) REFERENCES Users(userID),

    -- CODE MỚI: Mỗi người chỉ đánh giá một lần trên cùng hợp đồng.
    CONSTRAINT UQ_DanhGia_HopDong_NguoiDanhGia UNIQUE (maHD, nguoiDanhGiaID)
);
GO


-- =========================================================
-- 12. INSERT DU LIEU
-- =========================================================

-- ---------------------------------------------------------
-- 12.1 USERS
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO User (userID, userName, pashwordHash, email, phoneNumber, status, createdAt, loaiUser) VALUES
-- ('USR001', 'admin_sys', 'hash_pass_123', 'admin@freelancer.vn', '0901111111', 'ACTIVE', GETDATE(), 'ADMIN'),
-- ('USR002', 'khachhang_a', 'hash_pass_456', 'tuandaubo@gmail.com', '0902222222', 'ACTIVE', GETDATE(), 'CUS'),
-- ('USR003', 'freelancer_sv1', 'hash_pass_789', 'nguyenvanSon@student.edu.vn', '0903333333', 'ACTIVE', GETDATE(), 'FREELC');
-- LÝ DO SỬA:
-- 1. Tên bảng đúng là Users, không phải User.
-- 2. pashwordHash đổi thành passwordHash.
-- 3. Bảng Users mới có status, createdAt.
-- 4. loaiUser phải đúng CHECK: Admin, KhachHang, FreelancerSV.
-- 5. Thứ tự cột và giá trị trong code cũ không khớp nhau.

-- CODE MỚI:
INSERT INTO Users
(userID, userName, passwordHash, email, phoneNumber, status, createdAt, loaiUser)
VALUES
('USR001', N'admin_sys', 'hash_pass_123', 'admin@freelancer.vn', '0901111111', 'ACTIVE', GETDATE(), N'Admin'),
('USR002', N'khachhang_a', 'hash_pass_456', 'tuandaubo@gmail.com', '0902222222', 'ACTIVE', GETDATE(), N'KhachHang'),
('USR003', N'freelancer_sv1', 'hash_pass_789', 'nguyenvanSon@student.edu.vn', '0903333333', 'ACTIVE', GETDATE(), N'FreelancerSV');
GO

-- ---------------------------------------------------------
-- 12.2 ADMIN
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO Admin (userID, adminRole) VALUES
-- ('USR001', 'SUPER_ADMIN');
-- LÝ DO SỬA: Câu INSERT này trước đây không phù hợp vì adminRole là INT.
-- Sau khi đổi adminRole sang VARCHAR(30), dữ liệu SUPER_ADMIN là hợp lệ.

-- CODE MỚI:
INSERT INTO Admin (userID, adminRole)
VALUES ('USR001', 'SUPER_ADMIN');
GO

-- ---------------------------------------------------------
-- 12.3 FREELANCER SINH VIEN
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO FreelancerSV (userID, university, major, studentCardID, GPA) VALUES
-- ('USR003', N'Đại học Bách Khoa', N'Công nghệ thông tin', 'SV20230001', 3.6);
-- LÝ DO SỬA: free_ID là PRIMARY KEY NOT NULL nên khi INSERT bắt buộc phải truyền giá trị.

-- CODE MỚI:
INSERT INTO FreelancerSV
(free_ID, userID, university, major, studentCardID, GPA)
VALUES
('FREE001', 'USR003', N'Đại học Bách Khoa', N'Công nghệ thông tin', 'SV20230001', 3.6);
GO

-- ---------------------------------------------------------
-- 12.4 KHACH HANG
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO KhachHang (userID, cusID, rating, walletID) VALUES
-- ('USR002', 'CUS001', 4.8, NULL);
-- LÝ DO SỬA: walletID đã được bỏ khỏi KhachHang vì Wallet liên kết trực tiếp với Users bằng userID.

-- CODE MỚI:
INSERT INTO KhachHang (userID, cusID, rating)
VALUES ('USR002', 'CUS001', 4.8);
GO

-- ---------------------------------------------------------
-- 12.5 WALLET
-- ---------------------------------------------------------

INSERT INTO Wallet (walletID, userID, soDuKhadung, soDuDongBang)
VALUES
('WAL001', 'USR002', 10000000.00, 2000000.00),
('WAL002', 'USR003', 1500000.00, 0.00);
GO

-- CODE CŨ:
-- UPDATE KhachHang SET walletID = 'WAL001' WHERE cusID = 'CUS001';
-- LÝ DO SỬA: KhachHang không còn cột walletID; quan hệ với ví được xác định qua Users.userID -> Wallet.userID.
-- CODE MỚI: Không cần UPDATE walletID nữa.

-- ---------------------------------------------------------
-- 12.6 JOB POST
-- ---------------------------------------------------------

INSERT INTO JobPost
(jobID, cusID, title, descr, thulao, thoigianthuchien, status, soluongtuyen)
VALUES
('JOB001', 'CUS001', N'Thiết kế giao diện Website bằng Figma',
 N'Cần thiết kế 5 màn hình ứng dụng web', 2000000.00,
 DATEADD(day, 7, GETDATE()), N'Đang tuyển', 1);
GO

-- ---------------------------------------------------------
-- 12.7 HOP DONG
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO HopDong (maHD, jobID, freeID, soTienKyQuy, trangThai, ngayBatDau, ngayKetThuc) VALUES
-- ('HD001', 'JOB001', 'USR003', 2000000.00, N'Đang thực hiện', GETDATE(), DATEADD(day, 10, GETDATE()));
-- LÝ DO SỬA:
-- 1. freeID là FK tới FreelancerSV.free_ID nên phải dùng FREE001, không dùng USR003.
-- 2. Tên cột trong bảng là sotienkyquy, cần thống nhất tên.

-- CODE MỚI:
INSERT INTO HopDong
(maHD, jobID, freeID, sotienkyquy, trangThai, ngayBatDau, ngayKetThuc)
VALUES
('HD001', 'JOB001', 'FREE001', 2000000.00,
 N'Đang thực hiện', GETDATE(), DATEADD(day, 10, GETDATE()));
GO

-- ---------------------------------------------------------
-- 12.8 DANH GIA
-- ---------------------------------------------------------

INSERT INTO DanhGia
(maDG, maHD, nguoiDanhGiaID, nguoiDuocDanhGiaID, soSao, nhanXet, ngayDanhGia)
VALUES
('DG001', 'HD001', 'USR002', 'USR003', 5,
 N'Sinh viên làm việc rất nhanh và đúng deadline.', GETDATE());
GO

-- ---------------------------------------------------------
-- 12.9 PORTFOLIO
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO Portfolio (portfolioID, free_ID, moTaBanThan, url_Video) VALUES
-- ('HS001', 'USR003', N'Hồ sơ năng lực Lập trình & Thiết kế', 'https://youtube.com/demo_portfolio');
-- LÝ DO SỬA:
-- 1. free_ID là FK tới FreelancerSV.free_ID nên phải dùng FREE001.
-- 2. Thống nhất tên cột thành url_video theo khai báo bảng.

-- CODE MỚI:
INSERT INTO Portfolio
(portfolioID, free_ID, moTaBanThan, url_video)
VALUES
('HS001', 'FREE001', N'Hồ sơ năng lực Lập trình & Thiết kế',
 'https://youtube.com/demo_portfolio');
GO

-- ---------------------------------------------------------
-- 12.10 DU AN PORTFOLIO
-- ---------------------------------------------------------

-- CODE CŨ:
-- INSERT INTO DuAnPortfolio (maDA, portfolioID, tenDuAn, moTa, vaiTro, congNghe, linkGithub, linkDemo, link_file) VALUES
-- ('DA001', 'HS001', N'Ứng dụng Quản lý Chi tiêu', N'App Flutter di động quản lý tài chính', N'Fullstack Developer', 'Flutter, Dart, Firebase', 'https://github.com/demo/app', 'https://demo.app', 'https://file.app/spec.pdf');
-- LÝ DO SỬA: Thống nhất cách viết tên cột congNghe thành congnghe theo CREATE TABLE.

-- CODE MỚI:
INSERT INTO DuAnPortfolio
(maDA, portfolioID, tenDuAn, moTa, vaiTro, congnghe, linkGithub, linkDemo, link_file)
VALUES
('DA001', 'HS001', N'Ứng dụng Quản lý Chi tiêu',
 N'App Flutter di động quản lý tài chính',
 N'Fullstack Developer', N'Flutter, Dart, Firebase',
 'https://github.com/demo/app', 'https://demo.app', 'https://file.app/spec.pdf');
GO

-- =========================================================
-- 13. VI DU AD-UC-01: ADMIN CAP NHAT THONG TIN TAI KHOAN
-- =========================================================

-- CODE CŨ:
-- Chưa có thao tác UPDATE Users kèm ghi lịch sử thay đổi.
-- LÝ DO THÊM: AD-UC-01 yêu cầu Admin có thể cập nhật thông tin tài khoản và hệ thống lưu kết quả.

-- CODE MỚI:
-- Bước 1: Cập nhật dữ liệu thật
UPDATE Users
SET phoneNumber = '0908888888',
    updatedAt = GETDATE()
WHERE userID = 'USR002';

-- Bước 2: Ghi lịch sử thay đổi
INSERT INTO UserAuditLog
(adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
VALUES
('USR001', 'USR002', 'UPDATE', 'phoneNumber',
 '0902222222', '0908888888', N'Cập nhật số điện thoại khách hàng');
GO

-- =========================================================
-- 14. CAC TRUY VAN PHUC VU AD-UC-01
-- =========================================================

-- 14.1 Xem danh sách Khách hàng và Freelancer
SELECT
    u.userID,
    u.userName,
    u.email,
    u.phoneNumber,
    u.loaiUser,
    u.status,
    u.createdAt,
    u.updatedAt,
    kh.cusID,
    kh.rating,
    fr.free_ID,
    fr.university,
    fr.major,
    fr.studentCardID,
    fr.GPA
FROM Users u
LEFT JOIN KhachHang kh ON u.userID = kh.userID
LEFT JOIN FreelancerSV fr ON u.userID = fr.userID
WHERE u.loaiUser IN (N'KhachHang', N'FreelancerSV');
GO

-- 14.2 Tìm kiếm tài khoản theo tên/email/SĐT
DECLARE @SearchKeyword NVARCHAR(100) = N'khachhang';

SELECT
    userID,
    userName,
    email,
    phoneNumber,
    loaiUser,
    status,
    createdAt,
    updatedAt
FROM Users
WHERE userName LIKE N'%' + @SearchKeyword + N'%'
   OR email LIKE '%' + @SearchKeyword + '%'
   OR phoneNumber LIKE '%' + @SearchKeyword + '%';
GO

-- 14.3 Lọc tài khoản theo loại User
SELECT userID, userName, email, phoneNumber, loaiUser, status
FROM Users
WHERE loaiUser = N'KhachHang';
GO

-- 14.4 Lọc tài khoản theo trạng thái
SELECT userID, userName, email, phoneNumber, loaiUser, status
FROM Users
WHERE status = 'ACTIVE';
GO

-- 14.5 Xem chi tiết Khách hàng
SELECT
    u.userID,
    u.userName,
    u.email,
    u.phoneNumber,
    u.status,
    u.createdAt,
    u.updatedAt,
    kh.cusID,
    kh.rating,
    w.walletID,
    w.soDuKhadung,
    w.soDuDongBang
FROM Users u
INNER JOIN KhachHang kh ON u.userID = kh.userID
LEFT JOIN Wallet w ON u.userID = w.userID
WHERE u.userID = 'USR002';
GO

-- 14.6 Xem chi tiết Freelancer
SELECT
    u.userID,
    u.userName,
    u.email,
    u.phoneNumber,
    u.status,
    u.createdAt,
    u.updatedAt,
    fr.free_ID,
    fr.university,
    fr.major,
    fr.studentCardID,
    fr.GPA,
    p.portfolioID,
    p.moTaBanThan,
    p.url_video,
    w.walletID,
    w.soDuKhadung,
    w.soDuDongBang
FROM Users u
INNER JOIN FreelancerSV fr ON u.userID = fr.userID
LEFT JOIN Portfolio p ON fr.free_ID = p.free_ID
LEFT JOIN Wallet w ON u.userID = w.userID
WHERE u.userID = 'USR003';
GO

-- 14.7 Xem lịch sử Admin chỉnh sửa tài khoản
SELECT
    al.auditID,
    al.adminUserID,
    adminUser.userName AS adminName,
    al.targetUserID,
    targetUser.userName AS targetUserName,
    al.actionType,
    al.fieldName,
    al.oldValue,
    al.newValue,
    al.reason,
    al.createdAt
FROM UserAuditLog al
INNER JOIN Users adminUser ON al.adminUserID = adminUser.userID
INNER JOIN Users targetUser ON al.targetUserID = targetUser.userID
ORDER BY al.createdAt DESC;
GO


-- =========================================================
-- 15. BO SUNG DATABASE DE KHOP VOI CAC UI DA CHOT
--     - UI HOP DONG CUA FREELANCER
--     - UI ADMIN QUAN LY DANH SACH FREELANCER SINH VIEN
--     - UI ADMIN QUAN LY DANH SACH NHA TUYEN DUNG
-- =========================================================
-- LUU Y QUAN TRONG:
-- 1. TOAN BO CODE CU O PHIA TREN DUOC GIU NGUYEN, KHONG XOA.
-- 2. CAC PHAN DUOI DAY CHI BO SUNG / MO RONG CAU TRUC DE PHUC VU UI.
-- 3. KHACHHANG TRONG CSDL CU DUOC HIEU LA TAI KHOAN PHIA THUE DICH VU.
--    UI MOI GOI DOI TUONG NAY LA "NHA TUYEN DUNG".
--    VI VAY KHONG XOA BANG KhachHang, MA TAO THEM HO SO NhaTuyenDung LIEN KET 1-1.
-- =========================================================


-- =========================================================
-- 15.1 MO RONG FREELANCERSV CHO UI ADMIN QUAN LY FREELANCER
-- =========================================================

-- CODE CU (GIU NGUYEN O PHAN CREATE TABLE FreelancerSV):
-- FreelancerSV chi co:
-- free_ID, userID, university, major, studentCardID, GPA
--
-- LY DO CAN BO SUNG:
-- UI Admin danh sach/chi tiet Freelancer hien thi them:
-- - Anh dai dien
-- - Trang thai xac minh sinh vien
-- - Ngay xac minh
-- - Nam hoc / nam bat dau / nam ket thuc
-- - Hinh thuc hoc
-- - Mo ta ban than ngan
-- Cac thong tin nay chua co trong database cu.

-- CODE MOI: BO SUNG CAC COT, KHONG XOA CAC COT CU
ALTER TABLE FreelancerSV ADD
    avatarUrl VARCHAR(500) NULL,
    verificationStatus VARCHAR(20) NOT NULL
        CONSTRAINT DF_Freelancer_VerificationStatus DEFAULT 'PENDING',
    verifiedAt DATETIME NULL,
    enrollmentYear INT NULL,
    graduationYear INT NULL,
    studyMode NVARCHAR(50) NULL,
    bio NVARCHAR(1000) NULL;
GO

-- CODE MOI:
-- LY DO THEM CHECK: Gioi han trang thai xac minh de UI hien thi dung nhan.
ALTER TABLE FreelancerSV ADD CONSTRAINT CK_Freelancer_VerificationStatus
CHECK (verificationStatus IN ('PENDING', 'VERIFIED', 'REJECTED'));
GO


-- =========================================================
-- 15.2 BANG NHA TUYEN DUNG
-- =========================================================

-- CODE CU (GIU NGUYEN):
-- Bang KhachHang hien tai chi co cusID, userID, rating.
--
-- LY DO KHONG XOA / DOI TEN KhachHang:
-- JobPost va du lieu cu dang tham chieu KhachHang.cusID.
-- Neu doi ten truc tiep se lam vo FK va cac INSERT cu.
--
-- CODE MOI:
-- Tao bang NhaTuyenDung nhu mot HO SO MO RONG 1-1 cua KhachHang.
-- Bang nay phuc vu truc tiep UI Admin -> Quan ly nguoi dung -> Nha tuyen dung.
CREATE TABLE NhaTuyenDung (
    employerID VARCHAR(20) PRIMARY KEY,
    cusID VARCHAR(20) UNIQUE NOT NULL,
    companyName NVARCHAR(200) NOT NULL,
    companyLogoUrl VARCHAR(500) NULL,
    industry NVARCHAR(150) NULL,
    companySize VARCHAR(30) NULL,
    companyWebsite VARCHAR(255) NULL,
    companyAddress NVARCHAR(300) NULL,
    -- CODE CŨ:
    -- taxCode VARCHAR(30) NULL,
    -- GHI CHÚ SỬA: Mã số thuế có dữ liệu phải duy nhất; NULL vẫn được phép.
    -- CODE MỚI:
    taxCode VARCHAR(30) NULL,
    companyVerified BIT NOT NULL DEFAULT 0,
    verifiedAt DATETIME NULL,
    description NVARCHAR(MAX) NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL,

    CONSTRAINT FK_NhaTuyenDung_KhachHang
        FOREIGN KEY (cusID) REFERENCES KhachHang(cusID) ON DELETE CASCADE,

    CONSTRAINT CK_NhaTuyenDung_CompanySize
        CHECK (companySize IS NULL OR companySize IN
        ('1-10', '11-50', '51-200', '201-500', '501-1000', '1000+'))
);
GO

-- CODE MOI:
-- LY DO THEM: UI Admin can hien thi doanh nghiep co dang hoat dong/xac minh hay khong.
-- Trang thai tai khoan thuc te van lay tu Users.status,
-- con companyVerified la trang thai xac minh thong tin doanh nghiep.


-- =========================================================
-- 15.3 CODE CŨ ALTER HOPDONG (ĐÃ COMMENT - KHÔNG CHẠY)
-- GHI CHÚ SỬA:
-- Các cột và CHECK của phần này đã được đưa trực tiếp vào CREATE TABLE HopDong ở mục 10.
-- Giữ lại nội dung cũ dưới dạng comment để theo dõi lịch sử thay đổi.
-- 15.3 MO RONG HOPDONG CHO UI "HOP DONG CUA TOI"
-- =========================================================

-- CODE CU (GIU NGUYEN):
-- CREATE TABLE HopDong (
--     maHD VARCHAR(20) PRIMARY KEY,
--     jobID VARCHAR(20) UNIQUE NOT NULL,
--     freeID VARCHAR(20) NOT NULL,
--     ngayBatDau DATETIME DEFAULT GETDATE(),
--     ngayKetThuc DATETIME,
--     sotienkyquy DECIMAL(18,2),
--     trangThai NVARCHAR(30) DEFAULT N'DangThucHien',
--     ...
-- );
--
-- LY DO BO SUNG:
-- UI Hop dong cua Freelancer can hien thi them:
-- - Ma du an
-- - Loai hop dong Fixed Price / Theo gio
-- - Tong gia tri
-- - Tien do %
-- - Han hoan thanh
-- - Trang thai thanh toan
-- - Linh vuc
-- - Hinh thuc Remote/Onsite/Hybrid
-- - Thoi diem tao/cap nhat/hoan thanh/huy

-- CODE MOI: CHI ADD COT, KHONG XOA CODE CU
-- ALTER TABLE HopDong ADD
--     projectCode VARCHAR(30) NULL,
--     contractType VARCHAR(20) NOT NULL
--         CONSTRAINT DF_HopDong_ContractType DEFAULT 'FIXED_PRICE',
--     tongGiaTri DECIMAL(18,2) NULL,
--     tienDo INT NOT NULL CONSTRAINT DF_HopDong_TienDo DEFAULT 0,
--     hanHoanThanh DATETIME NULL,
--     paymentStatus VARCHAR(20) NOT NULL
--         CONSTRAINT DF_HopDong_PaymentStatus DEFAULT 'UNPAID',
--     workMode VARCHAR(20) NULL,
--     fieldName NVARCHAR(150) NULL,
--     completedAt DATETIME NULL,
--     cancelledAt DATETIME NULL,
--     cancelReason NVARCHAR(500) NULL,
--     createdAt DATETIME NOT NULL CONSTRAINT DF_HopDong_CreatedAt DEFAULT GETDATE(),
--     updatedAt DATETIME NULL;
-- GO

-- CODE MOI:
-- LY DO THEM CHECK: Dam bao UI khong nhan du lieu trang thai/phan tram sai.
-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_ContractType
-- CHECK (contractType IN ('FIXED_PRICE', 'HOURLY'));
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_TienDo
-- CHECK (tienDo BETWEEN 0 AND 100);
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_PaymentStatus
-- CHECK (paymentStatus IN ('UNPAID', 'ESCROWED', 'PENDING', 'PAID', 'REFUNDED'));
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_WorkMode
-- CHECK (workMode IS NULL OR workMode IN ('REMOTE', 'ONSITE', 'HYBRID'));
-- GO


-- =========================================================
-- 15.4 KY QUY ESCROW CHO HOP DONG
-- =========================================================

-- CODE CU (GIU NGUYEN):
-- HopDong chi co cot sotienkyquy.
--
-- LY DO THEM BANG MOI:
-- UI chi tiet hop dong va nghiep vu thanh toan can biet:
-- - Da ky quy hay chua
-- - So tien dang nam trong trung gian
-- - Thoi diem nap / giai ngan / hoan tien
-- sotienkyquy trong HopDong chi la gia tri, khong du de theo doi vong doi Escrow.

-- CODE MOI:
CREATE TABLE Escrow (
    escrowID VARCHAR(20) PRIMARY KEY,
    maHD VARCHAR(20) UNIQUE NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    fundedAt DATETIME NULL,
    releasedAt DATETIME NULL,
    refundedAt DATETIME NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL,

    CONSTRAINT FK_Escrow_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD),

    CONSTRAINT CK_Escrow_Status
        CHECK (status IN ('PENDING', 'FUNDED', 'RELEASED', 'REFUNDED', 'DISPUTED')),

    CONSTRAINT CK_Escrow_Amount
        CHECK (amount >= 0),

    -- CODE MỚI: Ràng buộc mốc thời gian phải phù hợp trạng thái Escrow.
    CONSTRAINT CK_Escrow_StatusTime CHECK (
        (status = 'PENDING'  AND releasedAt IS NULL AND refundedAt IS NULL)
     OR (status = 'FUNDED'   AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
     OR (status = 'RELEASED' AND fundedAt IS NOT NULL AND releasedAt IS NOT NULL AND refundedAt IS NULL)
     OR (status = 'REFUNDED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NOT NULL)
     OR (status = 'DISPUTED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
    )
);
GO


-- =========================================================
-- 15.5 TAI LIEU HOP DONG
-- =========================================================

-- CODE MOI:
-- LY DO THEM:
-- UI chi tiet hop dong co khu vuc "Tai lieu hop dong".
-- Cho phep luu PDF, DOCX va cac tai lieu phu luc lien quan.
CREATE TABLE ContractDocument (
    documentID VARCHAR(20) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    fileName NVARCHAR(255) NOT NULL,
    fileUrl VARCHAR(500) NOT NULL,
    fileType VARCHAR(20) NULL,
    fileSizeKB INT NULL,
    uploadedBy VARCHAR(20) NOT NULL,
    uploadedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ContractDocument_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD) ON DELETE CASCADE,

    CONSTRAINT FK_ContractDocument_User
        FOREIGN KEY (uploadedBy) REFERENCES Users(userID),

    -- CODE MỚI: Kích thước file không được âm.
    CONSTRAINT CK_ContractDocument_FileSize
        CHECK (fileSizeKB IS NULL OR fileSizeKB >= 0)
);
GO


-- =========================================================
-- 15.6 LICH SU HOAT DONG HOP DONG
-- =========================================================

-- CODE MOI:
-- LY DO THEM:
-- UI chi tiet hop dong co timeline "Lich su hoat dong".
-- Bang nay ghi lai cac moc nhu:
-- - Khach hang chap nhan de xuat
-- - Da nap tien ky quy
-- - Bat dau thuc hien
-- - Hoan thanh
-- - Giai ngan / hoan tien
CREATE TABLE ContractActivityLog (
    activityID BIGINT IDENTITY(1,1) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    actorUserID VARCHAR(20) NULL,
    actionType VARCHAR(40) NOT NULL,
    description NVARCHAR(500) NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ContractActivity_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD) ON DELETE CASCADE,

    CONSTRAINT FK_ContractActivity_User
        FOREIGN KEY (actorUserID) REFERENCES Users(userID)
);
GO


-- =========================================================
-- 15.7 VIEW PHUC VU UI ADMIN - DANH SACH FREELANCER SINH VIEN
-- =========================================================

-- CODE MOI:
-- LY DO THEM:
-- UI danh sach Freelancer can 1 query tong hop:
-- User + FreelancerSV + so hop dong + diem danh gia.
-- View giup backend lay du lieu bang quan ly gon hon.
CREATE VIEW VW_Admin_FreelancerList
AS
WITH ContractAgg AS (
    SELECT
        freeID,
        COUNT(*) AS totalContracts,
        SUM(CASE WHEN trangThai IN (N'DangThucHien', N'Đang thực hiện') THEN 1 ELSE 0 END) AS activeContracts,
        SUM(CASE WHEN trangThai IN (N'HoanThanh', N'Đã hoàn thành', N'Hoàn thành') THEN 1 ELSE 0 END) AS completedContracts
    FROM HopDong
    GROUP BY freeID
),
RatingAgg AS (
    SELECT
        nguoiDuocDanhGiaID AS userID,
        AVG(CAST(soSao AS DECIMAL(4,2))) AS averageRating,
        COUNT(*) AS ratingCount
    FROM DanhGia
    GROUP BY nguoiDuocDanhGiaID
)
SELECT
    u.userID,
    fr.free_ID,
    u.userName,
    u.email,
    u.phoneNumber,
    u.status,
    u.createdAt AS joinedAt,
    fr.avatarUrl,
    fr.university,
    fr.major,
    fr.studentCardID,
    fr.GPA,
    fr.verificationStatus,
    fr.verifiedAt,
    fr.enrollmentYear,
    fr.graduationYear,
    fr.studyMode,
    ISNULL(ca.totalContracts, 0) AS totalContracts,
    ISNULL(ca.activeContracts, 0) AS activeContracts,
    ISNULL(ca.completedContracts, 0) AS completedContracts,
    ra.averageRating,
    ISNULL(ra.ratingCount, 0) AS ratingCount
FROM Users u
INNER JOIN FreelancerSV fr ON u.userID = fr.userID
LEFT JOIN ContractAgg ca ON fr.free_ID = ca.freeID
LEFT JOIN RatingAgg ra ON u.userID = ra.userID
WHERE u.loaiUser = N'FreelancerSV';
GO


-- =========================================================
-- 15.8 VIEW PHUC VU UI ADMIN - DANH SACH NHA TUYEN DUNG
-- =========================================================

-- CODE MOI:
-- LY DO THEM:
-- UI nha tuyen dung hien thi:
-- ten cong ty, linh vuc, quy mo, status, ngay tham gia,
-- so tin dang va diem danh gia.
CREATE VIEW VW_Admin_EmployerList
AS
SELECT
    u.userID,
    kh.cusID,
    nt.employerID,
    u.userName,
    u.email,
    u.phoneNumber,
    u.status,
    u.createdAt AS joinedAt,
    nt.companyName,
    nt.companyLogoUrl,
    nt.industry,
    nt.companySize,
    nt.companyWebsite,
    nt.companyAddress,
    nt.taxCode,
    nt.companyVerified,
    nt.verifiedAt,
    kh.rating,
    COUNT(DISTINCT jp.jobID) AS totalJobPosts,
    SUM(CASE WHEN jp.status IN (N'DangTuyen', N'Đang tuyển') THEN 1 ELSE 0 END) AS activeJobPosts
FROM Users u
INNER JOIN KhachHang kh ON u.userID = kh.userID
INNER JOIN NhaTuyenDung nt ON kh.cusID = nt.cusID
LEFT JOIN JobPost jp ON kh.cusID = jp.cusID
WHERE u.loaiUser = N'KhachHang'
GROUP BY
    u.userID, kh.cusID, nt.employerID, u.userName, u.email,
    u.phoneNumber, u.status, u.createdAt, nt.companyName,
    nt.companyLogoUrl, nt.industry, nt.companySize,
    nt.companyWebsite, nt.companyAddress, nt.taxCode,
    nt.companyVerified, nt.verifiedAt, kh.rating;
GO


-- =========================================================
-- 15.9 VIEW PHUC VU UI FREELANCER - DANH SACH HOP DONG CUA TOI
-- =========================================================

-- CODE MOI:
-- LY DO THEM:
-- UI "Hop dong cua toi" can thong tin hop dong + job + nha tuyen dung.
CREATE VIEW VW_Freelancer_ContractList
AS
SELECT
    hd.maHD,
    hd.freeID,
    hd.jobID,
    hd.projectCode,
    hd.contractType,
    hd.tongGiaTri,
    hd.sotienkyquy,
    hd.tienDo,
    hd.hanHoanThanh,
    hd.ngayBatDau,
    hd.completedAt AS ngayKetThuc,
    hd.trangThai,
    hd.paymentStatus,
    hd.workMode,
    hd.fieldName,
    hd.completedAt,
    hd.cancelledAt,
    hd.cancelReason,
    jp.title AS jobTitle,
    jp.descr AS jobDescription,
    kh.cusID,
    nt.employerID,
    nt.companyName,
    nt.companyLogoUrl,
    nt.companyVerified,
    es.escrowID,
    es.status AS escrowStatus,
    es.amount AS escrowAmount
FROM HopDong hd
INNER JOIN JobPost jp ON hd.jobID = jp.jobID
INNER JOIN KhachHang kh ON jp.cusID = kh.cusID
LEFT JOIN NhaTuyenDung nt ON kh.cusID = nt.cusID
LEFT JOIN Escrow es ON hd.maHD = es.maHD;
GO


-- =========================================================
-- 15.10 INDEX PHUC VU TIM KIEM / LOC TREN UI
-- =========================================================

-- CODE MOI:
-- LY DO THEM: Tang toc cac bo loc thuong dung tren UI Admin va Hop dong.
CREATE INDEX IX_Users_LoaiUser_Status
ON Users(loaiUser, status);
GO

CREATE INDEX IX_FreelancerSV_University_Major
ON FreelancerSV(university, major);
GO

CREATE INDEX IX_HopDong_FreeID_Status
ON HopDong(freeID, trangThai);
GO

CREATE INDEX IX_NhaTuyenDung_Industry
ON NhaTuyenDung(industry);
GO

CREATE INDEX IX_NhaTuyenDung_CompanyName
ON NhaTuyenDung(companyName);
GO

-- CODE CŨ:
-- taxCode VARCHAR(30) NULL không có ràng buộc duy nhất.
-- GHI CHÚ SỬA: SQL Server cho phép tạo UNIQUE FILTERED INDEX để nhiều NULL hợp lệ nhưng mã có dữ liệu không trùng.
-- CODE MỚI:
CREATE UNIQUE INDEX UX_NhaTuyenDung_TaxCode
ON NhaTuyenDung(taxCode)
WHERE taxCode IS NOT NULL;
GO


-- =========================================================
-- 16. DU LIEU MAU BO SUNG CHO CAC UI MOI
-- =========================================================

-- ---------------------------------------------------------
-- 16.1 BO SUNG HO SO FREELANCER CHO UI ADMIN
-- ---------------------------------------------------------

-- CODE MOI:
-- LY DO THEM: Cac cot vua them dang NULL; cap nhat FREE001 de co du thong tin demo.
UPDATE FreelancerSV
SET avatarUrl = 'https://example.com/avatar/freelancer1.jpg',
    verificationStatus = 'VERIFIED',
    verifiedAt = GETDATE(),
    enrollmentYear = 2023,
    graduationYear = 2027,
    studyMode = N'Chính quy',
    bio = N'Freelancer sinh viên chuyên về công nghệ thông tin và thiết kế.'
WHERE free_ID = 'FREE001';
GO


-- ---------------------------------------------------------
-- 16.2 THEM HO SO NHA TUYEN DUNG CHO KHACH HANG CU
-- ---------------------------------------------------------

-- CODE CU (GIU NGUYEN):
-- CUS001 truoc day chi la mot dong trong KhachHang.
--
-- CODE MOI:
-- LY DO THEM: Gan CUS001 vao mot ho so doanh nghiep de UI Nha tuyen dung co du lieu.
INSERT INTO NhaTuyenDung
(employerID, cusID, companyName, industry, companySize,
 companyWebsite, companyAddress, taxCode, companyVerified,
 verifiedAt, description)
VALUES
('EMP001', 'CUS001', N'Công ty ABC', N'Công nghệ thông tin', '51-200',
 'https://abc.example.vn', N'TP. Hồ Chí Minh', '0312345678', 1,
 GETDATE(), N'Doanh nghiệp tuyển dụng freelancer cho các dự án công nghệ và thiết kế.');
GO


-- ---------------------------------------------------------
-- 16.3 CAP NHAT HOP DONG CU DE HIEN THI DUNG UI HOP DONG
-- ---------------------------------------------------------

-- CODE CU (GIU NGUYEN):
-- HD001 chi co thong tin co ban o INSERT 12.7.
--
-- CODE MOI:
-- LY DO THEM: Bo sung cac thong tin ma UI Hop dong can hien thi.
UPDATE HopDong
SET projectCode = 'PRJ-1024',
    contractType = 'FIXED_PRICE',
    tongGiaTri = 2000000.00,
    tienDo = 75,
    hanHoanThanh = DATEADD(day, 10, GETDATE()),
    paymentStatus = 'ESCROWED',
    workMode = 'REMOTE',
    fieldName = N'Công nghệ thông tin',
    updatedAt = GETDATE()
WHERE maHD = 'HD001';
GO

-- CODE MOI:
-- LY DO THEM: Tao Escrow mau cho HD001.
INSERT INTO Escrow
(escrowID, maHD, amount, status, fundedAt)
VALUES
('ESC001', 'HD001', 2000000.00, 'FUNDED', GETDATE());
GO

-- CODE MOI:
-- LY DO THEM: Tao tai lieu hop dong mau.
INSERT INTO ContractDocument
(documentID, maHD, fileName, fileUrl, fileType, fileSizeKB, uploadedBy)
VALUES
('DOC001', 'HD001', N'Hop_dong_HD001.pdf',
 'https://example.com/contracts/Hop_dong_HD001.pdf', 'PDF', 1200, 'USR001');
GO

-- CODE MOI:
-- LY DO THEM: Tao timeline mau cho UI chi tiet hop dong.
INSERT INTO ContractActivityLog
(maHD, actorUserID, actionType, description)
VALUES
('HD001', 'USR002', 'ACCEPTED', N'Khách hàng đã chấp nhận đề xuất hợp tác'),
('HD001', 'USR002', 'ESCROW_FUNDED', N'Khách hàng đã nạp tiền ký quỹ'),
('HD001', 'USR003', 'STARTED', N'Freelancer bắt đầu thực hiện dự án');
GO


-- =========================================================
-- 17. CAC QUERY MAU TRUC TIEP PHUC VU UI
-- =========================================================

-- ---------------------------------------------------------
-- 17.1 ADMIN -> DANH SACH FREELANCER SINH VIEN
-- ---------------------------------------------------------
-- UI co the SEARCH theo ten/email/ma user,
-- FILTER theo status/truong/nganh/ngay tham gia.
SELECT *
FROM VW_Admin_FreelancerList
ORDER BY joinedAt DESC;
GO

-- Tong so Freelancer
SELECT COUNT(*) AS totalFreelancers
FROM VW_Admin_FreelancerList;
GO

-- Dang hoat dong
SELECT COUNT(*) AS activeFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'ACTIVE';
GO

-- Tam khoa: hien tai map INACTIVE -> "Tam khoa" tren UI
SELECT COUNT(*) AS suspendedFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'INACTIVE';
GO

-- Da khoa
SELECT COUNT(*) AS lockedFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'LOCKED';
GO

-- Moi tham gia 7 ngay gan nhat
SELECT COUNT(*) AS newFreelancersLast7Days
FROM VW_Admin_FreelancerList
WHERE joinedAt >= DATEADD(day, -7, GETDATE());
GO


-- ---------------------------------------------------------
-- 17.2 ADMIN -> DANH SACH NHA TUYEN DUNG
-- ---------------------------------------------------------
SELECT *
FROM VW_Admin_EmployerList
ORDER BY joinedAt DESC;
GO

-- Tong so Nha tuyen dung
SELECT COUNT(*) AS totalEmployers
FROM VW_Admin_EmployerList;
GO

-- Dang hoat dong
SELECT COUNT(*) AS activeEmployers
FROM VW_Admin_EmployerList
WHERE status = 'ACTIVE';
GO

-- Tam khoa
SELECT COUNT(*) AS suspendedEmployers
FROM VW_Admin_EmployerList
WHERE status = 'INACTIVE';
GO

-- Da khoa
SELECT COUNT(*) AS lockedEmployers
FROM VW_Admin_EmployerList
WHERE status = 'LOCKED';
GO

-- Moi tham gia 7 ngay gan nhat
SELECT COUNT(*) AS newEmployersLast7Days
FROM VW_Admin_EmployerList
WHERE joinedAt >= DATEADD(day, -7, GETDATE());
GO


-- ---------------------------------------------------------
-- 17.3 FREELANCER -> HOP DONG CUA TOI
-- ---------------------------------------------------------
-- Vi du lay hop dong cua FREE001
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
ORDER BY ngayBatDau DESC;
GO

-- Dang thuc hien
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND trangThai IN (N'DangThucHien', N'Đang thực hiện');
GO

-- Cho thanh toan
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND paymentStatus = 'PENDING';
GO

-- Da hoan thanh
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND trangThai IN (N'HoanThanh', N'Đã hoàn thành', N'Hoàn thành');
GO

-- Da huy
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND cancelledAt IS NOT NULL;
GO


-- =========================================================
-- 18. GHI CHU MAPPING UI <-> DATABASE
-- =========================================================
-- UI ADMIN - FREELANCER:
-- "Thông tin người dùng"  -> Users + FreelancerSV
-- "Trường học"            -> FreelancerSV.university
-- "Ngành học"             -> FreelancerSV.major
-- "Trạng thái"            -> Users.status
-- "Ngày tham gia"         -> Users.createdAt
-- "Công việc"             -> COUNT(HopDong.maHD)
-- "Đánh giá"              -> AVG(DanhGia.soSao)
-- "Xác minh sinh viên"    -> FreelancerSV.verificationStatus
--
-- UI ADMIN - NHA TUYEN DUNG:
-- "Thông tin nhà tuyển dụng" -> Users + KhachHang + NhaTuyenDung
-- "Lĩnh vực hoạt động"       -> NhaTuyenDung.industry
-- "Quy mô"                   -> NhaTuyenDung.companySize
-- "Trạng thái"               -> Users.status
-- "Ngày tham gia"            -> Users.createdAt
-- "Số tin đăng"               -> COUNT(JobPost.jobID)
-- "Đánh giá"                 -> KhachHang.rating
--
-- UI FREELANCER - HOP DONG:
-- "Tên dự án"              -> JobPost.title
-- "Nhà tuyển dụng"         -> NhaTuyenDung.companyName
-- "Mã dự án"               -> HopDong.projectCode
-- "Loại hợp đồng"          -> HopDong.contractType
-- "Tổng giá trị"           -> HopDong.tongGiaTri
-- "Tiến độ"                -> HopDong.tienDo
-- "Hạn hoàn thành"         -> HopDong.hanHoanThanh
-- "Trạng thái"             -> HopDong.trangThai
-- "Thanh toán"             -> HopDong.paymentStatus
-- "Ký quỹ"                 -> Escrow
-- "Tài liệu hợp đồng"      -> ContractDocument
-- "Lịch sử hoạt động"      -> ContractActivityLog
-- =========================================================


-- =========================================================
-- =========================================================
-- 19. CODE CŨ PATCH (ĐÃ COMMENT - KHÔNG CHẠY)
-- =========================================================
-- GHI CHÚ SỬA:
-- Các PATCH ALTER/DROP cũ không còn cần thiết vì thay đổi đã được đưa trực tiếp
-- vào CREATE TABLE/CONSTRAINT ở phía trên. Giữ lại dưới dạng comment để theo dõi lịch sử.
-- 19. PATCH SUA LOI / CHUAN HOA SAU KHI RA SOAT
-- =========================================================
-- LUU Y:
-- 1. KHONG XOA / SUA TRUC TIEP CODE CU PHIA TREN.
-- 2. CAC LENH DUOI DAY LA PATCH BO SUNG DE KHAC PHUC CAC DIEM CHUA PHU HOP.
-- 3. CAC COMMENT GHI RO CODE CU / LY DO / CODE MOI.
-- =========================================================

-- ---------------------------------------------------------
-- 19.1 HOPDONG.jobID: BO UNIQUE DE MOT JOB CO THE CO NHIEU HOP DONG
-- ---------------------------------------------------------
-- CODE CU:
-- jobID VARCHAR(20) UNIQUE NOT NULL
-- LY DO SUA:
-- JobPost co cot soluongtuyen, do do mot tin tuyen dung co the tuyen nhieu Freelancer.
-- Moi Freelancer duoc chon co the co mot HopDong rieng.
-- UNIQUE(jobID) se chan truong hop nay.
-- CODE MOI: Xoa UNIQUE constraint tren HopDong.jobID neu ton tai.
-- DECLARE @UQ_HopDong_JobID SYSNAME;
-- SELECT TOP 1 @UQ_HopDong_JobID = kc.name
-- FROM sys.key_constraints kc
-- JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
-- JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
-- WHERE kc.parent_object_id = OBJECT_ID('HopDong')
--   AND kc.type = 'UQ'
--   AND c.name = 'jobID';
-- IF @UQ_HopDong_JobID IS NOT NULL
--     EXEC('ALTER TABLE HopDong DROP CONSTRAINT [' + @UQ_HopDong_JobID + ']');
-- GO

-- Khong cho cung mot Freelancer co 2 hop dong cho cung mot Job.
-- ALTER TABLE HopDong ADD CONSTRAINT UQ_HopDong_Job_Freelancer UNIQUE(jobID, freeID);
-- GO

-- ---------------------------------------------------------
-- 19.2 DANHGIA.maHD: CHO PHEP HAI BEN DANH GIA NHAU
-- ---------------------------------------------------------
-- CODE CU:
-- maHD VARCHAR(20) UNIQUE NOT NULL
-- LY DO SUA:
-- UNIQUE(maHD) chi cho phep 1 danh gia tren mot hop dong.
-- Can cho phep Khach hang danh gia Freelancer va Freelancer danh gia Khach hang.
-- DECLARE @UQ_DanhGia_MaHD SYSNAME;
-- SELECT TOP 1 @UQ_DanhGia_MaHD = kc.name
-- FROM sys.key_constraints kc
-- JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
-- JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
-- WHERE kc.parent_object_id = OBJECT_ID('DanhGia')
--   AND kc.type = 'UQ'
--   AND c.name = 'maHD';
-- IF @UQ_DanhGia_MaHD IS NOT NULL
--     EXEC('ALTER TABLE DanhGia DROP CONSTRAINT [' + @UQ_DanhGia_MaHD + ']');
-- GO

-- Moi nguoi chi duoc danh gia mot lan tren cung hop dong.
-- ALTER TABLE DanhGia ADD CONSTRAINT UQ_DanhGia_HD_NguoiDanhGia UNIQUE(maHD, nguoiDanhGiaID);
-- GO

-- Khong cho phep tu danh gia chinh minh.
-- ALTER TABLE DanhGia ADD CONSTRAINT CK_DanhGia_KhongTuDanhGia
-- CHECK (nguoiDanhGiaID <> nguoiDuocDanhGiaID);
-- GO

-- ---------------------------------------------------------
-- 19.3 CHUAN HOA USERS.status
-- ---------------------------------------------------------
-- CODE CU:
-- CHECK (status IN ('ACTIVE', 'LOCKED', 'INACTIVE'))
-- VA UI TAM MAP INACTIVE = "Tam khoa".
-- LY DO SUA:
-- Can tach ro Tam khoa (SUSPENDED), Khoa (LOCKED), Ngung hoat dong (INACTIVE).
-- DECLARE @CK_Users_Status SYSNAME;
-- SELECT TOP 1 @CK_Users_Status = cc.name
-- FROM sys.check_constraints cc
-- WHERE cc.parent_object_id = OBJECT_ID('Users')
--   AND cc.definition LIKE '%status%ACTIVE%LOCKED%INACTIVE%';
-- IF @CK_Users_Status IS NOT NULL
--     EXEC('ALTER TABLE Users DROP CONSTRAINT [' + @CK_Users_Status + ']');
-- GO
-- ALTER TABLE Users ADD CONSTRAINT CK_Users_Status
-- CHECK (status IN ('ACTIVE', 'SUSPENDED', 'LOCKED', 'INACTIVE'));
-- GO

-- ---------------------------------------------------------
-- 19.4 RANG BUOC DU LIEU TIEN TRONG HOP DONG
-- ---------------------------------------------------------
-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_TongGiaTri
-- CHECK (tongGiaTri IS NULL OR tongGiaTri >= 0);
-- GO
-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_SoTienKyQuy
-- CHECK (sotienkyquy IS NULL OR sotienkyquy >= 0);
-- GO

-- ---------------------------------------------------------
-- 19.5 NHA TUYEN DUNG: MA SO THUE KHONG DUOC TRUNG
-- ---------------------------------------------------------
-- SQL Server UNIQUE cho phep mot NULL nhung khong cho phep nhieu NULL.
-- Vi taxCode dang cho phep NULL, dung UNIQUE FILTERED INDEX de chi rang buoc cac gia tri co du lieu.
-- CREATE UNIQUE INDEX UX_NhaTuyenDung_TaxCode
-- ON NhaTuyenDung(taxCode)
-- WHERE taxCode IS NOT NULL;
-- GO

-- ---------------------------------------------------------
-- 19.6 CONTRACT DOCUMENT: FILE SIZE KHONG AM
-- ---------------------------------------------------------
-- ALTER TABLE ContractDocument ADD CONSTRAINT CK_ContractDocument_FileSize
-- CHECK (fileSizeKB IS NULL OR fileSizeKB >= 0);
-- GO

-- ---------------------------------------------------------
-- 19.7 ESCROW: BO SUNG RANG BUOC THOI GIAN THEO TRANG THAI
-- ---------------------------------------------------------
-- ALTER TABLE Escrow ADD CONSTRAINT CK_Escrow_StatusTime
-- CHECK (
--     (status = 'PENDING'  AND releasedAt IS NULL AND refundedAt IS NULL)
--  OR (status = 'FUNDED'   AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
--  OR (status = 'RELEASED' AND fundedAt IS NOT NULL AND releasedAt IS NOT NULL AND refundedAt IS NULL)
--  OR (status = 'REFUNDED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NOT NULL)
--  OR (status = 'DISPUTED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
-- );
-- GO

-- ---------------------------------------------------------
-- 19.8 AD-UC-01: UPDATE + AUDIT LOG PHAI CUNG MOT TRANSACTION
-- ---------------------------------------------------------
-- CODE CU O MUC 13 CHAY UPDATE VA INSERT AUDIT DOC LAP.
-- NEU INSERT LOG LOI SAU KHI UPDATE THANH CONG THI DU LIEU VA AUDIT BI LECH.
-- CODE MOI: Stored Procedure cap nhat so dien thoai an toan.
-- CREATE OR ALTER PROCEDURE SP_Admin_UpdateUserPhone
--     @AdminUserID VARCHAR(20),
--     @TargetUserID VARCHAR(20),
--     @NewPhoneNumber VARCHAR(15),
--     @Reason NVARCHAR(500)
-- AS
-- BEGIN
--     SET NOCOUNT ON;
--     SET XACT_ABORT ON;

--     DECLARE @OldPhoneNumber VARCHAR(15);

--     BEGIN TRY
--         BEGIN TRANSACTION;

--         IF NOT EXISTS (
--             SELECT 1
--             FROM Admin a
--             INNER JOIN Users u ON a.userID = u.userID
--             WHERE a.userID = @AdminUserID AND u.status = 'ACTIVE'
--         )
--             THROW 50001, N'Tài khoản thực hiện không phải Admin đang hoạt động.', 1;

--         SELECT @OldPhoneNumber = phoneNumber
--         FROM Users WITH (UPDLOCK, HOLDLOCK)
--         WHERE userID = @TargetUserID;

--         IF @@ROWCOUNT = 0
--             THROW 50002, N'Không tìm thấy tài khoản cần cập nhật.', 1;

--         UPDATE Users
--         SET phoneNumber = @NewPhoneNumber,
--             updatedAt = GETDATE()
--         WHERE userID = @TargetUserID;

--         INSERT INTO UserAuditLog
--         (adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
--         VALUES
--         (@AdminUserID, @TargetUserID, 'UPDATE', 'phoneNumber',
--          @OldPhoneNumber, @NewPhoneNumber, @Reason);

--         COMMIT TRANSACTION;
--     END TRY
--     BEGIN CATCH
--         IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
--         THROW;
--     END CATCH
-- END;
-- GO

-- ---------------------------------------------------------
-- 19.9 AD-UC-02: KHOA / TAM KHOA / MO KHOA TAI KHOAN + AUDIT
-- ---------------------------------------------------------
-- CREATE OR ALTER PROCEDURE SP_Admin_ChangeUserStatus
--     @AdminUserID VARCHAR(20),
--     @TargetUserID VARCHAR(20),
--     @NewStatus VARCHAR(20),
--     @Reason NVARCHAR(500)
-- AS
-- BEGIN
--     SET NOCOUNT ON;
--     SET XACT_ABORT ON;

--     DECLARE @OldStatus VARCHAR(20);
--     DECLARE @ActionType VARCHAR(30);

--     IF @NewStatus NOT IN ('ACTIVE', 'SUSPENDED', 'LOCKED', 'INACTIVE')
--         THROW 50003, N'Trạng thái tài khoản không hợp lệ.', 1;

--     IF NULLIF(LTRIM(RTRIM(@Reason)), N'') IS NULL
--         THROW 50004, N'Phải nhập lý do thay đổi trạng thái.', 1;

--     IF @AdminUserID = @TargetUserID
--         THROW 50005, N'Admin không được tự khóa hoặc thay đổi trạng thái tài khoản của chính mình bằng chức năng này.', 1;

--     BEGIN TRY
--         BEGIN TRANSACTION;

--         SELECT @OldStatus = status
--         FROM Users WITH (UPDLOCK, HOLDLOCK)
--         WHERE userID = @TargetUserID;

--         IF @@ROWCOUNT = 0
--             THROW 50006, N'Không tìm thấy tài khoản.', 1;

--         IF @OldStatus = @NewStatus
--             THROW 50007, N'Tài khoản đã ở trạng thái yêu cầu.', 1;

--         UPDATE Users
--         SET status = @NewStatus,
--             updatedAt = GETDATE()
--         WHERE userID = @TargetUserID;

--         SET @ActionType = CASE WHEN @NewStatus = 'ACTIVE' THEN 'UNLOCK' ELSE 'LOCK' END;

--         INSERT INTO UserAuditLog
--         (adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
--         VALUES
--         (@AdminUserID, @TargetUserID, @ActionType, 'status', @OldStatus, @NewStatus, @Reason);

--         COMMIT TRANSACTION;
--     END TRY
--     BEGIN CATCH
--         IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
--         THROW;
--     END CATCH
-- END;
-- GO

-- ---------------------------------------------------------
-- 19.10 QUERY UI ADMIN SAU KHI THEM SUSPENDED
-- ---------------------------------------------------------
-- SELECT COUNT(*) AS suspendedFreelancers
-- FROM VW_Admin_FreelancerList
-- WHERE status = 'SUSPENDED';
-- GO

-- SELECT COUNT(*) AS suspendedEmployers
-- FROM VW_Admin_EmployerList
-- WHERE status = 'SUSPENDED';
-- GO

-- =========================================================-- 20. GHI CHU CAC DIEM CODE CU CHUA DUOC XOA
-- =========================================================
-- [CAN LUU Y 1]
-- Muc 13 van con UPDATE Users + INSERT UserAuditLog doc lap vi yeu cau giu code cu.
-- Khi code ASP.NET MVC, KHONG nen dung truc tiep mau cu do.
-- Nen goi SP_Admin_UpdateUserPhone hoac dung transaction tuong duong.
--
-- [CAN LUU Y 2]
-- Muc 17.1 va 17.2 van co query cu map INACTIVE = Tam khoa.
-- Query moi dung SUSPENDED da duoc bo sung tai muc 19.10.
--
-- [CAN LUU Y 3]
-- HopDong.trangThai trong du lieu cu dang ton tai nhieu cach viet tieng Viet/khong dau.
-- Chua sua truc tiep code cu de tranh pha du lieu mau.
-- Khi trien khai backend nen chot mot bo ma trang thai duy nhat, vi du:
-- PENDING, ACTIVE, COMPLETED, CANCELLED, DISPUTED.
--
-- [CAN LUU Y 4]
-- File nay bat dau bang CREATE DATABASE va cac CREATE TABLE truc tiep.
-- Vi vay day la script khoi tao database, KHONG phai migration idempotent.
-- Neu chay lai tren cung database da ton tai se bao loi object/data da ton tai.
-- =========================================================


-- =========================================================
-- 21. BO SUNG DATABASE THEO FORM TAO HOP DONG MOI (GOP 4 BUOC -> 1 FORM)
-- =========================================================
-- NGUYEN TAC:
-- 1. TOAN BO CODE CU TU MUC 1 -> 20 DUOC GIU NGUYEN, KHONG XOA.
-- 2. CAC PHAN CHUA PHU HOP DUOC GIU LAI VA BO SUNG PATCH O PHIA DUOI.
-- 3. CAC COT/BANG MOI PHUC VU FORM TAO HOP DONG DA CHOT TREN UI.
-- =========================================================


-- =========================================================
-- 21.1-21.4 CODE CŨ ALTER (ĐÃ COMMENT - KHÔNG CHẠY)
-- GHI CHÚ: Các cột/constraint đã được tích hợp trực tiếp trong CREATE TABLE HopDong.
-- 21.1 BO SUNG NOI DUNG DIEU KHOAN CHI TIET CHO HOP DONG
-- =========================================================
-- CODE CU:
-- HopDong chua co cot luu noi dung dieu khoan chi tiet.
--
-- LY DO THEM:
-- Form tao hop dong moi co khu vuc "Dieu khoan chi tiet".
-- Noi dung dieu khoan co the dai nen su dung NVARCHAR(MAX).
--
-- CODE MOI:
-- ALTER TABLE HopDong ADD
--     dieuKhoanChiTiet NVARCHAR(MAX) NULL;
-- GO


-- =========================================================
-- 21.2 BO SUNG HINH THUC / LICH THANH TOAN
-- =========================================================
-- CODE CU:
-- HopDong da co paymentStatus de luu TRANG THAI thanh toan:
-- UNPAID, ESCROWED, PENDING, PAID, REFUNDED.
--
-- LY DO THEM:
-- paymentStatus khong cho biet hop dong thanh toan theo cach nao.
-- Form moi can phan biet:
-- - Thanh toan mot lan
-- - Thanh toan theo moc
-- - Thanh toan theo thoi gian
--
-- CODE MOI:
-- ALTER TABLE HopDong ADD
--     paymentType VARCHAR(30) NOT NULL
--         CONSTRAINT DF_HopDong_PaymentType DEFAULT 'ONE_TIME';
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_PaymentType
-- CHECK (paymentType IN ('ONE_TIME', 'MILESTONE', 'TIME_BASED'));
-- GO


-- =========================================================
-- 21.3 BO SUNG TRANG THAI DE XUAT / XAC NHAN HOP DONG
-- =========================================================
-- CODE CU:
-- HopDong.trangThai dang mo ta trang thai thuc hien hop dong.
-- Chua co trang thai rieng cho qua trinh Freelancer tao de xuat ->
-- Nha tuyen dung xem -> chap nhan/tu choi.
--
-- LY DO THEM:
-- Form hop dong duoc tao tu giao dien Chat bang nut "Tao hop dong".
-- Sau khi Freelancer gui, hop dong can cho Nha tuyen dung chap nhan
-- truoc khi chuyen sang giai doan ky quy/thuc hien.
--
-- CODE MOI:
-- ALTER TABLE HopDong ADD
--     proposalStatus VARCHAR(20) NOT NULL
--         CONSTRAINT DF_HopDong_ProposalStatus DEFAULT 'DRAFT',
--     proposedAt DATETIME NULL,
--     acceptedAt DATETIME NULL,
--     rejectedAt DATETIME NULL,
--     rejectionReason NVARCHAR(500) NULL;
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_ProposalStatus
-- CHECK (proposalStatus IN ('DRAFT', 'SENT', 'ACCEPTED', 'REJECTED', 'CANCELLED'));
-- GO


-- =========================================================
-- 21.4 GHI NHAN XAC NHAN DIEU KHOAN
-- =========================================================
-- CODE CU:
-- Chua co du lieu ghi nhan viec nguoi tao da xac nhan noi dung hop dong.
--
-- LY DO THEM:
-- Form moi co buoc/xac nhan truoc khi gui de xuat hop dong.
-- Khong dung cot nay thay cho chu ky so; no chi ghi nhan xac nhan tren he thong.
--
-- CODE MOI:
-- ALTER TABLE HopDong ADD
--     termsConfirmed BIT NOT NULL
--         CONSTRAINT DF_HopDong_TermsConfirmed DEFAULT 0,
--     termsConfirmedAt DATETIME NULL;
-- GO

-- ALTER TABLE HopDong ADD CONSTRAINT CK_HopDong_TermsConfirmedTime
-- CHECK (
--        (termsConfirmed = 0 AND termsConfirmedAt IS NULL)
--     OR (termsConfirmed = 1 AND termsConfirmedAt IS NOT NULL)
-- );
-- GO


-- =========================================================
-- 21.5 MOC THANH TOAN HOP DONG
-- =========================================================
-- CODE CU:
-- HopDong chi co tongGiaTri, sotienkyquy, paymentStatus.
-- Khong co noi luu nhieu dot/moc thanh toan.
--
-- LY DO THEM:
-- Mot hop dong co the thanh toan theo nhieu moc.
-- Quan he: HopDong 1 --- N ContractMilestone.
-- Khong nen nhan ban cac cot moc1, moc2, moc3 trong HopDong.
--
-- CODE MOI:
CREATE TABLE ContractMilestone (
    milestoneID INT IDENTITY(1,1) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    milestoneOrder INT NOT NULL,
    milestoneName NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX) NULL,
    amount DECIMAL(18,2) NOT NULL,
    dueDate DATETIME NULL,
    status VARCHAR(20) NOT NULL
        CONSTRAINT DF_ContractMilestone_Status DEFAULT 'PENDING',
    completedAt DATETIME NULL,
    paidAt DATETIME NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL,

    CONSTRAINT FK_ContractMilestone_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD) ON DELETE CASCADE,

    CONSTRAINT UQ_ContractMilestone_Order
        UNIQUE (maHD, milestoneOrder),

    CONSTRAINT CK_ContractMilestone_Order
        CHECK (milestoneOrder > 0),

    CONSTRAINT CK_ContractMilestone_Amount
        CHECK (amount > 0),

    CONSTRAINT CK_ContractMilestone_Status
        CHECK (status IN ('PENDING', 'IN_PROGRESS', 'COMPLETED', 'PAID', 'CANCELLED'))
);
GO


-- =========================================================
-- 21.6 DANH MUC KY NANG
-- =========================================================
-- CODE CU:
-- DuAnPortfolio.congnghe chi la chuoi text.
-- HopDong chua co cau truc ky nang rieng.
--
-- LY DO THEM:
-- Form hop dong co the chon nhieu ky nang nhu React, Node.js,
-- MongoDB, Web Design. Can chuan hoa de tim kiem/loc/matching sau nay.
--
-- CODE MOI:
CREATE TABLE Skill (
    skillID INT IDENTITY(1,1) PRIMARY KEY,
    skillName NVARCHAR(100) NOT NULL,
    isActive BIT NOT NULL DEFAULT 1,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_Skill_Name UNIQUE (skillName)
);
GO


-- =========================================================
-- 21.7 KY NANG YEU CAU CUA HOP DONG
-- =========================================================
-- CODE CU:
-- Chua co bang lien ket HopDong voi Skill.
--
-- LY DO THEM:
-- Mot HopDong co nhieu Skill va mot Skill co the xuat hien trong nhieu HopDong.
-- Day la quan he N-N nen tach bang trung gian ContractSkill.
--
-- CODE MOI:
CREATE TABLE ContractSkill (
    maHD VARCHAR(20) NOT NULL,
    skillID INT NOT NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_ContractSkill PRIMARY KEY (maHD, skillID),

    CONSTRAINT FK_ContractSkill_HopDong
        FOREIGN KEY (maHD) REFERENCES HopDong(maHD) ON DELETE CASCADE,

    CONSTRAINT FK_ContractSkill_Skill
        FOREIGN KEY (skillID) REFERENCES Skill(skillID)
);
GO


-- =========================================================
-- 21.8 INDEX CHO CAC BANG MOI
-- =========================================================
-- CODE MOI:
-- LY DO THEM: Ho tro lay danh sach milestone va ky nang theo hop dong nhanh hon.
CREATE INDEX IX_ContractMilestone_MaHD_Status
ON ContractMilestone(maHD, status);
GO

CREATE INDEX IX_ContractSkill_SkillID
ON ContractSkill(skillID);
GO

CREATE INDEX IX_HopDong_ProposalStatus
ON HopDong(proposalStatus);
GO


-- =========================================================
-- 21.9 CODE CŨ UPDATE (ĐÃ COMMENT - KHÔNG CHẠY)
-- GHI CHÚ: Dữ liệu form mới đã được đưa trực tiếp vào INSERT HopDong HD001.
-- 21.9 DU LIEU MAU CHO FORM HOP DONG MOI
-- =========================================================
-- CODE CU:
-- HD001 da duoc tao va cap nhat tai muc 12.7 + 16.3.
--
-- LY DO KHONG TAO LAI HD001:
-- Neu INSERT lai se trung PRIMARY KEY.
-- Vi vay chi UPDATE cac cot moi de giu nguyen du lieu cu.
--
-- CODE MOI:
-- UPDATE HopDong
-- SET dieuKhoanChiTiet = N'- Freelancer thực hiện đúng phạm vi công việc đã thống nhất.\n- Hai bên trao đổi thay đổi yêu cầu qua hệ thống.\n- Thanh toán được xử lý theo cơ chế ký quỹ Escrow.',
--     paymentType = 'MILESTONE',
--     proposalStatus = 'ACCEPTED',
--     proposedAt = DATEADD(day, -3, GETDATE()),
--     acceptedAt = DATEADD(day, -2, GETDATE()),
--     termsConfirmed = 1,
--     termsConfirmedAt = DATEADD(day, -3, GETDATE()),
--     updatedAt = GETDATE()
-- WHERE maHD = 'HD001';
-- GO


-- ---------------------------------------------------------
-- 21.9.1 DU LIEU MAU KY NANG
-- ---------------------------------------------------------
INSERT INTO Skill (skillName)
VALUES
(N'React'),
(N'Node.js'),
(N'MongoDB'),
(N'Web Design');
GO

-- Gan ky nang cho HD001.
INSERT INTO ContractSkill (maHD, skillID)
SELECT 'HD001', skillID
FROM Skill
WHERE skillName IN (N'React', N'Node.js', N'MongoDB', N'Web Design');
GO


-- ---------------------------------------------------------
-- 21.9.2 DU LIEU MAU MOC THANH TOAN
-- ---------------------------------------------------------
-- Tong 3 moc = 2.000.000d, khop tongGiaTri demo cua HD001.
INSERT INTO ContractMilestone
(maHD, milestoneOrder, milestoneName, description, amount, dueDate, status)
VALUES
('HD001', 1, N'Hoàn thành thiết kế giao diện',
 N'Hoàn thành và bàn giao thiết kế giao diện đã thống nhất.',
 600000.00, DATEADD(day, 3, GETDATE()), 'IN_PROGRESS'),

('HD001', 2, N'Hoàn thành chức năng chính',
 N'Hoàn thành các chức năng chính theo phạm vi hợp đồng.',
 900000.00, DATEADD(day, 7, GETDATE()), 'PENDING'),

('HD001', 3, N'Kiểm thử và bàn giao',
 N'Sửa lỗi cuối, nghiệm thu và bàn giao sản phẩm.',
 500000.00, DATEADD(day, 10, GETDATE()), 'PENDING');
GO


-- =========================================================
-- 21.10 CAP NHAT VIEW DANH SACH HOP DONG DE KHOP FORM MOI
-- =========================================================
-- CODE CU:
-- VW_Freelancer_ContractList tai muc 15.9 chua tra ve cac cot moi:
-- dieuKhoanChiTiet, paymentType, proposalStatus, termsConfirmed.
--
-- LY DO SUA:
-- Can backend/UI co the lay thong tin moi ma khong xoa CREATE VIEW cu.
-- CREATE OR ALTER VIEW ben duoi la phien ban moi duoc ap dung sau cung.
--
-- CODE MOI:
CREATE OR ALTER VIEW VW_Freelancer_ContractList
AS
SELECT
    hd.maHD,
    hd.freeID,
    hd.jobID,
    hd.projectCode,
    hd.contractType,
    hd.tongGiaTri,
    hd.sotienkyquy,
    hd.tienDo,
    hd.hanHoanThanh,
    hd.ngayBatDau,
    hd.completedAt AS ngayKetThuc,
    hd.trangThai,
    hd.paymentStatus,
    hd.paymentType,
    hd.workMode,
    hd.fieldName,
    hd.dieuKhoanChiTiet,
    hd.proposalStatus,
    hd.proposedAt,
    hd.acceptedAt,
    hd.rejectedAt,
    hd.rejectionReason,
    hd.termsConfirmed,
    hd.termsConfirmedAt,
    hd.completedAt,
    hd.cancelledAt,
    hd.cancelReason,
    hd.createdAt,
    hd.updatedAt,
    jp.title AS jobTitle,
    jp.descr AS jobDescription,
    kh.cusID,
    nt.employerID,
    nt.companyName,
    nt.companyLogoUrl,
    nt.companyVerified,
    es.escrowID,
    es.status AS escrowStatus,
    es.amount AS escrowAmount
FROM HopDong hd
INNER JOIN JobPost jp ON hd.jobID = jp.jobID
INNER JOIN KhachHang kh ON jp.cusID = kh.cusID
LEFT JOIN NhaTuyenDung nt ON kh.cusID = nt.cusID
LEFT JOIN Escrow es ON hd.maHD = es.maHD;
GO


-- =========================================================
-- 21.11 VIEW CHI TIET HOP DONG + TONG HOP MOC THANH TOAN
-- =========================================================
-- CODE MOI:
-- LY DO THEM:
-- Ho tro trang chi tiet hop dong hien thi tong so moc,
-- so moc da thanh toan va tong tien da thanh toan.
CREATE VIEW VW_Contract_PaymentSummary
AS
SELECT
    hd.maHD,
    hd.tongGiaTri,
    hd.paymentType,
    COUNT(cm.milestoneID) AS totalMilestones,
    SUM(CASE WHEN cm.status = 'PAID' THEN 1 ELSE 0 END) AS paidMilestones,
    ISNULL(SUM(CASE WHEN cm.status = 'PAID' THEN cm.amount ELSE 0 END), 0) AS paidAmount,
    ISNULL(SUM(CASE WHEN cm.status <> 'CANCELLED' THEN cm.amount ELSE 0 END), 0) AS milestoneTotalAmount
FROM HopDong hd
LEFT JOIN ContractMilestone cm ON hd.maHD = cm.maHD
GROUP BY hd.maHD, hd.tongGiaTri, hd.paymentType;
GO


-- =========================================================
-- 21.12 QUERY LAY TOAN BO DU LIEU CHO MAN HINH CHI TIET HOP DONG
-- =========================================================
-- 1. Thong tin chinh cua hop dong
SELECT *
FROM VW_Freelancer_ContractList
WHERE maHD = 'HD001';
GO

-- 2. Danh sach ky nang cua hop dong
SELECT
    cs.maHD,
    s.skillID,
    s.skillName
FROM ContractSkill cs
INNER JOIN Skill s ON cs.skillID = s.skillID
WHERE cs.maHD = 'HD001'
ORDER BY s.skillName;
GO

-- 3. Danh sach moc thanh toan
SELECT
    milestoneID,
    maHD,
    milestoneOrder,
    milestoneName,
    description,
    amount,
    dueDate,
    status,
    completedAt,
    paidAt
FROM ContractMilestone
WHERE maHD = 'HD001'
ORDER BY milestoneOrder;
GO

-- 4. Tai lieu hop dong
SELECT *
FROM ContractDocument
WHERE maHD = 'HD001'
ORDER BY uploadedAt DESC;
GO

-- 5. Lich su hoat dong
SELECT *
FROM ContractActivityLog
WHERE maHD = 'HD001'
ORDER BY createdAt DESC;
GO

-- 6. Thong tin ky quy
SELECT *
FROM Escrow
WHERE maHD = 'HD001';
GO


-- =========================================================
-- 21.13 GHI CHU MAPPING FORM TAO HOP DONG MOI <-> DATABASE
-- =========================================================
-- FORM: Ten du an              -> JobPost.title
-- FORM: Mo ta du an            -> JobPost.descr
-- FORM: Ma du an               -> HopDong.projectCode
-- FORM: Freelancer             -> HopDong.freeID -> FreelancerSV
-- FORM: Nha tuyen dung         -> JobPost.cusID -> KhachHang -> NhaTuyenDung
-- FORM: Linh vuc               -> HopDong.fieldName
-- FORM: Ky nang                -> ContractSkill -> Skill
-- FORM: Loai hop dong          -> HopDong.contractType
-- FORM: Hinh thuc lam viec     -> HopDong.workMode
-- FORM: Ngay bat dau           -> HopDong.ngayBatDau
-- FORM: Deadline du kien       -> HopDong.hanHoanThanh
-- FORM: Ngay ket thuc thuc te  -> HopDong.completedAt
-- FORM: Tong gia tri           -> HopDong.tongGiaTri
-- FORM: So tien can ky quy     -> HopDong.sotienkyquy
-- THUC TE TIEN TRONG ESCROW    -> Escrow.amount
-- FORM: Kieu thanh toan        -> HopDong.paymentType
-- FORM: Cac moc thanh toan     -> ContractMilestone
-- FORM: Dieu khoan chi tiet    -> HopDong.dieuKhoanChiTiet
-- FORM: Tai lieu dinh kem      -> ContractDocument
-- FORM: Xac nhan dieu khoan    -> HopDong.termsConfirmed + termsConfirmedAt
-- TRANG THAI DE XUAT           -> HopDong.proposalStatus
-- TRANG THAI THUC HIEN         -> HopDong.trangThai
-- TRANG THAI THANH TOAN        -> HopDong.paymentStatus
-- LICH SU HOAT DONG            -> ContractActivityLog
-- =========================================================


-- =========================================================
-- 22. LUU Y CODE CU VAN DUOC GIU NGUYEN
-- =========================================================
-- [LUU Y 1]
-- HopDong.ngayKetThuc van duoc GIU NGUYEN vi la code cu.
-- Trong code backend moi nen quy uoc:
--   ngayBatDau     = ngay bat dau du kien/thuc te cua hop dong
--   hanHoanThanh   = deadline du kien
--   completedAt    = thoi diem hoan thanh thuc te
-- Khong nen dung ngayKetThuc va completedAt cho cung mot y nghia.
--
-- [LUU Y 2]
-- HopDong.sotienkyquy van duoc GIU NGUYEN.
-- Quy uoc moi:
--   sotienkyquy = so tien hop dong YEU CAU phai ky quy
--   Escrow.amount = so tien THUC TE dang/da duoc dua vao Escrow
--
-- [LUU Y 3]
-- VW_Freelancer_ContractList cu tai muc 15.9 van nam trong file.
-- Phien ban CREATE OR ALTER VIEW tai muc 21.10 chay sau nen se la
-- dinh nghia view duoc su dung cuoi cung.
--
-- [LUU Y 4]
-- Neu paymentType = 'MILESTONE', backend can kiem tra tong amount
-- cua cac ContractMilestone (khong CANCELLED) khong vuot tongGiaTri.
-- Quy tac tong nhieu dong nay nen xu ly bang transaction/service
-- hoac stored procedure, khong the hien day du bang CHECK constraint mot dong.
--
-- [LUU Y 5]
-- termsConfirmed chi la ghi nhan nguoi dung da xac nhan tren UI,
-- KHONG duoc xem nhu chu ky so/phap ly neu he thong chua co co che ky so.
-- =========================================================


-- =========================================================
-- 22. STORED PROCEDURE AD-UC-01 / AD-UC-02 - CODE MỚI
-- =========================================================
-- GHI CHÚ:
-- PATCH cũ được giữ ở dạng comment; procedure dưới đây là code chạy chính thức.
CREATE OR ALTER PROCEDURE SP_Admin_UpdateUserPhone
    @AdminUserID VARCHAR(20),
    @TargetUserID VARCHAR(20),
    @NewPhoneNumber VARCHAR(15),
    @Reason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OldPhoneNumber VARCHAR(15);

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
            FROM Admin a
            INNER JOIN Users u ON a.userID = u.userID
            WHERE a.userID = @AdminUserID AND u.status = 'ACTIVE'
        )
            THROW 50001, N'Tài khoản thực hiện không phải Admin đang hoạt động.', 1;

        SELECT @OldPhoneNumber = phoneNumber
        FROM Users WITH (UPDLOCK, HOLDLOCK)
        WHERE userID = @TargetUserID;

        IF @@ROWCOUNT = 0
            THROW 50002, N'Không tìm thấy tài khoản cần cập nhật.', 1;

        UPDATE Users
        SET phoneNumber = @NewPhoneNumber,
            updatedAt = GETDATE()
        WHERE userID = @TargetUserID;

        INSERT INTO UserAuditLog
        (adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
        VALUES
        (@AdminUserID, @TargetUserID, 'UPDATE', 'phoneNumber',
         @OldPhoneNumber, @NewPhoneNumber, @Reason);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ---------------------------------------------------------
-- 19.9 AD-UC-02: KHOA / TAM KHOA / MO KHOA TAI KHOAN + AUDIT
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_Admin_ChangeUserStatus
    @AdminUserID VARCHAR(20),
    @TargetUserID VARCHAR(20),
    @NewStatus VARCHAR(20),
    @Reason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OldStatus VARCHAR(20);
    DECLARE @ActionType VARCHAR(30);

    IF @NewStatus NOT IN ('ACTIVE', 'SUSPENDED', 'LOCKED', 'INACTIVE')
        THROW 50003, N'Trạng thái tài khoản không hợp lệ.', 1;

    IF NULLIF(LTRIM(RTRIM(@Reason)), N'') IS NULL
        THROW 50004, N'Phải nhập lý do thay đổi trạng thái.', 1;

    IF @AdminUserID = @TargetUserID
        THROW 50005, N'Admin không được tự khóa hoặc thay đổi trạng thái tài khoản của chính mình bằng chức năng này.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @OldStatus = status
        FROM Users WITH (UPDLOCK, HOLDLOCK)
        WHERE userID = @TargetUserID;

        IF @@ROWCOUNT = 0
            THROW 50006, N'Không tìm thấy tài khoản.', 1;

        IF @OldStatus = @NewStatus
            THROW 50007, N'Tài khoản đã ở trạng thái yêu cầu.', 1;

        UPDATE Users
        SET status = @NewStatus,
            updatedAt = GETDATE()
        WHERE userID = @TargetUserID;

        SET @ActionType = CASE WHEN @NewStatus = 'ACTIVE' THEN 'UNLOCK' ELSE 'LOCK' END;

        INSERT INTO UserAuditLog
        (adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
        VALUES
        (@AdminUserID, @TargetUserID, @ActionType, 'status', @OldStatus, @NewStatus, @Reason);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ---------------------------------------------------------


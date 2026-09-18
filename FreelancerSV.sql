CREATE DATABASE QL_Freelancer1;
GO

USE QL_Freelancer1;
GO


USE QL_Freelancer1;
GO

-- Xóa tất cả Foreign Key
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 
    N'ALTER TABLE ' 
    + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))
    + N'.' 
    + QUOTENAME(OBJECT_NAME(parent_object_id))
    + N' DROP CONSTRAINT ' 
    + QUOTENAME(name) 
    + N';' + CHAR(13)
FROM sys.foreign_keys;

EXEC sp_executesql @sql;
GO


-- Xóa tất cả TABLE
DECLARE @sql2 NVARCHAR(MAX) = N'';

SELECT @sql2 += 
    N'DROP TABLE ' 
    + QUOTENAME(SCHEMA_NAME(schema_id))
    + N'.' 
    + QUOTENAME(name) 
    + N';' + CHAR(13)
FROM sys.tables;

EXEC sp_executesql @sql2;
GO

-- 1. USERS

CREATE TABLE Users (
    userID VARCHAR(20) PRIMARY KEY,
    userName NVARCHAR(100) NOT NULL,

    passwordHash VARCHAR(255) NOT NULL,

    email VARCHAR(100) UNIQUE NOT NULL,
    phoneNumber VARCHAR(15),

    loaiUser NVARCHAR(20) NOT NULL
        CHECK (loaiUser IN (N'KhachHang', N'FreelancerSV', N'Admin')),

    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE'
        CHECK (status IN ('ACTIVE', 'SUSPENDED', 'LOCKED', 'INACTIVE')),

    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    updatedAt DATETIME NULL
);
GO


-- 2. USER AUDIT LOG

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


-- 3. ADMIN

CREATE TABLE Admin (
    userID VARCHAR(20) PRIMARY KEY,

    adminRole VARCHAR(30) NOT NULL DEFAULT 'ADMIN'
        CHECK (adminRole IN ('ADMIN', 'SUPER_ADMIN')),

    CONSTRAINT FK_Admin_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO


-- 4. KHACH HANG

CREATE TABLE KhachHang (
    cusID VARCHAR(20) PRIMARY KEY,

    userID VARCHAR(20) UNIQUE NOT NULL,

    rating FLOAT,

    CONSTRAINT FK_KhachHang_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO


-- 5. FREELANCER SINH VIEN

CREATE TABLE FreelancerSV (
    free_ID VARCHAR(20) PRIMARY KEY NOT NULL,

    userID VARCHAR(20) UNIQUE NOT NULL,

    university NVARCHAR(150),

    major NVARCHAR(100),

    studentCardID VARCHAR(20),
    GPA FLOAT,

    CONSTRAINT FK_Freelancer_User
        FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO


-- 21.6 DANH MUC KY NANG

CREATE TABLE Skill (
    skillID INT IDENTITY(1,1) PRIMARY KEY,
    skillName NVARCHAR(100) NOT NULL,
    isActive BIT NOT NULL DEFAULT 1,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_Skill_Name UNIQUE (skillName)
);
GO

-- Kỹ năng của Freelancer sinh viên
CREATE TABLE FreelancerSkill (
    freeID VARCHAR(20) NOT NULL,
    skillID INT NOT NULL,
    PRIMARY KEY (freeID, skillID),
    FOREIGN KEY (freeID) REFERENCES FreelancerSV(free_ID) ON DELETE CASCADE,
    FOREIGN KEY (skillID) REFERENCES Skill(skillID)
);


-- 6. WALLET

CREATE TABLE Wallet (
    walletID VARCHAR(20) PRIMARY KEY,
    userID VARCHAR(20) UNIQUE NOT NULL,

    soDuKhadung DECIMAL(18,2) NOT NULL DEFAULT 0,
    soDuDongBang DECIMAL(18,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_Wallet_User
        FOREIGN KEY (userID) REFERENCES Users(userID)
);
GO


-- 7. PORTFOLIO

CREATE TABLE Portfolio (
    portfolioID VARCHAR(20) PRIMARY KEY,
    free_ID VARCHAR(20) UNIQUE NOT NULL,
    moTaBanThan NVARCHAR(MAX),
    url_video VARCHAR(255),

    CONSTRAINT FK_Portfolio_Freelancer
        FOREIGN KEY (free_ID) REFERENCES FreelancerSV(free_ID)
);
GO


-- 8. DU AN PORTFOLIO

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


-- 9. JOB POST

CREATE TABLE JobPost (
    jobID VARCHAR(20) PRIMARY KEY,
    cusID VARCHAR(20) NOT NULL,
    title NVARCHAR(200) NOT NULL,
    descr NVARCHAR(MAX),

    thulao DECIMAL(18,2),

    thoigianthuchien DATETIME,
    status NVARCHAR(30) DEFAULT N'DangTuyen',
    soluongtuyen INT,

    CONSTRAINT FK_JobPost_KhachHang
        FOREIGN KEY (cusID) REFERENCES KhachHang(cusID)
);
GO

ALTER TABLE JobPost
ADD createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL;


-- ỨNG TUYỂN
CREATE TABLE UngTuyen (
    applicationID VARCHAR(20) PRIMARY KEY,
    jobID VARCHAR(20) NOT NULL,
    free_ID VARCHAR(20) NOT NULL,
    coverLetter NVARCHAR(MAX),
    appliedAt DATETIME NOT NULL DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING',

    CONSTRAINT FK_UngTuyen_Job
        FOREIGN KEY (jobID) REFERENCES JobPost(jobID),

    CONSTRAINT FK_UngTuyen_Freelancer
        FOREIGN KEY (free_ID) REFERENCES FreelancerSV(free_ID),

    CONSTRAINT UQ_UngTuyen_Job_Freelancer
        UNIQUE (jobID, free_ID),

    CONSTRAINT CK_UngTuyen_Status
        CHECK (status IN (
            'PENDING',
            'APPROVED',
            'REJECTED',
            'CANCELLED'
        ))
);


-- 10. HOP DONG

CREATE TABLE HopDong (
    maHD VARCHAR(20) PRIMARY KEY,

    jobID VARCHAR(20) NOT NULL,

    freeID VARCHAR(20) NOT NULL,
    ngayBatDau DATETIME DEFAULT GETDATE(),

    hanHoanThanh DATETIME NULL,
    completedAt DATETIME NULL,

    sotienkyquy DECIMAL(18,2) NULL,

    trangThai NVARCHAR(30) NOT NULL DEFAULT N'DangThucHien',

    projectCode VARCHAR(30) NULL,
    contractType VARCHAR(20) NOT NULL DEFAULT 'FIXED_PRICE',
    tongGiaTri DECIMAL(18,2) NULL,
    tienDo INT NOT NULL DEFAULT 0,
    paymentStatus VARCHAR(20) NOT NULL DEFAULT 'UNPAID',
    paymentType VARCHAR(30) NOT NULL DEFAULT 'ONE_TIME',
    workMode VARCHAR(20) NULL,
    fieldName NVARCHAR(150) NULL,
    dieuKhoanChiTiet NVARCHAR(MAX) NULL,

    proposalStatus VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    proposedAt DATETIME NULL,
    acceptedAt DATETIME NULL,
    rejectedAt DATETIME NULL,
    rejectionReason NVARCHAR(500) NULL,

    termsConfirmed BIT NOT NULL DEFAULT 0,
    termsConfirmedAt DATETIME NULL,

    cancelledAt DATETIME NULL,
    cancelReason NVARCHAR(500) NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE(),
    updatedAt DATETIME NULL,

    CONSTRAINT FK_HopDong_Job
        FOREIGN KEY (jobID) REFERENCES JobPost(jobID),

    CONSTRAINT FK_HopDong_Freelancer
        FOREIGN KEY (freeID) REFERENCES FreelancerSV(free_ID),

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


-- 11. DANH GIA

CREATE TABLE DanhGia (
    maDG VARCHAR(20) PRIMARY KEY,
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

    CONSTRAINT UQ_DanhGia_HopDong_NguoiDanhGia UNIQUE (maHD, nguoiDanhGiaID)
);
GO


-- 12. INSERT DU LIEU

-- 12.1 USERS
-- 5. Thứ tự cột và giá trị trong code cũ không khớp nhau.
INSERT INTO Users
(userID, userName, passwordHash, email, phoneNumber, status, createdAt, loaiUser)
VALUES
('USR001', N'admin_sys', 'hash_pass_123', 'admin@freelancer.vn', '0901111111', 'ACTIVE', GETDATE(), N'Admin'),
('USR002', N'khachhang_a', 'hash_pass_456', 'tuandaubo@gmail.com', '0902222222', 'ACTIVE', GETDATE(), N'KhachHang'),
('USR003', N'freelancer_sv1', 'hash_pass_789', 'nguyenvanSon@student.edu.vn', '0903333333', 'ACTIVE', GETDATE(), N'FreelancerSV');
GO

INSERT INTO Users
(userID, userName, passwordHash, email, phoneNumber, status, createdAt, loaiUser)
VALUES
('USR004', N'freelancer_sv2', 'hash_pass_004', 'freelancer2@student.edu.vn', '0904444444', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR005', N'freelancer_sv3', 'hash_pass_005', 'freelancer3@student.edu.vn', '0905555555', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR006', N'freelancer_sv4', 'hash_pass_006', 'freelancer4@student.edu.vn', '0906666666', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR007', N'freelancer_sv5', 'hash_pass_007', 'freelancer5@student.edu.vn', '0907777777', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR008', N'freelancer_sv6', 'hash_pass_008', 'freelancer6@student.edu.vn', '0908888888', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR009', N'freelancer_sv7', 'hash_pass_009', 'freelancer7@student.edu.vn', '0909999999', 'ACTIVE', GETDATE(), N'FreelancerSV'),

('USR010', N'freelancer_sv8', 'hash_pass_010', 'freelancer8@student.edu.vn', '0910000010', 'ACTIVE', GETDATE(), N'FreelancerSV');
GO

-- 12.2 ADMIN
INSERT INTO Admin (userID, adminRole)
VALUES ('USR001', 'SUPER_ADMIN');
GO

-- 12.3 FREELANCER SINH VIEN
INSERT INTO FreelancerSV
(free_ID, userID, university, major, studentCardID, GPA)
VALUES
('FREE001', 'USR003', N'Đại học Bách Khoa', N'Công nghệ thông tin', 'SV20230001', 3.6);
GO

INSERT INTO FreelancerSV
(free_ID, userID, university, major, studentCardID, GPA)
VALUES
('FREE002', 'USR004', N'Đại học Kinh tế TP.HCM',
 N'Thương mại điện tử', 'SV20230002', 3.7),

('FREE003', 'USR005', N'Đại học Bách Khoa',
 N'Công nghệ thông tin', 'SV20230003', 3.5),

('FREE004', 'USR006', N'Đại học Khoa học Tự nhiên',
 N'Khoa học máy tính', 'SV20230004', 3.8),

('FREE005', 'USR007', N'Đại học Công nghệ Thông tin',
 N'Kỹ thuật phần mềm', 'SV20230005', 3.6),

('FREE006', 'USR008', N'Đại học Văn Lang',
 N'Thiết kế đồ họa', 'SV20230006', 3.4),

('FREE007', 'USR009', N'Đại học FPT',
 N'Kỹ thuật phần mềm', 'SV20230007', 3.9),

('FREE008', 'USR010', N'Đại học Công nghiệp TP.HCM',
 N'Marketing', 'SV20230008', 3.6);
GO

-- 12.4 KHACH HANG
INSERT INTO KhachHang (userID, cusID, rating)
VALUES ('USR002', 'CUS001', 4.8);
GO




INSERT INTO Skill (skillName, isActive)
VALUES
(N'UI/UX Design', 1),
(N'Figma', 1),
(N'Web Development', 1),
(N'ReactJS', 1),
(N'NodeJS', 1),
(N'Content Writing', 1),
(N'SEO', 1),
(N'Graphic Design', 1),
(N'Photoshop', 1),
(N'Illustrator', 1),
(N'Marketing', 1),
(N'Data Entry', 1);
GO




INSERT INTO FreelancerSkill (freeID, skillID)
VALUES

-- FREE001 - Công nghệ thông tin
('FREE001', 1),  -- UI/UX Design
('FREE001', 2),  -- Figma
('FREE001', 3),  -- Web Development
('FREE001', 4),  -- ReactJS

-- FREE002 - Thương mại điện tử
('FREE002', 3),  -- Web Development
('FREE002', 4),  -- ReactJS
('FREE002', 5),  -- NodeJS
('FREE002', 11), -- Marketing

-- FREE003 - Công nghệ thông tin
('FREE003', 3),  -- Web Development
('FREE003', 4),  -- ReactJS
('FREE003', 5),  -- NodeJS
('FREE003', 1),  -- UI/UX Design

-- FREE004 - Khoa học máy tính
('FREE004', 3),  -- Web Development
('FREE004', 8),  -- Graphic Design
('FREE004', 9),  -- Photoshop
('FREE004', 10), -- Illustrator

-- FREE005 - Kỹ thuật phần mềm
('FREE005', 3),  -- Web Development
('FREE005', 4),  -- ReactJS
('FREE005', 5),  -- NodeJS
('FREE005', 6),  -- Content Writing

-- FREE006 - Thiết kế đồ họa
('FREE006', 8),  -- Graphic Design
('FREE006', 9),  -- Photoshop
('FREE006', 10), -- Illustrator
('FREE006', 2),  -- Figma

-- FREE007 - Kỹ thuật phần mềm
('FREE007', 3),  -- Web Development
('FREE007', 4),  -- ReactJS
('FREE007', 5),  -- NodeJS
('FREE007', 1),  -- UI/UX Design

-- FREE008 - Marketing
('FREE008', 6),  -- Content Writing
('FREE008', 7),  -- SEO
('FREE008', 11), -- Marketing
('FREE008', 12); -- Data Entry

GO


-- 12.5 WALLET
INSERT INTO Wallet (walletID, userID, soDuKhadung, soDuDongBang)
VALUES
('WAL001', 'USR002', 10000000.00, 2000000.00),
('WAL002', 'USR003', 1500000.00, 0.00);
GO

-- 12.6 JOB POST
INSERT INTO JobPost
(jobID, cusID, title, descr, thulao, thoigianthuchien, status, soluongtuyen)
VALUES
('JOB001', 'CUS001', N'Thiết kế giao diện Website bằng Figma',
 N'Cần thiết kế 5 màn hình ứng dụng web', 2000000.00,
 DATEADD(day, 7, GETDATE()), N'Đang tuyển', 1);
GO

INSERT INTO JobPost
(jobID, cusID, title, descr, thulao, thoigianthuchien, status, soluongtuyen)
VALUES
('JOB002', 'CUS001',
 N'Xây dựng website thương mại điện tử',
 N'Cần Freelancer xây dựng website thương mại điện tử.',
 5000000.00,
 DATEADD(day, 15, GETDATE()),
 N'Đang tuyển',
 1),

('JOB003', 'CUS001',
 N'Thiết kế Logo thương hiệu',
 N'Thiết kế logo và bộ nhận diện thương hiệu.',
 3000000.00,
 DATEADD(day, 10, GETDATE()),
 N'Đang tuyển',
 1),

('JOB004', 'CUS001',
 N'Viết nội dung Marketing',
 N'Cần Freelancer viết nội dung cho Fanpage và Website.',
 1500000.00,
 DATEADD(day, 7, GETDATE()),
 N'Đang tuyển',
 1),

('JOB005', 'CUS001',
 N'Thiết kế Banner sự kiện',
 N'Thiết kế banner cho chiến dịch truyền thông.',
 2000000.00,
 DATEADD(day, 12, GETDATE()),
 N'Đang tuyển',
 1);
GO

INSERT INTO UngTuyen
(applicationID, jobID, free_ID, coverLetter, appliedAt, status)
VALUES

-- JOB001 - 6 ung vien

('APP001', 'JOB001', 'FREE001',
 N'Em có kinh nghiệm thiết kế UI bằng Figma và có thể hoàn thành đúng deadline.',
 DATEADD(day, -10, GETDATE()), 'APPROVED'),

('APP002', 'JOB001', 'FREE002',
 N'Em có kinh nghiệm thiết kế giao diện web và mobile.',
 DATEADD(day, -9, GETDATE()), 'PENDING'),

('APP003', 'JOB001', 'FREE003',
 N'Em có kiến thức về UI/UX và Frontend.',
 DATEADD(day, -8, GETDATE()), 'PENDING'),

('APP004', 'JOB001', 'FREE004',
 N'Em từng thực hiện nhiều dự án thiết kế website.',
 DATEADD(day, -7, GETDATE()), 'REJECTED'),

('APP005', 'JOB001', 'FREE005',
 N'Em có thể thiết kế giao diện hiện đại, responsive.',
 DATEADD(day, -6, GETDATE()), 'PENDING'),

('APP006', 'JOB001', 'FREE006',
 N'Em chuyên thiết kế đồ họa và có kinh nghiệm sử dụng Figma.',
 DATEADD(day, -5, GETDATE()), 'PENDING'),


-- JOB002 - 7 ung vien

('APP007', 'JOB002', 'FREE001',
 N'Em có kiến thức về phát triển Web và Frontend.',
 DATEADD(day, -9, GETDATE()), 'PENDING'),

('APP008', 'JOB002', 'FREE002',
 N'Em có kinh nghiệm phát triển website thương mại điện tử.',
 DATEADD(day, -8, GETDATE()), 'APPROVED'),

('APP009', 'JOB002', 'FREE003',
 N'Em có thể phát triển website bằng React và Node.js.',
 DATEADD(day, -7, GETDATE()), 'PENDING'),

('APP010', 'JOB002', 'FREE004',
 N'Em có kinh nghiệm lập trình Web và cơ sở dữ liệu.',
 DATEADD(day, -6, GETDATE()), 'PENDING'),

('APP011', 'JOB002', 'FREE005',
 N'Em chuyên về phát triển phần mềm và Web.',
 DATEADD(day, -5, GETDATE()), 'REJECTED'),

('APP012', 'JOB002', 'FREE007',
 N'Em có thể xây dựng website hoàn chỉnh từ Frontend đến Backend.',
 DATEADD(day, -4, GETDATE()), 'PENDING'),

('APP013', 'JOB002', 'FREE008',
 N'Em có kinh nghiệm làm nội dung và quản trị website.',
 DATEADD(day, -3, GETDATE()), 'PENDING'),


-- JOB003 - 6 ung vien

('APP014', 'JOB003', 'FREE002',
 N'Em có kinh nghiệm thiết kế nhận diện thương hiệu.',
 DATEADD(day, -8, GETDATE()), 'PENDING'),

('APP015', 'JOB003', 'FREE004',
 N'Em có khả năng thiết kế Logo và các sản phẩm đồ họa.',
 DATEADD(day, -7, GETDATE()), 'PENDING'),

('APP016', 'JOB003', 'FREE005',
 N'Em từng tham gia nhiều dự án thiết kế thương hiệu.',
 DATEADD(day, -6, GETDATE()), 'APPROVED'),

('APP017', 'JOB003', 'FREE006',
 N'Em chuyên thiết kế đồ họa và Branding.',
 DATEADD(day, -5, GETDATE()), 'PENDING'),

('APP018', 'JOB003', 'FREE007',
 N'Em có khả năng sử dụng Photoshop và Illustrator.',
 DATEADD(day, -4, GETDATE()), 'REJECTED'),

('APP019', 'JOB003', 'FREE008',
 N'Em có kinh nghiệm thiết kế nội dung Marketing.',
 DATEADD(day, -3, GETDATE()), 'PENDING'),


-- JOB004 - 5 ung vien

('APP020', 'JOB004', 'FREE002',
 N'Em có kinh nghiệm viết Content Marketing.',
 DATEADD(day, -7, GETDATE()), 'PENDING'),

('APP021', 'JOB004', 'FREE005',
 N'Em từng quản lý nội dung cho Fanpage.',
 DATEADD(day, -6, GETDATE()), 'APPROVED'),

('APP022', 'JOB004', 'FREE006',
 N'Em có khả năng viết nội dung sáng tạo.',
 DATEADD(day, -5, GETDATE()), 'PENDING'),

('APP023', 'JOB004', 'FREE007',
 N'Em có kinh nghiệm SEO Content.',
 DATEADD(day, -4, GETDATE()), 'PENDING'),

('APP024', 'JOB004', 'FREE008',
 N'Em đang học Marketing và có kinh nghiệm viết bài.',
 DATEADD(day, -2, GETDATE()), 'REJECTED'),


-- JOB005 - 4 ung vien

('APP025', 'JOB005', 'FREE004',
 N'Em có kinh nghiệm thiết kế Banner và Poster.',
 DATEADD(day, -6, GETDATE()), 'PENDING'),

('APP026', 'JOB005', 'FREE005',
 N'Em sử dụng thành thạo Photoshop.',
 DATEADD(day, -5, GETDATE()), 'PENDING'),

('APP027', 'JOB005', 'FREE006',
 N'Em chuyên thiết kế đồ họa và Banner.',
 DATEADD(day, -4, GETDATE()), 'APPROVED'),

('APP028', 'JOB005', 'FREE008',
 N'Em có kinh nghiệm thiết kế nội dung truyền thông.',
 DATEADD(day, -2, GETDATE()), 'PENDING');

GO

-- 12.7 HOP DONG
INSERT INTO HopDong
(maHD, jobID, freeID, sotienkyquy, trangThai, ngayBatDau, ngayKetThuc)
VALUES
('HD001', 'JOB001', 'FREE001', 2000000.00,
 N'Đang thực hiện', GETDATE(), DATEADD(day, 10, GETDATE()));
GO

-- 12.8 DANH GIA
INSERT INTO DanhGia
(maDG, maHD, nguoiDanhGiaID, nguoiDuocDanhGiaID, soSao, nhanXet, ngayDanhGia)
VALUES
('DG001', 'HD001', 'USR002', 'USR003', 5,
 N'Sinh viên làm việc rất nhanh và đúng deadline.', GETDATE());
GO

-- 12.9 PORTFOLIO
-- 1. free_ID là FK tới FreelancerSV.free_ID nên phải dùng FREE001.
INSERT INTO Portfolio
(portfolioID, free_ID, moTaBanThan, url_video)
VALUES
('HS001', 'FREE001', N'Hồ sơ năng lực Lập trình & Thiết kế',
 'https://youtube.com/demo_portfolio');
GO

-- 12.10 DU AN PORTFOLIO
INSERT INTO DuAnPortfolio
(maDA, portfolioID, tenDuAn, moTa, vaiTro, congnghe, linkGithub, linkDemo, link_file)
VALUES
('DA001', 'HS001', N'Ứng dụng Quản lý Chi tiêu',
 N'App Flutter di động quản lý tài chính',
 N'Fullstack Developer', N'Flutter, Dart, Firebase',
 'https://github.com/demo/app', 'https://demo.app', 'https://file.app/spec.pdf');
GO


-- 13. VI DU AD-UC-01: ADMIN CAP NHAT THONG TIN TAI KHOAN

UPDATE Users
SET phoneNumber = '0908888888',
    updatedAt = GETDATE()
WHERE userID = 'USR002';

INSERT INTO UserAuditLog
(adminUserID, targetUserID, actionType, fieldName, oldValue, newValue, reason)
VALUES
('USR001', 'USR002', 'UPDATE', 'phoneNumber',
 '0902222222', '0908888888', N'Cập nhật số điện thoại khách hàng');
GO


-- 14. CAC TRUY VAN PHUC VU AD-UC-01

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


-- 15. BO SUNG DATABASE DE KHOP VOI CAC UI DA CHOT

-- 2. CAC PHAN DUOI DAY CHI BO SUNG / MO RONG CAU TRUC DE PHUC VU UI.
-- 3. KHACHHANG TRONG CSDL CU DUOC HIEU LA TAI KHOAN PHIA THUE DICH VU.

-- 15.1 MO RONG FREELANCERSV CHO UI ADMIN QUAN LY FREELANCER

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

ALTER TABLE FreelancerSV ADD CONSTRAINT CK_Freelancer_VerificationStatus
CHECK (verificationStatus IN ('PENDING', 'VERIFIED', 'REJECTED'));
GO


-- 15.2 BANG NHA TUYEN DUNG

CREATE TABLE NhaTuyenDung (
    employerID VARCHAR(20) PRIMARY KEY,
    cusID VARCHAR(20) UNIQUE NOT NULL,
    companyName NVARCHAR(200) NOT NULL,
    companyLogoUrl VARCHAR(500) NULL,
    industry NVARCHAR(150) NULL,
    companySize VARCHAR(30) NULL,
    companyWebsite VARCHAR(255) NULL,
    companyAddress NVARCHAR(300) NULL,
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


-- 15.3 MO RONG HOPDONG CHO UI "HOP DONG CUA TOI"

-- 15.4 KY QUY ESCROW CHO HOP DONG

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

    CONSTRAINT CK_Escrow_StatusTime CHECK (
        (status = 'PENDING'  AND releasedAt IS NULL AND refundedAt IS NULL)
     OR (status = 'FUNDED'   AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
     OR (status = 'RELEASED' AND fundedAt IS NOT NULL AND releasedAt IS NOT NULL AND refundedAt IS NULL)
     OR (status = 'REFUNDED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NOT NULL)
     OR (status = 'DISPUTED' AND fundedAt IS NOT NULL AND releasedAt IS NULL AND refundedAt IS NULL)
    )
);
GO


-- 15.5 TAI LIEU HOP DONG

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

    CONSTRAINT CK_ContractDocument_FileSize
        CHECK (fileSizeKB IS NULL OR fileSizeKB >= 0)
);
GO


-- 15.6 LICH SU HOAT DONG HOP DONG

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


-- 15.7 VIEW PHUC VU UI ADMIN - DANH SACH FREELANCER SINH VIEN

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


-- 15.8 VIEW PHUC VU UI ADMIN - DANH SACH NHA TUYEN DUNG

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


-- 15.9 VIEW PHUC VU UI FREELANCER - DANH SACH HOP DONG CUA TOI

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


-- 15.10 INDEX PHUC VU TIM KIEM / LOC TREN UI

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

CREATE UNIQUE INDEX UX_NhaTuyenDung_TaxCode
ON NhaTuyenDung(taxCode)
WHERE taxCode IS NOT NULL;
GO


-- 16. DU LIEU MAU BO SUNG CHO CAC UI MOI

-- 16.1 BO SUNG HO SO FREELANCER CHO UI ADMIN
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

-- 16.2 THEM HO SO NHA TUYEN DUNG CHO KHACH HANG CU
INSERT INTO NhaTuyenDung
(employerID, cusID, companyName, industry, companySize,
 companyWebsite, companyAddress, taxCode, companyVerified,
 verifiedAt, description)
VALUES
('EMP001', 'CUS001', N'Công ty ABC', N'Công nghệ thông tin', '51-200',
 'https://abc.example.vn', N'TP. Hồ Chí Minh', '0312345678', 1,
 GETDATE(), N'Doanh nghiệp tuyển dụng freelancer cho các dự án công nghệ và thiết kế.');
GO

-- 16.3 CAP NHAT HOP DONG CU DE HIEN THI DUNG UI HOP DONG
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

INSERT INTO Escrow
(escrowID, maHD, amount, status, fundedAt)
VALUES
('ESC001', 'HD001', 2000000.00, 'FUNDED', GETDATE());
GO

INSERT INTO ContractDocument
(documentID, maHD, fileName, fileUrl, fileType, fileSizeKB, uploadedBy)
VALUES
('DOC001', 'HD001', N'Hop_dong_HD001.pdf',
 'https://example.com/contracts/Hop_dong_HD001.pdf', 'PDF', 1200, 'USR001');
GO

INSERT INTO ContractActivityLog
(maHD, actorUserID, actionType, description)
VALUES
('HD001', 'USR002', 'ACCEPTED', N'Khách hàng đã chấp nhận đề xuất hợp tác'),
('HD001', 'USR002', 'ESCROW_FUNDED', N'Khách hàng đã nạp tiền ký quỹ'),
('HD001', 'USR003', 'STARTED', N'Freelancer bắt đầu thực hiện dự án');
GO


-- 17. CAC QUERY MAU TRUC TIEP PHUC VU UI

-- 17.1 ADMIN -> DANH SACH FREELANCER SINH VIEN
SELECT *
FROM VW_Admin_FreelancerList
ORDER BY joinedAt DESC;
GO

SELECT COUNT(*) AS totalFreelancers
FROM VW_Admin_FreelancerList;
GO

SELECT COUNT(*) AS activeFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'ACTIVE';
GO

SELECT COUNT(*) AS suspendedFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'INACTIVE';
GO

SELECT COUNT(*) AS lockedFreelancers
FROM VW_Admin_FreelancerList
WHERE status = 'LOCKED';
GO

SELECT COUNT(*) AS newFreelancersLast7Days
FROM VW_Admin_FreelancerList
WHERE joinedAt >= DATEADD(day, -7, GETDATE());
GO

-- 17.2 ADMIN -> DANH SACH NHA TUYEN DUNG
SELECT *
FROM VW_Admin_EmployerList
ORDER BY joinedAt DESC;
GO

SELECT COUNT(*) AS totalEmployers
FROM VW_Admin_EmployerList;
GO

SELECT COUNT(*) AS activeEmployers
FROM VW_Admin_EmployerList
WHERE status = 'ACTIVE';
GO

SELECT COUNT(*) AS suspendedEmployers
FROM VW_Admin_EmployerList
WHERE status = 'INACTIVE';
GO

SELECT COUNT(*) AS lockedEmployers
FROM VW_Admin_EmployerList
WHERE status = 'LOCKED';
GO

SELECT COUNT(*) AS newEmployersLast7Days
FROM VW_Admin_EmployerList
WHERE joinedAt >= DATEADD(day, -7, GETDATE());
GO

-- 17.3 FREELANCER -> HOP DONG CUA TOI
SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
ORDER BY ngayBatDau DESC;
GO

SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND trangThai IN (N'DangThucHien', N'Đang thực hiện');
GO

SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND paymentStatus = 'PENDING';
GO

SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND trangThai IN (N'HoanThanh', N'Đã hoàn thành', N'Hoàn thành');
GO

SELECT *
FROM VW_Freelancer_ContractList
WHERE freeID = 'FREE001'
  AND cancelledAt IS NOT NULL;
GO


-- 19. PATCH SUA LOI / CHUAN HOA SAU KHI RA SOAT

-- 2. CAC LENH DUOI DAY LA PATCH BO SUNG DE KHAC PHUC CAC DIEM CHUA PHU HOP.

-- 19.1 HOPDONG.jobID: BO UNIQUE DE MOT JOB CO THE CO NHIEU HOP DONG
-- 19.2 DANHGIA.maHD: CHO PHEP HAI BEN DANH GIA NHAU
-- 19.3 CHUAN HOA USERS.status
-- 19.4 RANG BUOC DU LIEU TIEN TRONG HOP DONG
-- 19.5 NHA TUYEN DUNG: MA SO THUE KHONG DUOC TRUNG
-- 19.6 CONTRACT DOCUMENT: FILE SIZE KHONG AM
-- 19.7 ESCROW: BO SUNG RANG BUOC THOI GIAN THEO TRANG THAI
-- 19.8 AD-UC-01: UPDATE + AUDIT LOG PHAI CUNG MOT TRANSACTION
-- 19.9 AD-UC-02: KHOA / TAM KHOA / MO KHOA TAI KHOAN + AUDIT
-- 19.10 QUERY UI ADMIN SAU KHI THEM SUSPENDED
-- 21. BO SUNG DATABASE THEO FORM TAO HOP DONG MOI (GOP 4 BUOC -> 1 FORM)

-- 2. CAC PHAN CHUA PHU HOP DUOC GIU LAI VA BO SUNG PATCH O PHIA DUOI.
-- 3. CAC COT/BANG MOI PHUC VU FORM TAO HOP DONG DA CHOT TREN UI.

-- 21.1 BO SUNG NOI DUNG DIEU KHOAN CHI TIET CHO HOP DONG

-- 21.2 BO SUNG HINH THUC / LICH THANH TOAN

-- 21.3 BO SUNG TRANG THAI DE XUAT / XAC NHAN HOP DONG

-- 21.4 GHI NHAN XAC NHAN DIEU KHOAN

-- 21.5 MOC THANH TOAN HOP DONG

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




-- 21.7 KY NANG YEU CAU CUA HOP DONG

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


-- 21.8 INDEX CHO CAC BANG MOI

CREATE INDEX IX_ContractMilestone_MaHD_Status
ON ContractMilestone(maHD, status);
GO

CREATE INDEX IX_ContractSkill_SkillID
ON ContractSkill(skillID);
GO

CREATE INDEX IX_HopDong_ProposalStatus
ON HopDong(proposalStatus);
GO


-- 21.9 DU LIEU MAU CHO FORM HOP DONG MOI

-- 21.9.1 DU LIEU MAU KY NANG
INSERT INTO Skill (skillName)
VALUES
(N'React'),
(N'Node.js'),
(N'MongoDB'),
(N'Web Design');
GO

INSERT INTO ContractSkill (maHD, skillID)
SELECT 'HD001', skillID
FROM Skill
WHERE skillName IN (N'React', N'Node.js', N'MongoDB', N'Web Design');
GO

-- 21.9.2 DU LIEU MAU MOC THANH TOAN
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


-- 21.10 CAP NHAT VIEW DANH SACH HOP DONG DE KHOP FORM MOI

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


-- 21.11 VIEW CHI TIET HOP DONG + TONG HOP MOC THANH TOAN

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


-- 21.12 QUERY LAY TOAN BO DU LIEU CHO MAN HINH CHI TIET HOP DONG

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


-- 22. STORED PROCEDURE AD-UC-01 / AD-UC-02 - CODE MỚI

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

-- 19.9 AD-UC-02: KHOA / TAM KHOA / MO KHOA TAI KHOAN + AUDIT
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

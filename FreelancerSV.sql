CREATE DATABASE QL_Freelancer;
GO

USE QL_Freelancer;
GO

--  User
CREATE TABLE Users (
    userID VARCHAR(20) PRIMARY KEY,
    userName NVARCHAR(100) NOT NULL,
	pashwordHash VARCHAR(255) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phoneNumber VARCHAR(15),
    loaiUser NVARCHAR(20) CHECK (loaiUser IN (N'KhachHang', N'FreelancerSV', N'Admin'))
);

--  Admin
CREATE TABLE Admin (
    userID VARCHAR(20) PRIMARY KEY,
    adminRole INT DEFAULT 1,
    CONSTRAINT FK_Admin_User FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

-- KhachHang
CREATE TABLE KhachHang (
	cusID VARCHAR(20) PRIMARY KEY,
    userID VARCHAR(20) ,
    rating FLOAT,
	walletID VARCHAR(20),
    CONSTRAINT FK_KhachHang_User FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

-- FreelancerSV
CREATE TABLE FreelancerSV (
    free_ID VARCHAR(20) PRIMARY KEY NOT NULL, 
    userID VARCHAR(20) NOT NULL,
    university NVARCHAR(150),
    major VARCHAR(20),
    studentCardID VARCHAR(20),
	GPA FLOAT,
    CONSTRAINT FK_Freelancer_User FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

--  Wallet
CREATE TABLE Wallet (
    walletID VARCHAR(20) PRIMARY KEY,
    userID VARCHAR(20) UNIQUE NOT NULL,
    soDuKhadung DECIMAL(18,2) DEFAULT 0,
    soDuDongBang DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_Wallet_User FOREIGN KEY (userID) REFERENCES Users(userID)
);

-- Bảng Portfolio 
CREATE TABLE Portfolio (
    portfolioID VARCHAR(20) PRIMARY KEY,
    free_ID VARCHAR(20) UNIQUE NOT NULL, 
    moTaBanThan NVARCHAR(MAX),
    url_video VARCHAR(255),
    CONSTRAINT FK_Portfolio_Freelancer FOREIGN KEY (free_ID) REFERENCES FreelancerSV(free_ID)
);

-- DuAnPortfolio
CREATE TABLE DuAnPortfolio (
    maDA VARCHAR(20) PRIMARY KEY,
	portfolioID VARCHAR(20) NOT NULL,
    tenDuAn NVARCHAR(200) NOT NULL,
    moTa NVARCHAR(MAX),
	vaiTro	NVARCHAR(50),
	congnghe NVARCHAR(50),
	linkGithub VARCHAR(255),
    linkDemo VARCHAR(255),
	link_file VARCHAR(255),
    CONSTRAINT FK_DuAn_Portfolio FOREIGN KEY (portfolioID) REFERENCES Portfolio(portfolioID) ON DELETE CASCADE
);

-- JobPost
CREATE TABLE JobPost (
    jobID VARCHAR(20) PRIMARY KEY,
    cusID VARCHAR(20) NOT NULL, -- Khóa ngoại trỏ tới KhachHang (1-n)
    title NVARCHAR(200) NOT NULL,
    descr NVARCHAR(MAX),
    thulao FLOAT,
    thoigianthuchien DATETIME,
    status NVARCHAR(30) DEFAULT N'DangTuyen',
	soluongtuyen INT,
    CONSTRAINT FK_JobPost_KhachHang FOREIGN KEY (cusID) REFERENCES KhachHang(cusID)
);

-- HopDong
CREATE TABLE HopDong (
    maHD VARCHAR(20) PRIMARY KEY,
    jobID VARCHAR(20) UNIQUE NOT NULL, 
    freeID VARCHAR(20) NOT NULL,      
    ngayBatDau DATETIME DEFAULT GETDATE(),
    ngayKetThuc DATETIME,
    sotienkyquy FLOAT,
    trangThai NVARCHAR(30) DEFAULT N'DangThucHien',
    CONSTRAINT FK_HopDong_Job FOREIGN KEY (jobID) REFERENCES JobPost(jobID),
    CONSTRAINT FK_HopDong_Freelancer FOREIGN KEY (freeID) REFERENCES FreelancerSV(freeID)
);

-- DanhGia
CREATE TABLE DanhGia (
    maDG VARCHAR(20) PRIMARY KEY,
    maHD VARCHAR(20) UNIQUE NOT NULL,          
    nguoiDanhGiaID VARCHAR(20) NOT NULL,       
    nguoiDuocDanhGiaID VARCHAR(20) NOT NULL,   
    soSao INT CHECK (soSao BETWEEN 1 AND 5),
    nhanXet NVARCHAR(MAX),
    ngayDanhGia DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DanhGia_HopDong FOREIGN KEY (maHD) REFERENCES HopDong(maHD),
    CONSTRAINT FK_DanhGia_NguoiGui FOREIGN KEY (nguoiDanhGiaID) REFERENCES Users(userID),
    CONSTRAINT FK_DanhGia_NguoiNhan FOREIGN KEY (nguoiDuocDanhGiaID) REFERENCES Users(userID)
);
GO

INSERT INTO User (userID, userName, pashwordHash, email, phoneNumber, status, createdAt, loaiUser) VALUES
('USR001', 'admin_sys', 'hash_pass_123', 'admin@freelancer.vn', '0901111111', 'ACTIVE', GETDATE(), 'ADMIN'),
('USR002', 'khachhang_a', 'hash_pass_456', 'tuandaubo@gmail.com', '0902222222', 'ACTIVE', GETDATE(), 'CUS'),
('USR003', 'freelancer_sv1', 'hash_pass_789', 'nguyenvanSon@student.edu.vn', '0903333333', 'ACTIVE', GETDATE(), 'FREELC');

INSERT INTO Admin (userID, adminRole) VALUES
('USR001', 'SUPER_ADMIN');

INSERT INTO FreelancerSV (userID, university, major, studentCardID, GPA) VALUES
('USR003', N'Đại học Bách Khoa', N'Công nghệ thông tin', 'SV20230001', 3.6);

INSERT INTO KhachHang (userID, cusID, rating, walletID) VALUES
('USR002', 'CUS001', 4.8, NULL);

INSERT INTO Wallet (walletID, userID, soDuKhadung, soDuDongBang) VALUES
('WAL001', 'USR002', 10000000.00, 2000000.00),
('WAL002', 'USR003', 1500000.00, 0.00);

UPDATE KhachHang SET walletID = 'WAL001' WHERE cusID = 'CUS001';

INSERT INTO JobPost (jobID, cusID, title, descr, thulao, thoigianthuchien, status, soluongtuyen) VALUES
('JOB001', 'CUS001', N'Thiết kế giao diện Website bằng Figma', N'Cần thiết kế 5 màn hình ứng dụng web', 2000000.00, DATEADD(day, 7, GETDATE()), N'Đang tuyển', 1);

INSERT INTO HopDong (maHD, jobID, freeID, soTienKyQuy, trangThai, ngayBatDau, ngayKetThuc) VALUES
('HD001', 'JOB001', 'USR003', 2000000.00, N'Đang thực hiện', GETDATE(), DATEADD(day, 10, GETDATE()));

-- 3.6 Insert DanhGia
INSERT INTO DanhGia (maDG, maHD, nguoiDanhGiaID, nguoiDuocDanhGiaID, soSao, nhanXet, ngayDanhGia) VALUES
('DG001', 'HD001', 'USR002', 'USR003', 5, N'Sinh viên làm việc rất nhanh và đúng deadline.', GETDATE());

-- 3.7 Insert Portfolio & DuAnPortfolio
INSERT INTO Portfolio (portfolioID, free_ID, moTaBanThan, url_Video) VALUES
('HS001', 'USR003', N'Hồ sơ năng lực Lập trình & Thiết kế', 'https://youtube.com/demo_portfolio');

INSERT INTO DuAnPortfolio (maDA, portfolioID, tenDuAn, moTa, vaiTro, congNghe, linkGithub, linkDemo, link_file) VALUES
('DA001', 'HS001', N'Ứng dụng Quản lý Chi tiêu', N'App Flutter di động quản lý tài chính', N'Fullstack Developer', 'Flutter, Dart, Firebase', 'https://github.com/demo/app', 'https://demo.app', 'https://file.app/spec.pdf');
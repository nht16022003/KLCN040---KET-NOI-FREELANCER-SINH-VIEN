CREATE DATABASE KETNOI_FREELANCERSV
GO

USE KETNOI_FREELANCERSV
GO


CREATE TABLE Roles
(
    maRole INT PRIMARY KEY,
    tenRole NVARCHAR(30) NOT NULL,
    CONSTRAINT CK_Roles_maRole CHECK (maRole IN (1,2,3)),
    CONSTRAINT CK_ten_Role CHECK ((maRole = 1 AND tenRole = N'FreelancerStudent') OR 
    (maRole = 2 AND tenRole = N'NhaTuyenDung') OR (maRole = 3 AND tenRole = N'Admin'))
)
Go

CREATE TABLE Users
(
    maUser INT IDENTITY(1,1) PRIMARY KEY,
    hotenUser NVARCHAR(100)  NOT NULL,
    tenTaiKhoanUser VARCHAR(30) NOT NULL,
    pashwordHash VARCHAR(255) NOT NULL,
    emailUser VARCHAR(100) UNIQUE NOT NULL,
    sdtUser VARCHAR(15),
    ngaysinh Date NULL,
    status NVARCHAR(20)  NOT NULL DEFAULT N'ACTIVE',
    ngayTao  DATETIME2 NOT NULL DEFAULT GETDATE(),
    maRole INT NOT NULL,
    CONSTRAINT FK_Users_Roles Foreign Key(maRole) References Roles(maRole)
)
GO

ALTER TABLE Users
ADD avatarUrl NVARCHAR(500) NULL;
GO


CREATE TABLE Admin
(
    maAdmin INT IDENTITY(1,1) PRIMARY KEY,
    hotenAdmin NVARCHAR(30) NOT NULL,
    maUser INT NOT NULL,
    CONSTRAINT FK_Admin_Users Foreign Key(maUser) References Users(maUser),
    CONSTRAINT UQ_Admin_maUser UNIQUE(maUser)
)
GO

CREATE TABLE LichSu_XuLyTaiKhoan(
    
    maLichSu INT IDENTITY(1,1) PRIMARY KEY,
    maUser INT NOT NULL, --Tài khoản bị xử lý
    maAdmin INT NOT NULL,
    hanhDong NVARCHAR(20) NULL, --KHÓA TÀI KHOẢN, MỞ KHÓA TÀI KHOẢN
    lyDo NVARCHAR(500) NOT NULL, 
    trangThaiTruocKhiXuLy NVARCHAR(20) NULL,
    trangThaiSauKhiXuLy NVARCHAR(20) NULL,
    ngayXuLy DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_LichSuXuLyTaiKhoan_User Foreign Key (maUser) References Users(maUser),
    CONSTRAINT FK_LichSuXuLyTaiKhoan_Admin Foreign Key (maAdmin) References Admin(maAdmin)

)
GO




CREATE TABLE MinhChung_FreelancerStudents
(
    maMinhChung INT IDENTITY(1,1) PRIMARY KEY,
    maUser INT NOT NULL,
    loaiMinhChung NVARCHAR(50) NOT NULL,
    --thẻ sinh viên, giấy xác nhận sinh viên
    fileMinhChung VARCHAR(500) NOT NULL,
    ngayNop DATETIME2 NOT NULL DEFAULT GETDATE(),
    ngayXacMinh DATETIME2 NULL,
    trangThaiGuiMinhChung NVARCHAR(20) NOT NULL DEFAULT N'Đang gửi', 
    lydoTuChoi NVARCHAR(500) NULL,
    nguoiXacMinh INT NULL, --Có thể admin xác minh
    CONSTRAINT FK_MinhChung_FreelancerStudents Foreign Key (maUser) References Users(maUser),
    Constraint CK_MinhChung_FreelancerStudents_trangThaiGuiMinhChung CHECK (trangThaiGuiMinhChung IN(N'Đang gửi', N'Đã xác minh', N'Đã từ chối')),
    CONSTRAINT FK_MinhChung_Admin FOREIGN KEY (nguoiXacMinh) REFERENCES Admin(maAdmin)
)
GO

CREATE TABLE ChuyenNganh
(
    maChuyenNganh VARCHAR(20) PRIMARY KEY,
    tenChuyenNganh NVARCHAR(150) NOT NULL
);

CREATE TABLE KyNang
(
    maKyNang INT IDENTITY(1,1) PRIMARY KEY,
    tenKyNang NVARCHAR(50) NOT NULL
)
Go


CREATE TABLE FreelancerStudents
(
    maFreelancerStudents INT IDENTITY PRIMARY KEY,
    maChuyenNganh VARCHAR(20) NOT NULL,
    maUser INT NOT NULL, --Foreign
    maTruong CHAR(10) NOT NULL,
    tenTruong NVARCHAR(150) NOT NULL,
    diaDiemTruong NVARCHAR(100) NOT NULL,
    diaDiemFreelancerStudent NVARCHAR(100) NOT NULL,
    ngonNgu NVARCHAR(100) NULL, --Có thể là ngoại ngữ, này chưa phải là kỹ năng chuyên ngành,
    kyNangCoBan NVARCHAR(150) NULL, --Tin học văn phòng v..v..
    namThu INT NOT NULL,
    GPA float NOT NULL,
    nienKhoa VARCHAR(20) NOT NULL,
    gioithieu NVARCHAR(MAX) NULL,
    avatar VARCHAR(500) NULL,
    trangthaiNhanViec BIT NOT NULL DEFAULT 1, --1 đang nhận việc
    chiPhiTu DECIMAL(18,2) NULL CHECK (chiPhiTu >= 0),
    CONSTRAINT FK_FreelancerStudents_Users Foreign Key (maUser) References Users (maUser),
    CONSTRAINT FK_FreelancerStudents_ChuyenNganh Foreign Key (maChuyenNganh) References ChuyenNganh(maChuyenNganh),

    CONSTRAINT CK_FreelancerStudents_GPA CHECK (GPA >= 0 AND GPA <= 4),
    CONSTRAINT CK_FreelancerStudents_NamThu CHECK (namThu BETWEEN 1 AND 6),
    CONSTRAINT UQ_FreelancerStudents_maUser UNIQUE (maUser)
)
Go

ALTER TABLE FreelancerStudents
DROP COLUMN avatar;

CREATE TABLE FreelancerStudent_KyNang
(
    maFreelancerStudents INT NOT NULL,
    maKyNang INT NOT NULL,
    PRIMARY KEY (maFreelancerStudents, maKyNang),
    FOREIGN KEY (maFreelancerStudents) REFERENCES FreelancerStudents(maFreelancerStudents),
    FOREIGN KEY (maKyNang) REFERENCES KyNang (maKyNang)
)
Go

CREATE TABLE BaiDangTimViec_FreelancerStudent (
    maBaiDang VARCHAR(20) PRIMARY KEY, 
    maFreelancerStudent INT NOT NULL,  
    tieude NVARCHAR(200) NOT NULL,      
    mota NVARCHAR(MAX),                 
    kynang NVARCHAR(100),              
    mucGiaTu FLOAT,                     
    thoigiandang DATETIME NOT NULL DEFAULT GETDATE(),
    trangthai NVARCHAR(30) DEFAULT N'DangHienThi', 
    CONSTRAINT FK_BaiDang_Freelancer FOREIGN KEY (maFreelancerStudent) 
        REFERENCES FreelancerStudents(maFreelancerStudents) ON DELETE CASCADE
);
GO

SELECT * FROM BaiDangTimViec_FreelancerStudent;

CREATE TABLE NhaTuyenDung
(
    maNhaTuyenDung INT IDENTITY PRIMARY KEY,
    maUser INT NOT NULL, 
    avatar VARCHAR(500) NULL, 
    gioithieu NVARCHAR(MAX) NULL, 
    tencongty NVARCHAR(100) NULL, 
    linhvuc NVARCHAR(100) NULL,
    diachi NVARCHAR(200) NULL, 
    link VARCHAR(500) NULL,
    logo VARCHAR(500) NULL,
    ngayDangKy DATETIME2 NOT NULL DEFAULT GETDATE(),
    trangthai NVARCHAR(20) NOT NULL DEFAULT N'Active',
    sosaodanhgia Float NULL, 
    CONSTRAINT FK_NhaTuyenDung_Users Foreign Key (maUser) References Users(maUser), 
    CONSTRAINT UQ_NhaTuyenDung_maUser UNIQUE (maUser)
)
Go

ALTER TABLE NhaTuyenDung
DROP COLUMN avatar;

select * from users;


CREATE TABLE FreelancerYeuThich
(
    maBookMark INT IDENTITY PRIMARY KEY,
    maNhaTuyenDung INT NOT NULL,
    maFreelancerStudent INT NOT NULL,
    ghiChu NVARCHAR(255) NULL,
    ngayLuu DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_FreelancerYeuThich_NhaTuyenDung Foreign Key (maNhaTuyenDung) References NhaTuyenDung(maNhaTuyenDung), --Nha tuyển dụng lưu
    CONSTRAINT FK_FreelancerYeuThich_FreelancerStudents Foreign Key (maFreelancerStudent) References FreelancerStudents(maFreelancerStudents)
)
Go

SELECT * FROM NhaTuyenDung;
SELECT * FROM Wallet;

CREATE TABLE Wallet
(
    maWallet INT IDENTITY NOT NULL PRIMARY KEY,
    maUser INT NOT NULL, 
    soDuKhaDung DECIMAL (18,2) NOT NULL DEFAULT 0,
    soDuDongBang DECIMAL (18,2) NOT NULL DEFAULT 0 ,
    CONSTRAINT FK_Wallet_Users FOREIGN KEY (maUser) REFERENCES Users(maUser),
    CONSTRAINT UQ_Wallet_User UNIQUE (maUser),
    CONSTRAINT CK_Wallet_SoDuKhaDung CHECK (soDuKhaDung >= 0),
    CONSTRAINT CK_Wallet_SoDuDongBang CHECK (soDuDongBang >= 0)
)
Go


CREATE TABLE DieuChinhSoDu
(
    maDieuChinh INT IDENTITY(1,1) PRIMARY KEY, 
    maWallet INT NOT NULL, --Ví được điều chỉnh số dư
    maAdmin INT NOT NULL, 
    loaiDieuChinh NVARCHAR(20) NOT NULL, --Cong Tien, Tru Tien
    soTienDieuChinh DECIMAL(18,2) NOT NULL,
    soDuTruoc DECIMAL(18,2) NOT NULL,
    soDuSau DECIMAL(18,2) NOT NULL,
    lyDo NVARCHAR(500) NOT NULL, 
    ngayDieuChinh DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DieuChinhSoDu_Wallet Foreign Key (maWallet) References Wallet(maWallet),
    CONSTRAINT FK_DieuChinhSoDu_Admin Foreign Key (maAdmin) References Admin(maAdmin)
)
GO

CREATE TABLE CauHinhPhiHoaHong_PhiDangBai
(
    maCauHinh INT IDENTITY(1,1) PRIMARY KEY,
    tenCauHinh NVARCHAR(100) UNIQUE, --Phí đăng bài, Hoa hồng dự án
    giaTri DECIMAL(18,4) NOT NULL,
    loaiCauHinh NVARCHAR(20) NOT NULL, --Tiền mặt hoặc phần trăm
    moTa NVARCHAR(500) NULL, 
    maAdmin INT NOT NULL,
    ngayCapNhat DATETIME DEFAULT GETDATE(),
    hieuLuc BIT DEFAULT 1, --1 đang áp dụng
    CONSTRAINT FK_CauHinhPhiHoaHong_PhiDangBai_Admin Foreign Key (maAdmin) References Admin(maAdmin)
)
GO

CREATE TABLE YeuCauHoTro
(
    maYeuCau INT IDENTITY(1,1) PRIMARY KEY,
    maUser INT NOT NULL,
    loaiYeuCau NVARCHAR(50), --HỖ TRỢ NẠP TIỀN, BÁO CÁO VI PHẠM, LỖI HỆ THỐNG, KHIẾU NẠI, V..V..
    tieuDe NVARCHAR(255) NOT NULL,
    noiDung NVARCHAR(MAX) NOT NULL,
    fileDinhKem VARCHAR(500) NULL,
    trangThai NVARCHAR(50) DEFAULT 'ChoDuyen', --DangXuLy, DaDong
    phanHoiAdmin NVARCHAR(MAX) NULL, 
    maAdmin INT NULL,
    ngayGui DATETIME DEFAULT GETDATE(),
    ngayXuLy DATETIME NULL,
    CONSTRAINT FK_YeuCauHoTro_Users Foreign Key (maUser) References Users(maUser),
    CONSTRAINT FK_YeuCauHoTro_Admin Foreign Key (maAdmin) References Admin(maAdmin)
)
GO


CREATE TABLE LichSuGiaoDich (
    maGiaoDich INT IDENTITY(1,1) PRIMARY KEY,
    maWallet INT NOT NULL,
    loaiGiaoDich NVARCHAR(50) NOT NULL, -- 'NapTien', 'RutTien', 'KyQuyHopDong', 'NhanThuLao', 'HoanTienKyQuy'
    soTien DECIMAL(18,2) NOT NULL,
    soDuTruocGD DECIMAL(18,2) NOT NULL,
    soDuSauGD DECIMAL(18,2) NOT NULL,
    noiDung NVARCHAR(255),
    phuongThucThanhToan NVARCHAR(50),   -- 'VNPay', 'MoMo', 'ChuyenKhoanNganHang', 'ViNoiBo'
    maGiaoDichNgoai VARCHAR(100),
    trangThai NVARCHAR(30) DEFAULT N'ThanhCong',
    ngayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_LichSuGD_Wallet FOREIGN KEY (maWallet) REFERENCES Wallet(maWallet)
);
GO


CREATE TABLE YeuCauRutTien (
    maRutTien INT IDENTITY(1,1) PRIMARY KEY,
    maWallet INT NOT NULL,
    soTienRut DECIMAL(18,2) NOT NULL,
    tenNganHang NVARCHAR(100) NOT NULL,
    soTaiKhoan VARCHAR(50) NOT NULL,
    tenChuTaiKhoan NVARCHAR(100) NOT NULL,
    ngayYeuCau DATETIME DEFAULT GETDATE(),
    ngayXuLy DATETIME,
    trangThai NVARCHAR(30) DEFAULT N'ChoDuyet', -- 'ChoDuyet', 'DaChuyenKhoan', 'TuChoi'
    lyDoTuChoi NVARCHAR(255),
    maAdminXuLy INT NULL,
    CONSTRAINT FK_RutTien_Wallet FOREIGN KEY (maWallet) REFERENCES Wallet(maWallet),
    CONSTRAINT FK_YeuCauRutTien_Admin FOREIGN KEY (maAdminXuLy) REFERENCES Admin(maAdmin)
);
GO



CREATE TABLE Portfolio (
    maPortfolio VARCHAR (20)   PRIMARY KEY,
    maFreelancerStudents INT UNIQUE NOT NULL,
    moTaBanThan NVARCHAR (MAX),
    url_video   VARCHAR (255) ,
    CONSTRAINT FK_Portfolio Foreign Key (maFreelancerStudents) References FreelancerStudents(maFreelancerStudents)
    )
GO

-- DuAnPortfolio
CREATE TABLE DuAn_Trong_Portfolio (
    maDA VARCHAR (20)   PRIMARY KEY,
    maPortfolio VARCHAR (20)   NOT NULL,
    tenDuAn NVARCHAR (200) NOT NULL,
    moTa NVARCHAR (MAX) NULL,
    vaiTro NVARCHAR (50)  NULL,
    congnghe NVARCHAR (50)  NULL,
    linkGithub VARCHAR (255)  NULL,
    linkDemo VARCHAR (255)  NULL,
    link_file VARCHAR (255)  NULL,
    CONSTRAINT FK_DuAn_Portfolio FOREIGN KEY (maPortfolio) REFERENCES Portfolio(maPortfolio) ON DELETE CASCADE
);

-- JobPost
CREATE TABLE JobPost (
    maJob VARCHAR (20) PRIMARY KEY,
    maNhaTuyenDung INT NOT NULL, -- Khóa ngoại trỏ tới KhachHang (1-n)
    tieude NVARCHAR (200) NOT NULL,
    mota NVARCHAR (MAX),
    kynangyeucau NVARCHAR(100),
    thulao DECIMAL(18,2),
    fileDinhKem VARCHAR(500) NULL,
    phiDangBai DECIMAL(18,2) NOT NULL,
    thoigiandangtuyen DATETIME NOT NULL DEFAULT GETDATE(),
    thoigiandukienhoanthanh DATETIME NOT NULL,
    status NVARCHAR (30)  DEFAULT N'DangTuyen',
    soluongtuyen INT           ,
    CONSTRAINT FK_JobPost_NhaTuyenDung FOREIGN KEY (maNhaTuyenDung) REFERENCES NhaTuyenDung (maNhaTuyenDung)
);

SELECT * FROM Users;
SELECT * FROM NhaTuyenDung;

DELETE FROM Users
Where maUser = 10

SELECT * FROM Wallet;

SELECT * FROM FreelancerStudents;

SELECT * FROM NhaTuyenDung;

CREATE TABLE UngTuyen (
    maUngTuyen INT IDENTITY(1,1) PRIMARY KEY,
    maJob VARCHAR(20) NOT NULL,               
    maFreelancerStudent INT NOT NULL,         
    thuGioiThieu NVARCHAR(MAX),               
    thulaoDeXuat DECIMAL(18,2),                       
    thoiGianHoanThanhDeXuat VARCHAR(50),      
    fileCV NVARCHAR(255),                    
    ngayUngTuyen DATETIME DEFAULT GETDATE(),
    trangThaiUngTuyen NVARCHAR(30) DEFAULT N'ChoDuyet', 
    
    CONSTRAINT FK_UngTuyen_Job FOREIGN KEY (maJob) REFERENCES JobPost(maJob) ON DELETE CASCADE,
    CONSTRAINT FK_UngTuyen_Freelancer FOREIGN KEY (maFreelancerStudent) REFERENCES FreelancerStudents(maFreelancerStudents)
);



CREATE TABLE UngThue (
    maYeuCau INT IDENTITY(1,1) PRIMARY KEY,
    maNhaTuyenDung INT NOT NULL,              
    maFreelancerStudent INT NOT NULL,         
    tieuDeCongViec NVARCHAR(200) NOT NULL,   
    moTaCongViec NVARCHAR(MAX),               
    nganSachDeNghi DECIMAL(18,2),                     
    thoiHanDuKien VARCHAR(50),                
    ngayGui DATETIME DEFAULT GETDATE(),
    trangThai NVARCHAR(30) DEFAULT N'ChoPhanHoi', 
    lyDoTuChoi NVARCHAR(255),
    
    CONSTRAINT FK_YeuCau_NTD FOREIGN KEY (maNhaTuyenDung) REFERENCES NhaTuyenDung(maNhaTuyenDung),
    CONSTRAINT FK_YeuCau_Freelancer FOREIGN KEY (maFreelancerStudent) REFERENCES FreelancerStudents(maFreelancerStudents)
);


-- HopDong
CREATE TABLE HopDong (
    maHD        VARCHAR (20)  PRIMARY KEY,
    maJob       VARCHAR (20)  UNIQUE NOT NULL,
    maFreelancerStudent INT  NOT NULL,
    ngayBatDau  DATETIME DEFAULT GETDATE(),
    ngayKetThuc DATETIME,
    sotienkyquy DECIMAL(18,2),
    hinhthuclamviec NVARCHAR(20) NULL,
    trangThai   NVARCHAR (30) DEFAULT N'DangThucHien',
    CONSTRAINT FK_HopDong_Job FOREIGN KEY (maJob) REFERENCES JobPost (maJob),
    CONSTRAINT FK_HopDong_Freelancer FOREIGN KEY (maFreelancerStudent) REFERENCES FreelancerStudents(maFreelancerStudents)
);




CREATE TABLE BanGiao_SanPham
(
    maBanGiao INT IDENTITY PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    phienBan NVARCHAR(20) NOT NULL, --v1.0, v1.1 v..v..
    fileSanPhamUrl nvarchar(500) NOT NULL, 
    linkDemo NVARCHAR(500) NULL,
    motaBanGiao NVARCHAR(1000) NOT NULL,
    ngayNop DATETIME NOT NULL DEFAULT GETDATE(),
    trangThai NVARCHAR(30) NOT NULL DEFAULT 'ChoNghiemThu', --'YeuCauChinhSua, DaNghiemThu'
    phanHoi NVARCHAR(1000) NULL, 
    ngayPhanHoi DATETIME NULL,
    CONSTRAINT FK_BanGiao_SanPham_HopDong FOREIGN KEY (maHD) REFERENCES HopDong (maHD)
)
GO

CREATE TABLE YeuCauNapTien (
  maYeuCauNap INT IDENTITY(1,1) PRIMARY KEY,
  maWallet INT NOT NULL,
  soTien DECIMAL(18,2) NOT NULL CHECK (soTien > 0),
  maGiaoDichNganHang VARCHAR(100) NOT NULL,
  anhBienLai VARCHAR(500) NOT NULL,
  ngayGui DATETIME DEFAULT GETDATE(),
  trangThai NVARCHAR(30) DEFAULT N'ChoXacMinh',
  ghiChuAdmin NVARCHAR(500) NULL,
  ngayXacMinh DATETIME NULL,
  maAdminXuLy INT NULL,
  CONSTRAINT FK_YeuCauNapTien_Wallet FOREIGN KEY (maWallet) REFERENCES Wallet(maWallet),
  CONSTRAINT FK_YeuCauNapTien_Admin FOREIGN KEY (maAdminXuLy) REFERENCES Admin(maAdmin)
)
GO

CREATE TABLE TaiKhoanNganHang(

    maTKNH INT IDENTITY(1,1) PRIMARY KEY,
    maUser INT NOT NULL, 
    tenNganHang NVARCHAR(100) NOT NULL,
    chiNhanh NVARCHAR(100) NULL,
    soTaiKhoan VARCHAR(50) NOT NULL,
    tenChuTaiKhoan NVARCHAR(100) NOT NULL,
    laMacDinh BIT NOT NULL DEFAULT 1,
    ngayThem DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TaiKhoanNganHang_Users FOREIGN KEY (maUser) REFERENCES Users(maUser)

)
GO


CREATE TABLE YeuCauGiaHanDeadline
(
  maGiaHan INT IDENTITY(1,1) PRIMARY KEY,
  maHD VARCHAR(20) NOT NULL,
  freelancerYeuCau INT NOT NULL,
  deadlineCu DATETIME NOT NULL,
  deadlineMoiDeXuat DATETIME NOT NULL,
  lyDo NVARCHAR(1000) NOT NULL,
  trangThai NVARCHAR(30) DEFAULT N'ChoDuyet',
  ngayTao DATETIME DEFAULT GETDATE(),
  ngayPhanHoi DATETIME NULL,
  CONSTRAINT FK_GiaHan_HopDong FOREIGN KEY (maHD) REFERENCES HopDong(maHD),
  CONSTRAINT FK_GiaHan_Freelancer FOREIGN KEY (freelancerYeuCau) REFERENCES Users(maUser)
)
GO

CREATE TABLE TranhChap
(
    maTranhChap INT IDENTITY(1,1) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    nguoiTao INT NOT NULL,
    lyDo NVARCHAR(255) NOT NULL,
    moTaChiTiet NVARCHAR(500) NOT NULL,
    trangThai NVARCHAR(30) NOT NULL DEFAULT N'DangXacMinh',
    ketLuanAdmin NVARCHAR(MAX) NULL,
    ngayTao DATETIME DEFAULT GETDATE(),
    ngayGiaiQuyet DATETIME NULL,
    maAdminXuLy INT NULL,
    CONSTRAINT FK_TranhChap_HopDong FOREIGN KEY (maHD) REFERENCES HopDong(maHD),
    CONSTRAINT FK_TranhChap_NguoiTao FOREIGN KEY (nguoiTao) REFERENCES Users(maUser),
    CONSTRAINT FK_TranhChap_Admin FOREIGN KEY (maAdminXuLy) REFERENCES Admin(maAdmin)
)
GO


CREATE TABLE BANGCHUNG_TRANHCHAP
(
    maBangChung INT IDENTITY(1,1) PRIMARY KEY,
    maTranhChap INT NOT NULL,
    nguoiTaiLenBangChung INT NOT NULL,
    fileUrl NVARCHAR(500) NOT NULL,
    loaiFile NVARCHAR(50) NULL,
    ghiChu NVARCHAR(500) NULL,
    ngayTaiLen DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_BangChung_TranhChap FOREIGN KEY (maTranhChap) REFERENCES TranhChap(maTranhChap) ON DELETE CASCADE,
    CONSTRAINT FK_BangChung_NguoiTaiLen FOREIGN KEY (nguoiTaiLenBangChung) REFERENCES Users(maUser)
)
GO


CREATE TABLE PhongChat
(
    maPhongChat INT IDENTITY(1,1) PRIMARY KEY,
    maUserClient INT NOT NULL, 
    maFreelancerStudent INT NOT NULL,
    maJob VARCHAR(20) NULL,
    ngayTao DATETIME DEFAULT GETDATE(),
    trangThai NVARCHAR(20) DEFAULT N'Active',
    CONSTRAINT FK_PhongChat_Client FOREIGN KEY (maUserClient) REFERENCES Users(maUser),
    CONSTRAINT FK_PhongChat_FreelancerStudents FOREIGN KEY (maFreelancerStudent) REFERENCES FreelancerStudents(maFreelancerStudents),
    CONSTRAINT FK_PhongChat_JobPost FOREIGN KEY (maJob) REFERENCES JobPost(maJob)
)
GO

CREATE TABLE TinNhanChat 
(
    maTinNhan INT IDENTITY(1,1) PRIMARY KEY,
    maPhongChat INT NOT NULL,
    maNguoiGui INT NOT NULL,
    noiDung NVARCHAR(MAX) NULL,
    fileDinhKem VARCHAR(500) NULL,
    loaiTinNhan NVARCHAR(30) DEFAULT N'Text',
    daDoc BIT DEFAULT 0,
    ngayGui DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TinNhan_PhongChat FOREIGN KEY (maPhongChat) REFERENCES PhongChat(maPhongChat) ON DELETE CASCADE,
    CONSTRAINT FK_TinNhan_NguoiGui FOREIGN KEY (maNguoiGui) REFERENCES Users(maUser)
)
GO


CREATE TABLE FileGhimChat 
(
    maFileGhim INT IDENTITY(1,1) PRIMARY KEY,
    maPhongChat INT NOT NULL,
    maTinNhan INT NOT NULL,
    tenFile NVARCHAR(255) NOT NULL,
    fileUrl VARCHAR(500) NOT NULL,
    nguoiGhim INT NOT NULL,
    ngayGhim DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_FileGhim_PhongChat FOREIGN KEY (maPhongChat) REFERENCES PhongChat(maPhongChat),
    CONSTRAINT FK_FileGhim_TinNhan FOREIGN KEY (maTinNhan) REFERENCES TinNhanChat(maTinNhan),
    CONSTRAINT FK_FileGhim_NguoiGhim FOREIGN KEY (nguoiGhim) REFERENCES Users(maUser)
)
GO


CREATE TABLE DanhGia_NhanXet 
(
    maDanhGia INT IDENTITY(1,1) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    maNguoiDanhGia INT NOT NULL,
    maNguoiDuocDanhGia INT NOT NULL,
    soSao INT NOT NULL CHECK (soSao BETWEEN 1 AND 5),
    nhanXet NVARCHAR(1000) NULL,
    ngayDanhGia DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DanhGia_HopDong FOREIGN KEY (maHD) REFERENCES HopDong(maHD),
    CONSTRAINT FK_DanhGia_NguoiDanhGia FOREIGN KEY (maNguoiDanhGia) REFERENCES Users(maUser),
    CONSTRAINT FK_DanhGia_NguoiDuocDanhGia FOREIGN KEY (maNguoiDuocDanhGia) REFERENCES Users(maUser)
)
GO

CREATE TABLE ThongBao
(
    maThongBao INT IDENTITY(1,1) PRIMARY KEY, 
    maUser INT NOT NULL,
    tieuDe NVARCHAR(255) NOT NULL,
    noiDung NVARCHAR(1000) NOT NULL,
    loaiThongBao NVARCHAR(50) NOT NULL,
    linkDieuHuong VARCHAR(500) NULL,
    daDoc BIT DEFAULT 0 , -- 0 Chưa đọc
    ngayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ThongBao_Users Foreign Key (maUser) References Users(maUser)
)
GO

CREATE TABLE GiaiDoan_HopDong
(
    maGiaiDoan INT IDENTITY(1,1) PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL, 
    tenGiaiDoan NVARCHAR(200) NOT NULL, ---GIAI ĐOẠN 1 - THIẾT KẾ V..V..
    moTa NVARCHAR(1000) NULL,
    hanChot DATETIME NOT NULL,
    trangThai NVARCHAR(30) DEFAULT N'ChuaDat', --cHo duyet, DaNghiemThu
    ngayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_GiaiDoan_HopDong_HopDong Foreign Key (maHD) References HopDong(maHD)
)
GO


CREATE TABLE Task_CongViec
(
    maTask INT IDENTITY PRIMARY KEY,
    maHD VARCHAR(20) NOT NULL,
    maGiaiDoan INT NULL,
    tenTask NVARCHAR(500) NOT NULL,
    moTa NVARCHAR(1000) NOT NULL,
    hanHoanThanh DATETIME NULL, --này là hạn hoàn thành của task
    trangThai NVARCHAR(30) NOT NULL Default 'ChuaThucHien',--DangThucHien, ChoDuyet, HoanThanh
    ngayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Task_CongViec_HopDong FOREIGN KEY (maHD) REFERENCES HopDong (maHD),
    CONSTRAINT FK_Task_CongViec_GiaiDoan_HopDong Foreign Key (maGiaiDoan) References GiaiDoan_HopDong(maGiaiDoan)

)
GO

-- 1. ROLES (Phân quyền người dùng)

INSERT INTO Roles (maRole, tenRole) VALUES 
(1, N'FreelancerStudent'),
(2, N'NhaTuyenDung'),
(3, N'Admin');
GO


-- 2. USERS (Tài khoản người dùng)

-- maUser: 1, 2, 3 -> Freelancer | 4 -> Nhà tuyển dụng | 5 -> Admin
INSERT INTO Users (hotenUser, tenTaiKhoanUser, pashwordHash, emailUser, sdtUser, ngaysinh, status, ngayTao, maRole) 
VALUES
(N'Nguyễn Văn An', 'an_freelancer', 'hash_pass_123', 'an.nguyen@student.edu.vn', '0901234567', '2003-05-15', N'ACTIVE', GETDATE(), 1),
(N'Trần Thị Bích', 'bich_designer', 'hash_pass_456', 'bich.tran@student.edu.vn', '0912345678', '2002-11-20', N'ACTIVE', GETDATE(), 1),
(N'Lê Hoàng Long', 'long_coder', 'hash_pass_789', 'long.le@student.edu.vn', '0923456789', '2004-02-10', N'ACTIVE', GETDATE(), 1),
(N'Công Ty Công Nghệ ABC', 'employer_abc', 'hash_pass_abc', 'hr@abc-tech.com', '0283838383', '2015-08-12', N'ACTIVE', GETDATE(), 2),
(N'Quản Trị Viên Hệ Thống', 'admin_sys', 'hash_pass_admin', 'admin@freelancego.vn', '0988888888', '1995-01-01', N'ACTIVE', GETDATE(), 3);
GO

SELECT * FROM USERS;
SELECT * FROM FreelancerStudents;

-- 2.1. ADMIN
INSERT INTO Admin (hotenAdmin, maUser) VALUES (N'Quản Trị Viên Hệ Thống', 5);
GO

-- 3. MINH CHỨNG SINH VIÊN (Người xác minh là Admin - maUser = 5, maAdmin = 1)

INSERT INTO MinhChung_FreelancerStudents (maUser, loaiMinhChung, fileMinhChung, ngayNop, trangThaiGuiMinhChung, ngayXacMinh, lydoTuChoi, nguoiXacMinh)
VALUES
(1, N'Thẻ sinh viên', 'uploads/minhchung/user1_the_sinh_vien.jpg', GETDATE(), N'Đã xác minh', GETDATE(), NULL, 1),
(2, N'Giấy xác nhận sinh viên', 'uploads/minhchung/user2_giay_xac_nhan.pdf', GETDATE(), N'Đã xác minh', GETDATE(), NULL, 1),
(3, N'Thẻ sinh viên', 'uploads/minhchung/user3_the_sinh_vien.jpg', GETDATE(), N'Đang gửi', NULL, NULL, NULL);
GO

-- 1. Xem mã Nhà tuyển dụng của Nguyễn Hoàng Tuấn
SELECT u.maUser, u.hotenUser, u.maRole, ntd.maNhaTuyenDung 
FROM Users u 
LEFT JOIN NhaTuyenDung ntd ON u.maUser = ntd.maUser 
WHERE u.hotenUser LIKE N'%Nguyễn Hoàng Tuấn%';

-- 2. Xem các bài Job mà Nguyễn Hoàng Tuấn đã đăng
SELECT maJob, tieude, maNhaTuyenDung FROM JobPost WHERE maNhaTuyenDung = 2;

-- 3. Xem các đơn ứng tuyển đã nộp vào các bài Job trên
SELECT * FROM UngTuyen WHERE maJob IN (SELECT maJob FROM JobPost WHERE maNhaTuyenDung = 2);


-- 4. CHUYÊN NGÀNH

INSERT INTO ChuyenNganh (maChuyenNganh, tenChuyenNganh) 
VALUES
('CNTT', N'Công nghệ thông tin'),
('KTPM', N'Kỹ thuật phần mềm'),
('HTTT', N'Hệ thống thông tin'),
('KHMT', N'Khoa học máy tính'),
('KHDL', N'Khoa học dữ liệu'),
('TTNT', N'Trí tuệ nhân tạo'),
('ATTT', N'An toàn thông tin'),
('KTMT', N'Kỹ thuật máy tính'),
('TMDT', N'Thương mại điện tử'),
('QTKD', N'Quản trị kinh doanh'),
('MKT', N'Marketing'),
('TNNH', N'Tài chính - Ngân hàng'),
('KT', N'Kế toán'),
('NNA', N'Ngôn ngữ Anh'),
('NNT', N'Ngôn ngữ Trung Quốc'),
('TKDH', N'Thiết kế đồ họa'),
('TKTT', N'Thiết kế thời trang'),
('KTR', N'Kiến trúc'),
('LAW', N'Luật'),
('DL', N'Du lịch'),
('QTNH', N'Quản trị nhà hàng và dịch vụ ăn uống');
GO



INSERT INTO KyNang(tenKyNang)
VALUES
-- Kỹ năng của sinh viên 1 (KTPM)
( N'C# / ASP.NET Core'),
( N'SQL Server'),
( N'HTML/CSS/JavaScript'),
( N'Flutter'),
-- Kỹ năng của sinh viên 2 (Thiết kế đồ họa)
( N'Photoshop'),
( N'Illustrator'),
( N'Figma'),
( N'Canva'),
( N'Poster Design');
GO

-- 5. PROFILE FREELANCER STUDENTS


INSERT INTO FreelancerStudents (maChuyenNganh, maUser, maTruong, tenTruong, diaDiemTruong, diaDiemFreelancerStudent, ngonNgu, kyNangCoBan, namThu, GPA, nienKhoa, gioithieu, avatar, trangthaiNhanViec, chiPhiTu)
VALUES
('KTPM', 1, 'UIT01', N'Đại học Công nghệ Thông tin', N'Thủ Đức, TP.HCM', N'Thủ Đức, TP.HCM', N'Tiếng Anh', N'Word, Excel, PowerPoint', 3, 3.45, '2023-2027', N'Sinh viên ngành Kỹ thuật phần mềm, có kinh nghiệm phát triển website và ứng dụng web.', 'uploads/avatar/user1.jpg', 1, 100000),
('TKDH', 2, 'DHC01', N'Đại học Mỹ thuật Công nghiệp', N'Quận 3, TP.HCM', N'Quận 3, TP.HCM', N'Tiếng Anh', N'Word, PowerPoint', 4, 3.20, '2022-2026', N'Sinh viên thiết kế đồ họa, nhận thiết kế poster, banner và nội dung mạng xã hội.', 'uploads/avatar/user2.jpg', 1, 150000);
GO


INSERT INTO FreelancerStudent_KyNang (maFreelancerStudents, maKyNang)
VALUES
(1,2),
(1,3),
(1,5),
(2,1),
(2,2)


-- 7. NHÀ TUYỂN DỤNG

INSERT INTO NhaTuyenDung (maUser, avatar, gioithieu, tencongty, linhvuc, diachi, link, logo, ngayDangKy, trangthai, sosaodanhgia)
VALUES
(4, 'uploads/avatar/employer1.jpg', N'Công ty hoạt động trong lĩnh vực công nghệ và phát triển phần mềm.', N'Công ty TNHH Công Nghệ ABC', N'Công nghệ thông tin', N'Quận 1, TP.HCM', 'https://abc.com', 'uploads/logo/abc.png', GETDATE(), N'Active', 4.8);
GO


-- 8. VÍ TIỀN (WALLET)

INSERT INTO Wallet (maUser, soDuKhaDung, soDuDongBang)
VALUES
(1, 1500000, 0),
(2, 800000, 0),
(3, 2000000, 300000),
(4, 5000000, 1000000);
GO


-- 9. PORTFOLIO

INSERT INTO Portfolio (maPortfolio, maFreelancerStudents, moTaBanThan, url_video)
VALUES
('PORT_01', 1, N'Xin chào! Mình là An, sinh viên năm 3 KTPM đam mê phát triển web và ứng dụng di động. Từng hoàn thành nhiều đồ án thực tế.', 'https://youtube.com/watch?v=demo_an_port'),
('PORT_02', 2, N'Xin chào, mình là Bích, sinh viên thiết kế đồ họa với tư duy sáng tạo và khả năng biến ý tưởng thương hiệu thành hình ảnh trực quan.', 'https://youtube.com/watch?v=demo_bich_port');
GO


-- 10. DỰ ÁN TRONG PORTFOLIO

INSERT INTO DuAn_Trong_Portfolio (maDA, maPortfolio, tenDuAn, moTa, vaiTro, congnghe, linkGithub, linkDemo, link_file)
VALUES
('DA_01', 'PORT_01', N'Website Đặt Lịch Khám Bệnh Trực Tuyến', N'Hệ thống quản lý đặt khám bệnh viện, thông báo lịch hẹn qua email.', N'Full-stack Developer', N'ASP.NET Core MVC, SQL Server', 'https://github.com/demo/clinic-booking', 'https://clinic-demo.io', NULL),
('DA_02', 'PORT_01', N'Ứng Dụng Quản Lý Chi Tiêu Cá Nhân', N'App di động ghi chép thu chi hàng ngày và vẽ biểu đồ phân tích.', N'Mobile Developer', N'Flutter, SQLite', 'https://github.com/demo/expense-tracker', NULL, 'uploads/projects/app_release.apk'),
('DA_03', 'PORT_02', N'Bộ Nhận Diện Thương Hiệu Quán Cà Phê Mộc', N'Thiết kế trọn gói: Logo, Menu, Poster, Áo đồng phục và bao bì sản phẩm.', N'Graphic Designer', N'Photoshop, Illustrator', NULL, 'https://behance.net/gallery/cafe-moc-branding', 'uploads/projects/branding_mockup.pdf');
GO

-- 11. BÀI ĐĂNG TUYỂN DỤNG (JOB POST)

INSERT INTO JobPost (maJob, maNhaTuyenDung, tieude, mota, kynangyeucau, thulao, phiDangBai, thoigiandangtuyen, thoigiandukienhoanthanh, status, soluongtuyen)
VALUES
('JOB_01', 1, N'Thiết kế Poster & Banner sự kiện khai trương', N'Cần 1 bạn sinh viên thiết kế bộ ấn phẩm gồm 2 poster đứng, 3 banner Facebook và voucher giảm giá cho chuỗi cửa hàng.', N'Photoshop, Illustrator, Banner Design', 1500000,0, GETDATE(), '2026-10-01', N'DangTuyen', 1),
('JOB_02', 1, N'Xây dựng module Quản lý kho bằng ASP.NET MVC', N'Viết module nhập xuất tồn kho hàng, hỗ trợ in phiếu xuất kho dạng PDF/Crystal Reports.', N'C#, ASP.NET Core, SQL Server', 3500000,0, GETDATE(), '2026-10-15', N'DangThucHien', 1),
('JOB_03', 1, N'Phát triển Landing Page giới thiệu sản phẩm', N'Cần code giao diện Responsive chuẩn UI/UX từ file Figma có sẵn, ưu tiên HTML/CSS/Tailwind.', N'HTML, CSS, JavaScript, Figma', 1000000,0, GETDATE(), '2026-10-05', N'DangTuyen', 2);
GO

-- 12. HỢP ĐỒNG (HOP DONG)

INSERT INTO HopDong (maHD, maJob, maFreelancerStudent, ngayBatDau, ngayKetThuc, sotienkyquy, hinhthuclamviec, trangThai)
VALUES
('HD_2026_001', 'JOB_02', 1, GETDATE(), '2026-10-15', 3500000, N'Remote', N'DangThucHien');
GO

-- 13. LỊCH SỬ GIAO DỊCH

INSERT INTO LichSuGiaoDich (maWallet, loaiGiaoDich, soTien, soDuTruocGD, soDuSauGD, noiDung, phuongThucThanhToan, maGiaoDichNgoai, trangThai) VALUES
(4, N'KyQuyHopDong', 3500000, 8500000, 5000000, N'Ký quỹ hợp đồng HD_2026_001', N'ViNoiBo', 'GD_HD_001', N'ThanhCong'),
(4, N'NapTien', 2000000, 3000000, 5000000, N'Nạp tiền vào ví', N'VNPay', 'VNP_001', N'ThanhCong'),
(1, N'NhanThuLao', 1500000, 1500000, 3000000, N'Nhận thù lao từ hợp đồng', N'ViNoiBo', 'GD_HD_000', N'ThanhCong'),
(2, N'NapTien', 1000000, 800000, 1800000, N'Nạp tiền vào ví', N'MoMo', 'MOMO_001', N'ThanhCong');
GO

-- 14. YÊU CẦU RÚT TIỀN

INSERT INTO YeuCauRutTien (maWallet, soTienRut, tenNganHang, soTaiKhoan, tenChuTaiKhoan, ngayYeuCau, ngayXuLy, trangThai, lyDoTuChoi, maAdminXuLy) VALUES
(1, 500000, N'Vietcombank', '0123456789', N'Nguyễn Văn An', GETDATE(), NULL, N'ChoDuyet', NULL, NULL),
(2, 300000, N'Thương mại Cổ phần Á Châu', '0987654321', N'Trần Thị Bích', GETDATE(), GETDATE(), N'DaChuyenKhoan', NULL, 1);
GO

-- 15. BÀI ĐĂNG TÌM VIỆC CỦA FREELANCER

INSERT INTO BaiDangTimViec_FreelancerStudent (maBaiDang, maFreelancerStudent, tieude, mota, kynang, mucGiaTu, thoigiandang, trangthai) VALUES
('BD_01', 1, N'Nhận lập trình Website ASP.NET Core', N'Nhận phát triển website quản lý bán hàng, quản lý kho và các hệ thống web.', N'C#, ASP.NET Core, SQL Server', 1500000, GETDATE(), N'DangHienThi'),
('BD_02', 2, N'Nhận thiết kế Poster và Banner', N'Nhận thiết kế poster, banner Facebook, banner sự kiện và ấn phẩm quảng cáo.', N'Photoshop, Illustrator, Figma', 500000, GETDATE(), N'DangHienThi');
GO

-- 16. FREELANCER YÊU THÍCH

INSERT INTO FreelancerYeuThich (maNhaTuyenDung, maFreelancerStudent, ghiChu, ngayLuu) VALUES
(1, 1, N'Có kỹ năng ASP.NET Core phù hợp với các dự án web', GETDATE()),
(1, 2, N'Có khả năng thiết kế Poster và Banner', GETDATE());
GO

-- 17. ỨNG TUYỂN

INSERT INTO UngTuyen (maJob, maFreelancerStudent, thuGioiThieu, thulaoDeXuat, thoiGianHoanThanhDeXuat, fileCV, ngayUngTuyen, trangThaiUngTuyen) VALUES
('JOB_01', 2, N'Tôi có kinh nghiệm thiết kế poster, banner và các ấn phẩm truyền thông.', 1500000, N'7 ngày', N'uploads/cv/bich_cv.pdf', GETDATE(), N'DaDuyet'),
('JOB_02', 1, N'Tôi có kinh nghiệm phát triển ứng dụng web bằng ASP.NET Core và SQL Server.', 3500000, N'20 ngày', N'uploads/cv/an_cv.pdf', GETDATE(), N'DaDuyet'),
('JOB_03', 1, N'Tôi có thể xây dựng Landing Page responsive theo thiết kế Figma.', 1000000, N'10 ngày', N'uploads/cv/an_cv.pdf', GETDATE(), N'ChoDuyet');
GO

-- 18. YÊU CẦU THUÊ FREELANCER

INSERT INTO UngThue (maNhaTuyenDung, maFreelancerStudent, tieuDeCongViec, moTaCongViec, nganSachDeNghi, thoiHanDuKien, ngayGui, trangThai, lyDoTuChoi) VALUES
(1, 1, N'Phát triển module quản lý kho', N'Công ty muốn thuê freelancer phát triển module quản lý nhập xuất tồn kho bằng ASP.NET Core.', 3500000, N'20 ngày', GETDATE(), N'DaChapNhan', NULL),
(1, 2, N'Thiết kế bộ nhận diện sự kiện', N'Thiết kế poster, banner Facebook và voucher cho sự kiện khai trương.', 1500000, N'7 ngày', GETDATE(), N'DaChapNhan', NULL);
GO

-- 19. TASK CÔNG VIỆC

INSERT INTO Task_CongViec (maHD, tenTask, moTa, hanHoanThanh, trangThai, ngayCapNhat) VALUES
('HD_2026_001', N'Phân tích yêu cầu module quản lý kho', N'Phân tích chức năng nhập kho, xuất kho, tồn kho và xác định yêu cầu hệ thống.', '2026-09-25', N'HoanThanh', GETDATE()),
('HD_2026_001', N'Thiết kế cơ sở dữ liệu', N'Thiết kế các bảng và quan hệ phục vụ module quản lý kho.', '2026-09-28', N'DangThucHien', GETDATE()),
('HD_2026_001', N'Lập trình chức năng nhập kho', N'Xây dựng chức năng nhập sản phẩm vào kho.', '2026-10-03', N'ChuaThucHien', GETDATE()),
('HD_2026_001', N'Lập trình chức năng xuất kho', N'Xây dựng chức năng xuất sản phẩm khỏi kho.', '2026-10-07', N'ChuaThucHien', GETDATE());
GO

-- 20. BÀN GIAO SẢN PHẨM

INSERT INTO BanGiao_SanPham (maHD, phienBan, fileSanPhamUrl, linkDemo, motaBanGiao, ngayNop, trangThai, phanHoi, ngayPhanHoi) VALUES
('HD_2026_001', N'v1.0', N'uploads/products/HD_2026_001_v1.0.zip', N'https://demo.freelancergo.vn/kho', N'Bản đầu tiên của module quản lý kho.', GETDATE(), N'ChoNghiemThu', NULL, NULL),
('HD_2026_001', N'v1.1', N'uploads/products/HD_2026_001_v1.1.zip', N'https://demo.freelancergo.vn/kho-v11', N'Bổ sung chức năng nhập kho và sửa lỗi phiên bản trước.', GETDATE(), N'DaNghiemThu', N'Đã kiểm tra và nghiệm thu phiên bản.', GETDATE());
GO


-- 21. YÊU CẦU NẠP TIỀN


INSERT INTO YeuCauNapTien (maWallet, soTien, maGiaoDichNganHang, anhBienLai, ngayGui, trangThai, ghiChuAdmin, ngayXacMinh, maAdminXuLy)
VALUES
(1, 1000000, 'BANK_NAP_001', 'uploads/bienlai/bienlai_user1_001.jpg', GETDATE(), N'ChoXacMinh', NULL, NULL, NULL),
(2, 2000000, 'BANK_NAP_002', 'uploads/bienlai/bienlai_user2_001.jpg', GETDATE(), N'DaXacMinh', N'Đã kiểm tra giao dịch ngân hàng.', GETDATE(), 1),
(4, 5000000, 'BANK_NAP_003', 'uploads/bienlai/bienlai_company_001.jpg', GETDATE(), N'DaXacMinh', N'Giao dịch hợp lệ.', GETDATE(), 1);
GO

-- 22. TÀI KHOẢN NGÂN HÀNG

INSERT INTO TaiKhoanNganHang (maUser, tenNganHang, chiNhanh, soTaiKhoan, tenChuTaiKhoan, laMacDinh, ngayThem)
VALUES
(1, N'Vietcombank', N'Chi nhánh Thủ Đức', '0123456789', N'Nguyễn Văn An', 1, GETDATE()),
(2, N'ACB', N'Chi nhánh TP.HCM', '0987654321', N'Trần Thị Bích', 1, GETDATE()),
(4, N'Vietcombank', N'Chi nhánh Quận 1', '1234567890', N'Công Ty TNHH Công Nghệ ABC', 1, GETDATE());
GO

-- 23. YÊU CẦU GIA HẠN DEADLINE

INSERT INTO YeuCauGiaHanDeadline (maHD, freelancerYeuCau, deadlineCu, deadlineMoiDeXuat, lyDo, trangThai, ngayTao, ngayPhanHoi)
VALUES
('HD_2026_001', 1, '2026-10-15', '2026-10-20', N'Cần thêm thời gian để hoàn thiện chức năng xuất kho và kiểm thử hệ thống.', N'DaDuyet', GETDATE(), GETDATE());
GO

-- 24. TRANH CHẤP

INSERT INTO TranhChap (maHD, nguoiTao, lyDo, moTaChiTiet, ketLuanAdmin, ngayTao, ngayGiaiQuyet, maAdminXuLy)
VALUES
('HD_2026_001', 1, N'Yêu cầu thanh toán', N'Freelancer yêu cầu kiểm tra tiến độ thanh toán hợp đồng.', N'Admin kiểm tra và xác nhận hai bên tiếp tục thực hiện hợp đồng.', GETDATE(), GETDATE(), 1);
GO

-- 25. BẰNG CHỨNG TRANH CHẤP

INSERT INTO BANGCHUNG_TRANHCHAP (maTranhChap, nguoiTaiLenBangChung, fileUrl, loaiFile, ghiChu, ngayTaiLen)
VALUES
(1, 1, N'uploads/tranhchap/HD_2026_001_bangchung_01.pdf', N'PDF', N'File trao đổi và xác nhận tiến độ công việc.', GETDATE()),
(1, 4, N'uploads/tranhchap/HD_2026_001_bangchung_02.png', N'IMAGE', N'Ảnh chụp trao đổi giữa hai bên.', GETDATE());
GO

-- 26. PHÒNG CHAT

INSERT INTO PhongChat (maUserClient, maFreelancerStudent, maJob, ngayTao, trangThai)
VALUES
(4, 1, 'JOB_02', GETDATE(), N'Active'),
(4, 2, 'JOB_01', GETDATE(), N'Active');
GO

-- 27. TIN NHẮN CHAT

INSERT INTO TinNhanChat (maPhongChat, maNguoiGui, noiDung, fileDinhKem, loaiTinNhan, daDoc, ngayGui)
VALUES
(1, 4, N'Chào bạn, bên mình muốn trao đổi thêm về yêu cầu của dự án.', NULL, N'Text', 1, GETDATE()),
(1, 1, N'Chào anh/chị, em đã xem yêu cầu. Em có thể bắt đầu ngay.', NULL, N'Text', 1, GETDATE()),
(1, 4, N'Bạn có thể gửi bản thiết kế database trước không?', NULL, N'Text', 0, GETDATE()),
(2, 4, N'Bạn có thể gửi demo thiết kế poster không?', NULL, N'Text', 1, GETDATE()),
(2, 2, N'Dạ được, em sẽ gửi bản demo trong hôm nay.', NULL, N'Text', 1, GETDATE());
GO

-- 28. FILE GHIM CHAT

INSERT INTO FileGhimChat (maPhongChat, maTinNhan, tenFile, fileUrl, nguoiGhim, ngayGhim)
VALUES
(1, 1, N'Yêu cầu dự án.pdf', 'uploads/chat/yeu_cau_du_an.pdf', 4, GETDATE()),
(1, 2, N'Database Design.pdf', 'uploads/chat/database_design.pdf', 1, GETDATE());
GO

-- 29. ĐÁNH GIÁ / NHẬN XÉT

INSERT INTO DanhGia_NhanXet (maHD, maNguoiDanhGia, maNguoiDuocDanhGia, soSao, nhanXet, ngayDanhGia)
VALUES
('HD_2026_001', 4, 1, 5, N'Freelancer làm việc đúng tiến độ, giao tiếp tốt và đáp ứng yêu cầu kỹ thuật.', GETDATE()),
('HD_2026_001', 1, 4, 5, N'Khách hàng cung cấp yêu cầu rõ ràng và phối hợp tốt trong quá trình thực hiện.', GETDATE());
GO

-- 30. LỊCH SỬ XỬ LÝ TÀI KHOẢN
INSERT INTO LichSu_XuLyTaiKhoan (maUser, maAdmin, hanhDong, lyDo, trangThaiTruocKhiXuLy, trangThaiSauKhiXuLy, ngayXuLy) 
VALUES (3, 1, N'KhoaTaiKhoan', N'Vi phạm điều khoản: Bàn giao sản phẩm sao chép từ dự án mẫu mà không có sự đồng ý của khách hàng', N'ACTIVE', N'LOCKED', GETDATE()), (3, 1, N'MoKhoaTaiKhoan', N'Người dùng đã gửi giải trình hợp lệ và cam kết khắc phục bàn giao lại sản phẩm đạt chuẩn', N'LOCKED', N'ACTIVE', GETDATE());
GO

-- 31. ĐIỀU CHỈNH SỐ DƯ
INSERT INTO DieuChinhSoDu (maWallet, maAdmin, loaiDieuChinh, soTienDieuChinh, soDuTruoc, soDuSau, lyDo, ngayDieuChinh) 
VALUES (4, 1, N'CongTien', 500000.00, 4500000.00, 5000000.00, N'Hoàn trả số dư do sự cố nạp tiền qua cổng thanh toán liên ngân hàng bị gián đoạn', GETDATE()), (1, 1, N'TruTien', 100000.00, 1600000.00, 1500000.00, N'Khấu trừ phí xử lý khiếu nại tranh chấp theo phán quyết hòa giải của Admin', GETDATE());
GO

-- 32. CẤU HÌNH PHÍ VÀ HOA HỒNG
INSERT INTO CauHinhPhiHoaHong_PhiDangBai (tenCauHinh, giaTri, loaiCauHinh, moTa, maAdmin, ngayCapNhat, hieuLuc) 
VALUES (N'PhiDangBai', 50000.0000, N'TienMat', N'Phí xuất bản bài đăng tuyển dụng tiêu chuẩn trên hệ thống', 1, GETDATE(), 1), (N'HoaHongDuAn', 10.0000, N'PhanTram', N'Tỷ lệ chiết khấu hoa hồng nền tảng khi giải ngân hợp đồng Escrow', 1, GETDATE(), 1);
GO

-- 33. YÊU CẦU HỖ TRỢ
INSERT INTO YeuCauHoTro (maUser, loaiYeuCau, tieuDe, noiDung, fileDinhKem, trangThai, phanHoiAdmin, maAdmin, ngayGui, ngayXuLy) 
VALUES (4, N'HoTroNapTien', N'Tiền nạp chưa vào ví sau 30 phút chuyển khoản', N'Tôi đã chuyển khoản 1.000.000đ qua VietQR nhưng số dư ví chưa được cập nhật.', 'uploads/hotro/bienlai_1M.png', N'DangXuLy', N'Admin đang kiểm tra đối soát với cổng thanh toán MBBank.', 1, GETDATE(), GETDATE()), (1, N'LoiHeThong', N'Không tải được file zip sản phẩm bàn giao', N'Khi nhấn nút tải file bàn giao của hợp đồng HD_2026_001 thì bị báo lỗi 404.', NULL, N'ChoDuyen', NULL, NULL, GETDATE(), NULL), (2, N'KhieuNai', N'Khách hàng không phản hồi nghiệm thu sau 7 ngày', N'Em đã nộp phiên bản v1.1 đúng hạn nhưng khách hàng chưa phản hồi nhận xét.', 'uploads/hotro/bangchung_nopbai.png', N'ChoDuyen', NULL, NULL, GETDATE(), NULL);
GO

-- 34. THÔNG BÁO
INSERT INTO ThongBao (maUser, tieuDe, noiDung, loaiThongBao, linkDieuHuong, daDoc, ngayTao) 
VALUES (1, N'Hợp đồng đã được kích hoạt', N'Nhà tuyển dụng Công Ty Công Nghệ ABC đã ký quỹ Escrow cho hợp đồng HD_2026_001.', N'ViecLam', '/HopDong/Details/HD_2026_001', 1, GETDATE()), (1, N'Tiền thù lao đã vào ví', N'Bạn đã nhận được thù lao giải ngân 3.500.000đ từ hợp đồng hoàn thành.', N'ViTien', '/Wallet/Index', 0, GETDATE()), (4, N'Sản phẩm mới đã bàn giao', N'Freelancer Nguyễn Văn An đã nộp bàn giao v1.0 cho hợp đồng HD_2026_001.', N'ViecLam', '/HopDong/NghiemThu/HD_2026_001', 0, GETDATE()), (4, N'Yêu cầu nạp tiền đã duyệt', N'Yêu cầu nạp tiền 2.000.000đ của bạn đã được Admin xác minh thành công.', N'ViTien', '/Wallet/LichSuGiaoDich', 1, GETDATE()), (2, N'Ứng tuyển được duyệt', N'Hồ sơ ứng tuyển bài đăng JOB_01 của bạn đã được nhà tuyển dụng chấp thuận.', N'ViecLam', '/Freelancer/DanhSachNopTuyen', 0, GETDATE());
GO

-- 35. GIAI ĐOẠN HỢP ĐỒNG (MILESTONES)
INSERT INTO GiaiDoan_HopDong (maHD, tenGiaiDoan, moTa, hanChot, trangThai, ngayTao) 
VALUES ('HD_2026_001', N'Giai đoạn 1: Thiết kế giao diện Figma & Database', N'Hoàn thiện toàn bộ Wireframe, Prototype và thiết kế Schema cơ sở dữ liệu.', DATEADD(DAY, 7, GETDATE()), N'DaNghiemThu', GETDATE()), ('HD_2026_001', N'Giai đoạn 2: Lập trình Backend API & Controller', N'Xây dựng mã nguồn xử lý logic nghiệp vụ và phân quyền người dùng.', DATEADD(DAY, 20, GETDATE()), N'ChoDuyet', GETDATE()), ('HD_2026_001', N'Giai đoạn 3: Kiểm thử & Bàn giao nghiệm thu', N'Chạy thử nghiệm toàn diện, sửa lỗi và đóng gói tài liệu bàn giao sản phẩm.', DATEADD(DAY, 30, GETDATE()), N'ChuaDat', GETDATE());
GO



SELECT * FROM Roles;
SELECT * FROM Users;
SELECT * FROM Admin;
SELECT * FROM LichSu_XuLyTaiKhoan;
SELECT * FROM MinhChung_FreelancerStudents;
SELECT * FROM ChuyenNganh;
SELECT * FROM FreelancerStudents;

SELECT * FROM BaiDangTimViec_FreelancerStudent;
SELECT * FROM FreelancerStudent_KyNang;
SELECT * FROM NhaTuyenDung;
SELECT * FROM FreelancerYeuThich;
SELECT * FROM Wallet;
SELECT * FROM DieuChinhSoDu;
SELECT * FROM CauHinhPhiHoaHong_PhiDangBai;
SELECT * FROM YeuCauHoTro;
SELECT * FROM LichSuGiaoDich;
SELECT * FROM YeuCauRutTien;
SELECT * FROM Portfolio;
SELECT * FROM DuAn_Trong_Portfolio;
SELECT * FROM JobPost;
SELECT * FROM UngTuyen;
SELECT * FROM UngThue;
SELECT * FROM HopDong;
SELECT * FROM BanGiao_SanPham;
SELECT * FROM YeuCauNapTien;
SELECT * FROM TaiKhoanNganHang;
SELECT * FROM YeuCauGiaHanDeadline;
SELECT * FROM TranhChap;
SELECT * FROM BANGCHUNG_TRANHCHAP;
SELECT * FROM PhongChat;
SELECT * FROM TinNhanChat;
SELECT * FROM FileGhimChat;
SELECT * FROM DanhGia_NhanXet;
SELECT * FROM ThongBao;
SELECT * FROM GiaiDoan_HopDong;
SELECT * FROM Task_CongViec;





SELECT * FROM JobPost T, UngTuyen J
WHERE T.maJob = J.maJob


DROP TABLE Task_CongViec;
DROP TABLE GiaiDoan_HopDong;

DROP TABLE ThongBao;
DROP TABLE DanhGia_NhanXet;

DROP TABLE FileGhimChat;
DROP TABLE TinNhanChat;
DROP TABLE PhongChat;

DROP TABLE BANGCHUNG_TRANHCHAP;
DROP TABLE TranhChap;

DROP TABLE YeuCauGiaHanDeadline;
DROP TABLE TaiKhoanNganHang;
DROP TABLE YeuCauNapTien;

DROP TABLE BanGiao_SanPham;
DROP TABLE HopDong;

DROP TABLE UngThue;
DROP TABLE UngTuyen;
DROP TABLE JobPost;

DROP TABLE DuAn_Trong_Portfolio;
DROP TABLE Portfolio;

DROP TABLE YeuCauRutTien;
DROP TABLE LichSuGiaoDich;
DROP TABLE YeuCauHoTro;
DROP TABLE CauHinhPhiHoaHong_PhiDangBai;
DROP TABLE DieuChinhSoDu;

DROP TABLE FreelancerYeuThich;
DROP TABLE NhaTuyenDung;

DROP TABLE BaiDangTimViec_FreelancerStudent;
DROP TABLE KyNang;
DROP TABLE FreelancerStudents;

DROP TABLE ChuyenNganh;

DROP TABLE MinhChung_FreelancerStudents;
DROP TABLE LichSu_XuLyTaiKhoan;

DROP TABLE Admin;
DROP TABLE Wallet;

DROP TABLE Users;
DROP TABLE Roles;
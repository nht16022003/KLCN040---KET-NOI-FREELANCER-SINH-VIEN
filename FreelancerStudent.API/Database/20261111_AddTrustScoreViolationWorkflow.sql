IF COL_LENGTH('dbo.FreelancerStudents', 'diemUyTin') IS NULL
BEGIN
	ALTER TABLE dbo.FreelancerStudents
	ADD diemUyTin decimal(8,2) NOT NULL CONSTRAINT DF_FreelancerStudents_diemUyTin DEFAULT (5) WITH VALUES;
END;

IF COL_LENGTH('dbo.FreelancerStudents', 'soLanViPham') IS NULL
BEGIN
	ALTER TABLE dbo.FreelancerStudents
	ADD soLanViPham int NOT NULL CONSTRAINT DF_FreelancerStudents_soLanViPham DEFAULT (0) WITH VALUES;
END;

IF OBJECT_ID('dbo.BaoCaoViPhamHopDong', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.BaoCaoViPhamHopDong
	(
		MaBaoCao int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BaoCaoViPhamHopDong PRIMARY KEY,
		MaHD varchar(20) NOT NULL,
		MaNhaTuyenDungUser int NOT NULL,
		MaFreelancerStudent int NOT NULL,
		LoaiViPham nvarchar(100) NOT NULL,
		MoTa nvarchar(2000) NOT NULL,
		ThoiGianViPham datetime2 NULL,
		NgayBaoCao datetime2 NOT NULL,
		HanGiaiTrinh datetime2 NOT NULL,
		TrangThai nvarchar(40) NOT NULL,
		FreelancerDongY bit NULL,
		GiaiTrinh nvarchar(2000) NULL,
		NgayGiaiTrinh datetime2 NULL,
		PhanHoiNhaTuyenDung nvarchar(2000) NULL,
		NgayPhanHoiNhaTuyenDung datetime2 NULL,
		FreelancerDongYGiaHan bit NOT NULL CONSTRAINT DF_BaoCaoViPham_FreelancerDongYGiaHan DEFAULT (0),
		NhaTuyenDungDongYGiaHan bit NOT NULL CONSTRAINT DF_BaoCaoViPham_NhaTuyenDungDongYGiaHan DEFAULT (0),
		KetLuan nvarchar(50) NULL,
		MucDoViPham nvarchar(50) NULL,
		GhiChuAdmin nvarchar(2000) NULL,
		MaAdminUser int NULL,
		NgayXuLy datetime2 NULL,
		SoLanViPham int NULL,
		SoDiemTru decimal(8,2) NULL,
		DiemTruocXuLy decimal(8,2) NULL,
		DiemSauXuLy decimal(8,2) NULL,
		CONSTRAINT FK_BaoCaoViPhamHopDong_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDong(maHD),
		CONSTRAINT FK_BaoCaoViPhamHopDong_Freelancer FOREIGN KEY (MaFreelancerStudent) REFERENCES dbo.FreelancerStudents(maFreelancerStudents),
		CONSTRAINT FK_BaoCaoViPhamHopDong_NhaTuyenDung FOREIGN KEY (MaNhaTuyenDungUser) REFERENCES dbo.Users(maUser),
		CONSTRAINT FK_BaoCaoViPhamHopDong_Admin FOREIGN KEY (MaAdminUser) REFERENCES dbo.Users(maUser)
	);
	CREATE INDEX IX_BaoCaoViPhamHopDong_Freelancer_TrangThai ON dbo.BaoCaoViPhamHopDong(MaFreelancerStudent, TrangThai);
END;

IF OBJECT_ID('dbo.TepMinhChungViPham', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.TepMinhChungViPham
	(
		MaTep int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TepMinhChungViPham PRIMARY KEY,
		MaBaoCao int NOT NULL,
		MaNguoiTai int NOT NULL,
		LoaiTep nvarchar(30) NOT NULL,
		TenTep nvarchar(255) NOT NULL,
		DuongDan nvarchar(500) NOT NULL,
		NgayTai datetime2 NOT NULL,
		CONSTRAINT FK_TepMinhChungViPham_BaoCao FOREIGN KEY (MaBaoCao) REFERENCES dbo.BaoCaoViPhamHopDong(MaBaoCao) ON DELETE CASCADE,
		CONSTRAINT FK_TepMinhChungViPham_NguoiTai FOREIGN KEY (MaNguoiTai) REFERENCES dbo.Users(maUser)
	);
END;

IF OBJECT_ID('dbo.KhieuNaiViPham', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.KhieuNaiViPham
	(
		MaKhieuNai int IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhieuNaiViPham PRIMARY KEY,
		MaBaoCao int NOT NULL,
		MaFreelancerStudent int NOT NULL,
		NoiDung nvarchar(2000) NOT NULL,
		TrangThai nvarchar(30) NOT NULL,
		KetQuaAdmin nvarchar(2000) NULL,
		MaAdminUser int NULL,
		NgayTao datetime2 NOT NULL,
		NgayXuLy datetime2 NULL,
		CONSTRAINT FK_KhieuNaiViPham_BaoCao FOREIGN KEY (MaBaoCao) REFERENCES dbo.BaoCaoViPhamHopDong(MaBaoCao),
		CONSTRAINT FK_KhieuNaiViPham_Freelancer FOREIGN KEY (MaFreelancerStudent) REFERENCES dbo.FreelancerStudents(maFreelancerStudents),
		CONSTRAINT FK_KhieuNaiViPham_Admin FOREIGN KEY (MaAdminUser) REFERENCES dbo.Users(maUser)
	);
	CREATE UNIQUE INDEX IX_KhieuNaiViPham_MaBaoCao ON dbo.KhieuNaiViPham(MaBaoCao);
END;

IF OBJECT_ID('dbo.LichSuDiemUyTin', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.LichSuDiemUyTin
	(
		MaLichSu int IDENTITY(1,1) NOT NULL CONSTRAINT PK_LichSuDiemUyTin PRIMARY KEY,
		MaFreelancerStudent int NOT NULL,
		MaBaoCao int NULL,
		MaNguoiThucHien int NULL,
		LoaiThayDoi nvarchar(30) NOT NULL,
		LyDo nvarchar(500) NOT NULL,
		SoDiemThayDoi decimal(8,2) NOT NULL,
		DiemTruoc decimal(8,2) NOT NULL,
		DiemSau decimal(8,2) NOT NULL,
		NgayTao datetime2 NOT NULL,
		CONSTRAINT FK_LichSuDiemUyTin_Freelancer FOREIGN KEY (MaFreelancerStudent) REFERENCES dbo.FreelancerStudents(maFreelancerStudents),
		CONSTRAINT FK_LichSuDiemUyTin_BaoCao FOREIGN KEY (MaBaoCao) REFERENCES dbo.BaoCaoViPhamHopDong(MaBaoCao),
		CONSTRAINT FK_LichSuDiemUyTin_NguoiThucHien FOREIGN KEY (MaNguoiThucHien) REFERENCES dbo.Users(maUser)
	);
	CREATE INDEX IX_LichSuDiemUyTin_Freelancer_NgayTao ON dbo.LichSuDiemUyTin(MaFreelancerStudent, NgayTao);
END;

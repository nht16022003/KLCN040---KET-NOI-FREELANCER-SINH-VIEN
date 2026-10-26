SET XACT_ABORT ON;
BEGIN TRY
	BEGIN TRANSACTION;

	IF COL_LENGTH('dbo.HopDong', 'dieuKhoan') IS NULL
		ALTER TABLE dbo.HopDong ADD dieuKhoan NVARCHAR(MAX) NULL;

	IF COL_LENGTH('dbo.DanhGia_NhanXet', 'soSaoChatLuong') IS NULL
		ALTER TABLE dbo.DanhGia_NhanXet ADD soSaoChatLuong TINYINT NULL;
	IF COL_LENGTH('dbo.DanhGia_NhanXet', 'soSaoDungHan') IS NULL
		ALTER TABLE dbo.DanhGia_NhanXet ADD soSaoDungHan TINYINT NULL;
	IF COL_LENGTH('dbo.DanhGia_NhanXet', 'soSaoGiaoTiep') IS NULL
		ALTER TABLE dbo.DanhGia_NhanXet ADD soSaoGiaoTiep TINYINT NULL;
	IF COL_LENGTH('dbo.DanhGia_NhanXet', 'ngayCapNhat') IS NULL
		ALTER TABLE dbo.DanhGia_NhanXet ADD ngayCapNhat DATETIME NULL;
	IF COL_LENGTH('dbo.DanhGia_NhanXet', 'soLanChinhSua') IS NULL
		ALTER TABLE dbo.DanhGia_NhanXet ADD soLanChinhSua INT NOT NULL CONSTRAINT DF_DanhGia_NhanXet_soLanChinhSua DEFAULT (0);

	IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_DanhGia_NhanXet_soSaoChatLuong' AND parent_object_id = OBJECT_ID('dbo.DanhGia_NhanXet'))
		ALTER TABLE dbo.DanhGia_NhanXet ADD CONSTRAINT CK_DanhGia_NhanXet_soSaoChatLuong CHECK (soSaoChatLuong IS NULL OR soSaoChatLuong BETWEEN 1 AND 5);
	IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_DanhGia_NhanXet_soSaoDungHan' AND parent_object_id = OBJECT_ID('dbo.DanhGia_NhanXet'))
		ALTER TABLE dbo.DanhGia_NhanXet ADD CONSTRAINT CK_DanhGia_NhanXet_soSaoDungHan CHECK (soSaoDungHan IS NULL OR soSaoDungHan BETWEEN 1 AND 5);
	IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_DanhGia_NhanXet_soSaoGiaoTiep' AND parent_object_id = OBJECT_ID('dbo.DanhGia_NhanXet'))
		ALTER TABLE dbo.DanhGia_NhanXet ADD CONSTRAINT CK_DanhGia_NhanXet_soSaoGiaoTiep CHECK (soSaoGiaoTiep IS NULL OR soSaoGiaoTiep BETWEEN 1 AND 5);

	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_DanhGia_NhanXet_maHD_maNguoiDanhGia' AND object_id = OBJECT_ID('dbo.DanhGia_NhanXet'))
		CREATE UNIQUE INDEX UX_DanhGia_NhanXet_maHD_maNguoiDanhGia ON dbo.DanhGia_NhanXet(maHD, maNguoiDanhGia);
	IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhGia_NhanXet_maNguoiDuocDanhGia' AND object_id = OBJECT_ID('dbo.DanhGia_NhanXet'))
		CREATE INDEX IX_DanhGia_NhanXet_maNguoiDuocDanhGia ON dbo.DanhGia_NhanXet(maNguoiDuocDanhGia);

	IF OBJECT_ID('dbo.DanhGia_NhanXet_LichSu', 'U') IS NULL
	BEGIN
		CREATE TABLE dbo.DanhGia_NhanXet_LichSu
		(
			maLichSu INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DanhGia_NhanXet_LichSu PRIMARY KEY,
			maDanhGia INT NOT NULL,
			soSao INT NOT NULL,
			soSaoChatLuong TINYINT NULL,
			soSaoDungHan TINYINT NULL,
			soSaoGiaoTiep TINYINT NULL,
			nhanXet NVARCHAR(1000) NULL,
			ngayLuu DATETIME NOT NULL CONSTRAINT DF_DanhGia_NhanXet_LichSu_ngayLuu DEFAULT (GETDATE()),
			CONSTRAINT FK_DanhGia_NhanXet_LichSu_DanhGia FOREIGN KEY (maDanhGia) REFERENCES dbo.DanhGia_NhanXet(maDanhGia) ON DELETE CASCADE
		);
	END;

	IF COL_LENGTH('dbo.FreelancerStudents', 'diemDanhGiaTrungBinh') IS NULL
		ALTER TABLE dbo.FreelancerStudents ADD diemDanhGiaTrungBinh DECIMAL(3,2) NOT NULL CONSTRAINT DF_FreelancerStudents_diemDanhGiaTrungBinh DEFAULT (0);
	IF COL_LENGTH('dbo.FreelancerStudents', 'soLuotDanhGia') IS NULL
		ALTER TABLE dbo.FreelancerStudents ADD soLuotDanhGia INT NOT NULL CONSTRAINT DF_FreelancerStudents_soLuotDanhGia DEFAULT (0);
	IF COL_LENGTH('dbo.NhaTuyenDung', 'soLuotDanhGia') IS NULL
		ALTER TABLE dbo.NhaTuyenDung ADD soLuotDanhGia INT NOT NULL CONSTRAINT DF_NhaTuyenDung_soLuotDanhGia DEFAULT (0);

	IF COL_LENGTH('dbo.HopDong', 'ngayHoanThanh') IS NULL
		ALTER TABLE dbo.HopDong ADD ngayHoanThanh DATETIME NULL;
	IF COL_LENGTH('dbo.HopDong', 'ngayDongHopDong') IS NULL
		ALTER TABLE dbo.HopDong ADD ngayDongHopDong DATETIME NULL;
	IF COL_LENGTH('dbo.HopDong', 'thanhToanTuDong') IS NULL
		ALTER TABLE dbo.HopDong ADD thanhToanTuDong BIT NOT NULL CONSTRAINT DF_HopDong_thanhToanTuDong DEFAULT (0);

	;WITH FreelancerRating AS
	(
		SELECT f.maFreelancerStudents, CAST(ROUND(AVG(CAST(d.soSao AS DECIMAL(10,4))), 2) AS DECIMAL(3,2)) AS diem, COUNT(*) AS soLuot
		FROM dbo.FreelancerStudents f
		INNER JOIN dbo.DanhGia_NhanXet d ON d.maNguoiDuocDanhGia = f.maUser
		GROUP BY f.maFreelancerStudents
	)
	UPDATE f SET diemDanhGiaTrungBinh = r.diem, soLuotDanhGia = r.soLuot
	FROM dbo.FreelancerStudents f
	INNER JOIN FreelancerRating r ON r.maFreelancerStudents = f.maFreelancerStudents;

	;WITH EmployerRating AS
	(
		SELECT n.maNhaTuyenDung, CAST(ROUND(AVG(CAST(d.soSao AS DECIMAL(10,4))), 2) AS FLOAT) AS diem, COUNT(*) AS soLuot
		FROM dbo.NhaTuyenDung n
		INNER JOIN dbo.DanhGia_NhanXet d ON d.maNguoiDuocDanhGia = n.maUser
		GROUP BY n.maNhaTuyenDung
	)
	UPDATE n SET sosaodanhgia = r.diem, soLuotDanhGia = r.soLuot
	FROM dbo.NhaTuyenDung n
	INNER JOIN EmployerRating r ON r.maNhaTuyenDung = n.maNhaTuyenDung;

	COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
	THROW;
END CATCH;

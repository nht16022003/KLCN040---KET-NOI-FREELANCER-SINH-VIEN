IF COL_LENGTH('dbo.DuAn_Trong_Portfolio', 'laDuAnNoiBat') IS NULL
BEGIN
    ALTER TABLE dbo.DuAn_Trong_Portfolio
    ADD laDuAnNoiBat BIT NOT NULL
        CONSTRAINT DF_DuAn_Trong_Portfolio_laDuAnNoiBat DEFAULT (0);
END;

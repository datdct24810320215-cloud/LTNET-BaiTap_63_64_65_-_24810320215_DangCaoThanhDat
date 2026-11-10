/* =========================================================
   SCRIPT TẠO CSDL KaraokeDB
   Chạy trên SQL Server (localhost\SQLEXPRESS)
   ========================================================= */

USE master;
GO

IF DB_ID(N'KaraokeDB') IS NOT NULL
BEGIN
    ALTER DATABASE KaraokeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE KaraokeDB;
END
GO

CREATE DATABASE KaraokeDB;
GO

USE KaraokeDB;
GO

/* ---------------- 1. BẢNG PHÒNG HÁT ---------------- */
CREATE TABLE PhongHat
(
    MaPhong     INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong    NVARCHAR(50)  NOT NULL,
    LoaiPhong   NVARCHAR(30)  NOT NULL,
    GiaTheoGio  DECIMAL(18,2) NOT NULL CONSTRAINT CK_PhongHat_Gia CHECK (GiaTheoGio >= 0),
    TrangThai   NVARCHAR(20)  NOT NULL CONSTRAINT DF_PhongHat_TrangThai DEFAULT N'Trống'
);
GO

/* ---------------- 2. BẢNG DỊCH VỤ ---------------- */
CREATE TABLE DichVu
(
    MaDV    INT IDENTITY(1,1) PRIMARY KEY,
    TenDV   NVARCHAR(100) NOT NULL,
    DonGia  DECIMAL(18,2) NOT NULL CONSTRAINT CK_DichVu_DonGia CHECK (DonGia >= 0),
    TonKho  INT           NOT NULL CONSTRAINT DF_DichVu_TonKho DEFAULT 0
                          CONSTRAINT CK_DichVu_TonKho CHECK (TonKho >= 0)
);
GO

/* ---------------- 3. BẢNG HÓA ĐƠN ---------------- */
CREATE TABLE HoaDon
(
    MaHD            INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong         INT           NOT NULL,
    GioVao          DATETIME      NOT NULL CONSTRAINT DF_HoaDon_GioVao DEFAULT GETDATE(),
    GioRa           DATETIME      NULL,
    TongTienPhong   DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_TTP  DEFAULT 0,
    TongTienDichVu  DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_TTDV DEFAULT 0,
    TrangThai       BIT           NOT NULL CONSTRAINT DF_HoaDon_TrangThai DEFAULT 0, -- 0: đang mở, 1: đã thanh toán
    CONSTRAINT FK_HoaDon_PhongHat FOREIGN KEY (MaPhong) REFERENCES PhongHat(MaPhong)
);
GO

/* ---------------- 4. BẢNG CHI TIẾT DỊCH VỤ ---------------- */
CREATE TABLE ChiTietDichVu
(
    MaHD    INT           NOT NULL,
    MaDV    INT           NOT NULL,
    SoLuong INT           NOT NULL CONSTRAINT CK_CTDV_SoLuong CHECK (SoLuong > 0),
    DonGia  DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_ChiTietDichVu PRIMARY KEY (MaHD, MaDV),
    CONSTRAINT FK_CTDV_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_CTDV_DichVu FOREIGN KEY (MaDV) REFERENCES DichVu(MaDV)
);
GO

/* ---------------- DỮ LIỆU MẪU ---------------- */
INSERT INTO PhongHat (TenPhong, LoaiPhong, GiaTheoGio, TrangThai) VALUES
    (N'P101', N'VIP',       200000, N'Trống'),
    (N'P102', N'VIP',       200000, N'Trống'),
    (N'P201', N'Thường',    120000, N'Trống'),
    (N'P202', N'Thường',    120000, N'Trống'),
    (N'P301', N'Thường',    150000, N'Trống');
GO

INSERT INTO DichVu (TenDV, DonGia, TonKho) VALUES
    (N'Bia Tiger (lon)',    25000, 100),
    (N'Nước ngọt Coca',     15000, 200),
    (N'Đĩa trái cây',       80000,  50),
    (N'Khô mực nướng',      90000,  40),
    (N'Bắp rang bơ',        35000,  60);
GO

PRINT N'>>> Tạo CSDL KaraokeDB thành công!';
GO
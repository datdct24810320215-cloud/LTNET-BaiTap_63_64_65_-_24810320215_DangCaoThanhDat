-- ================== TẠO DATABASE ==================
USE master;
GO
IF DB_ID('GarageDB') IS NOT NULL
    DROP DATABASE GarageDB;
GO
CREATE DATABASE GarageDB;
GO
USE GarageDB;
GO

-- ================== TẠO BẢNG ==================
CREATE TABLE KhachHang (
    MaKH        INT IDENTITY(1,1) PRIMARY KEY,
    TenKH       NVARCHAR(100) NOT NULL,
    DienThoai   VARCHAR(15)  NOT NULL UNIQUE,
    DiaChi      NVARCHAR(200) NULL
);
GO
CREATE TABLE Xe (
    BienSo      VARCHAR(20) PRIMARY KEY,
    MaKH        INT NOT NULL,
    HieuXe      NVARCHAR(50) NOT NULL,
    NamSanXuat  INT NULL,
    CONSTRAINT FK_Xe_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO
CREATE TABLE PhuTung (
    MaPhuTung    INT IDENTITY(1,1) PRIMARY KEY,
    TenPhuTung   NVARCHAR(150) NOT NULL,
    DonGia       DECIMAL(18,2) NOT NULL CONSTRAINT CK_PhuTung_DonGia CHECK (DonGia >= 0),
    SoLuongTon   INT NOT NULL   CONSTRAINT CK_PhuTung_SoLuongTon CHECK (SoLuongTon >= 0)
);
GO
CREATE TABLE PhieuSuaChua (
    MaPhieu     INT IDENTITY(1,1) PRIMARY KEY,
    BienSo      VARCHAR(20) NOT NULL,
    NgayNhan    DATETIME NOT NULL CONSTRAINT DF_PSC_NgayNhan DEFAULT GETDATE(),
    NoiDungSua  NVARCHAR(MAX) NULL,
    TongTien    DECIMAL(18,2) NOT NULL CONSTRAINT DF_PSC_TongTien DEFAULT 0,
    TrangThai   NVARCHAR(30) NOT NULL CONSTRAINT DF_PSC_TrangThai DEFAULT N'Chưa thanh toán',
    CONSTRAINT FK_PSC_Xe FOREIGN KEY (BienSo) REFERENCES Xe(BienSo)
);
GO
CREATE TABLE ChiTietSuaChua (
    MaPhieu    INT NOT NULL,
    MaPhuTung  INT NOT NULL,
    SoLuong    INT NOT NULL CONSTRAINT CK_CTSC_SoLuong CHECK (SoLuong > 0),
    TienCong   DECIMAL(18,2) NOT NULL CONSTRAINT DF_CTSC_TienCong DEFAULT 0,
    CONSTRAINT PK_CTSC PRIMARY KEY (MaPhieu, MaPhuTung),
    CONSTRAINT FK_CTSC_Phieu   FOREIGN KEY (MaPhieu)   REFERENCES PhieuSuaChua(MaPhieu),
    CONSTRAINT FK_CTSC_PhuTung FOREIGN KEY (MaPhuTung) REFERENCES PhuTung(MaPhuTung)
);
GO

-- ================== DỮ LIỆU MẪU ==================
INSERT INTO KhachHang (TenKH, DienThoai, DiaChi) VALUES
(N'Nguyễn Văn A', '0901111111', N'Hà Nội'),
(N'Trần Thị B',   '0902222222', N'TP.HCM'),
(N'Lê Văn C',     '0903333333', N'Đà Nẵng');
GO
INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat) VALUES
('29A-12345', 1, N'Toyota Camry', 2020),
('51G-67890', 2, N'Honda Civic',  2019),
('43C-11111', 3, N'Mazda CX5',    2021);
GO
INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon) VALUES
(N'Dầu máy Castrol 5W-30',   450000,  50),
(N'Lọc gió động cơ',         250000,  30),
(N'Bugi NGK',                120000, 100),
(N'Má phanh trước',          850000,  20),
(N'Ắc quy GS 12V',          1800000,  15);
GO
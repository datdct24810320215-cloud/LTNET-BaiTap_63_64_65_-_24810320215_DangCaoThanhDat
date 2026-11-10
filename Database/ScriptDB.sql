-- =====================================================================
-- SCRIPT TẠO CSDL: ProjectTaskDB
-- Chạy 1 lần trong SSMS
-- =====================================================================

USE master;
GO
IF DB_ID('ProjectTaskDB') IS NOT NULL
BEGIN
    ALTER DATABASE ProjectTaskDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ProjectTaskDB;
END
GO
CREATE DATABASE ProjectTaskDB;
GO
USE ProjectTaskDB;
GO

-- ============= BẢNG DỰ ÁN =============
CREATE TABLE DuAn
(
    MaDuAn       INT IDENTITY(1,1) PRIMARY KEY,
    TenDuAn      NVARCHAR(200) NOT NULL,
    NgayBatDau   DATE NOT NULL,
    NgayKetThuc  DATE NULL,
    MoTa         NVARCHAR(500) NULL
);
GO

-- ============= BẢNG THÀNH VIÊN =============
CREATE TABLE ThanhVien
(
    MaTV    INT IDENTITY(1,1) PRIMARY KEY,
    HoTen   NVARCHAR(100) NOT NULL,
    Email   NVARCHAR(100) NULL,
    VaiTro  NVARCHAR(50)  NULL
);
GO

-- ============= BẢNG CÔNG VIỆC =============
CREATE TABLE CongViec
(
    MaCV        INT IDENTITY(1,1) PRIMARY KEY,
    TenCV       NVARCHAR(200) NOT NULL,
    MaDuAn      INT NOT NULL,
    NgayBatDau  DATE NULL,
    HanNop      DATE NULL,
    TrangThai   NVARCHAR(30) NOT NULL DEFAULT N'To Do',  -- To Do / In Progress / Review / Done
    MoTa        NVARCHAR(500) NULL,
    CONSTRAINT FK_CongViec_DuAn FOREIGN KEY (MaDuAn)
        REFERENCES DuAn(MaDuAn) ON DELETE CASCADE
);
GO

-- ============= BẢNG PHÂN CÔNG =============
CREATE TABLE PhanCong
(
    MaPhanCong  INT IDENTITY(1,1) PRIMARY KEY,
    MaCV        INT NOT NULL,
    MaTV        INT NOT NULL,
    VaiTro      NVARCHAR(50) NULL,
    CONSTRAINT FK_PhanCong_CongViec  FOREIGN KEY (MaCV) REFERENCES CongViec(MaCV) ON DELETE CASCADE,
    CONSTRAINT FK_PhanCong_ThanhVien FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE,
    CONSTRAINT UQ_PhanCong UNIQUE (MaCV, MaTV)
);
GO

-- ============= DỮ LIỆU MẪU =============
INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, MoTa) VALUES
(N'Website Bán Hàng',    '2024-01-01', '2024-06-30', N'Xây dựng website thương mại điện tử'),
(N'App Mobile Quản Lý',  '2024-03-01', '2024-09-30', N'App quản lý công việc cho doanh nghiệp'),
(N'Landing Page Sự Kiện','2024-02-15', '2024-04-01', N'Trang giới thiệu sự kiện công nghệ');

INSERT INTO ThanhVien (HoTen, Email, VaiTro) VALUES
(N'Nguyễn Văn A', 'a@example.com', N'Developer'),
(N'Trần Thị B',   'b@example.com', N'Designer'),
(N'Lê Văn C',     'c@example.com', N'Project Manager'),
(N'Phạm Thị D',   'd@example.com', N'Tester'),
(N'Hoàng Văn E',  'e@example.com', N'Developer');

INSERT INTO CongViec (TenCV, MaDuAn, NgayBatDau, HanNop, TrangThai, MoTa) VALUES
(N'Thiết kế CSDL',         1, '2024-01-05', '2024-01-20', N'Done',        N'Thiết kế schema'),
(N'Code backend API',      1, '2024-01-21', '2024-03-15', N'In Progress', N'REST API cho giỏ hàng'),
(N'Giao diện frontend',    1, '2024-02-01', '2024-04-10', N'Review',      N'React + Bootstrap'),
(N'Viết tài liệu',         2, '2024-03-05', '2024-04-15', N'To Do',       N'User manual'),
(N'Test chức năng',        2, '2024-04-01', '2024-05-01', N'To Do',       N'Kiểm thử các module'),
(N'Thiết kế poster',       3, '2024-02-20', '2024-03-10', N'Done',        N'Poster sự kiện');

INSERT INTO PhanCong (MaCV, MaTV, VaiTro) VALUES
(1, 1, N'Developer'),
(2, 1, N'Developer'),
(2, 3, N'Reviewer'),
(3, 2, N'Designer'),
(4, 3, N'PM'),
(5, 4, N'Tester'),
(6, 2, N'Designer');
GO

PRINT N'✅ Đã tạo xong CSDL ProjectTaskDB và nạp dữ liệu mẫu.';
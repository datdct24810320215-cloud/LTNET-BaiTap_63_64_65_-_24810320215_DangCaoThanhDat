using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class CongViecDAL
    {
        // Lấy task theo dự án
        public List<CongViecDTO> GetByDuAn(int maDuAn)
        {
            var list = new List<CongViecDTO>();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"SELECT MaCV, TenCV, MaDuAn, NgayBatDau, HanNop, TrangThai, MoTa
                                 FROM CongViec WHERE MaDuAn = @MaDuAn ORDER BY MaCV";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDuAn", maDuAn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CongViecDTO
                {
                    MaCV = (int)reader["MaCV"],
                    TenCV = reader["TenCV"].ToString(),
                    MaDuAn = (int)reader["MaDuAn"],
                    NgayBatDau = reader["NgayBatDau"] == DBNull.Value ? null : (DateTime?)reader["NgayBatDau"],
                    HanNop = reader["HanNop"] == DBNull.Value ? null : (DateTime?)reader["HanNop"],
                    TrangThai = reader["TrangThai"].ToString(),
                    MoTa = reader["MoTa"] == DBNull.Value ? null : reader["MoTa"].ToString()
                });
            }
            return list;
        }

        public int Insert(CongViecDTO dto)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                INSERT INTO CongViec (TenCV, MaDuAn, NgayBatDau, HanNop, TrangThai, MoTa)
                OUTPUT INSERTED.MaCV
                VALUES (@Ten, @MaDA, @BD, @Han, @TT, @MT)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", dto.TenCV);
            cmd.Parameters.AddWithValue("@MaDA", dto.MaDuAn);
            cmd.Parameters.AddWithValue("@BD", (object?)dto.NgayBatDau ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Han", (object?)dto.HanNop ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TT", dto.TrangThai);
            cmd.Parameters.AddWithValue("@MT", (object?)dto.MoTa ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }

        // Lọc theo dự án + trạng thái
        public List<CongViecDTO> GetFiltered(int maDuAn, string trangThai)
        {
            var list = new List<CongViecDTO>();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            string sql = @"SELECT MaCV, TenCV, MaDuAn, NgayBatDau, HanNop, TrangThai, MoTa
                           FROM CongViec WHERE MaDuAn = @MaDuAn";
            if (!string.IsNullOrEmpty(trangThai))
                sql += " AND TrangThai = @TT";
            sql += " ORDER BY HanNop";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDuAn", maDuAn);
            if (!string.IsNullOrEmpty(trangThai))
                cmd.Parameters.AddWithValue("@TT", trangThai);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CongViecDTO
                {
                    MaCV = (int)reader["MaCV"],
                    TenCV = reader["TenCV"].ToString(),
                    MaDuAn = (int)reader["MaDuAn"],
                    NgayBatDau = reader["NgayBatDau"] == DBNull.Value ? null : (DateTime?)reader["NgayBatDau"],
                    HanNop = reader["HanNop"] == DBNull.Value ? null : (DateTime?)reader["HanNop"],
                    TrangThai = reader["TrangThai"].ToString(),
                    MoTa = reader["MoTa"] == DBNull.Value ? null : reader["MoTa"].ToString()
                });
            }
            return list;
        }

        // Cập nhật trạng thái (Parameterized)
        public void UpdateTrangThai(int maCV, string trangThai)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = "UPDATE CongViec SET TrangThai = @TT WHERE MaCV = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TT", trangThai);
            cmd.Parameters.AddWithValue("@Ma", maCV);
            cmd.ExecuteNonQuery();
        }

        // ========== BÁO CÁO 1: Công việc trễ hạn (dùng SqlDataReader) ==========
        public DataTable GetTreHan()
        {
            var dt = new DataTable();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                SELECT  cv.MaCV          AS [Mã CV],
                        cv.TenCV         AS [Tên công việc],
                        da.TenDuAn       AS [Dự án],
                        cv.HanNop        AS [Hạn nộp],
                        cv.TrangThai     AS [Trạng thái],
                        DATEDIFF(DAY, cv.HanNop, CAST(GETDATE() AS DATE)) AS [Trễ (ngày)]
                FROM CongViec cv
                INNER JOIN DuAn da ON cv.MaDuAn = da.MaDuAn
                WHERE cv.HanNop < CAST(GETDATE() AS DATE)
                  AND cv.TrangThai <> N'Done'
                ORDER BY cv.HanNop";
            using var cmd = new SqlCommand(sql, conn);
            // Đọc bằng SqlDataReader rồi nạp vào DataTable
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        // ========== BÁO CÁO 2: % Hoàn thành theo thành viên (GROUP BY) ==========
        public DataTable ThongKeTheoDuAn(int maDuAn)
        {
            var dt = new DataTable();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                SELECT  tv.MaTV                                           AS [Mã TV],
                        tv.HoTen                                          AS [Họ tên],
                        COUNT(cv.MaCV)                                    AS [Tổng task],
                        SUM(CASE WHEN cv.TrangThai = N'Done' THEN 1 ELSE 0 END) AS [Đã xong],
                        CAST(
                            ISNULL(SUM(CASE WHEN cv.TrangThai = N'Done' THEN 1.0 ELSE 0 END) 
                                   * 100.0 / NULLIF(COUNT(cv.MaCV), 0), 0)
                            AS DECIMAL(5,2))                              AS [% Hoàn thành]
                FROM ThanhVien tv
                INNER JOIN PhanCong pc ON tv.MaTV = pc.MaTV
                INNER JOIN CongViec cv ON cv.MaCV = pc.MaCV
                WHERE cv.MaDuAn = @MaDuAn
                GROUP BY tv.MaTV, tv.HoTen
                ORDER BY [% Hoàn thành] DESC";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDuAn", maDuAn);
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
            return dt;
        }
    }
}
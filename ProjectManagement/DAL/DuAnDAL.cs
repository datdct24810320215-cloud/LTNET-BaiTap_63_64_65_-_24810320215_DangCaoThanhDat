using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class DuAnDAL
    {
        // Lấy toàn bộ dự án
        public List<DuAnDTO> GetAll()
        {
            var list = new List<DuAnDTO>();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = "SELECT MaDuAn, TenDuAn, NgayBatDau, NgayKetThuc, MoTa FROM DuAn ORDER BY MaDuAn";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DuAnDTO
                {
                    MaDuAn = (int)reader["MaDuAn"],
                    TenDuAn = reader["TenDuAn"].ToString(),
                    NgayBatDau = (DateTime)reader["NgayBatDau"],
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? null : (DateTime?)reader["NgayKetThuc"],
                    MoTa = reader["MoTa"] == DBNull.Value ? null : reader["MoTa"].ToString()
                });
            }
            return list;
        }

        // Thêm dự án, trả về MaDuAn mới
        public int Insert(DuAnDTO dto)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, MoTa)
                OUTPUT INSERTED.MaDuAn
                VALUES (@Ten, @BD, @KT, @MT)";
            using var cmd = new SqlCommand(sql, conn);
            // Parameterized SQL - chống SQL Injection
            cmd.Parameters.AddWithValue("@Ten", dto.TenDuAn);
            cmd.Parameters.AddWithValue("@BD", dto.NgayBatDau);
            cmd.Parameters.AddWithValue("@KT", (object?)dto.NgayKetThuc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MT", (object?)dto.MoTa ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }

        // Cập nhật dự án
        public void Update(DuAnDTO dto)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                UPDATE DuAn
                SET TenDuAn = @Ten, NgayBatDau = @BD, NgayKetThuc = @KT, MoTa = @MT
                WHERE MaDuAn = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", dto.TenDuAn);
            cmd.Parameters.AddWithValue("@BD", dto.NgayBatDau);
            cmd.Parameters.AddWithValue("@KT", (object?)dto.NgayKetThuc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MT", (object?)dto.MoTa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Ma", dto.MaDuAn);
            cmd.ExecuteNonQuery();
        }

        // Xóa dự án (Cascade xóa CongViec + PhanCong)
        public void Delete(int maDuAn)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = "DELETE FROM DuAn WHERE MaDuAn = @Ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", maDuAn);
            cmd.ExecuteNonQuery();
        }
    }
}
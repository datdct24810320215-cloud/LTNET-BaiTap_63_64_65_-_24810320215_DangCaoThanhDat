using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class PhanCongDAL
    {
        // Thêm 1 phân công
        public void Insert(int maCV, int maTV, string vaiTro)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"INSERT INTO PhanCong (MaCV, MaTV, VaiTro) VALUES (@MaCV, @MaTV, @VT)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaCV", maCV);
            cmd.Parameters.AddWithValue("@MaTV", maTV);
            cmd.Parameters.AddWithValue("@VT", (object?)vaiTro ?? System.DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // Xóa toàn bộ phân công của 1 công việc
        public void DeleteByCongViec(int maCV)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = "DELETE FROM PhanCong WHERE MaCV = @MaCV";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaCV", maCV);
            cmd.ExecuteNonQuery();
        }
    }
}
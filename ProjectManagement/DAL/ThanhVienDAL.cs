using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class ThanhVienDAL
    {
        public List<ThanhVienDTO> GetAll()
        {
            var list = new List<ThanhVienDTO>();
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = "SELECT MaTV, HoTen, Email, VaiTro FROM ThanhVien ORDER BY HoTen";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ThanhVienDTO
                {
                    MaTV = (int)reader["MaTV"],
                    HoTen = reader["HoTen"].ToString(),
                    Email = reader["Email"] == System.DBNull.Value ? null : reader["Email"].ToString(),
                    VaiTro = reader["VaiTro"] == System.DBNull.Value ? null : reader["VaiTro"].ToString()
                });
            }
            return list;
        }

        public int Insert(ThanhVienDTO dto)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            const string sql = @"
                INSERT INTO ThanhVien (HoTen, Email, VaiTro)
                OUTPUT INSERTED.MaTV VALUES (@Ten, @Mail, @VT)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ten", dto.HoTen);
            cmd.Parameters.AddWithValue("@Mail", (object?)dto.Email ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@VT", (object?)dto.VaiTro ?? System.DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
    }
}
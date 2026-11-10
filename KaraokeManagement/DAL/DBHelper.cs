using System.Data;
using Microsoft.Data.SqlClient;

namespace Karaoke.DAL
{
    /// <summary>
    /// Lớp tiện ích kết nối CSDL dùng Microsoft.Data.SqlClient
    /// </summary>
    public static class DBHelper
    {
        // Sửa lại nếu máy bạn dùng instance khác
        private const string ConnectionString =
            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=KaraokeDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        /// <summary>Lấy DataTable bằng SqlDataAdapter (Disconnected Model)</summary>
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
}
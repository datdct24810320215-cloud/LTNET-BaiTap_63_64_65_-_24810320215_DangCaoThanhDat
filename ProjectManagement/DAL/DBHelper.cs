using System.Configuration;
using Microsoft.Data.SqlClient;

namespace ProjectManagement.DAL
{
    // Lớp tiện ích quản lý kết nối CSDL
    public static class DBHelper
    {
        // Đọc chuỗi kết nối từ App.config
        public static string ConnectionString
            => ConfigurationManager.ConnectionStrings["ProjectTaskDB"].ConnectionString;

        // Tạo SqlConnection mới (dùng Microsoft.Data.SqlClient)
        public static SqlConnection GetConnection()
            => new SqlConnection(ConnectionString);
    }
}
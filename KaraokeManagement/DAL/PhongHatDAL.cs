using Microsoft.Data.SqlClient;
using Karaoke.DTO;

namespace Karaoke.DAL
{
    public class PhongHatDAL
    {
        public List<PhongHat> GetAll()
        {
            var list = new List<PhongHat>();
            const string sql = @"SELECT MaPhong, TenPhong, LoaiPhong, GiaTheoGio, TrangThai
                                 FROM PhongHat ORDER BY MaPhong";

            using var conn = DBHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new PhongHat
                {
                    MaPhong = rd.GetInt32(0),
                    TenPhong = rd.GetString(1),
                    LoaiPhong = rd.GetString(2),
                    GiaTheoGio = rd.GetDecimal(3),
                    TrangThai = rd.GetString(4)
                });
            }
            return list;
        }

        public PhongHat? GetById(int maPhong)
        {
            const string sql = @"SELECT MaPhong, TenPhong, LoaiPhong, GiaTheoGio, TrangThai
                                 FROM PhongHat WHERE MaPhong = @MaPhong";
            using var conn = DBHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new PhongHat
            {
                MaPhong = rd.GetInt32(0),
                TenPhong = rd.GetString(1),
                LoaiPhong = rd.GetString(2),
                GiaTheoGio = rd.GetDecimal(3),
                TrangThai = rd.GetString(4)
            };
        }

        /// <summary>Cập nhật trạng thái phòng (dùng ngoài transaction)</summary>
        public int CapNhatTrangThai(int maPhong, string trangThai)
        {
            const string sql = @"UPDATE PhongHat SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
            return DBHelper.ExecuteNonQuery(sql,
                new SqlParameter("@TrangThai", trangThai),
                new SqlParameter("@MaPhong", maPhong));
        }
    }
}
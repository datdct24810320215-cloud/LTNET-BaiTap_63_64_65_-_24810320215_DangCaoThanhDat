using Microsoft.Data.SqlClient;
using Karaoke.DTO;

namespace Karaoke.DAL
{
    public class DichVuDAL
    {
        public List<DichVu> GetAll()
        {
            var list = new List<DichVu>();
            const string sql = @"SELECT MaDV, TenDV, DonGia, TonKho FROM DichVu ORDER BY TenDV";

            using var conn = DBHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new DichVu
                {
                    MaDV = rd.GetInt32(0),
                    TenDV = rd.GetString(1),
                    DonGia = rd.GetDecimal(2),
                    TonKho = rd.GetInt32(3)
                });
            }
            return list;
        }

        public DichVu? GetById(int maDV)
        {
            const string sql = @"SELECT MaDV, TenDV, DonGia, TonKho FROM DichVu WHERE MaDV = @MaDV";
            using var conn = DBHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDV", maDV);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new DichVu
            {
                MaDV = rd.GetInt32(0),
                TenDV = rd.GetString(1),
                DonGia = rd.GetDecimal(2),
                TonKho = rd.GetInt32(3)
            };
        }
    }
}
using System.Collections.Generic;
using System.Data;
using GarageManagement.DTO;
using Microsoft.Data.SqlClient;

namespace GarageManagement.DAL
{
    public class PhuTungDAL
    {
        public List<PhuTungDTO> GetAll()
        {
            var list = new List<PhuTungDTO>();
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT MaPhuTung, TenPhuTung, DonGia, SoLuongTon FROM PhuTung", conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new PhuTungDTO
                {
                    MaPhuTung = rd.GetInt32(0),
                    TenPhuTung = rd.GetString(1),
                    DonGia = rd.GetDecimal(2),
                    SoLuongTon = rd.GetInt32(3)
                });
            return list;
        }

        public int Insert(PhuTungDTO pt)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon)
                VALUES (@Ten, @DonGia, @SoLuong);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.Add("@Ten", SqlDbType.NVarChar, 150).Value = pt.TenPhuTung;
            var p1 = cmd.Parameters.Add("@DonGia", SqlDbType.Decimal);
            p1.Value = pt.DonGia; p1.Precision = 18; p1.Scale = 2;
            cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = pt.SoLuongTon;
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void Update(PhuTungDTO pt)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                UPDATE PhuTung SET TenPhuTung=@Ten, DonGia=@DonGia, SoLuongTon=@SoLuong
                WHERE MaPhuTung=@Ma", conn);
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = pt.MaPhuTung;
            cmd.Parameters.Add("@Ten", SqlDbType.NVarChar, 150).Value = pt.TenPhuTung;
            var p1 = cmd.Parameters.Add("@DonGia", SqlDbType.Decimal);
            p1.Value = pt.DonGia; p1.Precision = 18; p1.Scale = 2;
            cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = pt.SoLuongTon;
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int maPhuTung)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "DELETE FROM PhuTung WHERE MaPhuTung=@Ma", conn);
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = maPhuTung;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
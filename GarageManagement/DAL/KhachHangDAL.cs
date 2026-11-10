using System.Collections.Generic;
using GarageManagement.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GarageManagement.DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> GetAll()
        {
            var list = new List<KhachHangDTO>();
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT MaKH, TenKH, DienThoai, DiaChi FROM KhachHang", conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new KhachHangDTO
                {
                    MaKH = rd.GetInt32(0),
                    TenKH = rd.GetString(1),
                    DienThoai = rd.GetString(2),
                    DiaChi = rd.IsDBNull(3) ? null : rd.GetString(3)
                });
            }
            return list;
        }

        public KhachHangDTO GetByPhone(string dienThoai)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT MaKH, TenKH, DienThoai, DiaChi FROM KhachHang WHERE DienThoai=@DienThoai", conn);
            cmd.Parameters.Add("@DienThoai", SqlDbType.VarChar, 15).Value = dienThoai;
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
                return new KhachHangDTO
                {
                    MaKH = rd.GetInt32(0),
                    TenKH = rd.GetString(1),
                    DienThoai = rd.GetString(2),
                    DiaChi = rd.IsDBNull(3) ? null : rd.GetString(3)
                };
            return null;
        }

        public int Insert(KhachHangDTO kh)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO KhachHang (TenKH, DienThoai, DiaChi)
                VALUES (@TenKH, @DienThoai, @DiaChi);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.Add("@TenKH", SqlDbType.NVarChar, 100).Value = kh.TenKH;
            cmd.Parameters.Add("@DienThoai", SqlDbType.VarChar, 15).Value = kh.DienThoai;
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 200).Value =
                (object)kh.DiaChi ?? DBNull.Value;
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void Update(KhachHangDTO kh)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                UPDATE KhachHang SET TenKH=@TenKH, DiaChi=@DiaChi WHERE MaKH=@MaKH", conn);
            cmd.Parameters.Add("@MaKH", SqlDbType.Int).Value = kh.MaKH;
            cmd.Parameters.Add("@TenKH", SqlDbType.NVarChar, 100).Value = kh.TenKH;
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 200).Value =
                (object)kh.DiaChi ?? DBNull.Value;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
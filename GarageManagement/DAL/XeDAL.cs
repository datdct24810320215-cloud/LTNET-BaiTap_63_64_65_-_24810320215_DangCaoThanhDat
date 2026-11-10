using System.Collections.Generic;
using System.Data;
using GarageManagement.DTO;
using Microsoft.Data.SqlClient;

namespace GarageManagement.DAL
{
    public class XeDAL
    {
        // Dùng cho DataGridView hiển thị xe kèm chủ xe
        public DataTable GetAllWithOwner()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                SELECT x.BienSo, x.HieuXe, x.NamSanXuat,
                       kh.MaKH, kh.TenKH, kh.DienThoai, kh.DiaChi
                FROM Xe x INNER JOIN KhachHang kh ON x.MaKH = kh.MaKH", conn);
            conn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public List<XeDTO> GetAll()
        {
            var list = new List<XeDTO>();
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT BienSo, MaKH, HieuXe, NamSanXuat FROM Xe", conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new XeDTO
                {
                    BienSo = rd.GetString(0),
                    MaKH = rd.GetInt32(1),
                    HieuXe = rd.GetString(2),
                    NamSanXuat = rd.IsDBNull(3) ? (int?)null : rd.GetInt32(3)
                });
            return list;
        }

        public XeDTO GetByBienSo(string bienSo)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT BienSo, MaKH, HieuXe, NamSanXuat FROM Xe WHERE BienSo=@BienSo", conn);
            cmd.Parameters.Add("@BienSo", SqlDbType.VarChar, 20).Value = bienSo;
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
                return new XeDTO
                {
                    BienSo = rd.GetString(0),
                    MaKH = rd.GetInt32(1),
                    HieuXe = rd.GetString(2),
                    NamSanXuat = rd.IsDBNull(3) ? (int?)null : rd.GetInt32(3)
                };
            return null;
        }

        public void Insert(XeDTO xe)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat)
                VALUES (@BienSo, @MaKH, @HieuXe, @NamSanXuat)", conn);
            cmd.Parameters.Add("@BienSo", SqlDbType.VarChar, 20).Value = xe.BienSo;
            cmd.Parameters.Add("@MaKH", SqlDbType.Int).Value = xe.MaKH;
            cmd.Parameters.Add("@HieuXe", SqlDbType.NVarChar, 50).Value = xe.HieuXe;
            cmd.Parameters.Add("@NamSanXuat", SqlDbType.Int).Value =
                (object)xe.NamSanXuat ?? DBNull.Value;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
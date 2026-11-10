using System;
using System.Collections.Generic;
using System.Data;
using GarageManagement.DTO;
using Microsoft.Data.SqlClient;

namespace GarageManagement.DAL
{
    public class PhieuSuaChuaDAL
    {
        // ============ HÀM TRỌNG TÂM: LẬP PHIẾU + TRỪ KHO (TRANSACTION) ============
        public int LapPhieuSuaChua_Transaction(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> chiTiet)
        {
            if (chiTiet == null || chiTiet.Count == 0)
                throw new ArgumentException("Danh sách chi tiết rỗng.");

            int maPhieuMoi;
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            conn.Open();
            using var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                // ----- BƯỚC 1: Insert PhieuSuaChua -----
                const string sqlInsertPhieu = @"
                    INSERT INTO PhieuSuaChua (BienSo, NoiDungSua, TongTien, TrangThai)
                    VALUES (@BienSo, @NoiDungSua, @TongTien, @TrangThai);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using (var cmd = new SqlCommand(sqlInsertPhieu, conn, trans))
                {
                    cmd.Parameters.Add("@BienSo", SqlDbType.VarChar, 20).Value = phieu.BienSo;
                    cmd.Parameters.Add("@NoiDungSua", SqlDbType.NVarChar, -1).Value =
                        (object)phieu.NoiDungSua ?? DBNull.Value;
                    var pTong = cmd.Parameters.Add("@TongTien", SqlDbType.Decimal);
                    pTong.Value = phieu.TongTien; pTong.Precision = 18; pTong.Scale = 2;
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 30).Value = phieu.TrangThai;
                    maPhieuMoi = (int)cmd.ExecuteScalar();
                }

                // ----- BƯỚC 2 & 3: Insert ChiTiet + Update SoLuongTon -----
                foreach (var ct in chiTiet)
                {
                    // Kiểm tra tồn kho (UPDLOCK tránh race-condition)
                    int tonKho;
                    using (var cmdCheck = new SqlCommand(
                        "SELECT SoLuongTon FROM PhuTung WITH (UPDLOCK, ROWLOCK) WHERE MaPhuTung=@Ma", conn, trans))
                    {
                        cmdCheck.Parameters.Add("@Ma", SqlDbType.Int).Value = ct.MaPhuTung;
                        var v = cmdCheck.ExecuteScalar();
                        if (v == null || v == DBNull.Value)
                            throw new Exception($"Phụ tùng mã {ct.MaPhuTung} không tồn tại.");
                        tonKho = Convert.ToInt32(v);
                    }
                    if (tonKho < ct.SoLuong)
                        throw new Exception(
                            $"Phụ tùng mã {ct.MaPhuTung} không đủ tồn kho (còn {tonKho}, cần {ct.SoLuong}).");

                    // Insert chi tiết
                    using (var cmdCT = new SqlCommand(@"
                        INSERT INTO ChiTietSuaChua (MaPhieu, MaPhuTung, SoLuong, TienCong)
                        VALUES (@MaPhieu, @MaPhuTung, @SoLuong, @TienCong);", conn, trans))
                    {
                        cmdCT.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieuMoi;
                        cmdCT.Parameters.Add("@MaPhuTung", SqlDbType.Int).Value = ct.MaPhuTung;
                        cmdCT.Parameters.Add("@SoLuong", SqlDbType.Int).Value = ct.SoLuong;
                        var pTC = cmdCT.Parameters.Add("@TienCong", SqlDbType.Decimal);
                        pTC.Value = ct.TienCong; pTC.Precision = 18; pTC.Scale = 2;
                        cmdCT.ExecuteNonQuery();
                    }

                    // Trừ kho
                    using (var cmdUpd = new SqlCommand(
                        "UPDATE PhuTung SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaPhuTung=@Ma",
                        conn, trans))
                    {
                        cmdUpd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = ct.SoLuong;
                        cmdUpd.Parameters.Add("@Ma", SqlDbType.Int).Value = ct.MaPhuTung;
                        cmdUpd.ExecuteNonQuery();
                    }
                }

                trans.Commit();
                return maPhieuMoi;
            }
            catch
            {
                trans.Rollback();
                throw;   // ném lại để GUI/BUS hiển thị
            }
        }

        // ============ THANH TOÁN ============
        public decimal ThanhToan(int maPhieu)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            conn.Open();
            using var trans = conn.BeginTransaction();
            try
            {
                // Tính tổng tiền từ DB (đảm bảo chính xác)
                decimal tong;
                using (var cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(ct.SoLuong * pt.DonGia + ct.TienCong), 0)
                    FROM ChiTietSuaChua ct
                    INNER JOIN PhuTung pt ON ct.MaPhuTung = pt.MaPhuTung
                    WHERE ct.MaPhieu = @MaPhieu", conn, trans))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                    tong = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                using (var cmd = new SqlCommand(@"
                    UPDATE PhieuSuaChua
                    SET TongTien = @TongTien, TrangThai = N'Đã thanh toán'
                    WHERE MaPhieu = @MaPhieu", conn, trans))
                {
                    var pT = cmd.Parameters.Add("@TongTien", SqlDbType.Decimal);
                    pT.Value = tong; pT.Precision = 18; pT.Scale = 2;
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("Không tìm thấy phiếu cần thanh toán.");
                }

                trans.Commit();
                return tong;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public PhieuSuaChuaDTO GetById(int maPhieu)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                SELECT MaPhieu, BienSo, NgayNhan, NoiDungSua, TongTien, TrangThai
                FROM PhieuSuaChua WHERE MaPhieu=@Ma", conn);
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = maPhieu;
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
                return new PhieuSuaChuaDTO
                {
                    MaPhieu = rd.GetInt32(0),
                    BienSo = rd.GetString(1),
                    NgayNhan = rd.GetDateTime(2),
                    NoiDungSua = rd.IsDBNull(3) ? null : rd.GetString(3),
                    TongTien = rd.GetDecimal(4),
                    TrangThai = rd.GetString(5)
                };
            return null;
        }

        public DataTable GetChiTietHienThi(int maPhieu)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(@"
                SELECT pt.TenPhuTung, pt.DonGia, ct.SoLuong, ct.TienCong,
                       (ct.SoLuong * pt.DonGia + ct.TienCong) AS ThanhTien
                FROM ChiTietSuaChua ct
                INNER JOIN PhuTung pt ON ct.MaPhuTung = pt.MaPhuTung
                WHERE ct.MaPhieu = @MaPhieu", conn);
            cmd.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
            conn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}
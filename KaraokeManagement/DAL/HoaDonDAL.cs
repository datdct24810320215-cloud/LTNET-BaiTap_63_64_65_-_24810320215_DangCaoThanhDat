using System.Data;
using Microsoft.Data.SqlClient;
using Karaoke.DTO;

namespace Karaoke.DAL
{
    public class HoaDonDAL
    {
        /* =========================================================
           1. LẤY HÓA ĐƠN ĐANG MỞ CỦA 1 PHÒNG
           ========================================================= */
        public HoaDon? GetHoaDonDangMo(int maPhong)
        {
            const string sql = @"
                SELECT TOP 1 MaHD, MaPhong, GioVao, GioRa,
                             TongTienPhong, TongTienDichVu, TrangThai
                FROM HoaDon
                WHERE MaPhong = @MaPhong AND TrangThai = 0
                ORDER BY MaHD DESC";

            using var conn = DBHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new HoaDon
            {
                MaHD = rd.GetInt32(0),
                MaPhong = rd.GetInt32(1),
                GioVao = rd.GetDateTime(2),
                GioRa = rd.IsDBNull(3) ? (DateTime?)null : rd.GetDateTime(3),
                TongTienPhong = rd.GetDecimal(4),
                TongTienDichVu = rd.GetDecimal(5),
                TrangThai = rd.GetBoolean(6)
            };
        }

        /* =========================================================
           2. MỞ PHÒNG  (INSERT HoaDon + UPDATE PhongHat)
              -> Cũng dùng transaction để đảm bảo toàn vẹn
           ========================================================= */
        public int MoPhong(int maPhong)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            using var trans = conn.BeginTransaction();

            try
            {
                int maHD;

                // (a) Tạo hóa đơn mới
                using (var cmd = new SqlCommand(@"
                        INSERT INTO HoaDon(MaPhong, GioVao, TongTienPhong, TongTienDichVu, TrangThai)
                        OUTPUT INSERTED.MaHD
                        VALUES(@MaPhong, @GioVao, 0, 0, 0)", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.Parameters.AddWithValue("@GioVao", DateTime.Now);
                    maHD = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // (b) Đổi trạng thái phòng
                using (var cmd = new SqlCommand(@"
                        UPDATE PhongHat SET TrangThai = @TrangThai
                        WHERE MaPhong = @MaPhong", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@TrangThai", TrangThaiPhong.DangHat);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
                return maHD;
            }
            catch
            {
                try { trans.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }

        /* =========================================================
           3. LẤY CHI TIẾT DỊCH VỤ  (Disconnected Model - SqlDataAdapter)
           ========================================================= */
        public DataTable GetChiTietDichVu(int maHD)
        {
            const string sql = @"
                SELECT ct.MaDV,
                       dv.TenDV,
                       ct.SoLuong,
                       ct.DonGia,
                       CAST(ct.SoLuong * ct.DonGia AS DECIMAL(18,2)) AS ThanhTien
                FROM ChiTietDichVu ct
                INNER JOIN DichVu dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD
                ORDER BY dv.TenDV";

            return DBHelper.ExecuteDataTable(sql,
                new SqlParameter("@MaHD", maHD));
        }

        /* =========================================================
           4. THÊM DỊCH VỤ  (UPSERT: có rồi thì cộng dồn số lượng)
           ========================================================= */
        public void ThemDichVu(int maHD, int maDV, int soLuong, decimal donGia)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM ChiTietDichVu WHERE MaHD = @MaHD AND MaDV = @MaDV)
                BEGIN
                    UPDATE ChiTietDichVu
                    SET SoLuong = SoLuong + @SoLuong,
                        DonGia  = @DonGia
                    WHERE MaHD = @MaHD AND MaDV = @MaDV;
                END
                ELSE
                BEGIN
                    INSERT INTO ChiTietDichVu(MaHD, MaDV, SoLuong, DonGia)
                    VALUES(@MaHD, @MaDV, @SoLuong, @DonGia);
                END";

            DBHelper.ExecuteNonQuery(sql,
                new SqlParameter("@MaHD", maHD),
                new SqlParameter("@MaDV", maDV),
                new SqlParameter("@SoLuong", soLuong),
                new SqlParameter("@DonGia", donGia));
        }

        /* =========================================================
           5. THANH TOÁN  (SqlTransaction)
              - Update HoaDon (GioRa, TongTienPhong, TongTienDichVu, TrangThai = 1)
              - Trừ TonKho trong DichVu
              - Update PhongHat.TrangThai = 'Trống'
           ========================================================= */
        public KetQuaThanhToan ThanhToan(int maHD, int maPhong, decimal giaTheoGio)
        {
            using var conn = DBHelper.GetConnection();
            conn.Open();
            using var trans = conn.BeginTransaction();

            try
            {
                /* ---- (1) Lấy GioVao từ CSDL (an toàn hơn truyền từ GUI) ---- */
                DateTime gioVao;
                using (var cmd = new SqlCommand(
                    "SELECT GioVao FROM HoaDon WHERE MaHD = @MaHD AND TrangThai = 0", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    var obj = cmd.ExecuteScalar();
                    if (obj == null || obj == DBNull.Value)
                        throw new InvalidOperationException(
                            "Hóa đơn không tồn tại hoặc đã được thanh toán trước đó.");
                    gioVao = Convert.ToDateTime(obj);
                }

                /* ---- (2) Tính tiền phòng: số giờ làm tròn LÊN, tối thiểu 1 giờ ---- */
                DateTime gioRa = DateTime.Now;
                int soGio = (int)Math.Ceiling((gioRa - gioVao).TotalHours);
                if (soGio < 1) soGio = 1;
                decimal tienPhong = soGio * giaTheoGio;

                /* ---- (3) Tính tiền dịch vụ từ ChiTietDichVu ---- */
                decimal tienDichVu;
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(SoLuong * DonGia), 0) FROM ChiTietDichVu WHERE MaHD = @MaHD",
                    conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    tienDichVu = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                /* ---- (4) Kiểm tra tồn kho trước khi trừ ---- */
                using (var cmd = new SqlCommand(@"
                        SELECT COUNT(*)
                        FROM ChiTietDichVu ct
                        INNER JOIN DichVu dv ON ct.MaDV = dv.MaDV
                        WHERE ct.MaHD = @MaHD AND dv.TonKho < ct.SoLuong", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    int soDichVuThieu = Convert.ToInt32(cmd.ExecuteScalar());
                    if (soDichVuThieu > 0)
                        throw new InvalidOperationException(
                            "Không đủ tồn kho để thanh toán. Vui lòng kiểm tra lại kho hàng.");
                }

                /* ---- (5) UPDATE HoaDon ---- */
                using (var cmd = new SqlCommand(@"
                        UPDATE HoaDon
                        SET GioRa           = @GioRa,
                            TongTienPhong   = @TienPhong,
                            TongTienDichVu  = @TienDichVu,
                            TrangThai       = 1
                        WHERE MaHD = @MaHD AND TrangThai = 0", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@GioRa", gioRa);
                    cmd.Parameters.AddWithValue("@TienPhong", tienPhong);
                    cmd.Parameters.AddWithValue("@TienDichVu", tienDichVu);
                    cmd.Parameters.AddWithValue("@MaHD", maHD);

                    if (cmd.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("Hóa đơn đã được thanh toán trước đó.");
                }

                /* ---- (6) TRỪ TỒN KHO ---- */
                using (var cmd = new SqlCommand(@"
                        UPDATE dv
                        SET dv.TonKho = dv.TonKho - ct.SoLuong
                        FROM DichVu dv
                        INNER JOIN ChiTietDichVu ct ON dv.MaDV = ct.MaDV
                        WHERE ct.MaHD = @MaHD", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    cmd.ExecuteNonQuery();
                }

                /* ---- (7) TRẢ PHÒNG ---- */
                using (var cmd = new SqlCommand(@"
                        UPDATE PhongHat SET TrangThai = @TrangThai
                        WHERE MaPhong = @MaPhong", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@TrangThai", TrangThaiPhong.Trong);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.ExecuteNonQuery();
                }

                /* ---- (8) COMMIT ---- */
                trans.Commit();

                return new KetQuaThanhToan
                {
                    GioVao = gioVao,
                    GioRa = gioRa,
                    SoGio = soGio,
                    TienPhong = tienPhong,
                    TienDichVu = tienDichVu
                };
            }
            catch
            {
                /* ---- ROLLBACK nếu có bất kỳ lỗi nào ---- */
                try { trans.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }
    }
}
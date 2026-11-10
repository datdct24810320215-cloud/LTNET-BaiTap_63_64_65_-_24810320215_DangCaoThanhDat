using System.Data;
using Karaoke.DAL;
using Karaoke.DTO;

namespace Karaoke.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL _dal = new();

        public HoaDon? LayHoaDonDangMo(int maPhong) => _dal.GetHoaDonDangMo(maPhong);

        /// <summary>Mở phòng: chỉ cho phép khi phòng đang Trống</summary>
        public int MoPhong(PhongHat phong)
        {
            ArgumentNullException.ThrowIfNull(phong);

            if (phong.TrangThai != TrangThaiPhong.Trong)
                throw new InvalidOperationException(
                    $"Phòng {phong.TenPhong} đang được sử dụng, không thể mở!");

            return _dal.MoPhong(phong.MaPhong);
        }

        public DataTable LayChiTietDichVu(int maHD) => _dal.GetChiTietDichVu(maHD);

        public void ThemDichVu(int maHD, DichVu dv, int soLuong)
        {
            ArgumentNullException.ThrowIfNull(dv);

            if (soLuong <= 0)
                throw new ArgumentException("Số lượng dịch vụ phải lớn hơn 0.");
            if (soLuong > dv.TonKho)
                throw new InvalidOperationException(
                    $"Dịch vụ '{dv.TenDV}' chỉ còn {dv.TonKho} trong kho.");

            _dal.ThemDichVu(maHD, dv.MaDV, soLuong, dv.DonGia);
        }

        public KetQuaThanhToan ThanhToan(HoaDon hoaDon, PhongHat phong)
        {
            ArgumentNullException.ThrowIfNull(hoaDon);
            ArgumentNullException.ThrowIfNull(phong);

            if (hoaDon.TrangThai)
                throw new InvalidOperationException("Hóa đơn này đã được thanh toán.");

            return _dal.ThanhToan(hoaDon.MaHD, phong.MaPhong, phong.GiaTheoGio);
        }
    }
}
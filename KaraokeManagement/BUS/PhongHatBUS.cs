using Karaoke.DAL;
using Karaoke.DTO;

namespace Karaoke.BUS
{
    public class PhongHatBUS
    {
        private readonly PhongHatDAL _dal = new();

        public List<PhongHat> LayDanhSach() => _dal.GetAll();

        public PhongHat? LayTheoMa(int maPhong) => _dal.GetById(maPhong);

        public bool KiemTraPhongTrong(int maPhong)
        {
            var p = _dal.GetById(maPhong);
            return p != null && p.TrangThai == TrangThaiPhong.Trong;
        }
    }
}
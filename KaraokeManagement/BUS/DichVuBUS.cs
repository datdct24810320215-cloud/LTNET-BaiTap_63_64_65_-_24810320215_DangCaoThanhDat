using Karaoke.DAL;
using Karaoke.DTO;

namespace Karaoke.BUS
{
    public class DichVuBUS
    {
        private readonly DichVuDAL _dal = new();

        public List<DichVu> LayDanhSach() => _dal.GetAll();

        public DichVu? LayTheoMa(int maDV) => _dal.GetById(maDV);
    }
}
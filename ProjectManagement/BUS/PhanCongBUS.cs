using ProjectManagement.DAL;

namespace ProjectManagement.BUS
{
    public class PhanCongBUS
    {
        private readonly PhanCongDAL dal = new PhanCongDAL();
        public void Insert(int maCV, int maTV, string vaiTro) => dal.Insert(maCV, maTV, vaiTro);
        public void DeleteByCongViec(int maCV) => dal.DeleteByCongViec(maCV);
    }
}
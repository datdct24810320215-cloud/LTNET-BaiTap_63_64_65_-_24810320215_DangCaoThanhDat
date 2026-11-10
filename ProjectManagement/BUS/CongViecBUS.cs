using System.Collections.Generic;
using System.Data;
using ProjectManagement.DAL;
using ProjectManagement.DTO;

namespace ProjectManagement.BUS
{
    public class CongViecBUS
    {
        private readonly CongViecDAL dal = new CongViecDAL();
        public List<CongViecDTO> GetByDuAn(int maDuAn) => dal.GetByDuAn(maDuAn);
        public List<CongViecDTO> GetFiltered(int maDuAn, string tt) => dal.GetFiltered(maDuAn, tt);
        public int Insert(CongViecDTO dto) => dal.Insert(dto);
        public void UpdateTrangThai(int maCV, string tt) => dal.UpdateTrangThai(maCV, tt);
        public DataTable GetTreHan() => dal.GetTreHan();
        public DataTable ThongKeTheoDuAn(int maDuAn) => dal.ThongKeTheoDuAn(maDuAn);
    }
}
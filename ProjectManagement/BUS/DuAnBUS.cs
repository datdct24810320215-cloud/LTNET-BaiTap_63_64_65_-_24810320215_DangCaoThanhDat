using System.Collections.Generic;
using ProjectManagement.DAL;
using ProjectManagement.DTO;

namespace ProjectManagement.BUS
{
    public class DuAnBUS
    {
        private readonly DuAnDAL dal = new DuAnDAL();
        public List<DuAnDTO> GetAll() => dal.GetAll();
        public int Insert(DuAnDTO dto) => dal.Insert(dto);
        public void Update(DuAnDTO dto) => dal.Update(dto);
        public void Delete(int maDuAn) => dal.Delete(maDuAn);
    }
}
using System.Collections.Generic;
using ProjectManagement.DAL;
using ProjectManagement.DTO;

namespace ProjectManagement.BUS
{
    public class ThanhVienBUS
    {
        private readonly ThanhVienDAL dal = new ThanhVienDAL();
        public List<ThanhVienDTO> GetAll() => dal.GetAll();
        public int Insert(ThanhVienDTO dto) => dal.Insert(dto);
    }
}
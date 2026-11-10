using System;
using System.Collections.Generic;
using GarageManagement.DAL;
using GarageManagement.DTO;

namespace GarageManagement.BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL dal = new KhachHangDAL();
        public List<KhachHangDTO> GetAll() => dal.GetAll();
        public KhachHangDTO GetByPhone(string phone) => dal.GetByPhone(phone);

        public int Insert(KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                throw new Exception("Tên khách hàng không được để trống.");
            if (string.IsNullOrWhiteSpace(kh.DienThoai))
                throw new Exception("Số điện thoại không được để trống.");
            if (kh.DienThoai.Length > 15)
                throw new Exception("Số điện thoại tối đa 15 ký tự.");
            return dal.Insert(kh);
        }
    }
}
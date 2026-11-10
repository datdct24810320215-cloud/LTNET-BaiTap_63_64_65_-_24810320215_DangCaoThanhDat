using System;
using System.Collections.Generic;
using GarageManagement.DAL;
using GarageManagement.DTO;

namespace GarageManagement.BUS
{
    public class PhuTungBUS
    {
        private readonly PhuTungDAL dal = new PhuTungDAL();
        public List<PhuTungDTO> GetAll() => dal.GetAll();

        public int Insert(PhuTungDTO pt)
        {
            Validate(pt);
            return dal.Insert(pt);
        }

        public void Update(PhuTungDTO pt)
        {
            if (pt.MaPhuTung <= 0) throw new Exception("Chưa chọn phụ tùng.");
            Validate(pt);
            dal.Update(pt);
        }

        public void Delete(int maPhuTung)
        {
            if (maPhuTung <= 0) throw new Exception("Chưa chọn phụ tùng.");
            dal.Delete(maPhuTung);
        }

        private void Validate(PhuTungDTO pt)
        {
            if (string.IsNullOrWhiteSpace(pt.TenPhuTung))
                throw new Exception("Tên phụ tùng không được trống.");
            if (pt.DonGia < 0)
                throw new Exception("Đơn giá phải >= 0.");
            if (pt.SoLuongTon < 0)
                throw new Exception("Số lượng tồn phải >= 0.");
        }
    }
}
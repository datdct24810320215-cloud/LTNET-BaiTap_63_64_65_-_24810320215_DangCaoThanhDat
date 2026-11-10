using System;
using System.Collections.Generic;
using System.Data;
using GarageManagement.DAL;
using GarageManagement.DTO;

namespace GarageManagement.BUS
{
    public class XeBUS
    {
        private readonly XeDAL dal = new XeDAL();
        private readonly KhachHangDAL khDal = new KhachHangDAL();

        public DataTable GetAllWithOwner() => dal.GetAllWithOwner();
        public List<XeDTO> GetAll() => dal.GetAll();
        public XeDTO GetByBienSo(string bienSo) => dal.GetByBienSo(bienSo);

        /// <summary>Tiếp nhận xe mới, tự động thêm/tái sử dụng khách hàng.</summary>
        public void TiepNhanXe(XeDTO xe, KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(xe.BienSo))
                throw new Exception("Biển số xe không được trống.");
            if (dal.GetByBienSo(xe.BienSo) != null)
                throw new Exception("Biển số xe đã tồn tại trong hệ thống.");
            if (string.IsNullOrWhiteSpace(xe.HieuXe))
                throw new Exception("Hiệu xe không được trống.");
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                throw new Exception("Tên khách hàng không được trống.");
            if (string.IsNullOrWhiteSpace(kh.DienThoai))
                throw new Exception("Số điện thoại không được trống.");
            if (xe.NamSanXuat.HasValue &&
                (xe.NamSanXuat.Value < 1900 || xe.NamSanXuat.Value > DateTime.Now.Year + 1))
                throw new Exception("Năm sản xuất không hợp lệ.");

            var existing = khDal.GetByPhone(kh.DienThoai);
            if (existing != null)
            {
                kh.MaKH = existing.MaKH;
                khDal.Update(kh);       // cập nhật tên/địa chỉ mới
            }
            else
            {
                kh.MaKH = khDal.Insert(kh);
            }

            xe.MaKH = kh.MaKH;
            dal.Insert(xe);
        }
    }
}
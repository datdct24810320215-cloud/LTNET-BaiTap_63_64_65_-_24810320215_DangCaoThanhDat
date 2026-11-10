using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GarageManagement.DAL;
using GarageManagement.DTO;

namespace GarageManagement.BUS
{
    public class PhieuSuaChuaBUS
    {
        private readonly PhieuSuaChuaDAL dal = new PhieuSuaChuaDAL();
        private readonly PhuTungDAL phuTungDal = new PhuTungDAL();

        public int LapPhieuSuaChua(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> chiTiet)
        {
            if (string.IsNullOrWhiteSpace(phieu.BienSo))
                throw new Exception("Chưa chọn xe.");
            if (chiTiet == null || chiTiet.Count == 0)
                throw new Exception("Chưa có phụ tùng nào trong phiếu.");
            foreach (var ct in chiTiet)
            {
                if (ct.SoLuong <= 0) throw new Exception("Số lượng phải > 0.");
                if (ct.TienCong < 0) throw new Exception("Tiền công không được âm.");
            }
            phieu.TongTien = TinhTongTien(chiTiet);
            if (string.IsNullOrWhiteSpace(phieu.TrangThai))
                phieu.TrangThai = "Chưa thanh toán";

            return dal.LapPhieuSuaChua_Transaction(phieu, chiTiet);
        }

        public decimal TinhTongTien(List<ChiTietSuaChuaDTO> chiTiet)
        {
            var dsPhuTung = phuTungDal.GetAll().ToDictionary(p => p.MaPhuTung);
            decimal tong = 0;
            foreach (var ct in chiTiet)
            {
                if (!dsPhuTung.TryGetValue(ct.MaPhuTung, out var pt))
                    throw new Exception($"Phụ tùng mã {ct.MaPhuTung} không tồn tại.");
                tong += ct.SoLuong * pt.DonGia + ct.TienCong;
            }
            return tong;
        }

        public decimal ThanhToan(int maPhieu)
        {
            if (maPhieu <= 0) throw new Exception("Mã phiếu không hợp lệ.");
            return dal.ThanhToan(maPhieu);
        }

        public PhieuSuaChuaDTO GetById(int maPhieu) => dal.GetById(maPhieu);
        public DataTable GetChiTietHienThi(int maPhieu) => dal.GetChiTietHienThi(maPhieu);
    }
}
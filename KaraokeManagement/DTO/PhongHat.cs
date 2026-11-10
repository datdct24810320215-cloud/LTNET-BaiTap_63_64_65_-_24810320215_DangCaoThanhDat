namespace Karaoke.DTO
{
    public class PhongHat
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public string LoaiPhong { get; set; } = string.Empty;
        public decimal GiaTheoGio { get; set; }
        public string TrangThai { get; set; } = DTO.TrangThaiPhong.Trong;

        public bool DangHat => TrangThai != DTO.TrangThaiPhong.Trong;
    }
}
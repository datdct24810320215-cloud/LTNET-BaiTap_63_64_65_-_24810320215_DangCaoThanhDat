namespace Karaoke.DTO
{
    public class HoaDon
    {
        public int MaHD { get; set; }
        public int MaPhong { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public bool TrangThai { get; set; } // false = đang mở, true = đã thanh toán
    }
}
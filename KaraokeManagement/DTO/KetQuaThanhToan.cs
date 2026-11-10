namespace Karaoke.DTO
{
    public class KetQuaThanhToan
    {
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
        public int SoGio { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongTien => TienPhong + TienDichVu;
    }
}
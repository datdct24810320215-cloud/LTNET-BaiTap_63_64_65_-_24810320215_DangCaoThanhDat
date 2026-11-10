namespace Karaoke.DTO
{
    public class ChiTietDichVu
    {
        public int MaHD { get; set; }
        public int MaDV { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // Thuộc tính mở rộng (chỉ để hiển thị)
        public string TenDV { get; set; } = string.Empty;
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
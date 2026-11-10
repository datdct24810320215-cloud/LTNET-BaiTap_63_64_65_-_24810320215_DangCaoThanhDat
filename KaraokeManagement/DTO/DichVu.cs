namespace Karaoke.DTO
{
    public class DichVu
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int TonKho { get; set; }

        // Tiện cho ComboBox
        public override string ToString() => $"{TenDV} - {DonGia:N0} đ";
    }
}
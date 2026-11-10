namespace ProjectManagement.DTO
{
    // DTO ánh xạ bảng ThanhVien
    public class ThanhVienDTO
    {
        public int MaTV { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }

        // Ghi đè ToString để CheckedListBox hiển thị đẹp
        public override string ToString() => $"{HoTen} ({VaiTro})";
    }
}
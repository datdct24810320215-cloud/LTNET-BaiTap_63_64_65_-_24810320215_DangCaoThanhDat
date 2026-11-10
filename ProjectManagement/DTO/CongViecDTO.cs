namespace ProjectManagement.DTO
{
    // DTO ánh xạ bảng CongViec
    public class CongViecDTO
    {
        public int MaCV { get; set; }
        public string TenCV { get; set; }
        public int MaDuAn { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? HanNop { get; set; }
        public string TrangThai { get; set; }   // "To Do", "In Progress", "Review", "Done"
        public string MoTa { get; set; }
    }
}
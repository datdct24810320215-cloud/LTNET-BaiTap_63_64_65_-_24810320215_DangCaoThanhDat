namespace ProjectManagement.DTO
{
    // DTO ánh xạ bảng DuAn
    public class DuAnDTO
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MoTa { get; set; }
    }
}
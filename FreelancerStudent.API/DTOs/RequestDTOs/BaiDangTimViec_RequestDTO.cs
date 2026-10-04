namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class BaiDangTimViec_RequestDTO
    {
        public int maUser { get; set; }
        public string tieude { get; set; } = string.Empty;
        public string? mota { get; set; }
        public string? kynang { get; set; }
        public double? mucGiaTu { get; set; }
    }
}

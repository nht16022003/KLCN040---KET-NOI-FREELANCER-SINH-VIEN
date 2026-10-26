namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class HopDongThanhToan_ReponseDTO
    {
        public string maHD { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string tenFreelancer { get; set; } = string.Empty;
        public decimal sotienkyquy { get; set; }
        public decimal soTienHoaHong { get; set; }
        public decimal soTienFreelancerNhan { get; set; }
        public decimal soDuKhaDung { get; set; }
        public decimal soDuDongBang { get; set; }
        public bool daCoDuLieuEscrow { get; set; }
    }
}

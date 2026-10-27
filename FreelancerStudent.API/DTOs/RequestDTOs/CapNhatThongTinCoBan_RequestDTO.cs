namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class CapNhatThongTinCoBan_RequestDTO
    {
        public int maUser { get; set; }
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;
        public string? ngonNgu { get; set; }
        public string? kyNangCoBan { get; set; }
        public decimal? chiPhiTu { get; set; }
    }

    public class CapNhatGPA_RequestDTO
    {
        public int maUser { get; set; }
        public double GPA { get; set; }
    }
}

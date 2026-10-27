namespace FreelancerStudent.Web.ViewModels
{
    public class CapNhatThongTinCoBanViewModel
    {
        public int MaUser { get; set; }
        public string DiaDiemFreelancerStudent { get; set; } = string.Empty;
        public string? NgonNgu { get; set; }
        public string? KyNangCoBan { get; set; }
        public decimal? ChiPhiTu { get; set; }
    }

    public class CapNhatGPAViewModel
    {
        public int MaUser { get; set; }
        public double GPA { get; set; }
    }
}

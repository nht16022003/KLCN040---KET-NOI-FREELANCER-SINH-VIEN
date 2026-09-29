using System.Collections.Generic;

namespace MOCK.Models.Entities
{
    public class Portfolio
    {
        public string MaPortfolio { get; set; } = string.Empty;
        public int MaFreelancerStudents { get; set; }
        public string? MoTaBanThan { get; set; }
        public string? Url_video { get; set; }

        // Navigation properties
        public FreelancerStudent? FreelancerStudent { get; set; }
        public List<DuAnTrongPortfolio> DuAnTrongPortfolios { get; set; } = new();
    }
}

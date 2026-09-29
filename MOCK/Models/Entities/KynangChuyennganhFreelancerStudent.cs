namespace MOCK.Models.Entities
{
    public class KynangChuyennganhFreelancerStudent
    {
        public int MaKyNang { get; set; }
        public int MaFreelancerStudents { get; set; }
        public string TenKyNang { get; set; } = string.Empty;

        // Navigation property
        public FreelancerStudent? FreelancerStudent { get; set; }
    }
}

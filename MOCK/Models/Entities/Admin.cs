namespace MOCK.Models.Entities
{
    public class Admin
    {
        public int MaAdmin { get; set; }
        public string HotenAdmin { get; set; } = string.Empty;
        public int MaUser { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}

namespace MOCK.Models.Entities
{
    public class Wallet
    {
        public int MaWallet { get; set; }
        public int MaUser { get; set; }
        public decimal SoDuKhaDung { get; set; } = 0;
        public decimal SoDuDongBang { get; set; } = 0;

        // Navigation property
        public User? User { get; set; }
    }
}

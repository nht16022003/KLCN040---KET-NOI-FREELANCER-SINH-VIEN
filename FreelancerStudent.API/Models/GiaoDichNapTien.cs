namespace FreelancerStudent.API.Models
{
    public class GiaoDichNapTien
    {
        public int maNapTien { get; set; }

        public int maWallet { get; set; }

        public string maGiaoDich { get; set; } = string.Empty;

        public decimal soTien { get; set; }

        public string noiDungChuyenKhoan { get; set; } = string.Empty;

        public string trangThai { get; set; } = "DangXuLy";

        public DateTime ngayTao { get; set; }

        public DateTime ngayHetHan { get; set; }

        public DateTime? ngayHoanThanh { get; set; }

        public string? maGiaoDichNganHang { get; set; }

        public int? maAdminXuLy { get; set; }

        public virtual Wallet? Wallet { get; set; }
    }
}
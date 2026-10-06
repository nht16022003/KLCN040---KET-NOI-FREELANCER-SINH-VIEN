namespace FreelancerStudent.API.DTOs.Reponse
{
    public class GiaoDichNapTien_ReponseDTO
    {
        public int maNapTien { get; set; }

        public string maGiaoDich { get; set; } = string.Empty;

        public decimal soTien { get; set; }

        public string noiDungChuyenKhoan { get; set; } = string.Empty;

        public string trangThai { get; set; } = string.Empty;

        public DateTime ngayTao { get; set; }

        public DateTime ngayHetHan { get; set; }
    }
}
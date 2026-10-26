using System;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class LichSuGiaoDich_ResponseDTO
    {
        public int maGiaoDich { get; set; }
        public int maWallet { get; set; }
        public string loaiGiaoDich { get; set; } = string.Empty;
        public decimal soTien { get; set; }
        public decimal soDuTruocGD { get; set; }
        public decimal soDuSauGD { get; set; }
        public string? noiDung { get; set; }
        public string? phuongThucThanhToan { get; set; }
        public string? maGiaoDichNgoai { get; set; }
        public string? trangThai { get; set; }
        public DateTime? ngayTao { get; set; }
    }
}

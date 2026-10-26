using System;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class YeuCauRutTien_ResponseDTO
    {
        public int maRutTien { get; set; }
        public int maWallet { get; set; }
        public decimal soTienRut { get; set; }
        public string tenNganHang { get; set; } = string.Empty;
        public string soTaiKhoan { get; set; } = string.Empty;
        public string tenChuTaiKhoan { get; set; } = string.Empty;
        public DateTime? ngayYeuCau { get; set; }
        public DateTime? ngayXuLy { get; set; }
        public string? trangThai { get; set; }
        public string? lyDoTuChoi { get; set; }
    }
}

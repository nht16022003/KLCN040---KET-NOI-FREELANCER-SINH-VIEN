
using System;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class BanGiaoResponseDTO
    {
        public int MaBanGiao { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string PhienBan { get; set; } = string.Empty;
        public string FileSanPhamUrl { get; set; } = string.Empty;
        public string? LinkDemo { get; set; }
        public string MoTaBanGiao { get; set; } = string.Empty;
        public DateTime NgayNop { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
    }
}

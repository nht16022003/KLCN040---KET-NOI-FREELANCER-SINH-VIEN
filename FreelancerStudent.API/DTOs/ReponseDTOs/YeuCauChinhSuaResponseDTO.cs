
using System;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class YeuCauChinhSuaResponseDTO
    {
        public int MaYeuCauChinhSua { get; set; }
        public int MaBanGiao { get; set; }
        public string NoiDungYeuCau { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
    }
}

using System;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class TaskCongViec_ResponseDTO
    {
        public int maTask { get; set; }
        public string maHD { get; set; } = string.Empty;
        public int? maGiaiDoan { get; set; }
        public string? tenGiaiDoan { get; set; }
        public string tenTask { get; set; } = string.Empty;
        public string moTa { get; set; } = string.Empty;
        public string? linkSanPham { get; set; }
        public DateTime? hanHoanThanh { get; set; }
        public string trangThai { get; set; } = "ChuaThucHien";
        public DateTime ngayCapNhat { get; set; } = DateTime.Now;
    }
}

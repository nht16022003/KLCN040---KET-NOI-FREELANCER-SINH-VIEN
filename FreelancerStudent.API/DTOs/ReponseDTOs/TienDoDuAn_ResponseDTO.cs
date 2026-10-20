using System;
using System.Collections.Generic;

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class TienDoDuAn_ResponseDTO
    {
        public string maHD { get; set; } = string.Empty;
        public string trangThaiHopDong { get; set; } = string.Empty;
        public DateTime? ngayBatDauHopDong { get; set; }
        public DateTime? ngayKetThucHopDong { get; set; }
        public int tongSoTask { get; set; }
        public int soTaskChuaThucHien { get; set; }
        public int soTaskDangThucHien { get; set; }
        public int soTaskHoanThanh { get; set; }
        public decimal tyLeHoanThanh { get; set; } // % tính từ 0 đến 100
        public bool coQuyenCapNhat { get; set; } // true nếu là Freelancer của hợp đồng và hợp đồng DangThucHien
        public bool coQuyenDanhGia { get; set; } // true nếu là Nhà tuyển dụng của hợp đồng
        public bool duAnDaDat { get; set; } // true nếu tất cả giai đoạn đều có trạng thái Dat
        public List<TaskCongViec_ResponseDTO> danhSachTask { get; set; } = new List<TaskCongViec_ResponseDTO>();
        public List<GiaiDoanHopDong_ResponseDTO> danhSachGiaiDoan { get; set; } = new List<GiaiDoanHopDong_ResponseDTO>();
    }
}

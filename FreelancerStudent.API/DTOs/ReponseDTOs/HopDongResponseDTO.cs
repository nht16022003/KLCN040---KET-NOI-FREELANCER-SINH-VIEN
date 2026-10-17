
namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class HopDongResponseDTO
    {
        public string maHD { get; set; } = string.Empty;

        public string maJob { get; set; } = string.Empty;

        public string tenCongViec { get; set; } = string.Empty;

        public string? moTaCongViec { get; set; }

        public int maFreelancerStudent { get; set; }

        public string tenFreelancer { get; set; } = string.Empty;

        public string? emailFreelancer { get; set; }

        public string? sdtFreelancer { get; set; }

        public string? truongFreelancer { get; set; }

        public string? chuyenNganhFreelancer { get; set; }

        public int? maUserFreelancer { get; set; }

        public int? maNhaTuyenDung { get; set; }

        public string? tenNhaTuyenDung { get; set; } = string.Empty;

        public string? emailNhaTuyenDung { get; set; }

        public string? sdtNhaTuyenDung { get; set; }

        public int? maUserNhaTuyenDung { get; set; }

        public DateTime? ngayBatDau { get; set; }

        public DateTime? ngayKetThuc { get; set; }

        public decimal? sotienkyquy { get; set; }

        public string? hinhthuclamviec { get; set; }

        public string? dieuKhoan { get; set; }

        public string trangThai { get; set; } = string.Empty;

        public List<GiaiDoanHopDong_ResponseDTO> giaiDoans { get; set; } = new List<GiaiDoanHopDong_ResponseDTO>();
    }

    public class GiaiDoanHopDong_ResponseDTO
    {
        public int maGiaiDoan { get; set; }
        public string maHD { get; set; } = string.Empty;
        public string tenGiaiDoan { get; set; } = string.Empty;
        public string? moTa { get; set; }
        public DateTime hanChot { get; set; }
        public string trangThai { get; set; } = string.Empty;
    }
}
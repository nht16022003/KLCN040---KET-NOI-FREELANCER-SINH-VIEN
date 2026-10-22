namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class HopDongDanhGia_ReponseDTO
    {
        public string maHD { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string tenDoiTac { get; set; } = string.Empty;
        public string trangThaiHopDong { get; set; } = string.Empty;
        public string trangThaiDanhGia { get; set; } = "ChoDanhGia";
        public int? maDanhGia { get; set; }
        public bool coTheDanhGia { get; set; }
        public bool coTheChinhSuaDanhGia { get; set; }
        public DateTime? hanDanhGia { get; set; }
    }
}

namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class DanhGia_FormInfo_ReponseDTO
    {
        public string maHD { get; set; } = string.Empty;
        public string tenDoiTac { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string vaiTroNguoiDanhGia { get; set; } = string.Empty;
        public string trangThaiDanhGia { get; set; } = "ChoDanhGia";
        public bool coTheDanhGia { get; set; }
        public DateTime? hanDanhGia { get; set; }
        public DanhGia_ReponseDTO? danhGiaHienTai { get; set; }
        public string? thongBaoLoi { get; set; }
    }
}

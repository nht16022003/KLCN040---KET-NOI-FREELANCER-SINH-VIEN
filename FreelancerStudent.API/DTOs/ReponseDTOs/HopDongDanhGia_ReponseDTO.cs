namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class HopDongDanhGia_ReponseDTO
    {
        public string maHD { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string tenDoiTac { get; set; } = string.Empty;
        public string vaiTroDoiTac { get; set; } = string.Empty;
        public int? maFreelancerStudent { get; set; }
        public int? maNhaTuyenDung { get; set; }
        public decimal? tongTien { get; set; }
        public DateTime? ngayHoanThanh { get; set; }
        public string trangThaiHopDong { get; set; } = string.Empty;
        public string trangThaiDanhGia { get; set; } = "ChoDanhGia";
        public int? maDanhGia { get; set; }
        public int? soSao { get; set; }
        public int? soSaoChatLuong { get; set; }
        public int? soSaoDungHan { get; set; }
        public int? soSaoGiaoTiep { get; set; }
        public string? nhanXet { get; set; }
        public DateTime? ngayDanhGia { get; set; }
        public bool coTheDanhGia { get; set; }
        public bool coTheChinhSuaDanhGia { get; set; }
        public DateTime? hanDanhGia { get; set; }
        public bool doiTacDaDanhGia { get; set; }
        public int? soSaoDoiTac { get; set; }
        public string? nhanXetDoiTac { get; set; }
        public DateTime? ngayDanhGiaDoiTac { get; set; }
    }
}

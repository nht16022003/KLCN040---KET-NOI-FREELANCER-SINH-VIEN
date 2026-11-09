namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class YeuCauThue_ReponseDTO
    {
        public int maYeuCau { get; set; }
        public int maNhaTuyenDung { get; set; }
        public int maUserNTD { get; set; }
        public string tenNhaTuyenDung { get; set; } = string.Empty;
        public string? avatarNhaTuyenDung { get; set; }
        public int maFreelancerStudent { get; set; }
        public int maUserFreelancer { get; set; }
        public string tenFreelancer { get; set; } = string.Empty;
        public string? avatarFreelancer { get; set; }
        public string? maJob { get; set; }
        public string? tieuDeJob { get; set; }
        public string tieuDeCongViec { get; set; } = string.Empty;
        public string? moTaCongViec { get; set; }
        public decimal? nganSachDeNghi { get; set; }
        public string? thoiHanDuKien { get; set; }
        public DateTime ngayGui { get; set; }
        public string trangThai { get; set; } = "ChoPhanHoi";
        public string? lyDoTuChoi { get; set; }
    }
}


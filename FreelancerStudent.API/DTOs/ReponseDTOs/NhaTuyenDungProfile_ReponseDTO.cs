using FreelancerStudent.API.DTOs.ReponseDTOs;

namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class NhaTuyenDungProfile_ReponseDTO
    {
        public ProfileUserResponse User { get; set; } = new();
        public ProfileNhaTuyenDungResponse NhaTuyenDung { get; set; } = new();
        public List<ProfileJobPostResponse> DanhSachTinTuyenDung { get; set; } = new();
    }

    public class ProfileNhaTuyenDungResponse
    {
        public int MaNhaTuyenDung { get; set; }
        public int MaUser { get; set; }
        public string? Tencongty { get; set; }
        public string? Linhvuc { get; set; }
        public string? Diachi { get; set; }
        public string? Gioithieu { get; set; }
        public string? Link { get; set; }
        public string? Logo { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string Trangthai { get; set; } = "Active";
        public double Sosaodanhgia { get; set; }
        public int SoLuotDanhGia { get; set; }
        public double TyLeHaiLong { get; set; }
        public int TongTinTuyenDung { get; set; }
        public int TinDangTuyen { get; set; }
        public int SoHopDongHoanThanh { get; set; }
        public int SoFreelancerHopTac { get; set; }
        public List<DanhGia_ReponseDTO> DanhGiaGanDay { get; set; } = new();
    }

    public class ProfileJobPostResponse
    {
        public string MaJob { get; set; } = string.Empty;
        public string Tieude { get; set; } = string.Empty;
        public string? Mota { get; set; }
        public decimal? Thulao { get; set; }
        public string? Kynangyeucau { get; set; }
        public string? Status { get; set; } = string.Empty;
        public DateTime Thoigiandangtuyen { get; set; }
        public DateTime? Thoigiandukienhoanthanh { get; set; }
        public int? Soluongtuyen { get; set; }
    }
}

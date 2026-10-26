namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class ThongBao_ReponseDTO
    {
        public int maThongBao { get; set; }
        public string tieuDe { get; set; } = string.Empty;
        public string noiDung { get; set; } = string.Empty;
        public string loaiThongBao { get; set; } = string.Empty;
        public string? linkDieuHuong { get; set; }
        public bool daDoc { get; set; }
        public DateTime ngayTao { get; set; }
    }

    public class ThongBaoDanhSach_ReponseDTO
    {
        public List<ThongBao_ReponseDTO> danhSach { get; set; } = new();
        public int trang { get; set; }
        public int kichThuocTrang { get; set; }
        public int tongSoThongBao { get; set; }
        public int soThongBaoChuaDoc { get; set; }
    }
}

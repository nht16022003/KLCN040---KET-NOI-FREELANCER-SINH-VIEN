namespace FreelancerStudent.Web.ViewModels
{
    public class ThongBaoViewModel
    {
        public int maThongBao { get; set; }
        public string tieuDe { get; set; } = string.Empty;
        public string noiDung { get; set; } = string.Empty;
        public string loaiThongBao { get; set; } = string.Empty;
        public string? linkDieuHuong { get; set; }
        public bool daDoc { get; set; }
        public DateTime ngayTao { get; set; }
    }

    public class ThongBaoDanhSachViewModel
    {
        public List<ThongBaoViewModel> danhSach { get; set; } = new();
        public int trang { get; set; }
        public int kichThuocTrang { get; set; }
        public int tongSoThongBao { get; set; }
        public int soThongBaoChuaDoc { get; set; }
        public string? Loi { get; set; }
    }
}

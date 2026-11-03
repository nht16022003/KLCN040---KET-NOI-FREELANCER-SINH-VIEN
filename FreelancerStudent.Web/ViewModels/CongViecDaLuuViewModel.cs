namespace FreelancerStudent.Web.ViewModels
{
    public class CongViecDaLuuViewModel
    {
        public int maLuuViec { get; set; }
        public string maJob { get; set; } = string.Empty;
        public string tieuDe { get; set; } = string.Empty;
        public string? moTa { get; set; }
        public string? kyNangYeuCau { get; set; }
        public decimal? thuLao { get; set; }
        public DateTime thoiGianDangTuyen { get; set; }
        public DateTime thoiGianDuKienHoanThanh { get; set; }
        public string status { get; set; } = string.Empty;
        public int? soLuongTuyen { get; set; }
        public int maNhaTuyenDung { get; set; }
        public string? tenCongTy { get; set; }
        public string? avatarNhaTuyenDung { get; set; }
        public DateTime ngayLuu { get; set; }
        public string? ghiChu { get; set; }
        public bool daUngTuyen { get; set; }
    }
}


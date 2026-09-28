namespace FreelancerStudent.Web.ViewModels
{
    public class NhaTuyenDungViewModel
    {
        public int maNhaTuyenDung { get; set; }
        public int maUser { get; set; }
        public string? hotenUser { get; set; }
        public string? emailUser { get; set; }
        public string? sdtUser { get; set; }
        public string? tencongty { get; set; }
        public string? linhvuc { get; set; }
        public string? diachi { get; set; }
        public string? gioithieu { get; set; }
        public string? avatar { get; set; }
        public string? logo { get; set; }
        public double? sosaodanhgia { get; set; }
        public string trangthai { get; set; } = string.Empty;
        public DateTime ngayDangKy { get; set; }
    }
}
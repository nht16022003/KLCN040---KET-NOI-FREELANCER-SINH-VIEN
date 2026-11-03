namespace FreelancerStudent.Web.ViewModels
{
    public class FreelancerYeuThichViewModel
    {
        public int maBookMark { get; set; }
        public int maFreelancerStudent { get; set; }
        public int maUser { get; set; }
        public string hoTen { get; set; } = string.Empty;
        public string? avatarUrl { get; set; }
        public string? email { get; set; }
        public string? tenChuyenNganh { get; set; }
        public string? tenTruong { get; set; }
        public int namThu { get; set; }
        public double gpa { get; set; }
        public double diemDanhGiaTrungBinh { get; set; }
        public int soLuotDanhGia { get; set; }
        public bool trangThaiNhanViec { get; set; }
        public decimal? chiPhiTu { get; set; }
        public List<string> kyNangs { get; set; } = new();
        public DateTime ngayLuu { get; set; }
        public string? ghiChu { get; set; }
    }
}


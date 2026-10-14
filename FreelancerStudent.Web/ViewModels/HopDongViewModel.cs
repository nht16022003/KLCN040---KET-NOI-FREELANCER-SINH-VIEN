
namespace FreelancerStudent.Web.ViewModels
{
    public class HopDongViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string maJob { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public int maFreelancerStudent { get; set; }
        public string tenFreelancer { get; set; } = string.Empty;
        public DateTime? ngayBatDau { get; set; }
        public DateTime? ngayKetThuc { get; set; }
        public decimal? sotienkyquy { get; set; }
        public string trangThai { get; set; } = string.Empty;
    }

    public class DanhSachHopDongViewModel
    {
        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }

        public List<HopDongViewModel> HopDongs { get; set; }
            = new List<HopDongViewModel>();

        public string? Loi { get; set; }
    }
}

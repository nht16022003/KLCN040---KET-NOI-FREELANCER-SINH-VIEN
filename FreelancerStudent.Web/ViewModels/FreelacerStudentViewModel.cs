
namespace FreelancerStudent.Web.ViewModels
{

    public class FreelacerStudentViewModel
    {
        public int maFreelancerStudents { get; set; }


        public string maChuyenNganh { get; set; } = string.Empty;

        public string tenChuyenNganh { get; set; } = string.Empty;


        public int maUser { get; set; }

        public string? tenUser { get; set; }

        public string maTruong { get; set; } = string.Empty;

        public string tenTruong { get; set; } = string.Empty;

        public string diaDiemTruong { get; set; } = string.Empty;
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;

        public string? ngonNgu { get; set; }

        public string? kyNangCoBan { get; set; }

        public int namThu { get; set; } = 1;

        public double GPA { get; set; } = 0;

        public string nienKhoa { get; set; } = string.Empty;

        public string? gioithieu { get; set; }


        public string? avatar { get; set; }


        public bool trangthaiNhanViec { get; set; } = true;

        public decimal? chiPhiTu { get; set; }

        public double diemDanhGiaTrungBinh { get; set; }
        public int soLuotDanhGia { get; set; }
        public DateTime ngayTao { get; set; } = DateTime.UtcNow;
        public List<string> danhSachKyNang { get; set; } = new();
    }

    public class FreelancerFilterViewModel
    {
        public List<FreelacerStudentViewModel> DanhSachFreelancer { get; set; } = new();

        // Tiêu chí tìm kiếm & lọc
        public string? TuKhoa { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? KyNang { get; set; }
        public double? SoSaoMin { get; set; }
        public double? GpaMin { get; set; }
        public int? NamThu { get; set; }
        public bool? TrangThaiNhanViec { get; set; }
        public string? SapXep { get; set; } = "moi-nhat"; // Mặc định sắp xếp hồ sơ mới nhất

        // Danh sách lựa chọn cho dropdown
        public List<string> DanhSachChuyenNganh { get; set; } = new();
        public List<string> DanhSachKyNang { get; set; } = new();

        public int TongSoFreelancer => DanhSachFreelancer.Count;
    }
}
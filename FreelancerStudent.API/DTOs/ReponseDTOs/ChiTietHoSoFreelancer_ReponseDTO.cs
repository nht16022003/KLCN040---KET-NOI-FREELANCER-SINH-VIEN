namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class ChiTietHoSoFreelancer_ReponseDTO
    {
        public int maFreelancerStudents { get; set; }
        public int maUser { get; set; }
        public string tenUser { get; set; } = string.Empty;
        public string? sdt { get; set; }
        public string? email { get; set; }
        public string? avatar { get; set; }

        // Thông tin trường & học vấn
        public string maChuyenNganh { get; set; } = string.Empty;
        public string tenChuyenNganh { get; set; } = string.Empty;
        public string tenTruong { get; set; } = string.Empty;
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;
        public int namThu { get; set; }
        public double GPA { get; set; }
        public string nienKhoa { get; set; } = string.Empty;

        // Giới thiệu & Thù lao
        public string? gioithieu { get; set; }
        public string? kyNangCoBan { get; set; }
        public string? ngonNgu { get; set; }
        public bool trangthaiNhanViec { get; set; } = true;
        public decimal? chiPhiTu { get; set; }

        // Danh sách kỹ năng chuyên môn
        public List<string> danhSachKyNang { get; set; } = new List<string>();
    }
}

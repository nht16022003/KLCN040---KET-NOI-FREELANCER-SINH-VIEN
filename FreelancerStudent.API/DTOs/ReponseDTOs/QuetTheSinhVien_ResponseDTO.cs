namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class QuetTheSinhVien_ResponseDTO
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public double DoTinCay { get; set; } // Độ tin cậy OCR (0.0 -> 100.0%)
        public string PhuongThucNhanDien { get; set; } = string.Empty; // "WINDOWS_OCR", "BARCODE_OR_QR", "HYBRID"

        // Thông tin trích xuất từ Thẻ Sinh Viên
        public string? Mssv { get; set; }
        public string? HoVaTen { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? KhoaNganh { get; set; }
        public string? MaChuyenNganhGoiY { get; set; }
        public string? TenChuyenNganhGoiY { get; set; }
        // true nếu chuyên ngành đọc từ thẻ chưa có trong bảng ChuyenNganh và vừa được thêm mới
        public bool ChuyenNganhMoi { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }
        public DateTime? NgaySinh { get; set; }

        // Đường dẫn file ảnh thẻ SV đã lưu trên server
        public string FileMinhChungUrl { get; set; } = string.Empty;

        // Dữ liệu đối chiếu hiện tại trong hồ sơ sinh viên
        public ThongTinHienTai_DTO HoSoHienTai { get; set; } = new();

        // Chuỗi text thô nhận diện được từ OCR
        public string? RawOcrText { get; set; }
    }

    public class ThongTinHienTai_DTO
    {
        public int MaUser { get; set; }
        public int MaFreelancerStudents { get; set; }
        public string HoTenHienTai { get; set; } = string.Empty;
        public DateTime? NgaySinhHienTai { get; set; }
        public string DiaDiemHienTai { get; set; } = string.Empty;
        public string TenTruong { get; set; } = string.Empty;
        public string ChuyenNganh { get; set; } = string.Empty;
    }

    public class MinhChungItem_ResponseDTO
    {
        public int MaMinhChung { get; set; }
        public int MaUser { get; set; }
        public string LoaiMinhChung { get; set; } = string.Empty;
        public string FileMinhChung { get; set; } = string.Empty;
        public DateTime NgayNop { get; set; }
        public DateTime? NgayXacMinh { get; set; }
        public string TrangThaiGuiMinhChung { get; set; } = string.Empty;
        public string? LyDoTuChoi { get; set; }
        public int? NguoiXacMinh { get; set; }

        // Thông tin hiện tại của User
        public string? HoTenUserHienTai { get; set; }
        public string? EmailUserHienTai { get; set; }
        public string? TenTruongHienTai { get; set; }
        public string? TenChuyenNganhHienTai { get; set; }

        // Thông tin trích xuất & xác nhận từ Thẻ sinh viên
        public string? MssvXacNhan { get; set; }
        public string? HoTenXacNhan { get; set; }
        public string? TenTruongXacNhan { get; set; }
        public string? MaTruongXacNhan { get; set; }
        public string? MaChuyenNganhXacNhan { get; set; }
        public string? TenChuyenNganhXacNhan { get; set; }
        public string? NienKhoaXacNhan { get; set; }
        public int? NamThuXacNhan { get; set; }
        public DateTime? NgaySinhXacNhan { get; set; }
    }

    public class ChuyenNganhOption_DTO
    {
        public string MaChuyenNganh { get; set; } = string.Empty;
        public string TenChuyenNganh { get; set; } = string.Empty;
    }

    public class KiemTraXacThuc_ResponseDTO
    {
        public bool DaXacMinh { get; set; }
        public string TrangThai { get; set; } = string.Empty; // "ChuaGui", "DangGui", "DaXacMinh", "DaTuChoi", "HetHanNienKhoa", "SaiDinhDangNienKhoa"
        public string ThongBao { get; set; } = string.Empty;
        public DateTime? NgayXacMinh { get; set; }
        public string? LyDoTuChoi { get; set; }
        public int? MaMinhChungMoiNhat { get; set; }

        // Bổ sung kiểm soát thời hạn niên khóa
        public bool HetHanNienKhoa { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamKetThucNienKhoa { get; set; }
    }

    public class PheDuyetMinhChung_RequestDTO
    {
        public int MaMinhChung { get; set; }
        public bool ChapNhan { get; set; } = true;
        public string? LyDoTuChoi { get; set; }
        public int? NguoiXacMinh { get; set; }

        // Tùy chọn thông tin cập nhật chính thức khi duyệt
        public string? HoVaTen { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? MaChuyenNganh { get; set; }
        public string? TenChuyenNganh { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }
        public DateTime? NgaySinh { get; set; }
    }
}



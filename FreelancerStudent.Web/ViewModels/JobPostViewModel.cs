using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FreelancerStudent.Web.ViewModels
{
    public class JobPostViewModel
    {
        public string maJob { get; set; } = string.Empty;

        public int maNhaTuyenDung { get; set; }


        public string tieude { get; set; } = string.Empty;

        public string? mota { get; set; }


        public string? kynangyeucau { get; set; }

        public decimal? thulao { get; set; }


        public string? fileDinhKem { get; set; } = string.Empty;


        public decimal phiDangBai { get; set; }


        public DateTime thoigiandangtuyen { get; set; } = DateTime.UtcNow;


        public DateTime thoigiandukienhoanthanh { get; set; }


        public string? status { get; set; } = "DangTuyen";

        public int? soluongtuyen { get; set; }
        public int soLuongUngTuyen { get; set; }

        public string? tenCongTy { get; set; }
        public string? avatarNhaTuyenDung { get; set; }

        public int maUser { get; set; }
        public bool DaTuyenDu { get; set; }

        public double? soSaoDanhGiaNhaTuyenDung { get; set; }
        public int soLuotDanhGiaNhaTuyenDung { get; set; }
        public string? diaChiNhaTuyenDung { get; set; }
        public string? linhVucNhaTuyenDung { get; set; }

        [JsonIgnore]
        public string MaJob => maJob;

        [JsonIgnore]
        public string Tieude => tieude;

        [JsonIgnore]
        public string? Mota => mota;

        [JsonIgnore]
        public string? Kynangyeucau => kynangyeucau;

        [JsonIgnore]
        public decimal? Thulao => thulao;

        [JsonIgnore]
        public DateTime Thoigiandangtuyen => thoigiandangtuyen;

        [JsonIgnore]
        public DateTime Thoigiandukienhoanthanh => thoigiandukienhoanthanh;

        [JsonIgnore]
        public string? Status => status;

        [JsonIgnore]
        public int? Soluongtuyen => soluongtuyen;

        [JsonIgnore]
        public string? TenCongTy => tenCongTy;

        [JsonIgnore]
        public string? AvatarNhaTuyenDung => avatarNhaTuyenDung;
    }

    public class JobPostFilterViewModel
    {
        public List<JobPostViewModel> DanhSachJobPost { get; set; } = new();

        // Tiêu chí tìm kiếm & lọc
        public string? TuKhoa { get; set; }
        public int? MaNhaTuyenDung { get; set; }
        public string? MucLuong { get; set; } // "duoi-1tr", "1tr-3tr", "3tr-10tr", "tren-10tr", "thoa-thuan"
        public string? KyNang { get; set; }
        public string? TrangThai { get; set; } = "DangTuyen"; // Mặc định hiển thị tin đang tuyển
        public double? SoSaoMin { get; set; }
        public string? SapXep { get; set; } = "moi-nhat"; // Mặc định sắp xếp mới nhất

        // Danh sách lựa chọn cho dropdown
        public List<KeyValuePair<int, string>> DanhSachNhaTuyenDung { get; set; } = new();
        public List<string> DanhSachKyNang { get; set; } = new();

        public int TongSoCongViec => DanhSachJobPost.Count;
    }
}
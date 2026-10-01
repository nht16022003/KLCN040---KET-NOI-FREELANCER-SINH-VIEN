//TUẤN
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class UngTuyen_ReponseDTO
    {

        public int maUngTuyen { get; set; }

        public string maJob { get; set; } = string.Empty; //Khóa ngoại


        [Required]
        public string tieude { get; set; } = string.Empty;

        public decimal? thulao { get; set; }

        public int maFreelancerStudents { get; set; } //Khóa ngoại

        public string thuGioiThieu { get; set; } = string.Empty;

        public decimal? thulaoDeXuat { get; set; }

        public string? thoiGianHoanThanhDeXuat { get; set; }

        public string? fileCV { get; set; }

        public DateTime ngayUngTuyen { get; set; } = DateTime.UtcNow;

        public string trangThaiUngTuyen { get; set; } = "ChoDuyet";


        public int maNhaTuyenDung { get; set; }


        public string? mota { get; set; }


        public string? kynangyeucau { get; set; }


        public string? fileDinhKem { get; set; } = string.Empty;


        public decimal phiDangBai { get; set; }


        public DateTime thoigiandangtuyen { get; set; } = DateTime.UtcNow;


        public DateTime thoigiandukienhoanthanh { get; set; }


        public string? status { get; set; } = "DangTuyen";

        public int? soluongtuyen { get; set; }


        public string maChuyenNganh { get; set; } = string.Empty;

        public string tenChuyenNganh { get; set; } = string.Empty;


        public int maUser { get; set; }

        public string? tenUser { get; set; }

        public string? emailUser { get; set; }

        public string? sdtUser { get; set; }

        public DateTime? ngaysinh { get; set; }

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

        public string? tenCongTy { get; set; }
        public string? avatarNhaTuyenDung { get; set; }
    }


}
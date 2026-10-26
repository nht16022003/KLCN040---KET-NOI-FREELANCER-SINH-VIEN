
using System;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class TaoBanGiaoViewModel
    {
        [Required]
        public string MaHD { get; set; } = string.Empty;

        [Required]
        public string FileSanPhamUrl { get; set; } = string.Empty;

        public string? LinkDemo { get; set; }

        [Required]
        public string MoTaBanGiao { get; set; } = string.Empty;
    }

    public class BanGiaoResponseViewModel
    {
        public int MaBanGiao { get; set; }
        public string MaHD { get; set; } = string.Empty;
        public string PhienBan { get; set; } = string.Empty;
        public string FileSanPhamUrl { get; set; } = string.Empty;
        public string? LinkDemo { get; set; }
        public string MoTaBanGiao { get; set; } = string.Empty;
        public DateTime NgayNop { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
    }

    public class TaoYeuCauChinhSuaViewModel
    {
        [Range(1, int.MaxValue)]
        public int MaBanGiao { get; set; }

        [Required]
        public string NoiDungYeuCau { get; set; } = string.Empty;
    }

    public class YeuCauChinhSuaResponseViewModel
    {
        public int MaYeuCauChinhSua { get; set; }
        public int MaBanGiao { get; set; }
        public string NoiDungYeuCau { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
    }
    
    public class ChiTietBanGiaoViewModel
    {
        public string MaHD { get; set; } = string.Empty;

        public string TrangThaiHopDong { get; set; }
            = string.Empty;

        public bool LaFreelancer { get; set; }

        public bool LaKhachHang { get; set; }

        public List<BanGiaoResponseViewModel>
            DanhSachBanGiao { get; set; } = new();

        public List<YeuCauChinhSuaResponseViewModel>
            DanhSachYeuCau { get; set; } = new();
    }

}

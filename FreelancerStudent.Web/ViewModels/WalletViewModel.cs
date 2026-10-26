using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class WalletViewModel
    {
        public int maWallet { get; set; }

        public string? tenUsers { get; set; }

        public decimal soDuKhaDung { get; set; }

        public decimal soDuDongBang { get; set; }

        public decimal tongTaiSan => soDuKhaDung + soDuDongBang;

        public List<LichSuGiaoDichItemViewModel> LichSuGiaoDichs { get; set; } = new();

        public List<YeuCauRutTienItemViewModel> DanhSachRutTiens { get; set; } = new();
    }

    public class LichSuGiaoDichItemViewModel
    {
        public int maGiaoDich { get; set; }
        public int maWallet { get; set; }
        public string loaiGiaoDich { get; set; } = string.Empty;
        public decimal soTien { get; set; }
        public decimal soDuTruocGD { get; set; }
        public decimal soDuSauGD { get; set; }
        public string? noiDung { get; set; }
        public string? phuongThucThanhToan { get; set; }
        public string? maGiaoDichNgoai { get; set; }
        public string? trangThai { get; set; }
        public DateTime? ngayTao { get; set; }
    }

    public class YeuCauRutTienItemViewModel
    {
        public int maRutTien { get; set; }
        public int maWallet { get; set; }
        public decimal soTienRut { get; set; }
        public string tenNganHang { get; set; } = string.Empty;
        public string soTaiKhoan { get; set; } = string.Empty;
        public string tenChuTaiKhoan { get; set; } = string.Empty;
        public DateTime? ngayYeuCau { get; set; }
        public DateTime? ngayXuLy { get; set; }
        public string? trangThai { get; set; }
        public string? lyDoTuChoi { get; set; }
    }

    public class RutTienRequestModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền rút.")]
        [Range(50000, 100000000, ErrorMessage = "Số tiền rút tối thiểu là 50.000 VNĐ.")]
        public decimal soTienRut { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng.")]
        [MaxLength(100)]
        public string tenNganHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng.")]
        [MaxLength(50)]
        public string soTaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản thụ hưởng.")]
        [MaxLength(100)]
        public string tenChuTaiKhoan { get; set; } = string.Empty;
    }
}
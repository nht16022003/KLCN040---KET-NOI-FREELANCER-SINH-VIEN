using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class TaiKhoanNganHangListViewModel
    {
        public List<TaiKhoanNganHang> TaiKhoans { get; set; } = new();
        public TaiKhoanNganHangCreateViewModel NewAccount { get; set; } = new();
    }

    public class TaiKhoanNganHangCreateViewModel
    {
        public int MaUser { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng chọn ngân hàng")]
        [Display(Name = "Tên ngân hàng")]
        public string TenNganHang { get; set; } = string.Empty;

        [Display(Name = "Chi nhánh (tùy chọn)")]
        public string? ChiNhanh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng")]
        [RegularExpression(@"^[0-9]{6,20}$", ErrorMessage = "Số tài khoản phải gồm 6-20 chữ số")]
        [Display(Name = "Số tài khoản")]
        public string SoTaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản")]
        [Display(Name = "Tên chủ tài khoản (In hoa không dấu)")]
        public string TenChuTaiKhoan { get; set; } = string.Empty;

        [Display(Name = "Đặt làm tài khoản nhận tiền mặc định")]
        public bool LaMacDinh { get; set; } = true;
    }
}

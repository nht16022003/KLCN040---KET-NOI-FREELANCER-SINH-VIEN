using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class RutTienViewModel
    {
        public int MaWallet { get; set; } = 1;
        public decimal SoDuHienTai { get; set; } = 0;
        public decimal SoDuKhaDung { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng nhập số tiền muốn rút")]
        [Range(50000, 100000000, ErrorMessage = "Số tiền rút tối thiểu từ 50,000 VNĐ và tối đa 100,000,000 VNĐ mỗi lần")]
        [Display(Name = "Số tiền rút (VNĐ)")]
        public decimal SoTienRut { get; set; } = 500000;

        [Required(ErrorMessage = "Vui lòng chọn hoặc nhập tên ngân hàng")]
        [Display(Name = "Tên ngân hàng")]
        public string TenNganHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng")]
        [Display(Name = "Số tài khoản ngân hàng")]
        public string SoTaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản (viết hoa không dấu)")]
        [Display(Name = "Tên chủ tài khoản")]
        public string TenChuTaiKhoan { get; set; } = string.Empty;

        public int? SelectedMaTKNH { get; set; }

        // Danh sách tài khoản ngân hàng đã lưu của người dùng
        public List<TaiKhoanNganHang> TaiKhoanDaLuus { get; set; } = new();

        // Lịch sử các yêu cầu rút tiền
        public List<YeuCauRutTien> LichSuRutTiens { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOCK.Models.ViewModels
{
    public class DeXuatHopDongViewModel
    {
        public string? MaJob { get; set; }
        public int? MaYeuCauUngThue { get; set; }

        public int MaFreelancerStudent { get; set; } = 1;
        public int MaNhaTuyenDung { get; set; } = 1;

        // Thông tin công việc & đối tác
        public string TieuDeCongViec { get; set; } = string.Empty;
        public string TenNhaTuyenDung { get; set; } = string.Empty;
        public string? AvatarNhaTuyenDung { get; set; }

        // Điều khoản hợp đồng
        [Required(ErrorMessage = "Vui lòng nhập tổng giá trị hợp đồng")]
        [Range(100000, 500000000, ErrorMessage = "Giá trị hợp đồng từ 100,000 VNĐ")]
        [Display(Name = "Tổng giá trị hợp đồng (VNĐ)")]
        public double? TongGiaTriHopDong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức làm việc")]
        [Display(Name = "Hình thức làm việc")]
        public string Hinhthuclamviec { get; set; } = "Remote"; // Remote, Hybrid, Onsite

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc dự kiến")]
        [Display(Name = "Ngày kết thúc dự kiến")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddDays(15);

        [Display(Name = "Điều khoản bổ sung / Phạm vi công việc")]
        public string? DieuKhoanHopDong { get; set; }

        // Các mốc giai đoạn (GiaiDoan_HopDong)
        public List<GiaiDoanDeXuatItem> GiaiDoans { get; set; } = new()
        {
            new GiaiDoanDeXuatItem { TenGiaiDoan = "Giai đoạn 1: Thiết kế giao diện & Bản mẫu (Wireframe/Mockup)", TyLePhanTram = 40, HanChot = DateTime.Now.AddDays(5) },
            new GiaiDoanDeXuatItem { TenGiaiDoan = "Giai đoạn 2: Phát triển chức năng & Kiểm thử", TyLePhanTram = 40, HanChot = DateTime.Now.AddDays(12) },
            new GiaiDoanDeXuatItem { TenGiaiDoan = "Giai đoạn 3: Bàn giao sản phẩm & Tài liệu nghiệm thu", TyLePhanTram = 20, HanChot = DateTime.Now.AddDays(15) }
        };
    }

    public class GiaiDoanDeXuatItem
    {
        public string TenGiaiDoan { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public int TyLePhanTram { get; set; } = 50;
        public double? SoTien { get; set; }
        public DateTime HanChot { get; set; } = DateTime.Now.AddDays(7);
    }
}

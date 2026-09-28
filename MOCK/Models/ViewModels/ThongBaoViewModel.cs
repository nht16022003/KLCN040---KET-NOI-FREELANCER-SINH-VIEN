using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class ThongBaoListViewModel
    {
        public List<ThongBaoItemViewModel> Items { get; set; } = new();
        public int TongSoThongBao => Items.Count;
        public int SoChuaDoc { get; set; }
        public string? FilterLoai { get; set; }
        public string? FilterTrangThai { get; set; } // "All", "Unread", "Read"
        public int CurrentUserId { get; set; }
    }

    public class ThongBaoItemViewModel
    {
        public int MaThongBao { get; set; }
        public int MaUser { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiThongBao { get; set; } = "HeThong";
        public string? LinkDieuHuong { get; set; }
        public bool DaDoc { get; set; }
        public DateTime NgayTao { get; set; }

        public string ThoiGianTuongDoi => FormatRelativeTime(NgayTao);
        public string IconClass => GetIconClass(LoaiThongBao);
        public string BadgeClass => GetBadgeClass(LoaiThongBao);
        public string LoaiDisplayName => GetDisplayName(LoaiThongBao);

        private static string FormatRelativeTime(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalMinutes < 1) return "Vừa xong";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} phút trước";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} giờ trước";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays} ngày trước";
            return dt.ToString("dd/MM/yyyy HH:mm");
        }

        private static string GetIconClass(string loai) => loai switch
        {
            "TuyenDung" => "bi-briefcase-fill text-primary",
            "UngTuyen" => "bi-person-check-fill text-success",
            "UngThue" => "bi-send-check-fill text-info",
            "HopDong" => "bi-file-earmark-check-fill text-warning",
            "ThanhToan" => "bi-wallet2 text-emerald",
            "TranhChap" => "bi-shield-exclamation text-danger",
            _ => "bi-bell-fill text-secondary"
        };

        private static string GetBadgeClass(string loai) => loai switch
        {
            "TuyenDung" => "bg-primary-subtle text-primary border border-primary-subtle",
            "UngTuyen" => "bg-success-subtle text-success border border-success-subtle",
            "UngThue" => "bg-info-subtle text-info border border-info-subtle",
            "HopDong" => "bg-warning-subtle text-warning border border-warning-subtle",
            "ThanhToan" => "bg-success-subtle text-success border border-success-subtle",
            "TranhChap" => "bg-danger-subtle text-danger border border-danger-subtle",
            _ => "bg-secondary-subtle text-secondary border border-secondary-subtle"
        };

        private static string GetDisplayName(string loai) => loai switch
        {
            "TuyenDung" => "Tuyển dụng",
            "UngTuyen" => "Ứng tuyển",
            "UngThue" => "Lời mời nhận việc",
            "HopDong" => "Hợp đồng",
            "ThanhToan" => "Thanh toán & Ví",
            "TranhChap" => "Khiếu nại / Tranh chấp",
            _ => "Hệ thống"
        };
    }
}

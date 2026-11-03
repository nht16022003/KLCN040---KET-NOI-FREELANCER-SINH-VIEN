using System.Text.RegularExpressions;

namespace FreelancerStudent.API.Helper
{
    public class KetQuaKiemTraNienKhoa
    {
        public bool HopLe { get; set; }
        public bool HetHan { get; set; }
        public bool KhongXacDinh { get; set; }
        public int? NamBatDau { get; set; }
        public int? NamKetThuc { get; set; }
        public int NamHienTai { get; set; }
        public string ThongBao { get; set; } = string.Empty;
    }

    public static class NienKhoaHelper
    {
        // Câu thông báo chuẩn bắt buộc theo yêu cầu đề bài
        public const string ThongBaoHetHan = "MINH CHỨNG XÁC NHẬN SINH VIÊN ĐÃ KHÔNG CÒN HỢP LỆ, VÌ NIÊN KHÓA CỦA BẠN ĐÃ VƯỢT QUA THỜI GIAN CHO PHÉP";

        /// <summary>
        /// Lấy năm hiện tại động theo thời gian hệ thống và múi giờ máy chủ
        /// </summary>
        public static int GetCurrentYear()
        {
            return DateTime.Now.Year;
        }

        /// <summary>
        /// Phân tích chuỗi niên khóa để lấy chính xác năm kết thúc (academicYearEnd).
        /// Hỗ trợ các định dạng: "2022-2026", "2022 - 2026", "2022–2026", "2022/2026", "K22 (2022-2026)".
        /// Tránh nhầm lẫn với năm bắt đầu niên khóa.
        /// </summary>
        public static bool TryParseNienKhoa(string? nienKhoa, out int? namBatDau, out int namKetThuc)
        {
            namBatDau = null;
            namKetThuc = 0;

            if (string.IsNullOrWhiteSpace(nienKhoa))
            {
                return false;
            }

            var text = nienKhoa.Trim();

            // Mẫu 1: Dạng khoảng năm có dấu gạch ngang, gạch nối, en-dash, em-dash hoặc gạch chéo: "2022-2026", "2022 - 2026", "2022–2026"
            var matchRange = Regex.Match(text, @"(?:\b|\D)(20\d{2}|\d{4})\s*[\-\–\—\/]\s*(20\d{2}|\d{4})(?:\b|\D)");
            if (matchRange.Success)
            {
                if (int.TryParse(matchRange.Groups[1].Value, out int start) &&
                    int.TryParse(matchRange.Groups[2].Value, out int end))
                {
                    namBatDau = start;
                    namKetThuc = end;
                    return true;
                }
            }

            // Mẫu 2: Dạng chỉ có 1 năm 4 chữ số (ví dụ năm tốt nghiệp ghi đơn: "2026")
            var matchSingle = Regex.Match(text, @"^\s*(20\d{2})\s*$");
            if (matchSingle.Success && int.TryParse(matchSingle.Groups[1].Value, out int singleYear))
            {
                namKetThuc = singleYear;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của niên khóa theo quy tắc nghiệp vụ:
        /// - academicYearEnd >= currentYear => Còn hạn (Hợp lệ)
        /// - academicYearEnd < currentYear => Hết hạn (Bị chặn toàn bộ chức năng sinh viên)
        /// - Thiếu hoặc sai định dạng => Không cấp quyền, yêu cầu xác minh lại theo chính sách an toàn
        /// </summary>
        public static KetQuaKiemTraNienKhoa KiemTraNienKhoa(string? nienKhoa, int? currentYearOverride = null)
        {
            int currentYear = currentYearOverride ?? GetCurrentYear();

            if (string.IsNullOrWhiteSpace(nienKhoa))
            {
                return new KetQuaKiemTraNienKhoa
                {
                    HopLe = false,
                    HetHan = false,
                    KhongXacDinh = true,
                    NamBatDau = null,
                    NamKetThuc = null,
                    NamHienTai = currentYear,
                    ThongBao = "Hồ sơ sinh viên chưa có thông tin niên khóa. Vui lòng xác minh thẻ sinh viên để tiếp tục!"
                };
            }

            if (!TryParseNienKhoa(nienKhoa, out int? start, out int end))
            {
                return new KetQuaKiemTraNienKhoa
                {
                    HopLe = false,
                    HetHan = false,
                    KhongXacDinh = true,
                    NamBatDau = null,
                    NamKetThuc = null,
                    NamHienTai = currentYear,
                    ThongBao = "Thông tin niên khóa không đúng định dạng. Vui lòng cập nhật hoặc xác minh lại minh chứng!"
                };
            }

            // Quy tắc: academicYearEnd < currentYear => Hết hạn
            if (end < currentYear)
            {
                return new KetQuaKiemTraNienKhoa
                {
                    HopLe = false,
                    HetHan = true,
                    KhongXacDinh = false,
                    NamBatDau = start,
                    NamKetThuc = end,
                    NamHienTai = currentYear,
                    ThongBao = ThongBaoHetHan
                };
            }

            // academicYearEnd >= currentYear => Còn hạn hợp lệ
            return new KetQuaKiemTraNienKhoa
            {
                HopLe = true,
                HetHan = false,
                KhongXacDinh = false,
                NamBatDau = start,
                NamKetThuc = end,
                NamHienTai = currentYear,
                ThongBao = "Niên khóa hợp lệ."
            };
        }
    }
}

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class KyNangGoiYService : IKyNangGoiYService
    {
        private readonly ApplicationDBContext _context;

        // Cấu trúc ánh xạ từ khóa nghiệp vụ sang kỹ năng chuẩn hóa (bằng code C#, không tạo bảng mới)
        private class QuyTacAnhXa
        {
            public string[] TuKhoas { get; set; } = Array.Empty<string>();
            public string[] KyNangs { get; set; } = Array.Empty<string>();
            public string LinhVuc { get; set; } = string.Empty;
        }

        private static readonly List<QuyTacAnhXa> DanhSachQuyTac = new()
        {
            // 1. THIẾT KẾ LOGO, BANNER, BỘ NHẬN DIỆN THƯƠNG HIỆU, ĐỒ HỌA
            new QuyTacAnhXa
            {
                LinhVuc = "Thiết kế đồ họa",
                TuKhoas = new[]
                {
                    "logo", "thiet ke logo", "ve logo", "branding", "nhan dien thuong hieu",
                    "banner", "poster", "standee", "menu", "namecard", "danh thiep",
                    "to roi", "catalogue", "do hoa", "graphic design", "photoshop", "pts",
                    "illustrator", "ai", "coreldraw", "canva", "indesign"
                },
                KyNangs = new[]
                {
                    "Thiết kế logo", "Photoshop", "Adobe Illustrator", "Illustrator",
                    "Canva", "Thiết kế banner", "Thiết kế poster", "Bộ nhận diện thương hiệu",
                    "Thiết kế menu", "CorelDRAW", "Adobe InDesign", "Poster Design"
                }
            },

            // 2. GIAO DIỆN & UI/UX DESIGN
            new QuyTacAnhXa
            {
                LinhVuc = "UI/UX",
                TuKhoas = new[]
                {
                    "ui", "ux", "ui/ux", "ui ux", "giao dien", "figma", "wireframe",
                    "prototype", "user interface", "user experience"
                },
                KyNangs = new[]
                {
                    "Figma", "UI/UX Design", "UI Design", "UX Design"
                }
            },

            // 3. LẬP TRÌNH WEB (FRONTEND / BACKEND)
            new QuyTacAnhXa
            {
                LinhVuc = "Lập trình Web",
                TuKhoas = new[]
                {
                    "web", "website", "landing page", "web ban hang", "frontend", "front-end",
                    "backend", "back-end", "fullstack", "asp.net", "asp.net core", "c#", "csharp",
                    "html", "css", "javascript", "js", "typescript", "ts", "react", "reactjs",
                    "angular", "vue", "vuejs", "node", "nodejs", "php", "laravel", "sql server",
                    "mysql", "rest api", "api"
                },
                KyNangs = new[]
                {
                    "HTML/CSS", "JavaScript", "ReactJS", "ASP.NET Core", "C#", "SQL Server",
                    "Node.js", "PHP", "Laravel", "REST API", "TypeScript", "MySQL",
                    "Angular", "Vue.js", "C# / ASP.NET Core", "HTML/CSS/JavaScript"
                }
            },

            // 4. LẬP TRÌNH MOBILE APP
            new QuyTacAnhXa
            {
                LinhVuc = "Lập trình Mobile",
                TuKhoas = new[]
                {
                    "app", "mobile", "ung dung di dong", "flutter", "dart", "react native",
                    "android", "ios", "kotlin", "swift", "firebase"
                },
                KyNangs = new[]
                {
                    "Flutter", "Dart", "React Native", "Java Android", "Kotlin", "Swift", "Firebase"
                }
            },

            // 5. DATA, PHÂN TÍCH DỮ LIỆU & TRÍ TUỆ NHÂN TẠO (AI)
            new QuyTacAnhXa
            {
                LinhVuc = "Data & AI",
                TuKhoas = new[]
                {
                    "data", "du lieu", "phan tich du lieu", "power bi", "powerbi", "excel",
                    "python", "machine learning", "deep learning", "tri tue nhan tao", "sql"
                },
                KyNangs = new[]
                {
                    "Python", "SQL", "Power BI", "Data Analysis", "Machine Learning", "Deep Learning"
                }
            },

            // 6. MARKETING, CONTENT, SEO, FANPAGE
            new QuyTacAnhXa
            {
                LinhVuc = "Marketing & Nội dung",
                TuKhoas = new[]
                {
                    "content", "viet bai", "copywriting", "seo", "marketing", "digital marketing",
                    "fanpage", "facebook ads", "fb ads", "google ads", "quang cao", "social media",
                    "bien tap", "dang bai"
                },
                KyNangs = new[]
                {
                    "Content Writing", "Copywriting", "SEO", "Digital Marketing",
                    "Social Media Marketing", "Facebook Ads", "Google Ads",
                    "Quản lý Fanpage", "Biên tập nội dung"
                }
            },

            // 7. DỊCH THUẬT & NGÔN NGỮ
            new QuyTacAnhXa
            {
                LinhVuc = "Dịch thuật & Ngôn ngữ",
                TuKhoas = new[]
                {
                    "dich", "dich thuat", "tieng anh", "english", "translation", "bien dich",
                    "phien dich", "ielts", "toeic", "tieng trung", "chinese", "hsk"
                },
                KyNangs = new[]
                {
                    "Dịch thuật Anh - Việt", "Dịch thuật Việt - Anh", "Tiếng Anh",
                    "Tiếng Trung", "Viết nội dung tiếng Anh"
                }
            },

            // 8. VIDEO, MEDIA, CHỈNH SỬA ẢNH
            new QuyTacAnhXa
            {
                LinhVuc = "Video & Media",
                TuKhoas = new[]
                {
                    "video", "clip", "tiktok", "reels", "youtube", "edit video", "chinh sua video",
                    "capcut", "premiere", "after effects", "motion graphics", "chup anh",
                    "quay phim", "chinh sua anh"
                },
                KyNangs = new[]
                {
                    "CapCut", "Adobe Premiere Pro", "Adobe After Effects", "Video Editing",
                    "Motion Graphics", "Chụp ảnh sản phẩm", "Chỉnh sửa ảnh"
                }
            },

            // 9. TIN HỌC VĂN PHÒNG, NHẬP LIỆU, QA, BA
            new QuyTacAnhXa
            {
                LinhVuc = "Văn phòng & Nghiệp vụ",
                TuKhoas = new[]
                {
                    "nhap lieu", "word", "excel", "powerpoint", "soan thao", "van phong",
                    "danh may", "kiem thu", "tester", "qc", "ba", "business analyst"
                },
                KyNangs = new[]
                {
                    "Microsoft Word", "Microsoft Excel", "Microsoft PowerPoint",
                    "Nhập liệu", "Kiểm thử phần mềm", "Phân tích nghiệp vụ (BA)"
                }
            }
        };

        // Danh mục kỹ năng mặc định (đồng bộ với file Seed_KyNang_BoSung.sql)
        private static readonly string[] DanhMucKyNangChuan = new[]
        {
            // Thiết kế & UI/UX
            "Photoshop", "Illustrator", "Adobe Illustrator", "Figma", "Canva", "Thiết kế logo",
            "Thiết kế banner", "Thiết kế poster", "Bộ nhận diện thương hiệu", "Thiết kế menu",
            "CorelDRAW", "Adobe InDesign", "UI Design", "UX Design", "UI/UX Design", "Poster Design",

            // Lập trình Web
            "ASP.NET Core", "C#", "HTML/CSS", "JavaScript", "TypeScript", "ReactJS",
            "Angular", "Vue.js", "Node.js", "PHP", "Laravel", "MySQL", "REST API",
            "SQL Server", "C# / ASP.NET Core", "HTML/CSS/JavaScript",

            // Mobile
            "Flutter", "Dart", "React Native", "Kotlin", "Java Android", "Swift", "Firebase",

            // Data & AI
            "Python", "SQL", "Power BI", "Data Analysis", "Machine Learning", "Deep Learning",

            // Marketing & Content
            "Content Writing", "Copywriting", "SEO", "Digital Marketing", "Social Media Marketing",
            "Facebook Ads", "Google Ads", "Quản lý Fanpage", "Biên tập nội dung",

            // Dịch thuật & Ngôn ngữ
            "Dịch thuật Anh - Việt", "Dịch thuật Việt - Anh", "Tiếng Anh", "Tiếng Trung", "Viết nội dung tiếng Anh",

            // Video & Media
            "Adobe Premiere Pro", "Adobe After Effects", "CapCut", "Video Editing", "Motion Graphics",
            "Chụp ảnh sản phẩm", "Chỉnh sửa ảnh",

            // Văn phòng
            "Microsoft Word", "Microsoft Excel", "Microsoft PowerPoint", "Nhập liệu",
            "Kiểm thử phần mềm", "Phân tích nghiệp vụ (BA)"
        };

        public KyNangGoiYService(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gợi ý danh sách kỹ năng chuẩn trong SQL Server dựa trên tiêu đề và mô tả công việc
        /// </summary>
        public async Task<List<KyNangGoiY_ResponseDTO>> GoiYKyNangTheoVanBanAsync(string? tieuDe, string? moTa = null)
        {
            if (string.IsNullOrWhiteSpace(tieuDe) && string.IsNullOrWhiteSpace(moTa))
            {
                return new List<KyNangGoiY_ResponseDTO>();
            }

            // Chuẩn hóa văn bản đầu vào: bỏ dấu tiếng Việt, chữ thường
            var tieuDeKd = ChuanHoaKhongDau(tieuDe ?? string.Empty);
            var moTaKd = ChuanHoaKhongDau(moTa ?? string.Empty);

            var diemTheoTenKyNang = new Dictionary<string, (double Diem, string LyDo)>(StringComparer.OrdinalIgnoreCase);

            // 1. Phân tích tiêu đề (Trọng số cao nhất: 1.0)
            if (!string.IsNullOrWhiteSpace(tieuDeKd))
            {
                foreach (var quyTac in DanhSachQuyTac)
                {
                    foreach (var tuKhoa in quyTac.TuKhoas)
                    {
                        var tuKhoaKd = ChuanHoaKhongDau(tuKhoa);
                        if (KiemTraChuaTuKhoa(tieuDeKd, tuKhoaKd))
                        {
                            foreach (var kn in quyTac.KyNangs)
                            {
                                var diemHienTai = diemTheoTenKyNang.ContainsKey(kn) ? diemTheoTenKyNang[kn].Diem : 0;
                                if (diemHienTai < 1.0)
                                {
                                    diemTheoTenKyNang[kn] = (1.0, $"Phù hợp với từ khóa '{tuKhoa}' trong tiêu đề");
                                }
                            }
                        }
                    }
                }
            }

            // 2. Phân tích mô tả (Trọng số bổ trợ: 0.6)
            if (!string.IsNullOrWhiteSpace(moTaKd))
            {
                foreach (var quyTac in DanhSachQuyTac)
                {
                    foreach (var tuKhoa in quyTac.TuKhoas)
                    {
                        var tuKhoaKd = ChuanHoaKhongDau(tuKhoa);
                        if (KiemTraChuaTuKhoa(moTaKd, tuKhoaKd))
                        {
                            foreach (var kn in quyTac.KyNangs)
                            {
                                if (!diemTheoTenKyNang.ContainsKey(kn))
                                {
                                    diemTheoTenKyNang[kn] = (0.6, $"Phù hợp với từ khóa '{tuKhoa}' trong mô tả yêu cầu");
                                }
                            }
                        }
                    }
                }
            }

            // 3. Nếu không khớp từ khóa chuyên biệt nào, kiểm tra xem có chứa trực tiếp tên kỹ năng nào không
            if (diemTheoTenKyNang.Count == 0)
            {
                var vanBanKetHop = $"{tieuDeKd} {moTaKd}";
                foreach (var knChuan in DanhMucKyNangChuan)
                {
                    var knKd = ChuanHoaKhongDau(knChuan);
                    if (KiemTraChuaTuKhoa(vanBanKetHop, knKd))
                    {
                        diemTheoTenKyNang[knChuan] = (0.8, $"Tìm thấy kỹ năng '{knChuan}' trực tiếp trong nội dung");
                    }
                }
            }

            if (diemTheoTenKyNang.Count == 0)
            {
                return new List<KyNangGoiY_ResponseDTO>();
            }

            // 4. Tra cứu bảng KyNang trong SQL Server để lấy maKyNang thực tế
            var danhSachTenCanTim = diemTheoTenKyNang.Keys.ToList();
            var kyNangsSQL = await _context.KyNangs
                .Where(k => danhSachTenCanTim.Contains(k.tenKyNang))
                .ToListAsync();

            // Nếu database chưa có một số kỹ năng được gợi ý, tự động bổ sung an toàn vào bảng KyNang hiện có
            var tenKyNangDaCo = new HashSet<string>(kyNangsSQL.Select(k => k.tenKyNang), StringComparer.OrdinalIgnoreCase);
            var tenCanThem = danhSachTenCanTim.Where(t => !tenKyNangDaCo.Contains(t)).ToList();
            if (tenCanThem.Count > 0)
            {
                foreach (var t in tenCanThem)
                {
                    var entityMoi = new KyNang { tenKyNang = t };
                    await _context.KyNangs.AddAsync(entityMoi);
                    kyNangsSQL.Add(entityMoi);
                }
                await _context.SaveChangesAsync();
            }

            // 5. Định dạng và sắp xếp kết quả theo điểm phù hợp
            var ketQua = kyNangsSQL
                .Select(k =>
                {
                    (double Diem, string LyDo) info = diemTheoTenKyNang.ContainsKey(k.tenKyNang)
                        ? diemTheoTenKyNang[k.tenKyNang]
                        : (Diem: 0.5, LyDo: "Gợi ý liên quan");
                    return new KyNangGoiY_ResponseDTO
                    {
                        maKyNang = k.maKyNang,
                        tenKyNang = k.tenKyNang,
                        diemPhuHop = info.Diem,
                        lyDo = info.LyDo
                    };
                })
                .OrderByDescending(x => x.diemPhuHop)
                .ThenBy(x => x.tenKyNang)
                .Take(12)
                .ToList();

            return ketQua;
        }

        /// <summary>
        /// Suy luận tập hợp các kỹ năng liên quan từ văn bản phục vụ thuật toán gợi ý việc làm
        /// </summary>
        public async Task<HashSet<string>> SuyLuanKyNangTuVanBanAsync(string? tieuDe, string? moTa = null)
        {
            var tapHopKyNang = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(tieuDe) && string.IsNullOrWhiteSpace(moTa))
            {
                return tapHopKyNang;
            }

            var tieuDeKd = ChuanHoaKhongDau(tieuDe ?? string.Empty);
            var moTaKd = ChuanHoaKhongDau(moTa ?? string.Empty);
            var vanBanTong = $"{tieuDeKd} {moTaKd}";

            foreach (var quyTac in DanhSachQuyTac)
            {
                foreach (var tuKhoa in quyTac.TuKhoas)
                {
                    var tuKhoaKd = ChuanHoaKhongDau(tuKhoa);
                    if (KiemTraChuaTuKhoa(vanBanTong, tuKhoaKd))
                    {
                        foreach (var kn in quyTac.KyNangs)
                        {
                            tapHopKyNang.Add(kn);
                        }
                        break;
                    }
                }
            }

            return tapHopKyNang;
        }

        /// <summary>
        /// Đảm bảo danh mục kỹ năng mặc định luôn có sẵn trong SQL Server mà không bị trùng lặp
        /// </summary>
        public async Task<bool> DamBaoKyNangMacDinhAsync()
        {
            try
            {
                var kyNangsHienCo = await _context.KyNangs
                    .Select(k => k.tenKyNang.Trim().ToLower())
                    .ToListAsync();
                var setHienCo = new HashSet<string>(kyNangsHienCo, StringComparer.OrdinalIgnoreCase);

                var danhSachThem = new List<KyNang>();
                foreach (var ten in DanhMucKyNangChuan)
                {
                    var normalized = ten.Trim().ToLower();
                    if (!setHienCo.Contains(normalized))
                    {
                        danhSachThem.Add(new KyNang { tenKyNang = ten.Trim() });
                        setHienCo.Add(normalized);
                    }
                }

                if (danhSachThem.Count > 0)
                {
                    await _context.KyNangs.AddRangeAsync(danhSachThem);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Helper Methods
        private static string ChuanHoaKhongDau(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private static bool KiemTraChuaTuKhoa(string text, string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(tuKhoa)) return false;

            if (tuKhoa.Contains(' ') || tuKhoa.Contains('/') || tuKhoa.Contains('-'))
            {
                return text.Contains(tuKhoa);
            }

            var pattern = $@"(^|[\s,;.|/\\_\-()]){Regex.Escape(tuKhoa)}([\s,;.|/\\_\-()]|$)";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }
        #endregion
    }
}

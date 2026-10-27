using System.Drawing;
using System.Text.RegularExpressions;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using ZXing;
using ZXing.Windows.Compatibility;

namespace FreelancerStudent.API.Services
{
    public class MinhChungService : IMinhChungService
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MinhChungService> _logger;

        public MinhChungService(
            ApplicationDBContext context,
            IWebHostEnvironment env,
            ILogger<MinhChungService> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<QuetTheSinhVien_ResponseDTO> QuetVaLuuTheSinhVienAsync(IFormFile file, int maUser)
        {
            var result = new QuetTheSinhVien_ResponseDTO();

            // 1. Kiểm tra file đầu vào
            if (file == null || file.Length == 0)
            {
                result.ThanhCong = false;
                result.ThongBao = "Vui lòng chọn ảnh thẻ sinh viên.";
                return result;
            }

            // Giới hạn kích thước 10MB
            if (file.Length > 10 * 1024 * 1024)
            {
                result.ThanhCong = false;
                result.ThongBao = "Dung lượng ảnh không được vượt quá 10MB.";
                return result;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
            {
                result.ThanhCong = false;
                result.ThongBao = "Định dạng file không hỗ trợ. Vui lòng tải ảnh JPG, JPEG, PNG hoặc WEBP.";
                return result;
            }

            // 2. Kiểm tra tài khoản sinh viên
            var student = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .FirstOrDefaultAsync(f => f.maUser == maUser);

            if (student == null || student.User == null)
            {
                result.ThanhCong = false;
                result.ThongBao = "Không tìm thấy hồ sơ sinh viên tương ứng với tài khoản.";
                return result;
            }

            // Điền thông tin hồ sơ hiện tại để đối chiếu
            result.HoSoHienTai = new ThongTinHienTai_DTO
            {
                MaUser = student.maUser,
                MaFreelancerStudents = student.maFreelancerStudents,
                HoTenHienTai = student.User.hotenUser ?? string.Empty,
                NgaySinhHienTai = student.User.ngaysinh,
                DiaDiemHienTai = student.diaDiemFreelancerStudent ?? string.Empty,
                TenTruong = student.tenTruong ?? string.Empty,
                ChuyenNganh = student.ChuyenNganh?.tenChuyenNganh ?? string.Empty
            };

            // 3. Lưu file an toàn vào thư mục wwwroot/uploads/minhchung
            string relativeFolder = Path.Combine("uploads", "minhchung");
            string apiWwwroot = Path.Combine(_env.ContentRootPath, "wwwroot", relativeFolder);
            string webWwwroot = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", relativeFolder);

            if (!Directory.Exists(apiWwwroot))
            {
                Directory.CreateDirectory(apiWwwroot);
            }

            string uniqueFileName = $"the_sv_{maUser}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            string absoluteFilePath = Path.Combine(apiWwwroot, uniqueFileName);

            using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Đồng bộ sang thư mục Web wwwroot nếu tồn tại
            try
            {
                string webBase = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot");
                if (Directory.Exists(webBase))
                {
                    if (!Directory.Exists(webWwwroot))
                    {
                        Directory.CreateDirectory(webWwwroot);
                    }
                    string webFilePath = Path.Combine(webWwwroot, uniqueFileName);
                    File.Copy(absoluteFilePath, webFilePath, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Không thể sao chép ảnh sang Web wwwroot: {Message}", ex.Message);
            }

            string fileUrl = $"uploads/minhchung/{uniqueFileName}";
            result.FileMinhChungUrl = fileUrl;

            // 4. Thực hiện OCR & Barcode Decoding
            string rawOcrText = string.Empty;
            string barcodeText = string.Empty;

            try
            {
                // Bước A: Quét mã vạch / QR Code trên thẻ sinh viên (nếu có)
                try
                {
                    using var bmp = new Bitmap(absoluteFilePath);
                    var reader = new BarcodeReader
                    {
                        AutoRotate = true,
                        Options = new ZXing.Common.DecodingOptions
                        {
                            TryHarder = true,
                            PossibleFormats = new List<BarcodeFormat>
                            {
                                BarcodeFormat.CODE_128,
                                BarcodeFormat.CODE_39,
                                BarcodeFormat.QR_CODE,
                                BarcodeFormat.EAN_13
                            }
                        }
                    };
                    var decodeResult = reader.Decode(bmp);
                    if (decodeResult != null && !string.IsNullOrWhiteSpace(decodeResult.Text))
                    {
                        barcodeText = decodeResult.Text.Trim();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Không đọc được Barcode/QR từ thẻ SV: {Message}", ex.Message);
                }

                // Bước B: Quét Text qua Windows Media OCR Engine
                try
                {
                    var storageFile = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(absoluteFilePath));
                    using var fileStream = await storageFile.OpenAsync(FileAccessMode.Read);
                    var decoder = await BitmapDecoder.CreateAsync(fileStream);
                    var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    var engine = OcrEngine.TryCreateFromUserProfileLanguages()
                                 ?? OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));

                    if (engine != null)
                    {
                        var ocrResult = await engine.RecognizeAsync(softwareBitmap);
                        rawOcrText = ocrResult?.Text ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Lỗi khi chạy Windows OCR: {Message}", ex.Message);
                }

                result.RawOcrText = rawOcrText;

                // 5. Bóc tách các trường từ OCR Text và Barcode
                BocTachThongTinTheSinhVien(rawOcrText, barcodeText, result);

                // 6. Đối chiếu chuyên ngành với danh mục database
                await MapChuyenNganhVoiDatabaseAsync(result, rawOcrText);

                // 7. Đánh giá độ tin cậy
                double score = 0;
                if (!string.IsNullOrWhiteSpace(result.Mssv)) score += 30;
                if (!string.IsNullOrWhiteSpace(result.HoVaTen)) score += 30;
                if (!string.IsNullOrWhiteSpace(result.TenTruong)) score += 20;
                if (!string.IsNullOrWhiteSpace(result.KhoaNganh) || !string.IsNullOrWhiteSpace(result.MaChuyenNganhGoiY)) score += 10;
                if (!string.IsNullOrWhiteSpace(result.NienKhoa)) score += 10;

                result.DoTinCay = score;

                if (score >= 30)
                {
                    result.ThanhCong = true;
                    result.ThongBao = "Đã quét và trích xuất thông tin thẻ sinh viên thành công! Vui lòng kiểm tra lại trước khi xác nhận.";
                    result.PhuongThucNhanDien = !string.IsNullOrWhiteSpace(barcodeText) ? "HYBRID (BARCODE + OCR)" : "WINDOWS_OCR";
                }
                else
                {
                    result.ThanhCong = false;
                    result.ThongBao = "Không nhận diện được rõ thông tin trên thẻ sinh viên. Vui lòng chụp lại ảnh rõ nét, không bị lóa hoặc mất góc.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình xử lý OCR thẻ sinh viên");
                result.ThanhCong = false;
                result.ThongBao = "Lỗi khi xử lý hình ảnh thẻ sinh viên: " + ex.Message;
            }

            return result;
        }

        private static void BocTachThongTinTheSinhVien(string ocrText, string barcodeText, QuetTheSinhVien_ResponseDTO dto)
        {
            if (string.IsNullOrWhiteSpace(ocrText) && string.IsNullOrWhiteSpace(barcodeText))
            {
                return;
            }

            var lines = (ocrText ?? string.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();

            // MSSV: Ưu tiên mã vạch nếu mã vạch là chuỗi ký tự/số 6-15 ký tự
            if (!string.IsNullOrWhiteSpace(barcodeText) && Regex.IsMatch(barcodeText, @"^[A-Z0-9]{6,15}$", RegexOptions.IgnoreCase))
            {
                dto.Mssv = barcodeText.Trim().ToUpperInvariant();
            }

            if (string.IsNullOrWhiteSpace(dto.Mssv))
            {
                var matchMssv = Regex.Match(ocrText, @"(?:MSSV|MSV|Mã\s*sinh\s*viên|Mã\s*số\s*SV|Ma\s*sinh\s*vien|Student\s*ID|ID)[\s\:\.\-]+([A-Z0-9]{6,15})", RegexOptions.IgnoreCase);
                if (matchMssv.Success)
                {
                    dto.Mssv = matchMssv.Groups[1].Value.Trim().ToUpperInvariant();
                }
                else
                {
                    var matchDigits = Regex.Match(ocrText, @"\b(\d{7,11}|[A-Z]{1,3}\d{6,8})\b");
                    if (matchDigits.Success)
                    {
                        dto.Mssv = matchDigits.Groups[1].Value.Trim().ToUpperInvariant();
                    }
                }
            }

            // 1. TÊN TRƯỜNG ĐH / CĐ
            dto.TenTruong = TrichXuatTenTruongTuOcr(lines, ocrText);
            if (!string.IsNullOrWhiteSpace(dto.TenTruong))
            {
                dto.MaTruong = SuyDoanMaTruong(dto.TenTruong);
            }

            // 2. HỌ VÀ TÊN SINH VIÊN
            dto.HoVaTen = TrichXuatHoVaTenTuOcr(lines, ocrText);

            // 3. KHOA / NGÀNH HỌC TRÊN THẺ
            dto.KhoaNganh = TrichXuatKhoaNganhTuOcr(lines, ocrText);

            // 4. NIÊN KHÓA & NĂM THỨ
            var matchNienKhoa = Regex.Match(ocrText, @"\b(20\d{2}\s*[\-\–]\s*20\d{2})\b");
            if (matchNienKhoa.Success)
            {
                dto.NienKhoa = Regex.Replace(matchNienKhoa.Groups[1].Value, @"\s+", "");

                // Ước lượng năm học (Năm 1 -> 6)
                var namBatDauStr = dto.NienKhoa.Substring(0, 4);
                if (int.TryParse(namBatDauStr, out int namBatDau))
                {
                    int namHienTai = DateTime.Now.Year;
                    int tinhNamThu = namHienTai - namBatDau + 1;
                    dto.NamThu = Math.Clamp(tinhNamThu, 1, 6);
                }
            }

            // 5. NGÀY SINH (hỗ trợ cả trường hợp dấu / bị OCR nhận nhầm thành 1 hoặc |)
            var matchNgaySinh = Regex.Match(ocrText, @"\b(\d{1,2})[/\-\.](\d{1,2})[/\-\.1\|](20\d{2}|19\d{2})\b");
            if (matchNgaySinh.Success)
            {
                string dStr = matchNgaySinh.Groups[1].Value.PadLeft(2, '0');
                string mStr = matchNgaySinh.Groups[2].Value.PadLeft(2, '0');
                string yStr = matchNgaySinh.Groups[3].Value;
                string nsStr = $"{dStr}/{mStr}/{yStr}";

                if (DateTime.TryParseExact(nsStr, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime ns))
                {
                    dto.NgaySinh = ns;
                }
            }
            else
            {
                var matchNsAlt = Regex.Match(ocrText, @"\b(\d{1,2}[/-]\d{1,2}[/-]\d{4})\b");
                if (matchNsAlt.Success)
                {
                    string nsStr = matchNsAlt.Groups[1].Value.Replace("-", "/");
                    if (DateTime.TryParseExact(nsStr, new[] { "dd/MM/yyyy", "d/M/yyyy" },
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime ns))
                    {
                        dto.NgaySinh = ns;
                    }
                }
            }
        }

        private static string? TrichXuatTenTruongTuOcr(List<string> lines, string fullText)
        {
            // Danh mục các trường phổ biến kèm tên chuẩn & mã trường
            var schoolDict = new (string[] Keys, string OfficialName, string Code)[]
            {
                (new[] { "CONG THUONG", "HUIT", "INDUSTRY AND TRADE" }, "Trường Đại học Công Thương TP. Hồ Chí Minh", "HUIT"),
                (new[] { "BACH KHOA", "HCMUT", "HUST" }, "Trường Đại học Bách Khoa", "HCMUT"),
                (new[] { "CONG NGHE THONG TIN", "UIT", "INFORMATION TECHNOLOGY" }, "Trường Đại học Công nghệ Thông tin - ĐHQG TP.HCM", "UIT01"),
                (new[] { "KHOA HOC TU NHIEN", "HCMUS" }, "Trường Đại học Khoa học Tự nhiên", "HCMUS"),
                (new[] { "SU PHAM KY THUAT", "HCMUTE", "TECHNICAL EDUCATION" }, "Trường Đại học Sư phạm Kỹ thuật TP.HCM", "HCMUTE"),
                (new[] { "KINH TE TP", "UEH", "UNIVERSITY OF ECONOMICS" }, "Trường Đại học Kinh tế TP.HCM", "UEH"),
                (new[] { "KINH TE - LUAT", "KINH TE LUAT", "UEL" }, "Trường Đại học Kinh tế - Luật", "UEL"),
                (new[] { "TON DUC THANG", "TDTU" }, "Trường Đại học Tôn Đức Thắng", "TDTU"),
                (new[] { "VAN LANG", "VLU" }, "Trường Đại học Văn Lang", "VLU"),
                (new[] { "HOA SEN", "HSU" }, "Trường Đại học Hoa Sen", "HSU"),
                (new[] { "HUTECH", "CONG NGHE TP.HCM", "CONG NGHE TPHCM" }, "Trường Đại học Công nghệ TP.HCM (HUTECH)", "HUTECH"),
                (new[] { "NGOAI THUONG", "FTU" }, "Trường Đại học Ngoại thương", "FTU"),
                (new[] { "BUU CHINH VIEN THONG", "PTIT" }, "Học viện Công nghệ Bưu chính Viễn thông", "PTIT"),
                (new[] { "QUOC TE", "INTERNATIONAL UNIVERSITY" }, "Trường Đại học Quốc Tế - ĐHQG TP.HCM", "IU"),
                (new[] { "CAN THO", "CTU" }, "Trường Đại học Cần Thơ", "CTU"),
                (new[] { "DA NANG", "DUT" }, "Đại học Đà Nẵng", "DUT"),
                (new[] { "Y DUOC", "UMP" }, "Đại học Y Dược TP.HCM", "UMP")
            };

            string fullNormalized = ChuanHoaKhongDau(fullText);
            foreach (var (keys, official, code) in schoolDict)
            {
                if (keys.Any(k => fullNormalized.Contains(k)))
                {
                    return official;
                }
            }

            // Quét các dòng theo từ khóa Trường / Đại học / Cao đẳng
            foreach (var line in lines)
            {
                string cleanLine = line.Trim();
                if (Regex.IsMatch(cleanLine, @"(?:TRƯỜNG|TRUONG)?\s*(?:ĐẠI\s*HỌC|DAI\s*HOC|HQC|HOC\s*VIEN|HỌC\s*VIỆN|CAO\s*ĐẲNG|CAO\s*DANG|VIỆN|VIEN|ĐH|DH|CĐ|CD)\b[^\r\n]{2,80}", RegexOptions.IgnoreCase))
                {
                    string res = cleanLine;
                    res = Regex.Replace(res, @"^(?:TRUONG|TRƯỜNG)?\s*HQC\s+", "Trường Đại học ", RegexOptions.IgnoreCase);
                    res = Regex.Replace(res, @"^(?:TRUONG|TRƯỜNG)\s+(?:DAI\s*HOC|ĐẠI\s*HỌC)\s+", "Trường Đại học ", RegexOptions.IgnoreCase);
                    return ChuanHoaTenTruong(res);
                }
            }

            return null;
        }

        private static string? TrichXuatHoVaTenTuOcr(List<string> lines, string fullText)
        {
            // 1. Nhãn rõ ràng có dấu hai chấm / gạch ngang
            var matchTen = Regex.Match(fullText, @"(?:Họ\s*(?:và|va|v\.)?\s*tên|Họ\s*tên|Full\s*name|Student\s*name|Sinh\s*viên|SV)\s*[:：\.\-\/]\s*([^\r\n0-9]{3,40})", RegexOptions.IgnoreCase);
            if (matchTen.Success && !string.IsNullOrWhiteSpace(matchTen.Groups[1].Value))
            {
                var ten = ChuanHoaHoTenTuOcr(matchTen.Groups[1].Value);
                if (!string.IsNullOrWhiteSpace(ten)) return ten;
            }

            // 2. Dòng theo sau "THẺ SINH VIÊN" / "STUDENT CARD"
            for (int i = 0; i < lines.Count; i++)
            {
                string norm = ChuanHoaKhongDau(lines[i]);
                if (norm.Contains("THE SINH VIEN") || norm.Contains("STUDENT CARD") || norm.Equals("SINH VIEN") || norm.Contains("THE SV"))
                {
                    for (int j = i + 1; j < Math.Min(i + 3, lines.Count); j++)
                    {
                        string candidate = lines[j].Trim();
                        string candNorm = ChuanHoaKhongDau(candidate);

                        if (candNorm.Contains("NGAY SINH") || candNorm.Contains("NGANH") || candNorm.Contains("KHOA") ||
                            candNorm.Contains("MSSV") || candNorm.Contains("LOP") || candNorm.Contains("TRUONG") ||
                            Regex.IsMatch(candidate, @"\d{4}"))
                        {
                            continue;
                        }

                        var words = candidate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length >= 2 && words.Length <= 5 && candidate.Length >= 4 && candidate.Length <= 40)
                        {
                            return ChuanHoaHoTenTuOcr(candidate);
                        }
                    }
                }
            }

            // 3. Quy luật cấu trúc thẻ SV Việt Nam: Dòng nằm ngay trước "Ngày sinh:" luôn là Họ và tên sinh viên
            for (int i = 0; i < lines.Count; i++)
            {
                string norm = ChuanHoaKhongDau(lines[i]);
                if (norm.Contains("NGAY SINH") || norm.Contains("DATE OF BIRTH") || norm.Contains("DOB") ||
                    Regex.IsMatch(lines[i], @"\b\d{1,2}[/\-\.]\d{1,2}[/\-\.1]\d{4}\b"))
                {
                    // Quét ngược 1-2 dòng trước
                    for (int j = i - 1; j >= Math.Max(0, i - 2); j--)
                    {
                        string candidate = lines[j].Trim();
                        string candNorm = ChuanHoaKhongDau(candidate);

                        if (candNorm.Contains("TRUONG") || candNorm.Contains("DAI HOC") || candNorm.Contains("BO CONG") ||
                            candNorm.Contains("THE SINH VIEN") || candNorm.Contains("MSSV") || candNorm.Contains("UNIVERSITY") ||
                            Regex.IsMatch(candidate, @"\d{4}"))
                        {
                            continue;
                        }

                        var words = candidate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length >= 2 && words.Length <= 5 && candidate.Length >= 4 && candidate.Length <= 40)
                        {
                            return ChuanHoaHoTenTuOcr(candidate);
                        }
                    }
                }
            }

            // 4. Fallback: Dòng dạng Họ Tên (2 - 5 từ, không chứa số hay từ khóa trường/thẻ)
            foreach (var line in lines)
            {
                string candidate = line.Trim();
                string candNorm = ChuanHoaKhongDau(candidate);
                if (candNorm.Contains("TRUONG") || candNorm.Contains("DAI HOC") || candNorm.Contains("BO CONG") ||
                    candNorm.Contains("THE SINH VIEN") || candNorm.Contains("NGAY SINH") || candNorm.Contains("NGANH") ||
                    candNorm.Contains("KHOA") || candNorm.Contains("MSSV") || candNorm.Contains("HO CHI MINH") ||
                    candNorm.Contains("UNIVERSITY") || candNorm.Contains("INDUSTRY") || candNorm.Contains("PHU") ||
                    Regex.IsMatch(candidate, @"\d"))
                {
                    continue;
                }

                var words = candidate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2 && words.Length <= 5 && candidate.Length >= 5 && candidate.Length <= 35)
                {
                    return ChuanHoaHoTenTuOcr(candidate);
                }
            }

            return null;
        }

        private static string? TrichXuatKhoaNganhTuOcr(List<string> lines, string fullText)
        {
            // 1. Ưu tiên cao nhất: "Chuyên ngành" (Specialization) - chuyên ngành có tính định hướng nghề nghiệp cụ thể nhất
            var matchChuyenNganh = Regex.Match(fullText, @"(?<!\p{L})(?:Chuyên\s*ngành\s*(?:đào\s*tạo)?|Chuyen\s*nganh|Specialization|Sub-major)\s*[:：\.\-\/]?\s*([^\r\n]{3,60})", RegexOptions.IgnoreCase);
            if (matchChuyenNganh.Success && !string.IsNullOrWhiteSpace(matchChuyenNganh.Groups[1].Value))
            {
                string val = matchChuyenNganh.Groups[1].Value.Trim();
                if (val.Length >= 3 && !Regex.IsMatch(val, @"^\d+$"))
                {
                    return ChuanHoaTieuDeNganh(val);
                }
            }

            // 2. Ưu tiên kế tiếp: "Ngành" (Major / Ngành đào tạo / Ngành học)
            var matchNganh = Regex.Match(fullText, @"(?<!\p{L})(?:Ngành\s*đào\s*tạo|Nganh\s*dao\s*tao|Ngành\s*học|Nganh\s*hoc|Ngành|Nganh|Major)\s*[:：\.\-\/]?\s*([^\r\n]{3,60})", RegexOptions.IgnoreCase);
            if (matchNganh.Success && !string.IsNullOrWhiteSpace(matchNganh.Groups[1].Value))
            {
                string val = matchNganh.Groups[1].Value.Trim();
                if (val.Length >= 3 && !Regex.IsMatch(val, @"^\d+$"))
                {
                    return ChuanHoaTieuDeNganh(val);
                }
            }

            // 3. Ưu tiên kế tiếp: "Khoa" (Faculty / Department)
            var matchKhoa = Regex.Match(fullText, @"(?<!\p{L})(?:Khoa|Faculty|Department)\s*[:：\.\-\/]?\s*([^\r\n]{3,60})", RegexOptions.IgnoreCase);
            if (matchKhoa.Success && !string.IsNullOrWhiteSpace(matchKhoa.Groups[1].Value))
            {
                string val = matchKhoa.Groups[1].Value.Trim();
                // Tránh nhầm "Khoa" trong "Khóa học" hay niên khóa
                if (val.Length >= 3 && !Regex.IsMatch(val, @"^\d+$") && !Regex.IsMatch(val, @"^(?:20\d{2}|học|\d)", RegexOptions.IgnoreCase))
                {
                    return ChuanHoaTieuDeNganh(val);
                }
            }

            // 4. Nhãn nằm độc lập trên 1 dòng, giá trị nằm dòng trước hoặc sau
            // Ưu tiên quét dòng có "CHUYEN NGANH" trước
            for (int i = 0; i < lines.Count; i++)
            {
                string lineNorm = ChuanHoaKhongDau(lines[i]);
                if (lineNorm == "CHUYEN NGANH" || lineNorm == "CHUYEN NGANH:" || lineNorm == "CHUYEN NGANH DAO TAO" || lineNorm == "CHUYEN NGANH DAO TAO:")
                {
                    if (i > 0 && LaTenNganhHopLe(ChuanHoaKhongDau(lines[i - 1])))
                        return ChuanHoaTieuDeNganh(lines[i - 1]);
                    if (i + 1 < lines.Count && LaTenNganhHopLe(ChuanHoaKhongDau(lines[i + 1])))
                        return ChuanHoaTieuDeNganh(lines[i + 1]);
                }
            }

            // Kế tiếp quét dòng có "NGANH" hoặc "KHOA"
            for (int i = 0; i < lines.Count; i++)
            {
                string lineNorm = ChuanHoaKhongDau(lines[i]);
                if (lineNorm == "NGANH" || lineNorm == "NGANH:" || lineNorm == "NGANH DAO TAO" || lineNorm == "NGANH DAO TAO:" || lineNorm == "KHOA" || lineNorm == "KHOA:")
                {
                    if (i > 0 && LaTenNganhHopLe(ChuanHoaKhongDau(lines[i - 1])))
                        return ChuanHoaTieuDeNganh(lines[i - 1]);
                    if (i + 1 < lines.Count && LaTenNganhHopLe(ChuanHoaKhongDau(lines[i + 1])))
                        return ChuanHoaTieuDeNganh(lines[i + 1]);
                }
            }

            // 3. Quét trực tiếp các ngành phổ biến trong các dòng OCR
            var dsNganhPhoBien = new (string Key, string TenChuan)[]
            {
                ("CONG NGHE THONG TIN", "Công nghệ thông tin"),
                ("KHOA HOC MAY TINH", "Khoa học máy tính"),
                ("KY THUAT PHAN MEM", "Kỹ thuật phần mềm"),
                ("HE THONG THONG TIN", "Hệ thống thông tin"),
                ("AN TOAN THONG TIN", "An toàn thông tin"),
                ("TRI TUE NHAN TAO", "Trí tuệ nhân tạo"),
                ("KHOA HOC DU LIEU", "Khoa học dữ liệu"),
                ("QUAN TRI KINH DOANH", "Quản trị kinh doanh"),
                ("MARKETING", "Marketing"),
                ("KE TOAN", "Kế toán"),
                ("TAI CHINH NGAN HANG", "Tài chính ngân hàng"),
                ("LOGISTICS", "Logistics và Quản lý chuỗi cung ứng"),
                ("THIET KE DO HOA", "Thiết kế đồ họa"),
                ("NGON NGU ANH", "Ngôn ngữ Anh"),
                ("CONG NGHE THUC PHAM", "Công nghệ thực phẩm"),
                ("CONG NGHE SINH HOC", "Công nghệ sinh học"),
                ("DIEN DIEN TU", "Điện - Điện tử"),
                ("CO KHI", "Kỹ thuật Cơ khí"),
                ("XAY DUNG", "Kỹ thuật Xây dựng"),
                ("LUAT", "Luật Kinh tế")
            };

            foreach (var line in lines)
            {
                string lineNorm = ChuanHoaKhongDau(line);
                foreach (var (key, tenChuan) in dsNganhPhoBien)
                {
                    if (lineNorm.Contains(key))
                    {
                        return tenChuan;
                    }
                }
            }

            return null;
        }

        private static bool LaTenNganhHopLe(string norm)
        {
            if (norm.Length < 4 || norm.Length > 50) return false;
            if (norm.Contains("NGAY SINH") || norm.Contains("THE SINH VIEN") || norm.Contains("TRUONG") || norm.Contains("MSSV")) return false;
            if (Regex.IsMatch(norm, @"^\d+$")) return false;
            return true;
        }

        private async Task MapChuyenNganhVoiDatabaseAsync(QuetTheSinhVien_ResponseDTO dto, string rawOcrText)
        {
            var dsChuyenNganh = await _context.ChuyenNganhs.AsNoTracking().ToListAsync();

            // Nếu chưa có KhoaNganh từ nhãn, thử quét toàn bộ rawOcrText qua danh sách ChuyenNganh trong DB
            if (string.IsNullOrWhiteSpace(dto.KhoaNganh) && !string.IsNullOrWhiteSpace(rawOcrText))
            {
                string ocrNorm = ChuanHoaKhongDau(rawOcrText);
                var matchDb = dsChuyenNganh
                    .OrderByDescending(c => c.tenChuyenNganh.Length)
                    .FirstOrDefault(c => ocrNorm.Contains(ChuanHoaKhongDau(c.tenChuyenNganh)));

                if (matchDb != null)
                {
                    dto.KhoaNganh = matchDb.tenChuyenNganh;
                    dto.MaChuyenNganhGoiY = matchDb.maChuyenNganh;
                    dto.TenChuyenNganhGoiY = matchDb.tenChuyenNganh;
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(dto.KhoaNganh)) return;

            string tenSach = ChuanHoaTenChuyenNganh(dto.KhoaNganh);
            if (string.IsNullOrWhiteSpace(tenSach)) return;

            string target = KhoaSoSanh(tenSach);

            // 1. Khớp chính xác theo tên (không phân biệt dấu/hoa thường) hoặc theo mã (VD: thẻ ghi "CNTT")
            var match = dsChuyenNganh.FirstOrDefault(c => KhoaSoSanh(c.tenChuyenNganh) == target)
                     ?? dsChuyenNganh.FirstOrDefault(c => string.Equals(c.maChuyenNganh, tenSach.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));

            // 2. Khớp gần đúng: tên trên thẻ chứa tên ngành trong DB hoặc ngược lại
            match ??= dsChuyenNganh
                .Where(c => target.Contains(KhoaSoSanh(c.tenChuyenNganh)) ||
                            (target.Length >= 6 && KhoaSoSanh(c.tenChuyenNganh).Contains(target)))
                .OrderByDescending(c => c.tenChuyenNganh.Length)
                .FirstOrDefault();

            // 3. Từ khóa tiếng Anh phổ biến
            if (match == null)
            {
                string? maTheoTuKhoa = null;
                if (Regex.IsMatch(target, @"\bsoftware\b")) maTheoTuKhoa = "KTPM";
                else if (Regex.IsMatch(target, @"\b(information technology|it|computer science)\b")) maTheoTuKhoa = "CNTT";
                else if (Regex.IsMatch(target, @"\bgraphic design\b")) maTheoTuKhoa = "TKDH";
                else if (Regex.IsMatch(target, @"\b(information security|cyber security|cybersecurity)\b")) maTheoTuKhoa = "ATTT";

                if (maTheoTuKhoa != null)
                    match = dsChuyenNganh.FirstOrDefault(c => c.maChuyenNganh == maTheoTuKhoa);
            }

            // 4. Không khớp ngành nào -> thêm chuyên ngành mới vào bảng ChuyenNganh
            if (match == null)
            {
                match = await TimHoacTaoChuyenNganhAsync(tenSach);
                if (match != null && !dsChuyenNganh.Any(c => c.maChuyenNganh == match.maChuyenNganh))
                {
                    dto.ChuyenNganhMoi = true;
                }
            }

            if (match != null)
            {
                dto.MaChuyenNganhGoiY = match.maChuyenNganh;
                dto.TenChuyenNganhGoiY = match.tenChuyenNganh;
            }
        }

        /// <summary>
        /// Tìm chuyên ngành theo tên (không phân biệt dấu/hoa thường). Nếu chưa có thì thêm mới vào bảng ChuyenNganh
        /// với mã viết tắt tự sinh (ASCII, duy nhất, tối đa 20 ký tự). Trả về null nếu tên không hợp lệ.
        /// </summary>
        private async Task<ChuyenNganh?> TimHoacTaoChuyenNganhAsync(string? tenRaw)
        {
            string ten = ChuanHoaTenChuyenNganh(tenRaw);
            if (string.IsNullOrWhiteSpace(ten)) return null;

            string key = KhoaSoSanh(ten);
            var ds = await _context.ChuyenNganhs.AsNoTracking().ToListAsync();

            var daCo = ds.FirstOrDefault(c => KhoaSoSanh(c.tenChuyenNganh) == key);
            if (daCo != null) return daCo;

            string maCoSo = TaoMaChuyenNganhCoSo(ten);
            var dsMa = ds.Select(c => c.maChuyenNganh.ToUpperInvariant()).ToHashSet();
            string ma = maCoSo;
            int hauTo = 2;
            while (dsMa.Contains(ma))
            {
                ma = maCoSo + hauTo;
                hauTo++;
            }

            var moi = new ChuyenNganh { maChuyenNganh = ma, tenChuyenNganh = ten };
            _context.ChuyenNganhs.Add(moi);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã thêm chuyên ngành mới từ thẻ sinh viên: {Ma} - {Ten}", ma, ten);
                return moi;
            }
            catch (DbUpdateException ex)
            {
                // Có thể một request khác vừa thêm cùng ngành/mã -> bỏ bản ghi lỗi và đọc lại từ DB
                _context.Entry(moi).State = EntityState.Detached;
                _logger.LogWarning("Không thể thêm chuyên ngành {Ten}: {Message}", ten, ex.InnerException?.Message ?? ex.Message);
                var dsMoi = await _context.ChuyenNganhs.AsNoTracking().ToListAsync();
                return dsMoi.FirstOrDefault(c => KhoaSoSanh(c.tenChuyenNganh) == key);
            }
        }

        /// <summary>
        /// Làm sạch tên ngành đọc từ OCR: cắt phần thừa (niên khóa, lớp, số...), bỏ ký tự rác, chuẩn hóa hoa/thường.
        /// Trả về chuỗi rỗng nếu không đủ tin cậy để coi là một tên ngành.
        /// </summary>
        private static string ChuanHoaTenChuyenNganh(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            string s = Regex.Split(raw,
                @"(?:Niên\s*khóa|Nien\s*khoa|Khóa\s*học|Khóa|Lớp|Lop|Class|MSSV|MSV|Course|Ngày\s*sinh|Ngay\s*sinh|Họ\s*(?:và\s*)?tên|Ho\s*(?:va\s*)?ten|Mã\s*số|Ma\s*so|Hệ\s*đào\s*tạo|He\s*dao\s*tao|Hiệu\s*lực|Hieu\s*luc|Valid|\d)",
                RegexOptions.IgnoreCase)[0];
            s = Regex.Replace(s, @"[^\p{L}\s\-&]", " ");
            s = Regex.Replace(s, @"\s+", " ").Trim(' ', '-', '&');

            if (s.Count(char.IsLetter) < 4 || s.Split(' ').Length > 10) return string.Empty;

            // Loại bỏ chuỗi rác của tiêu đề thẻ (OCR có thể gộp nhiều dòng làm một)
            string khongDau = KhoaSoSanh(s);
            if (Regex.IsMatch(khongDau, @"\b(the sinh vien|student|card|truong|dai hoc|cao dang|hoc vien|university|college|mssv)\b"))
                return string.Empty;

            // OCR thường trả về chữ IN HOA -> đưa về dạng "Viết hoa chữ đầu"
            if (s.Where(char.IsLetter).All(char.IsUpper))
            {
                s = s.ToLower();
            }
            s = char.ToUpper(s[0]) + s.Substring(1);

            return s.Length > 150 ? s.Substring(0, 150).Trim() : s;
        }

        private static string BoDauTiengViet(string s)
        {
            var normalized = s.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder(normalized.Length);
            foreach (var ch in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        private static string KhoaSoSanh(string s)
        {
            return Regex.Replace(BoDauTiengViet(s).ToLowerInvariant(), @"\s+", " ").Trim();
        }

        /// <summary>
        /// Sinh mã viết tắt từ chữ cái đầu mỗi từ (VD: "Công nghệ sinh học" -> "CNSH").
        /// Tối đa 16 ký tự để còn chỗ cho hậu tố chống trùng (cột maChuyenNganh là VARCHAR(20)).
        /// </summary>
        private static string TaoMaChuyenNganhCoSo(string ten)
        {
            var tuBoQua = new HashSet<string> { "va", "and", "of", "the" };
            var words = BoDauTiengViet(ten)
                .Split(new[] { ' ', '-', '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => new string(w.Where(c => c < 128 && char.IsLetter(c)).ToArray()))
                .Where(w => w.Length > 0 && !tuBoQua.Contains(w.ToLowerInvariant()))
                .ToList();

            string ma = string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
            if (ma.Length < 2 && words.Count > 0)
            {
                ma = words[0].ToUpperInvariant();
            }
            if (string.IsNullOrEmpty(ma)) ma = "CN";
            return ma.Length > 16 ? ma.Substring(0, 16) : ma;
        }

        private static string SuyDoanMaTruong(string tenTruong)
        {
            string t = ChuanHoaKhongDau(tenTruong);
            if (t.Contains("CONG THUONG") || t.Contains("HUIT") || t.Contains("INDUSTRY AND TRADE")) return "HUIT";
            if (t.Contains("CONG NGHE THONG TIN") || t.Contains("UIT")) return "UIT01";
            if (t.Contains("BACH KHOA") || t.Contains("HCMUT") || t.Contains("HUST")) return "HCMUT";
            if (t.Contains("SU PHAM KY THUAT") || t.Contains("HCMUTE")) return "HCMUTE";
            if (t.Contains("KHOA HOC TU NHIEN") || t.Contains("HCMUS")) return "HCMUS";
            if (t.Contains("KINH TE - LUAT") || t.Contains("KINH TE LUAT") || t.Contains("UEL")) return "UEL";
            if (t.Contains("KINH TE TP") || t.Contains("KINH TE") || t.Contains("UEH")) return "UEH";
            if (t.Contains("TON DUC THANG") || t.Contains("TDTU")) return "TDTU";
            if (t.Contains("VAN LANG") || t.Contains("VLU")) return "VLU";
            if (t.Contains("HOA SEN") || t.Contains("HSU")) return "HSU";
            if (t.Contains("HUTECH") || t.Contains("CONG NGHE TP")) return "HUTECH";
            if (t.Contains("NGOAI THUONG") || t.Contains("FTU")) return "FTU";
            if (t.Contains("BUU CHINH VIEN THONG") || t.Contains("PTIT")) return "PTIT";
            if (t.Contains("QUOC TE") || t.Contains("INTERNATIONAL UNIVERSITY")) return "IU";
            if (t.Contains("CAN THO") || t.Contains("CTU")) return "CTU";
            if (t.Contains("DA NANG") || t.Contains("DUT")) return "DUT";
            if (t.Contains("Y DUOC") || t.Contains("UMP")) return "UMP";
            if (t.Contains("MY THUAT")) return "DHC01";

            // Lấy ký tự viết tắt tối đa 10 ký tự
            var words = BoDauTiengViet(tenTruong).Split(new[] { ' ', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);
            string code = string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
            return code.Length > 10 ? code.Substring(0, 10) : code;
        }

        private static string ChuanHoaKhongDau(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            // Thay thế ký tự OCR hay nhận nhầm từ tiếng Việt
            s = s.Replace('å', 'a').Replace('Å', 'A')
                 .Replace('ä', 'a').Replace('Ä', 'A')
                 .Replace('ö', 'o').Replace('Ö', 'O')
                 .Replace('ü', 'u').Replace('Ü', 'U')
                 .Replace('é', 'e').Replace('É', 'E');
            var norm = s.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in norm)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return Regex.Replace(sb.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D').ToUpperInvariant(), @"\s+", " ").Trim();
        }

        private static string ChuanHoaHoTenTuOcr(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string s = raw.Trim();
            // Khắc phục các biến thể ký tự Latin mở rộng thường xuất hiện trên Windows OCR tiếng Việt
            s = s.Replace("Trån", "Trần").Replace("Vän", "Văn").Replace("Tin", "Tín")
                 .Replace("Tuån", "Tuấn").Replace("Düng", "Dũng").Replace("Hång", "Hằng")
                 .Replace("Thån", "Thành").Replace("Nguyén", "Nguyễn").Replace("Lé", "Lê")
                 .Replace("Nhu't", "Nhựt").Replace("Nhut", "Nhựt").Replace("Thién", "Thiên")
                 .Replace("Hoång", "Hoàng").Replace("Phü", "Phú").Replace("Vü", "Vũ");

            // Chuyển đổi các nguyên âm Latin mở rộng phổ biến sang tiếng Việt tương đương
            s = s.Replace('é', 'ê').Replace('É', 'Ê')
                 .Replace('å', 'ầ').Replace('Å', 'Ầ')
                 .Replace('ä', 'ă').Replace('Ä', 'Ă')
                 .Replace('ö', 'ô').Replace('Ö', 'Ô')
                 .Replace('ü', 'ư').Replace('Ü', 'Ư');

            s = Regex.Replace(s, @"[^\p{L}\s]", "").Trim();
            var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(w => char.ToUpper(w[0]) + (w.Length > 1 ? w.Substring(1).ToLower() : "")));
        }

        private static string ChuanHoaTenTruong(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string s = Regex.Replace(raw, @"[^\p{L}\s\-\.]", " ").Trim();
            s = Regex.Replace(s, @"\s+", " ");
            var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(w => char.ToUpper(w[0]) + (w.Length > 1 ? w.Substring(1).ToLower() : "")));
        }

        private static string ChuanHoaTieuDeNganh(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string norm = ChuanHoaKhongDau(raw);
            if (norm.Contains("CONG NGHE THONG TIN")) return "Công nghệ thông tin";
            if (norm.Contains("KY THUAT PHAN MEM")) return "Kỹ thuật phần mềm";
            if (norm.Contains("KHOA HOC MAY TINH")) return "Khoa học máy tính";
            if (norm.Contains("HE THONG THONG TIN")) return "Hệ thống thông tin";
            if (norm.Contains("AN TOAN THONG TIN")) return "An toàn thông tin";

            string s = Regex.Replace(raw, @"[^\p{L}\s\-&]", " ").Trim();
            s = Regex.Replace(s, @"\s+", " ");
            var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(w => char.ToUpper(w[0]) + (w.Length > 1 ? w.Substring(1).ToLower() : "")));
        }

        private static string ChuanHoaHoTen(string hoTenRaw)
        {
            return ChuanHoaHoTenTuOcr(hoTenRaw);
        }

        private string? GetMetaFilePath(string? fileMinhChungUrl)
        {
            if (string.IsNullOrWhiteSpace(fileMinhChungUrl)) return null;
            string cleanUrl = fileMinhChungUrl.TrimStart('/', '\\');

            string webWwwroot = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", cleanUrl);
            string metaWeb = webWwwroot + ".meta.json";
            if (File.Exists(metaWeb)) return metaWeb;

            string apiWwwroot = Path.Combine(_env.ContentRootPath, "wwwroot", cleanUrl);
            string metaApi = apiWwwroot + ".meta.json";
            if (File.Exists(metaApi)) return metaApi;

            // Nếu chưa có file, ưu tiên thư mục web wwwroot nếu tồn tại, ngược lại api wwwroot
            string dirWeb = Path.GetDirectoryName(webWwwroot) ?? "";
            if (Directory.Exists(dirWeb)) return metaWeb;
            return metaApi;
        }

        public async Task<bool> XacNhanVaCapNhatHoSoTheSinhVienAsync(XacNhanTheSinhVien_RequestDTO request)
        {
            if (request == null || request.MaUser <= 0)
            {
                throw new ArgumentException("Dữ liệu xác nhận không hợp lệ.");
            }

            var student = await _context.FreelancerStudents
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.maUser == request.MaUser);

            if (student == null || student.User == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hồ sơ sinh viên để cập nhật.");
            }

            // Lưu thông tin thẻ sinh viên đã trích xuất & xác nhận vào file companion metadata (.meta.json)
            // QUY TẮC NGHIỆP VỤ: KHÔNG cập nhật hồ sơ chính thức ngay từ OCR chưa duyệt.
            // Chỉ cập nhật khi Ban quản trị (Admin) phê duyệt hợp lệ.
            var meta = new MinhChungMetaData
            {
                MaUser = request.MaUser,
                Mssv = request.Mssv,
                HoVaTen = request.HoVaTen?.Trim(),
                TenTruong = request.TenTruong?.Trim(),
                MaTruong = request.MaTruong?.Trim(),
                MaChuyenNganh = request.MaChuyenNganh?.Trim(),
                TenChuyenNganh = request.TenChuyenNganh?.Trim(),
                NienKhoa = request.NienKhoa?.Trim(),
                NamThu = request.NamThu,
                NgaySinh = request.NgaySinh,
                NgayGui = DateTime.Now
            };

            string metaJson = System.Text.Json.JsonSerializer.Serialize(meta, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            string? metaPath = GetMetaFilePath(request.FileMinhChungUrl);
            if (!string.IsNullOrEmpty(metaPath))
            {
                var dir = Path.GetDirectoryName(metaPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await File.WriteAllTextAsync(metaPath, metaJson);
            }

            // Đồng bộ sang file meta ở Web wwwroot nếu có
            try
            {
                string cleanUrl = (request.FileMinhChungUrl ?? "").TrimStart('/', '\\');
                string webMeta = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", cleanUrl + ".meta.json");
                string dirWeb = Path.GetDirectoryName(webMeta) ?? "";
                if (Directory.Exists(Path.GetDirectoryName(dirWeb) ?? ""))
                {
                    if (!Directory.Exists(dirWeb)) Directory.CreateDirectory(dirWeb);
                    await File.WriteAllTextAsync(webMeta, metaJson);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Không thể lưu file meta sang Web wwwroot: {Message}", ex.Message);
            }

            // Lưu bản ghi minh chứng sinh viên với trạng thái "Đang gửi" (chờ Admin duyệt)
            var minhChung = new MinhChung_FreelancerStudents
            {
                maUser = request.MaUser,
                loaiMinhChung = "Thẻ sinh viên",
                fileMinhChung = request.FileMinhChungUrl,
                ngayNop = DateTime.Now,
                trangThaiGuiMinhChung = "Đang gửi"
            };

            await _context.MinhChung_FreelancerStudents.AddAsync(minhChung);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<MinhChungItem_ResponseDTO>> LayLichSuMinhChungAsync(int maUser)
        {
            return await _context.MinhChung_FreelancerStudents
                .AsNoTracking()
                .Where(m => m.maUser == maUser)
                .OrderByDescending(m => m.ngayNop)
                .Select(m => new MinhChungItem_ResponseDTO
                {
                    MaMinhChung = m.maMinhChung,
                    MaUser = m.maUser,
                    LoaiMinhChung = m.loaiMinhChung,
                    FileMinhChung = m.fileMinhChung,
                    NgayNop = m.ngayNop,
                    NgayXacMinh = m.ngayXacMinh,
                    TrangThaiGuiMinhChung = m.trangThaiGuiMinhChung,
                    LyDoTuChoi = m.lydoTuChoi
                })
                .ToListAsync();
        }

        public async Task<List<ChuyenNganhOption_DTO>> LayDanhSachChuyenNganhAsync()
        {
            return await _context.ChuyenNganhs
                .AsNoTracking()
                .OrderBy(c => c.tenChuyenNganh)
                .Select(c => new ChuyenNganhOption_DTO
                {
                    MaChuyenNganh = c.maChuyenNganh,
                    TenChuyenNganh = c.tenChuyenNganh
                })
                .ToListAsync();
        }

        public async Task<ThongTinHienTai_DTO?> LayThongTinHienTaiAsync(int maUser)
        {
            var student = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.maUser == maUser);

            if (student == null || student.User == null) return null;

            return new ThongTinHienTai_DTO
            {
                MaUser = student.maUser,
                MaFreelancerStudents = student.maFreelancerStudents,
                HoTenHienTai = student.User.hotenUser ?? string.Empty,
                NgaySinhHienTai = student.User.ngaysinh,
                DiaDiemHienTai = student.diaDiemFreelancerStudent ?? string.Empty,
                TenTruong = student.tenTruong ?? string.Empty,
                ChuyenNganh = student.ChuyenNganh?.tenChuyenNganh ?? string.Empty
            };
        }

        public async Task<KiemTraXacThuc_ResponseDTO> KiemTraTrangThaiXacThucAsync(int maUser)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.maUser == maUser);
            if (user == null)
            {
                return new KiemTraXacThuc_ResponseDTO
                {
                    DaXacMinh = false,
                    TrangThai = "KhongTonTai",
                    ThongBao = "Không tìm thấy người dùng trong hệ thống."
                };
            }

            // Nếu không phải FreelancerStudent (ví dụ Nhà tuyển dụng [role 2] hoặc Admin [role 3]), không áp dụng chặn minh chứng sinh viên
            if (user.maRole != 1)
            {
                return new KiemTraXacThuc_ResponseDTO
                {
                    DaXacMinh = true,
                    TrangThai = "KhongApDung",
                    ThongBao = "Tài khoản không thuộc nhóm Freelancer Sinh viên."
                };
            }

            var latest = await _context.MinhChung_FreelancerStudents
                .AsNoTracking()
                .Where(m => m.maUser == maUser)
                .OrderByDescending(m => m.ngayNop)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                return new KiemTraXacThuc_ResponseDTO
                {
                    DaXacMinh = false,
                    TrangThai = "ChuaGui",
                    ThongBao = "Bạn chưa nộp minh chứng Thẻ sinh viên. Vui lòng quét và xác thực Thẻ sinh viên để mở khóa tính năng này!"
                };
            }

            if (latest.trangThaiGuiMinhChung == "Đã xác minh")
            {
                return new KiemTraXacThuc_ResponseDTO
                {
                    DaXacMinh = true,
                    TrangThai = "DaXacMinh",
                    ThongBao = "Hồ sơ sinh viên đã được xác minh thành công.",
                    NgayXacMinh = latest.ngayXacMinh,
                    MaMinhChungMoiNhat = latest.maMinhChung
                };
            }

            if (latest.trangThaiGuiMinhChung == "Đang gửi")
            {
                return new KiemTraXacThuc_ResponseDTO
                {
                    DaXacMinh = false,
                    TrangThai = "DangGui",
                    ThongBao = "Minh chứng Thẻ sinh viên của bạn đang chờ Ban quản trị phê duyệt. Vui lòng chờ để mở khóa tính năng!",
                    MaMinhChungMoiNhat = latest.maMinhChung
                };
            }

            return new KiemTraXacThuc_ResponseDTO
            {
                DaXacMinh = false,
                TrangThai = "DaTuChoi",
                ThongBao = string.IsNullOrWhiteSpace(latest.lydoTuChoi)
                    ? "Minh chứng thẻ sinh viên bị từ chối. Vui lòng quét lại ảnh thẻ rõ nét."
                    : $"Minh chứng thẻ sinh viên bị từ chối ({latest.lydoTuChoi}). Vui lòng quét lại ảnh thẻ rõ nét.",
                LyDoTuChoi = latest.lydoTuChoi,
                MaMinhChungMoiNhat = latest.maMinhChung
            };
        }

        public async Task<bool> PheDuyetMinhChungAsync(PheDuyetMinhChung_RequestDTO request)
        {
            var mc = await _context.MinhChung_FreelancerStudents.FindAsync(request.MaMinhChung);
            if (mc == null) return false;

            // Sử dụng Transaction để đảm bảo tính nhất quán tuyệt đối giữa trạng thái duyệt và dữ liệu hồ sơ
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.ChapNhan)
                {
                    var student = await _context.FreelancerStudents
                        .Include(f => f.User)
                        .FirstOrDefaultAsync(f => f.maUser == mc.maUser);

                    if (student == null || student.User == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy hồ sơ sinh viên tương ứng với mã User: {mc.maUser}");
                    }

                    // Đọc file companion metadata (.meta.json) đã được sinh viên xác nhận khi nộp thẻ
                    MinhChungMetaData? meta = null;
                    string? metaPath = GetMetaFilePath(mc.fileMinhChung);
                    if (!string.IsNullOrEmpty(metaPath) && File.Exists(metaPath))
                    {
                        try
                        {
                            string json = await File.ReadAllTextAsync(metaPath);
                            meta = System.Text.Json.JsonSerializer.Deserialize<MinhChungMetaData>(json);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Không thể đọc file meta minh chứng: {Message}", ex.Message);
                        }
                    }

                    // Ưu tiên dữ liệu do Admin điều chỉnh nếu có, ngược lại dùng dữ liệu từ companion metadata
                    string? hoTen = !string.IsNullOrWhiteSpace(request.HoVaTen) ? request.HoVaTen : meta?.HoVaTen;
                    string? tenTruong = !string.IsNullOrWhiteSpace(request.TenTruong) ? request.TenTruong : meta?.TenTruong;
                    string? maTruong = !string.IsNullOrWhiteSpace(request.MaTruong) ? request.MaTruong : meta?.MaTruong;
                    string? maChuyenNganh = !string.IsNullOrWhiteSpace(request.MaChuyenNganh) ? request.MaChuyenNganh : meta?.MaChuyenNganh;
                    string? nienKhoa = !string.IsNullOrWhiteSpace(request.NienKhoa) ? request.NienKhoa : meta?.NienKhoa;
                    int? namThu = request.NamThu ?? meta?.NamThu;
                    DateTime? ngaySinh = request.NgaySinh ?? meta?.NgaySinh;

                    // 1. Cập nhật Họ và tên vào Users
                    if (!string.IsNullOrWhiteSpace(hoTen))
                    {
                        student.User.hotenUser = hoTen.Trim();
                    }

                    // 2. Cập nhật Ngày sinh vào Users (nếu có trên thẻ)
                    if (ngaySinh.HasValue)
                    {
                        student.User.ngaysinh = ngaySinh.Value.Date;
                    }

                    // 3. Cập nhật Tên trường học vào FreelancerStudents
                    if (!string.IsNullOrWhiteSpace(tenTruong))
                    {
                        student.tenTruong = tenTruong.Trim();
                    }

                    // 4. Cập nhật Mã trường (nếu có)
                    if (!string.IsNullOrWhiteSpace(maTruong))
                    {
                        string mt = maTruong.Trim();
                        student.maTruong = mt.Length > 10 ? mt.Substring(0, 10) : mt;
                    }

                    // 5. Cập nhật Chuyên ngành học vào FreelancerStudents
                    //    Ưu tiên mã hợp lệ; nếu mã thiếu/sai thì tìm theo tên ngành, chưa có thì thêm vào bảng ChuyenNganh
                    string? tenChuyenNganh = !string.IsNullOrWhiteSpace(request.TenChuyenNganh) ? request.TenChuyenNganh : meta?.TenChuyenNganh;
                    string? maHopLe = null;
                    if (!string.IsNullOrWhiteSpace(maChuyenNganh))
                    {
                        string maTrim = maChuyenNganh.Trim();
                        if (await _context.ChuyenNganhs.AnyAsync(c => c.maChuyenNganh == maTrim))
                        {
                            maHopLe = maTrim;
                        }
                    }
                    if (maHopLe == null && !string.IsNullOrWhiteSpace(tenChuyenNganh))
                    {
                        maHopLe = (await TimHoacTaoChuyenNganhAsync(tenChuyenNganh))?.maChuyenNganh;
                    }
                    if (maHopLe != null)
                    {
                        student.maChuyenNganh = maHopLe;
                    }

                    // 6. Cập nhật Niên khóa vào FreelancerStudents
                    if (!string.IsNullOrWhiteSpace(nienKhoa))
                    {
                        string nk = nienKhoa.Trim();
                        student.nienKhoa = nk.Length > 20 ? nk.Substring(0, 20) : nk;
                    }

                    // 7. Cập nhật Năm thứ vào FreelancerStudents
                    if (namThu.HasValue && namThu.Value >= 1 && namThu.Value <= 6)
                    {
                        student.namThu = namThu.Value;
                    }

                    // Xác định mã Admin hợp lệ tham chiếu tới bảng Admin (maAdmin)
                    int? maAdmin = null;
                    if (request.NguoiXacMinh.HasValue)
                    {
                        var adminRecord = await _context.Admins.FirstOrDefaultAsync(a => a.maAdmin == request.NguoiXacMinh.Value || a.maUser == request.NguoiXacMinh.Value);
                        maAdmin = adminRecord?.maAdmin ?? 1;
                    }
                    else
                    {
                        var firstAdmin = await _context.Admins.FirstOrDefaultAsync();
                        maAdmin = firstAdmin?.maAdmin ?? 1;
                    }

                    // Cập nhật trạng thái minh chứng thành công
                    mc.trangThaiGuiMinhChung = "Đã xác minh";
                    mc.ngayXacMinh = DateTime.Now;
                    mc.lydoTuChoi = null;
                    mc.nguoiXacMinh = maAdmin;
                }
                else
                {
                    int? maAdmin = null;
                    if (request.NguoiXacMinh.HasValue)
                    {
                        var adminRecord = await _context.Admins.FirstOrDefaultAsync(a => a.maAdmin == request.NguoiXacMinh.Value || a.maUser == request.NguoiXacMinh.Value);
                        maAdmin = adminRecord?.maAdmin ?? 1;
                    }
                    else
                    {
                        var firstAdmin = await _context.Admins.FirstOrDefaultAsync();
                        maAdmin = firstAdmin?.maAdmin ?? 1;
                    }

                    // Admin từ chối: KHÔNG cập nhật hồ sơ từ minh chứng bị từ chối, giữ nguyên dữ liệu hồ sơ chính thức
                    mc.trangThaiGuiMinhChung = "Đã từ chối";
                    mc.ngayXacMinh = DateTime.Now;
                    mc.lydoTuChoi = string.IsNullOrWhiteSpace(request.LyDoTuChoi)
                        ? "Hình ảnh thẻ không rõ nét hoặc không hợp lệ."
                        : request.LyDoTuChoi.Trim();
                    mc.nguoiXacMinh = maAdmin;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi phê duyệt minh chứng: {MaMinhChung}", request.MaMinhChung);
                throw;
            }
        }

        public async Task<List<MinhChungItem_ResponseDTO>> LayDanhSachMinhChungChoDuyetAsync()
        {
            var ds = await _context.MinhChung_FreelancerStudents
                .AsNoTracking()
                .Where(m => m.trangThaiGuiMinhChung == "Đang gửi")
                .OrderBy(m => m.ngayNop)
                .ToListAsync();

            var result = new List<MinhChungItem_ResponseDTO>();
            foreach (var m in ds)
            {
                var student = await _context.FreelancerStudents
                    .Include(f => f.User)
                    .Include(f => f.ChuyenNganh)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.maUser == m.maUser);

                MinhChungMetaData? meta = null;
                string? metaPath = GetMetaFilePath(m.fileMinhChung);
                if (!string.IsNullOrEmpty(metaPath) && File.Exists(metaPath))
                {
                    try
                    {
                        string json = await File.ReadAllTextAsync(metaPath);
                        meta = System.Text.Json.JsonSerializer.Deserialize<MinhChungMetaData>(json);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Không thể đọc file meta: {Message}", ex.Message);
                    }
                }

                string? mssvXacNhan = meta?.Mssv;
                if (string.IsNullOrWhiteSpace(mssvXacNhan) && !string.IsNullOrWhiteSpace(m.fileMinhChung))
                {
                    try
                    {
                        string cleanUrl = m.fileMinhChung.TrimStart('/', '\\');
                        string imgPath = Path.Combine(_env.ContentRootPath, "wwwroot", cleanUrl);
                        if (!File.Exists(imgPath))
                        {
                            imgPath = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", cleanUrl);
                        }
                        if (File.Exists(imgPath))
                        {
                            using var bmp = new Bitmap(imgPath);
                            var reader = new BarcodeReader
                            {
                                AutoRotate = true,
                                Options = new ZXing.Common.DecodingOptions
                                {
                                    TryHarder = true,
                                    PossibleFormats = new List<BarcodeFormat>
                                    {
                                        BarcodeFormat.CODE_128, BarcodeFormat.CODE_39, BarcodeFormat.QR_CODE, BarcodeFormat.EAN_13
                                    }
                                }
                            };
                            var decodeResult = reader.Decode(bmp);
                            if (decodeResult != null && !string.IsNullOrWhiteSpace(decodeResult.Text))
                            {
                                var bc = decodeResult.Text.Trim();
                                if (Regex.IsMatch(bc, @"^[A-Z0-9]{6,15}$", RegexOptions.IgnoreCase))
                                {
                                    mssvXacNhan = bc.ToUpperInvariant();
                                }
                            }
                        }
                    }
                    catch { }
                }

                var item = new MinhChungItem_ResponseDTO
                {
                    MaMinhChung = m.maMinhChung,
                    MaUser = m.maUser,
                    LoaiMinhChung = m.loaiMinhChung,
                    FileMinhChung = m.fileMinhChung,
                    NgayNop = m.ngayNop,
                    NgayXacMinh = m.ngayXacMinh,
                    TrangThaiGuiMinhChung = m.trangThaiGuiMinhChung,
                    LyDoTuChoi = m.lydoTuChoi,
                    NguoiXacMinh = m.nguoiXacMinh,

                    HoTenUserHienTai = student?.User?.hotenUser,
                    EmailUserHienTai = student?.User?.emailUser,
                    TenTruongHienTai = student?.tenTruong,
                    TenChuyenNganhHienTai = student?.ChuyenNganh?.tenChuyenNganh,

                    MssvXacNhan = mssvXacNhan,
                    HoTenXacNhan = meta?.HoVaTen,
                    TenTruongXacNhan = meta?.TenTruong,
                    MaTruongXacNhan = meta?.MaTruong,
                    MaChuyenNganhXacNhan = meta?.MaChuyenNganh,
                    TenChuyenNganhXacNhan = meta?.TenChuyenNganh,
                    NienKhoaXacNhan = meta?.NienKhoa,
                    NamThuXacNhan = meta?.NamThu,
                    NgaySinhXacNhan = meta?.NgaySinh
                };
                result.Add(item);
            }

            return result;
        }
    }

    public class MinhChungMetaData
    {
        public int MaUser { get; set; }
        public string? Mssv { get; set; }
        public string? HoVaTen { get; set; }
        public string? TenTruong { get; set; }
        public string? MaTruong { get; set; }
        public string? MaChuyenNganh { get; set; }
        public string? TenChuyenNganh { get; set; }
        public string? NienKhoa { get; set; }
        public int? NamThu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public DateTime NgayGui { get; set; } = DateTime.Now;
    }
}





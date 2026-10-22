using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FreelancerStudent.API.Services
{
    public class NoiDungValidator
    {
        private static readonly string[] TuCam =
        [
            "dit", "dcm", "dm", "vcl", "vl", "ngu", "sucvat", "matday", "khonann",
            "lua dao", "luadao", "scam", "vo hoc", "vohoc", "matnhet"
        ];
        private static readonly Regex SoDienThoai = new(@"(?:\+?84|0)(?:[\s().-]*\d){8,10}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex Email = new(@"\b[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex Url = new(@"(?:https?\s*:\s*//|www\s*\.|\b[a-z0-9-]+\.(?:com|vn|net|org|io|me|co)\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool HopLe(string? noiDung)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                return true;
            if (SoDienThoai.IsMatch(noiDung) || Email.IsMatch(noiDung) || Url.IsMatch(noiDung))
                return false;

            var noiDungThuongHoa = Regex.Replace(ChuanHoa(noiDung), @"[^a-z0-9\s]", string.Empty);
            if (noiDungThuongHoa.Contains("zalo") || noiDungThuongHoa.Contains("facebook") ||
                noiDungThuongHoa.Contains("telegram") || noiDungThuongHoa.Contains("messenger"))
                return false;

            var tokens = Regex.Matches(noiDungThuongHoa, @"[a-z0-9]+")
                .Select(match => match.Value)
                .ToHashSet(StringComparer.Ordinal);
            return !TuCam.Any(tu =>
            {
                var normalized = Regex.Replace(ChuanHoa(tu), @"[^a-z0-9\s]", string.Empty);
                return normalized.Contains(' ')
                    ? noiDungThuongHoa.Contains(normalized, StringComparison.Ordinal)
                    : tokens.Contains(normalized);
            });
        }

        private static string ChuanHoa(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(character == 'đ' || character == 'Đ' ? 'd' : char.ToLowerInvariant(character));
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

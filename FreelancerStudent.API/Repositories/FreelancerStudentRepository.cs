using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FreelancerStudent.API.Repositories
{
    public class FreelancerStudentRepository : IFreelancerStudentRepository
    {
        private readonly ApplicationDBContext _context;

        public FreelancerStudentRepository(ApplicationDBContext context)
        {
            _context = context;

        }


        public async Task<List<FreelancerStudents>> layTatCaFreelancerStudentsAsync()
        {
            var ketqua = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .Include(f => f.FreelancerStudent_KyNangs)
                    .ThenInclude(fk => fk.KyNang)
                .OrderByDescending(f => f.User != null ? f.User.ngayTao : DateTime.MinValue)
                .ThenByDescending(f => f.maFreelancerStudents)
                .ToListAsync();
            return ketqua;
        }

        //Tuấn
        public async Task<FreelancerStudents?> layFreelancerStudent_TheoMaUser(int maUser)
        {
            var freelancerStudent = await _context.FreelancerStudents.Include(f => f.User).Include(f => f.ChuyenNganh).FirstOrDefaultAsync(f => f.maUser == maUser);
            return freelancerStudent;
        }


        //Tuấn
        public async Task<FreelancerStudents> themFreelancerStudent_DuaVaoMaRoleCuaUsers(Users user, int maRole)
        {
            if (maRole != 1)
            {
                return null!; // Không phải FreelancerStudent
            }

            //lấy mã chuyên ngành mặc định đầu tiên trong DB để tránh lỗi khóa ngoại FK
            var maChuyenNganhMacDinh = await _context.ChuyenNganhs
                .Select(c => c.maChuyenNganh)
                .FirstOrDefaultAsync() ?? "CNTT";

            var freeMoi = new FreelancerStudents
            {
                maUser = user.maUser,
                maChuyenNganh = maChuyenNganhMacDinh,
                maTruong = "DH",
                tenTruong = "Chưa cập nhật trường học",
                diaDiemTruong = "Chưa cập nhật",
                diaDiemFreelancerStudent = "Chưa cập nhật",
                namThu = 1,
                GPA = 0,
                nienKhoa = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 4}",
                gioithieu = "Chưa cập nhật thông tin giới thiệu bản thân.",
                trangthaiNhanViec = true,
                chiPhiTu = 0
            };
            await _context.FreelancerStudents.AddAsync(freeMoi);
            await _context.SaveChangesAsync();

            return freeMoi;
        }


        //Tuấn
        public async Task<FreelancerStudents?> layChiTietTheoIdAsync(int id)
        {
            return await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .Include(f => f.FreelancerStudent_KyNangs)
                    .ThenInclude(k => k.KyNang)
                .FirstOrDefaultAsync(f => f.maFreelancerStudents == id || f.maUser == id); // Tìm theo mã Freelancer hoặc mã User
        }

                #region ChuyenNganh Helpers
        private static string BoDauTiengViet(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var normalized = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);
            foreach (var ch in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        private static string KhoaSoSanh(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            return Regex.Replace(BoDauTiengViet(s).ToLowerInvariant(), @"\s+", " ").Trim();
        }

        private static string ChuanHoaTenChuyenNganh(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string s = Regex.Replace(raw.Trim(), @"\s+", " ");
            if (s.Length > 150) s = s.Substring(0, 150).Trim();
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            if (s.Where(char.IsLetter).All(char.IsUpper))
            {
                s = textInfo.ToTitleCase(s.ToLower());
            }
            else
            {
                s = char.ToUpper(s[0]) + s.Substring(1);
            }
            return s;
        }

        private static string TaoMaChuyenNganhCoSo(string ten)
        {
            var words = BoDauTiengViet(ten)
                .Split(new[] { ' ', '-', '&', '/', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => new string(w.Where(c => c < 128 && char.IsLetterOrDigit(c)).ToArray()))
                .Where(w => w.Length > 0)
                .ToList();

            string ma = string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
            if (ma.Length < 2 && words.Count > 0)
            {
                string singleWord = words[0].ToUpperInvariant();
                ma = singleWord.Length > 4 ? singleWord.Substring(0, 4) : singleWord;
            }
            if (string.IsNullOrEmpty(ma)) ma = "CN";
            return ma.Length > 16 ? ma.Substring(0, 16) : ma;
        }

        private async Task<string> TimHoacTaoChuyenNganhAsync(string? maChuyenNganh, string? tenChuyenNganh)
        {
            var ds = await _context.ChuyenNganhs.ToListAsync();

            // Nếu người dùng chọn một mã chuyên ngành đã tồn tại trong danh mục
            if (!string.IsNullOrWhiteSpace(maChuyenNganh))
            {
                string maTrim = maChuyenNganh.Trim();
                var daCoTheoMa = ds.FirstOrDefault(c => c.maChuyenNganh.Equals(maTrim, StringComparison.OrdinalIgnoreCase));
                if (daCoTheoMa != null)
                {
                    if (string.IsNullOrWhiteSpace(tenChuyenNganh) ||
                        KhoaSoSanh(daCoTheoMa.tenChuyenNganh) == KhoaSoSanh(tenChuyenNganh))
                    {
                        return daCoTheoMa.maChuyenNganh;
                    }
                }
            }

            // Nếu người dùng nhập tên chuyên ngành (hoặc đổi sang tên mới)
            if (!string.IsNullOrWhiteSpace(tenChuyenNganh))
            {
                string tenChuanHoa = ChuanHoaTenChuyenNganh(tenChuyenNganh);
                if (!string.IsNullOrWhiteSpace(tenChuanHoa))
                {
                    string keyTen = KhoaSoSanh(tenChuanHoa);
                    // 1. Kiểm tra xem tên đã tồn tại trong DB chưa (không phân biệt dấu và hoa thường)
                    var daCoTheoTen = ds.FirstOrDefault(c => KhoaSoSanh(c.tenChuyenNganh) == keyTen);
                    if (daCoTheoTen != null)
                    {
                        return daCoTheoTen.maChuyenNganh;
                    }

                    // 2. Tự động sinh mã viết tắt từ chữ cái đầu của mỗi từ (viết hoa)
                    string maCoSo = TaoMaChuyenNganhCoSo(tenChuanHoa);
                    var dsMa = ds.Select(c => c.maChuyenNganh.ToUpperInvariant()).ToHashSet();

                    string maMoi = maCoSo;
                    int hauTo = 2;
                    // Xử lý va chạm mã: Nếu mã viết tắt đã thuộc về chuyên ngành khác, gắn hậu tố số tăng dần (VD: TTNT2, TTNT3)
                    while (dsMa.Contains(maMoi.ToUpperInvariant()))
                    {
                        maMoi = maCoSo + hauTo;
                        hauTo++;
                    }

                    var cnMoi = new ChuyenNganh
                    {
                        maChuyenNganh = maMoi,
                        tenChuyenNganh = tenChuanHoa
                    };

                    try
                    {
                        _context.ChuyenNganhs.Add(cnMoi);
                        await _context.SaveChangesAsync();
                        return maMoi;
                    }
                    catch (DbUpdateException)
                    {
                        // Xử lý tranh chấp đồng thời nếu bản ghi vừa được tạo bởi luồng khác
                        _context.Entry(cnMoi).State = EntityState.Detached;
                        var dsMoi = await _context.ChuyenNganhs.AsNoTracking().ToListAsync();
                        var tonTai = dsMoi.FirstOrDefault(c => KhoaSoSanh(c.tenChuyenNganh) == keyTen);
                        if (tonTai != null)
                        {
                            return tonTai.maChuyenNganh;
                        }
                        throw;
                    }
                }
            }

            return string.Empty;
        }
        #endregion

        //Tuấn - Tìm theo mã User rồi cập nhật
        public async Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO dto)
        {
            //  Tìm bản ghi Freelancer theo mã User đang đăng nhập
            var student = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.FreelancerStudent_KyNangs)
                .FirstOrDefaultAsync(f => f.maUser == dto.maUser);

            // Nếu không tìm thấy thì báo lỗi / trả về false
            if (student == null)
            {
                return false;
            }

            // Kiểm tra tính hợp lệ dữ liệu
            if (dto.GPA < 0.0 || dto.GPA > 4.0)
            {
                throw new ArgumentException("Điểm GPA phải nằm trong khoảng từ 0.0 đến 4.0.");
            }
            if (dto.chiPhiTu.HasValue && dto.chiPhiTu.Value < 0)
            {
                throw new ArgumentException("Mức giá tham khảo không được là số âm.");
            }

            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra xem sinh viên đã có minh chứng thẻ sinh viên được duyệt ("Đã xác minh") hay chưa
                bool daXacMinhThe = await _context.MinhChung_FreelancerStudents
                    .AsNoTracking()
                    .AnyAsync(m => m.maUser == dto.maUser && m.trangThaiGuiMinhChung == "Đã xác minh");

                // PHƯƠNG ÁN A: Cho phép Freelancer tự do thay đổi Chuyên ngành trong Hồ sơ năng lực
                // Tự động tìm kiếm hoặc tạo mới Chuyên ngành nếu chưa có trong Database
                if (!string.IsNullOrWhiteSpace(dto.tenChuyenNganh) || !string.IsNullOrWhiteSpace(dto.maChuyenNganh))
                {
                    string maChuyenNganhChot = await TimHoacTaoChuyenNganhAsync(dto.maChuyenNganh, dto.tenChuyenNganh);
                    if (!string.IsNullOrWhiteSpace(maChuyenNganhChot))
                    {
                        student.maChuyenNganh = maChuyenNganhChot;
                    }
                }

                // QUY TẮC BẢO VỆ DỮ LIỆU TẠI BACKEND:
                // Nếu đã xác minh thẻ sinh viên: KHÓA các trường được xác minh từ thẻ (Họ tên, Trường, Năm học, Niên khóa)
                // Từ chối mọi nỗ lực ghi đè các trường này kể cả khi client tự ý sửa payload.
                if (!daXacMinhThe)
                {
                    student.tenTruong = dto.tenTruong ?? string.Empty;
                    student.namThu = dto.namThu;
                    student.nienKhoa = dto.nienKhoa ?? string.Empty;

                    if (student.User != null && !string.IsNullOrWhiteSpace(dto.tenUser))
                    {
                        student.User.hotenUser = dto.tenUser.Trim();
                    }
                }

                // Các trường tự do không thuộc nhóm thẻ sinh viên vẫn cho phép cập nhật theo quyền hạn:
                student.diaDiemFreelancerStudent = dto.diaDiemFreelancerStudent ?? string.Empty;
                student.GPA = Math.Round(dto.GPA, 2);
                student.gioithieu = dto.gioithieu;
                student.kyNangCoBan = dto.kyNangCoBan;
                student.ngonNgu = dto.ngonNgu;
                student.trangthaiNhanViec = dto.trangthaiNhanViec;
                student.chiPhiTu = dto.chiPhiTu;

                // Cập nhật thông tin tài khoản (Users)
                if (student.User != null)
                {
                    student.User.sdtUser = dto.sdt;
                }

                // Cập nhật danh sách kỹ năng chuyên môn
                if (dto.danhSachKyNang != null)
                {
                    // Xóa danh sách kỹ năng cũ của sinh viên
                    var kyNangCu = _context.FreelancerStudent_KyNangs.Where(k => k.maFreelancerStudents == student.maFreelancerStudents);
                    _context.FreelancerStudent_KyNangs.RemoveRange(kyNangCu);

                    // Thêm danh sách kỹ năng mới
                    foreach (var tenKn in dto.danhSachKyNang.Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        var kn = await _context.KyNangs.FirstOrDefaultAsync(k => k.tenKyNang.ToLower() == tenKn.Trim().ToLower());
                        if (kn == null)
                        {
                            kn = new KyNang { tenKyNang = tenKn.Trim() };
                            await _context.KyNangs.AddAsync(kn);
                            await _context.SaveChangesAsync();
                        }

                        await _context.FreelancerStudent_KyNangs.AddAsync(new FreelancerStudent_KyNang
                        {
                            maFreelancerStudents = student.maFreelancerStudents,
                            maKyNang = kn.maKyNang
                        });
                    }
                }

                // Lưu toàn bộ thay đổi xuống SQL Server
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo
        public async Task<bool> capNhatThongTinCoBanAsync(DTOs.RequestDTOs.CapNhatThongTinCoBan_RequestDTO dto)
        {
            var student = await _context.FreelancerStudents
                .FirstOrDefaultAsync(f => f.maUser == dto.maUser);

            if (student == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.diaDiemFreelancerStudent))
            {
                throw new ArgumentException("Địa điểm không được để trống.");
            }

            if (dto.diaDiemFreelancerStudent.Trim().Length > 100)
            {
                throw new ArgumentException("Địa điểm không được vượt quá 100 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(dto.ngonNgu) && dto.ngonNgu.Trim().Length > 100)
            {
                throw new ArgumentException("Ngôn ngữ không được vượt quá 100 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(dto.kyNangCoBan) && dto.kyNangCoBan.Trim().Length > 150)
            {
                throw new ArgumentException("Kỹ năng cơ bản không được vượt quá 150 ký tự.");
            }

            if (dto.chiPhiTu.HasValue && dto.chiPhiTu.Value < 0)
            {
                throw new ArgumentException("Mức giá tham khảo không được là số âm.");
            }

            student.diaDiemFreelancerStudent = dto.diaDiemFreelancerStudent.Trim();
            student.ngonNgu = string.IsNullOrWhiteSpace(dto.ngonNgu) ? null : dto.ngonNgu.Trim();
            student.kyNangCoBan = string.IsNullOrWhiteSpace(dto.kyNangCoBan) ? null : dto.kyNangCoBan.Trim();
            student.chiPhiTu = dto.chiPhiTu;

            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật điểm GPA học tập
        public async Task<bool> capNhatGPAAsync(DTOs.RequestDTOs.CapNhatGPA_RequestDTO dto)
        {
            var student = await _context.FreelancerStudents
                .FirstOrDefaultAsync(f => f.maUser == dto.maUser);

            if (student == null)
            {
                return false;
            }

            if (dto.GPA < 0.0 || dto.GPA > 4.0)
            {
                throw new ArgumentException("Điểm GPA phải nằm trong khoảng hợp lệ từ 0.0 đến 4.0.");
            }

            student.GPA = Math.Round(dto.GPA, 2);
            await _context.SaveChangesAsync();
            return true;
        }




        //XS
        public async Task<FreelancerStudents?> layTheoMaFreelancerStudents(int maFreelancerStudents)
        {
            var freelancerStudent = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .Include(f => f.FreelancerStudent_KyNangs)
                    .ThenInclude(k => k.KyNang)
                .FirstOrDefaultAsync(f => f.maFreelancerStudents == maFreelancerStudents);
            return freelancerStudent;
        }

        public async Task<List<string>> layTatCaKyNangAsync()
        {
            return await _context.KyNangs
                .OrderBy(k => k.maKyNang)
                .Select(k => k.tenKyNang)
                .ToListAsync();
        }
    }
}
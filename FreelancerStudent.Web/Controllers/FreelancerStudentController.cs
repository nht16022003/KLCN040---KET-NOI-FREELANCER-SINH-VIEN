using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class FreelancerStudentController : Controller
    {
        private readonly IFreelancerStudentWebService _freelancerStudentWebService;

        private readonly IJobPostWebService _jobPostWebService;

        private readonly IBaiDangTimViecWebService _baiDangWebService;
        private readonly IDanhGiaWebService _danhGiaWebService;
        private readonly IMinhChungWebService _minhChungWebService;

        public FreelancerStudentController(IFreelancerStudentWebService freelancerStudentWebService, IJobPostWebService jobPostWebService,
        IBaiDangTimViecWebService baiDangTimViecWebService, IDanhGiaWebService danhGiaWebService,
        IMinhChungWebService minhChungWebService)
        {
            _freelancerStudentWebService = freelancerStudentWebService;
            _jobPostWebService = jobPostWebService;
            _baiDangWebService = baiDangTimViecWebService;
            _danhGiaWebService = danhGiaWebService;
            _minhChungWebService = minhChungWebService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? chuyenNganh,
            string? kyNang,
            double? soSaoMin,
            double? gpaMin,
            int? namThu,
            bool? trangThaiNhanViec,
            string? sapXep)
        {
            var model = await LayDanhSachFreelancerDaLoc(tuKhoa, chuyenNganh, kyNang, soSaoMin, gpaMin, namThu, trangThaiNhanViec, sapXep);
            return View(model);
        }

        private async Task<FreelancerFilterViewModel> LayDanhSachFreelancerDaLoc(
            string? tuKhoa,
            string? chuyenNganh,
            string? kyNang,
            double? soSaoMin,
            double? gpaMin,
            int? namThu,
            bool? trangThaiNhanViec,
            string? sapXep)
        {
            var layDanhSach = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            var rawList = layDanhSach.data ?? new List<FreelacerStudentViewModel>();

            // Danh sách lựa chọn cho dropdown bộ lọc
            var allMajors = rawList
                .Where(f => !string.IsNullOrWhiteSpace(f.tenChuyenNganh))
                .Select(f => f.tenChuyenNganh!)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            var allSkills = rawList
                .SelectMany(f => f.danhSachKyNang != null && f.danhSachKyNang.Any() 
                    ? f.danhSachKyNang 
                    : (!string.IsNullOrWhiteSpace(f.kyNangCoBan) 
                        ? f.kyNangCoBan.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()) 
                        : Enumerable.Empty<string>()))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            var query = rawList.AsEnumerable();

            // 1. Tìm theo từ khóa (tên, trường học, giới thiệu, kỹ năng)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(f =>
                    (f.tenUser != null && f.tenUser.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (f.tenTruong != null && f.tenTruong.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (f.gioithieu != null && f.gioithieu.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (f.kyNangCoBan != null && f.kyNangCoBan.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (f.danhSachKyNang != null && f.danhSachKyNang.Any(k => k.Contains(kw, StringComparison.OrdinalIgnoreCase))));
            }

            // 2. Lọc theo chuyên ngành
            if (!string.IsNullOrWhiteSpace(chuyenNganh))
            {
                query = query.Where(f => string.Equals(f.tenChuyenNganh, chuyenNganh, StringComparison.OrdinalIgnoreCase)
                                      || string.Equals(f.maChuyenNganh, chuyenNganh, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Lọc theo kỹ năng
            if (!string.IsNullOrWhiteSpace(kyNang))
            {
                var kn = kyNang.Trim();
                query = query.Where(f =>
                    (f.danhSachKyNang != null && f.danhSachKyNang.Any(k => k.Contains(kn, StringComparison.OrdinalIgnoreCase))) ||
                    (f.kyNangCoBan != null && f.kyNangCoBan.Contains(kn, StringComparison.OrdinalIgnoreCase)));
            }

            // 4. Lọc theo số sao đánh giá tối thiểu
            if (soSaoMin.HasValue && soSaoMin.Value > 0)
            {
                query = query.Where(f => f.diemDanhGiaTrungBinh >= soSaoMin.Value && f.soLuotDanhGia > 0);
            }

            // 5. Lọc theo GPA tối thiểu
            if (gpaMin.HasValue && gpaMin.Value > 0)
            {
                query = query.Where(f => f.GPA >= gpaMin.Value);
            }

            // 6. Lọc theo năm học
            if (namThu.HasValue && namThu.Value > 0)
            {
                query = query.Where(f => f.namThu == namThu.Value);
            }

            // 7. Lọc theo trạng thái nhận việc
            if (trangThaiNhanViec.HasValue)
            {
                query = query.Where(f => f.trangthaiNhanViec == trangThaiNhanViec.Value);
            }

            // 8. Sắp xếp (Mặc định bắt buộc theo Section 5: Ngày tạo giảm dần)
            string sortOrder = string.IsNullOrWhiteSpace(sapXep) ? "moi-nhat" : sapXep;
            switch (sortOrder)
            {
                case "sao-cao":
                    query = query.OrderByDescending(f => f.diemDanhGiaTrungBinh).ThenByDescending(f => f.soLuotDanhGia);
                    break;
                case "gpa-cao":
                    query = query.OrderByDescending(f => f.GPA).ThenByDescending(f => f.diemDanhGiaTrungBinh);
                    break;
                case "moi-nhat":
                default:
                    query = query.OrderByDescending(f => f.ngayTao).ThenByDescending(f => f.maFreelancerStudents);
                    break;
            }

            return new FreelancerFilterViewModel
            {
                DanhSachFreelancer = query.ToList(),
                TuKhoa = tuKhoa,
                ChuyenNganh = chuyenNganh,
                KyNang = kyNang,
                SoSaoMin = soSaoMin,
                GpaMin = gpaMin,
                NamThu = namThu,
                TrangThaiNhanViec = trangThaiNhanViec,
                SapXep = sortOrder,
                DanhSachChuyenNganh = allMajors,
                DanhSachKyNang = allSkills
            };
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Profile(int? maUser, int? maFreelancerStudents)
        {
            if ((!maUser.HasValue || maUser <= 0) && (!maFreelancerStudents.HasValue || maFreelancerStudents <= 0))
            {
                return NotFound();
            }

            FreelacerStudentViewModel? freelancer;
            if (maFreelancerStudents.HasValue && maFreelancerStudents > 0)
            {
                var listResult = await _freelancerStudentWebService
                    .layDanhSachFreelancerStudentAsync();
                freelancer = listResult.data?.FirstOrDefault(x => x.maFreelancerStudents == maFreelancerStudents.Value);
            }
            else
            {
                var listResult = await _freelancerStudentWebService
                    .layDanhSachFreelancerStudentAsync();
                freelancer = listResult.data?.FirstOrDefault(x => x.maUser == maUser!.Value);
            }

            if (freelancer == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ freelancer của tài khoản đang đăng nhập.";
                return NotFound();
            }

            ViewData["IsProfileOwner"] = freelancer.maUser == HttpContext.Session.GetInt32("maUser");

            var result = await _freelancerStudentWebService
                .layProfileFreelancerStudentAsync(freelancer.maFreelancerStudents);

            if (result.data != null)
            {
                var stats = await _danhGiaWebService.LayFreelancerAsync(freelancer.maFreelancerStudents, 1, 10);
                result.data.ThongKeDanhGia = stats.data;
            }

            return View(result.data);
        }

        //Sơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject(PortfolioViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var listResult = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            var freelancer = listResult.data?.FirstOrDefault(x => x.maUser == maUser.Value);
            if (freelancer == null)
            {
                return NotFound();
            }

            var project = new DuAnTrongPortfolioViewModel
            {
                tenDuAn = model.NewTenDuAn,
                vaiTro = model.NewVaiTro,
                moTa = model.NewMoTa,
                congnghe = model.NewCongNghe,
                linkGithub = model.NewLinkGithub,
                linkDemo = model.NewLinkDemo,
                link_file = model.NewLinkFile
            };
            var result = await _freelancerStudentWebService.themDuAnAsync(project, freelancer.maFreelancerStudents);
            if (!result.success)
            {
                TempData["Error"] = result.message;
            }
            else
            {
                TempData["Success"] = "Đã thêm dự án thành công.";
            }

            return RedirectToAction(nameof(Portfolio));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePortfolio(PortfolioViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            var freelancer = await GetCurrentFreelancerAsync(maUser);
            if (freelancer == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.maFreelancerStudents = freelancer.maFreelancerStudents;
            var result = await _freelancerStudentWebService.capNhatPortfolioAsync(model);
            TempData[result.success ? "Success" : "Error"] = result.success
                ? "Đã cập nhật video giới thiệu."
                : result.message;

            return RedirectToAction(nameof(Portfolio));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProject(PortfolioViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            var freelancer = await GetCurrentFreelancerAsync(maUser);
            if (freelancer == null) return RedirectToAction("DangNhap", "Account");

            var project = model.projects.FirstOrDefault(x => x.maDA == model.EditMaDA);
            if (project == null) return RedirectToAction(nameof(Portfolio));

            var result = await _freelancerStudentWebService.suaDuAnAsync(project, freelancer.maFreelancerStudents);
            TempData[result.success ? "Success" : "Error"] = result.success ? "Đã sửa dự án thành công." : result.message;
            return RedirectToAction(nameof(Portfolio));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(string maDA)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            var freelancer = await GetCurrentFreelancerAsync(maUser);
            if (freelancer == null) return RedirectToAction("DangNhap", "Account");

            var result = await _freelancerStudentWebService.xoaDuAnAsync(maDA, freelancer.maFreelancerStudents);
            TempData[result.success ? "Success" : "Error"] = result.success ? "Đã xóa dự án thành công." : result.message;
            return RedirectToAction(nameof(Portfolio));
        }

        [HttpGet]
        public async Task<IActionResult> Portfolio(int? maFreelancerStudents)
        {
            var currentUserId = HttpContext.Session.GetInt32("maUser");
            if (!currentUserId.HasValue && !maFreelancerStudents.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var listResult = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            var freelancer = maFreelancerStudents.HasValue
                ? listResult.data?.FirstOrDefault(x => x.maFreelancerStudents == maFreelancerStudents.Value)
                : listResult.data?.FirstOrDefault(x => x.maUser == currentUserId!.Value);

            if (freelancer == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ freelancer.";
                return NotFound();
            }

            var result = await _freelancerStudentWebService
                .layPortfolioAsync(freelancer.maFreelancerStudents);

            if (!result.success || result.data == null)
            {
                TempData["Error"] = result.message ?? "Không thể tải Portfolio.";
                return View(new PortfolioViewModel
                {
                    maFreelancerStudents = freelancer.maFreelancerStudents
                });
            }

            return View(result.data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFeaturedProjects(List<string> maDAs)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            var freelancer = await GetCurrentFreelancerAsync(maUser);
            if (freelancer == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var result = await _freelancerStudentWebService
                .capNhatDuAnNoiBatAsync(freelancer.maFreelancerStudents, maDAs ?? new List<string>());

            TempData[result.success ? "Success" : "Error"] = result.success
                ? "Đã cập nhật dự án nổi bật."
                : result.message;

            return RedirectToAction(nameof(Profile), new { maUser = maUser!.Value });
        }

        // Cập nhật kỹ năng chuyên ngành trực tiếp từ trang Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatKyNang(List<string> danhSachKyNang)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var freelancer = await GetCurrentFreelancerAsync(maUser.Value);
            if (freelancer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên của bạn.";
                return RedirectToAction("Profile", new { maUser = maUser.Value });
            }

            // Lấy chi tiết hồ sơ hiện tại để giữ nguyên các thông tin học vấn, trường học
            var hoSoKq = await _freelancerStudentWebService.layChiTietHoSoAsync(freelancer.maFreelancerStudents);
            var hoSo = hoSoKq.data ?? new ChiTietHoSoFreelancerViewModel();

            hoSo.MaUser = maUser.Value;
            hoSo.MaFreelancerStudents = freelancer.maFreelancerStudents;
            hoSo.DanhSachKyNang = danhSachKyNang?
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            var ketQua = await _freelancerStudentWebService.capNhatHoSoAsync(hoSo);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật kỹ năng chuyên ngành thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message ?? "Không thể cập nhật kỹ năng chuyên ngành.";
            }

            return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
        }

        // Cập nhật thông tin giới thiệu bản thân trực tiếp từ trang Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatGioiThieu(string gioiThieu, string? urlVideo)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var freelancer = await GetCurrentFreelancerAsync(maUser.Value);
            if (freelancer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ sinh viên của bạn.";
                return RedirectToAction("Profile", new { maUser = maUser.Value });
            }

            // 1. Cập nhật vào bảng FreelancerStudents (cột gioithieu)
            var hoSoKq = await _freelancerStudentWebService.layChiTietHoSoAsync(freelancer.maFreelancerStudents);
            if (hoSoKq.success && hoSoKq.data != null)
            {
                var hoSo = hoSoKq.data;
                hoSo.MaUser = maUser.Value;
                hoSo.MaFreelancerStudents = freelancer.maFreelancerStudents;
                hoSo.Gioithieu = gioiThieu?.Trim() ?? string.Empty;
                await _freelancerStudentWebService.capNhatHoSoAsync(hoSo);
            }

            // 2. Cập nhật vào bảng Portfolio (cột moTaBanThan & url_video)
            var portfolioModel = new PortfolioViewModel
            {
                maFreelancerStudents = freelancer.maFreelancerStudents,
                moTaBanThan = gioiThieu?.Trim() ?? string.Empty,
                url_video = urlVideo?.Trim()
            };
            var portfolioKq = await _freelancerStudentWebService.capNhatPortfolioAsync(portfolioModel);

            if (portfolioKq.success)
            {
                TempData["SuccessMessage"] = "Cập nhật giới thiệu bản thân thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = portfolioKq.message ?? "Không thể cập nhật giới thiệu bản thân.";
            }

            return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
        }

        // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo trực tiếp từ trang Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTinCoBan(string diaDiem, string? ngonNgu, string? kyNangCoBan, decimal? chiPhiTu)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(diaDiem))
            {
                TempData["ErrorMessage"] = "Địa điểm làm việc / sinh sống không được để trống.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            if (diaDiem.Trim().Length > 100)
            {
                TempData["ErrorMessage"] = "Địa điểm không được vượt quá 100 ký tự.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            if (!string.IsNullOrWhiteSpace(ngonNgu) && ngonNgu.Trim().Length > 100)
            {
                TempData["ErrorMessage"] = "Ngôn ngữ không được vượt quá 100 ký tự.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            if (!string.IsNullOrWhiteSpace(kyNangCoBan) && kyNangCoBan.Trim().Length > 150)
            {
                TempData["ErrorMessage"] = "Kỹ năng cơ bản không được vượt quá 150 ký tự.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            if (chiPhiTu.HasValue && chiPhiTu.Value < 0)
            {
                TempData["ErrorMessage"] = "Mức giá tham khảo không được là số âm.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            var model = new CapNhatThongTinCoBanViewModel
            {
                MaUser = maUser.Value,
                DiaDiemFreelancerStudent = diaDiem.Trim(),
                NgonNgu = string.IsNullOrWhiteSpace(ngonNgu) ? null : ngonNgu.Trim(),
                KyNangCoBan = string.IsNullOrWhiteSpace(kyNangCoBan) ? null : kyNangCoBan.Trim(),
                ChiPhiTu = chiPhiTu
            };

            var res = await _freelancerStudentWebService.capNhatThongTinCoBanAsync(model);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin cơ bản thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể cập nhật thông tin cơ bản.";
            }

            return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
        }

        // Cập nhật điểm GPA học tập trực tiếp từ trang Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatGPA(double? gpa)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (!gpa.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập điểm GPA.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            if (gpa.Value < 0.0 || gpa.Value > 4.0)
            {
                TempData["ErrorMessage"] = "Điểm GPA phải nằm trong khoảng hợp lệ từ 0.0 đến 4.0.";
                return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
            }

            var res = await _freelancerStudentWebService.capNhatGPAAsync(maUser.Value, gpa.Value);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Cập nhật điểm GPA học tập thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể cập nhật điểm GPA.";
            }

            return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
        }


        //Tuấn
        [HttpGet]

        public async Task<IActionResult> DanhSachNopTuyen()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");


            }

            //Lấy danh sách các đơn ứng tuyển của user này
            var kq = await _jobPostWebService.layDSUngTuyenTheoMaUser(maUser.Value);
            var dsUngTuyen = kq?.data ?? new List<UngTuyenViewModel>();
            return View(dsUngTuyen);
        }

        //Tuấn
        //Mở trang đăng tin tìm việc
        [HttpGet]
        public async Task<IActionResult> DangTinTimViec()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra trạng thái xác thực minh chứng sinh viên
            var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
            {
                TempData["ErrorMessage"] = "Bạn cần được xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') trước khi đăng tin tìm việc!";
                return RedirectToAction("MinhChung", "Account");
            }

            return View(new DangTinTimViecViewModel { maUser = maUser.Value });
        }

        //Tuấn
        // Submit form Đăng tin tìm việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangTinTimViec(DangTinTimViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra trạng thái xác thực minh chứng sinh viên
            var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
            {
                TempData["ErrorMessage"] = "Bạn cần được xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') trước khi đăng tin tìm việc!";
                return RedirectToAction("MinhChung", "Account");
            }

            model.maUser = maUser.Value;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var ketQua = await _baiDangWebService.taoBaiDangAsync(model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Đăng tin tìm việc thành công! Nhà tuyển dụng có thể xem hồ sơ dịch vụ của bạn.";
                return RedirectToAction("QuanLyTimViec");
            }
            ModelState.AddModelError(string.Empty, ketQua.message ?? "Đăng tin thất bại.");
            return View(model);
        }

        //Tuấn
        //Quản lý danh sách các bài đăng tìm việc của sinh viên
        [HttpGet]
        public async Task<IActionResult> QuanLyTimViec()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            var ketQua = await _baiDangWebService.layDanhSachCuaToiAsync(maUser.Value);
            var dsBaiDang = ketQua?.data ?? new List<BaiDangTimViecViewModel>();
            return View(dsBaiDang);
        }

        //Tuấn
        //Bật / Tắt trạng thái ẩn bài đăng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThaiBaiDang(string maBaiDang, string trangThai)
        {
            var ketQua = await _baiDangWebService.doiTrangThaiAsync(maBaiDang, trangThai);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái bài đăng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }
            return RedirectToAction("QuanLyTimViec");
        }


        //Tuấn
        // Xóa bài đăng tìm việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaBaiDang(string maBaiDang)
        {
            var ketQua = await _baiDangWebService.xoaBaiDangAsync(maBaiDang);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Đã xóa bài đăng tìm việc!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }
            return RedirectToAction("QuanLyTimViec");
        }


        // Khám phá và Tìm kiếm Freelancer Sinh viên
        [HttpGet]
        public async Task<IActionResult> Search(
            string? tuKhoa,
            string? chuyenNganh,
            string? kyNang,
            double? soSaoMin,
            double? gpaMin,
            int? namThu,
            bool? trangThaiNhanViec,
            string? sapXep)
        {
            var model = await LayDanhSachFreelancerDaLoc(tuKhoa, chuyenNganh, kyNang, soSaoMin, gpaMin, namThu, trangThaiNhanViec, sapXep);
            return View(model);
        }

        //Tuấn
        // Mở form chỉnh sửa bài tìm việc
        [HttpGet]
        public async Task<IActionResult> SuaTinTimViec(string maBaiDang)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var ketQua = await _baiDangWebService.layDanhSachCuaToiAsync(maUser.Value);
            var bai = ketQua.data?.FirstOrDefault(b => b.maBaiDang == maBaiDang);
            if (bai == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài đăng tìm việc!";
                return RedirectToAction("QuanLyTimViec");
            }

            var model = new DangTinTimViecViewModel
            {
                maUser = maUser.Value,
                tieude = bai.tieude,
                mota = bai.mota ?? string.Empty,
                kynang = bai.kynang ?? string.Empty,
                mucGiaTu = bai.mucGiaTu ?? 500000
            };

            ViewBag.MaBaiDang = maBaiDang;
            return View(model);
        }

        //Tuấn
        // Submit lưu chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTinTimViec(string maBaiDang, DangTinTimViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.maUser = maUser.Value;
            if (!ModelState.IsValid)
            {
                ViewBag.MaBaiDang = maBaiDang;
                return View(model);
            }

            var ketQua = await _baiDangWebService.capNhatBaiDangAsync(maBaiDang, model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật bài đăng tìm việc thành công!";
                return RedirectToAction("QuanLyTimViec");
            }

            ModelState.AddModelError(string.Empty, ketQua.message ?? "Cập nhật thất bại.");
            ViewBag.MaBaiDang = maBaiDang;
            return View(model);
        }




        // Tuấn - Hồ sơ cho freelancer xem
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            // 1. Lấy mã User từ Session khi đã đăng nhập
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var tenUser = HttpContext.Session.GetString("hovaten") ?? string.Empty;

            //  Gọi API lấy hồ sơ theo mã User
            var res = await _freelancerStudentWebService.layChiTietHoSoAsync(maUser.Value);

            //  Nếu đã có dữ liệu thì lấy, nếu chưa có (lần đầu tạo hồ sơ) thì tạo mới Model mặc định
            var model = res?.data ?? new ChiTietHoSoFreelancerViewModel
            {
                MaUser = maUser.Value,
                TenUser = tenUser,
                TrangthaiNhanViec = true
            };

            // Nếu NewSkillsInput chưa có nhưng đã có DanhSachKyNang thì gộp thành chuỗi cách nhau dấu phẩy
            if (string.IsNullOrEmpty(model.NewSkillsInput) && model.DanhSachKyNang != null && model.DanhSachKyNang.Any())
            {
                model.NewSkillsInput = string.Join(", ", model.DanhSachKyNang);
            }

            // Kiểm tra trạng thái xác minh thẻ sinh viên để thiết lập khóa các input tương ứng
            var xacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            ViewBag.DaXacMinh = xacThuc?.data?.DaXacMinh == true;
            ViewBag.TrangThaiMinhChung = xacThuc?.data?.TrangThai ?? "ChuaGui";
            ViewBag.ThongBaoMinhChung = xacThuc?.data?.ThongBao ?? string.Empty;

            return View(model);
        }

        //Tuấn
        //Submit lưu hồ sơ khi update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo(ChiTietHoSoFreelancerViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.MaUser = maUser.Value;

            // Kiểm tra trạng thái xác minh thẻ sinh viên
            var xacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            ViewBag.DaXacMinh = xacThuc?.data?.DaXacMinh == true;
            ViewBag.TrangThaiMinhChung = xacThuc?.data?.TrangThai ?? "ChuaGui";
            ViewBag.ThongBaoMinhChung = xacThuc?.data?.ThongBao ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi WebService gửi dữ liệu cập nhật xuống API
            var ketQua = await _freelancerStudentWebService.capNhatHoSoAsync(model);

            if (ketQua.success)
            {
                // Cập nhật lại tên trên Header nếu có đổi họ tên
                if (!string.IsNullOrEmpty(model.TenUser))
                {
                    HttpContext.Session.SetString("hovaten", model.TenUser);
                }

                TempData["SuccessMessage"] = "Cập nhật hồ sơ năng lực thành công!";
                return RedirectToAction("Profile", new { id = model.MaUser });
            }

            TempData["ErrorMessage"] = ketQua.message ?? "Có lỗi xảy ra khi lưu hồ sơ.";
            return View(model);
        }

        private async Task<FreelacerStudentViewModel?> GetCurrentFreelancerAsync(int? maUser)
        {
            if (!maUser.HasValue) return null;
            var result = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            return result.data?.FirstOrDefault(x => x.maUser == maUser.Value);
        }



    }
}
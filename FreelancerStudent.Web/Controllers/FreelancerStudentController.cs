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

        public FreelancerStudentController(IFreelancerStudentWebService freelancerStudentWebService, IJobPostWebService jobPostWebService,
        IBaiDangTimViecWebService baiDangTimViecWebService, IDanhGiaWebService danhGiaWebService)
        {
            _freelancerStudentWebService = freelancerStudentWebService;
            _jobPostWebService = jobPostWebService;
            _baiDangWebService = baiDangTimViecWebService;
            _danhGiaWebService = danhGiaWebService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var layDanhSachFreelancerStudent = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            Console.WriteLine($"===> [SỐ LƯỢNG NTD NHẬN ĐƯỢC]: {layDanhSachFreelancerStudent.data?.Count ?? 0}");

            var dsFree = layDanhSachFreelancerStudent.data ?? new List<FreelancerStudent.Web.ViewModels.FreelacerStudentViewModel>();
            return View(dsFree);
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
        public IActionResult DangTinTimViec()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
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


        //Tuấn
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var ketQua = await _baiDangWebService.layTatCaBaiDangAsync();
            var dsBaiDang = ketQua?.data ?? new List<BaiDangTimViecViewModel>();

            return View(dsBaiDang);
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
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class FreelancerStudentController : Controller
    {
        private readonly IFreelancerStudentWebService _freelancerStudentWebService;

        public FreelancerStudentController(IFreelancerStudentWebService freelancerStudentWebService)
        {
            _freelancerStudentWebService = freelancerStudentWebService;
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

            if (!result.success || result.data == null)
            {
                TempData["Error"] = result.message;
                return NotFound();
            }

            return View(result.data);
        }

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

            return RedirectToAction(nameof(Profile), new { maUser = maUser.Value });
        }

        public async Task<IActionResult> DanhSachNopTuyen()
        {
            return View();
        }

        private async Task<FreelacerStudentViewModel?> GetCurrentFreelancerAsync(int? maUser)
        {
            if (!maUser.HasValue) return null;
            var result = await _freelancerStudentWebService.layDanhSachFreelancerStudentAsync();
            return result.data?.FirstOrDefault(x => x.maUser == maUser.Value);
        }


    }
}
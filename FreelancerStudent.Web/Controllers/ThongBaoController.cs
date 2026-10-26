using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoWebService _service;

        public ThongBaoController(IThongBaoWebService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int trang = 1)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            var result = await _service.LayDanhSachAsync(maUser.Value, trang);
            var model = result.data ?? new ThongBaoDanhSachViewModel();
            if (!result.success)
                model.Loi = "Không tải được thông báo, vui lòng kiểm tra kết nối";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DiDen(int maThongBao, string? link, int trang = 1)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            await _service.DanhDauDaDocAsync(maThongBao, maUser.Value);
            return !string.IsNullOrWhiteSpace(link) && Url.IsLocalUrl(link)
                ? LocalRedirect(link)
                : RedirectToAction(nameof(Index), new { trang });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhDauTatCaDaDoc()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            await _service.DanhDauTatCaDaDocAsync(maUser.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}

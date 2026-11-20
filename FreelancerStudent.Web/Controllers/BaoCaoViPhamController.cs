using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class BaoCaoViPhamController : Controller
    {
        private readonly IBaoCaoViPhamWebService _service;

        public BaoCaoViPhamController(IBaoCaoViPhamWebService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.GetInt32("maUser").HasValue)
                return RedirectToAction("DangNhap", "Account");

            var model = new BaoCaoViPhamTrangViewModel
            {
                Role = HttpContext.Session.GetString("tenRole") ?? string.Empty
            };
            var cases = await _service.LayDanhSachAsync();
            model.BaoCaos = cases.data ?? new List<BaoCaoViPhamViewModel>();
            if (!cases.success) TempData["ErrorMessage"] = cases.message;

            if (model.Role == "FreelancerStudent")
            {
                var score = await _service.LayLichSuDiemUyTinAsync();
                model.DiemUyTin = score.data;
                if (!score.success) TempData["ErrorMessage"] = score.message;
            }
            if (model.Role == "Admin")
            {
                var appeals = await _service.LayDanhSachKhieuNaiAsync();
                model.KhieuNais = appeals.data ?? new List<KhieuNaiViPhamViewModel>();
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (!HttpContext.Session.GetInt32("maUser").HasValue)
                return RedirectToAction("DangNhap", "Account");
            var result = await _service.LayChiTietAsync(id);
            if (!result.success || result.data == null)
            {
                TempData["ErrorMessage"] = result.message ?? "Không thể tải hồ sơ báo cáo.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Role = HttpContext.Session.GetString("tenRole");
            return View(result.data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(TaoBaoCaoViPhamViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung") return Forbid();
            var result = await _service.TaoBaoCaoAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiGiaiTrinh(GiaiTrinhViPhamViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "FreelancerStudent") return Forbid();
            var result = await _service.GuiGiaiTrinhAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(ChiTiet), new { id = model.MaBaoCao });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanHoi(PhanHoiViPhamViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung") return Forbid();
            var result = await _service.PhanHoiAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(ChiTiet), new { id = model.MaBaoCao });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetLuan(KetLuanViPhamViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "Admin") return Forbid();
            var result = await _service.KetLuanAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(ChiTiet), new { id = model.MaBaoCao });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiKhieuNai(GuiKhieuNaiViPhamViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "FreelancerStudent") return Forbid();
            var result = await _service.GuiKhieuNaiAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(ChiTiet), new { id = model.MaBaoCao });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetLuanKhieuNai(KetLuanKhieuNaiViewModel model)
        {
            if (HttpContext.Session.GetString("tenRole") != "Admin") return Forbid();
            var result = await _service.KetLuanKhieuNaiAsync(model);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Index));
        }
    }

    public class BaoCaoViPhamTrangViewModel
    {
        public string Role { get; set; } = string.Empty;
        public List<BaoCaoViPhamViewModel> BaoCaos { get; set; } = new();
        public LichSuDiemUyTinViewModel? DiemUyTin { get; set; }
        public List<KhieuNaiViPhamViewModel> KhieuNais { get; set; } = new();
    }
}

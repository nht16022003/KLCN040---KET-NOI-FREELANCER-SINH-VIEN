using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly IDanhGiaWebService _service;

        public DanhGiaController(IDanhGiaWebService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Form(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return Unauthorized(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
            var result = await _service.LayFormAsync(maHD, maUser.Value);
            if (!result.success || result.data == null)
                return BadRequest(new { success = false, message = result.message });
            return PartialView("_FormDanhGia", result.data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gui([FromBody] DanhGiaRequestViewModel request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
            var result = await _service.GuiAsync(request, maUser.Value);
            return Json(new { result.success, result.message, data = result.data });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(int maDanhGia, [FromBody] DanhGiaRequestViewModel request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
            var result = await _service.CapNhatAsync(maDanhGia, request, maUser.Value);
            return Json(new { result.success, result.message, data = result.data });
        }

        [HttpGet]
        public async Task<IActionResult> CongViecDaHoanThanh()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            var result = await _service.LayCongViecDaThueAsync(maUser.Value);
            var model = new CongViecDaHoanThanhViewModel
            {
                DanhSach = result.data ?? new List<HopDongDanhGiaViewModel>(),
                LaFreelancer = HttpContext.Session.GetString("tenRole") == "FreelancerStudent",
                Loi = result.success ? null : result.message
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Freelancer(int maFreelancerStudent, int trang = 1, int kichThuocTrang = 10)
        {
            var result = await _service.LayFreelancerAsync(maFreelancerStudent, trang, kichThuocTrang);
            if (!result.success || result.data == null)
                return BadRequest(result.message);
            return PartialView("_DanhSachDanhGia", result.data);
        }

        [HttpGet]
        public async Task<IActionResult> NhaTuyenDung(int maNhaTuyenDung, int trang = 1, int kichThuocTrang = 10)
        {
            var result = await _service.LayNhaTuyenDungAsync(maNhaTuyenDung, trang, kichThuocTrang);
            if (!result.success || result.data == null)
                return BadRequest(result.message);
            return PartialView("_DanhSachDanhGia", result.data);
        }
    }
}

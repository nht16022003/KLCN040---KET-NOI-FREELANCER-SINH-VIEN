
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    [Authorize]
    public class BanGiaoSanPhamController : Controller
    {
        private readonly IBanGiaoSanPhamWebService _service;
        private readonly IHopDongWebService _hopDongService;
        private readonly IMinhChungWebService _minhChungWebService;

        public BanGiaoSanPhamController(
            IBanGiaoSanPhamWebService service,
            IHopDongWebService hopDongService,
            IMinhChungWebService minhChungWebService)
        {
            _service = service;
            _hopDongService = hopDongService;
            _minhChungWebService = minhChungWebService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string maHD)
        {
            if (string.IsNullOrWhiteSpace(maHD))
                return BadRequest();

            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");

            var hopDong = await _hopDongService.LayChiTietTheoMaAsync(maHD);

            if (hopDong == null)
                return NotFound();

            bool laFreelancer =
                hopDong.maUserFreelancer == maUser.Value;

            bool laKhachHang =
                hopDong.maUserNhaTuyenDung == maUser.Value;

            if (!laFreelancer && !laKhachHang)
                return Forbid();

            var banGiao = await _service.LayDanhSachAsync(maHD);
            var yeuCau = await _service.LayYeuCauAsync(maHD);

            if (!banGiao.success || !yeuCau.success)
            {
                TempData["ErrorMessage"] =
                    banGiao.message ?? yeuCau.message
                    ?? "Không thể tải dữ liệu bàn giao.";

                return RedirectToAction("ChiTiet", "HopDong",
                    new { maHD });
            }

            var model = new ChiTietBanGiaoViewModel
            {
                MaHD = maHD,
                TrangThaiHopDong = hopDong.trangThai,
                LaFreelancer = laFreelancer,
                LaKhachHang = laKhachHang,
                DanhSachBanGiao = banGiao.data ?? new(),
                DanhSachYeuCau = yeuCau.data ?? new()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoBanGiao(
            TaoBanGiaoViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            var maRole = HttpContext.Session.GetInt32("maRole");
            if (maRole == 1 && maUser.HasValue)
            {
                var check = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
                if (check?.data != null && !check.data.DaXacMinh)
                {
                    TempData["ErrorMessage"] = "Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') để bàn giao sản phẩm hợp đồng!";
                    return RedirectToAction("Index", new { maHD = model.MaHD });
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Thông tin bàn giao không hợp lệ.";

                return RedirectToAction("Index",
                    new { maHD = model.MaHD });
            }

            var ketQua = await _service.TaoBanGiaoAsync(model);

            TempData[ketQua.success
                ? "SuccessMessage"
                : "ErrorMessage"] =
                    ketQua.message ?? "Không thể bàn giao sản phẩm.";

            return RedirectToAction("Index",
                new { maHD = model.MaHD });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiYeuCau(
            TaoYeuCauChinhSuaViewModel model,
            string maHD)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Nội dung yêu cầu chỉnh sửa không hợp lệ.";

                return RedirectToAction("Index", new { maHD });
            }

            var ketQua = await _service.GuiYeuCauAsync(model);

            TempData[ketQua.success
                ? "SuccessMessage"
                : "ErrorMessage"] =
                    ketQua.message ?? "Không thể gửi yêu cầu chỉnh sửa.";

            return RedirectToAction("Index", new { maHD });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NghiemThu(
            int maBanGiao, string maHD)
        {
            var ketQua = await _service.NghiemThuAsync(maBanGiao);

            TempData[ketQua.success
                ? "SuccessMessage"
                : "ErrorMessage"] =
                    ketQua.message ?? "Không thể nghiệm thu.";

            return RedirectToAction("Index", new { maHD });
        }
    }
}

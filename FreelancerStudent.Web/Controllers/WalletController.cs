using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly IGiaoDichNapTienWebService _giaoDichNapTienWebService;
        private readonly IWalletWebService _walletWebService;

        public WalletController(
            IGiaoDichNapTienWebService giaoDichNapTienWebService,
            IWalletWebService walletWebService)
        {
            _giaoDichNapTienWebService = giaoDichNapTienWebService;
            _walletWebService = walletWebService;
        }


        // =====================================================
        // TRANG VÍ & NẠP TIỀN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var ketQua = await _walletWebService.layViTheoUser(maUser.Value);
            var model = ketQua?.data ?? new WalletViewModel();

            return View(model);
        }


        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> TaoGiaoDich(
            [FromBody] TaoGiaoDichNapTienRequest request)
        {
            // Lấy maUser của người đang đăng nhập
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (!maUser.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin người dùng."
                });
            }

            var ketQua =
                await _giaoDichNapTienWebService.taoGiaoDichNapTien(
                    maUser.Value,
                    request
                );

            if (!ketQua.success)
            {
                return Json(new
                {
                    success = false,
                    message = ketQua.message
                });
            }

            return Json(new
            {
                success = true,
                message = ketQua.message,
                data = ketQua.data
            });
        }


        // =====================================================
        // KIỂM TRA TRẠNG THÁI GIAO DỊCH
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> TrangThai(
            string maGiaoDich)
        {
            if (string.IsNullOrEmpty(maGiaoDich))
            {
                return Json(new
                {
                    success = false,
                    message = "Mã giao dịch không hợp lệ."
                });
            }

            var ketQua =
                await _giaoDichNapTienWebService
                    .layTrangThaiGiaoDich(
                        maGiaoDich
                    );

            if (!ketQua.success)
            {
                return Json(new
                {
                    success = false,
                    message = ketQua.message
                });
            }

            return Json(new
            {
                success = true,
                message = ketQua.message,
                data = ketQua.data
            });
        }
    }
}
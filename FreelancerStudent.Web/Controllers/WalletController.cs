using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly IGiaoDichNapTienWebService _giaoDichNapTienWebService;
        private readonly IWalletWebService _walletWebService;
        private readonly IMinhChungWebService _minhChungWebService;

        public WalletController(
            IGiaoDichNapTienWebService giaoDichNapTienWebService,
            IWalletWebService walletWebService,
            IMinhChungWebService minhChungWebService)
        {
            _giaoDichNapTienWebService = giaoDichNapTienWebService;
            _walletWebService = walletWebService;
            _minhChungWebService = minhChungWebService;
        }

        // =====================================================
        // TRANG VÍ, NẠP TIỀN & LỊCH SỬ GIAO DỊCH
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var role = HttpContext.Session.GetString("tenRole");
            if (role == "FreelancerStudent")
            {
                var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
                if (checkXacThuc?.data != null)
                {
                    ViewBag.DaXacMinhSinhVien = checkXacThuc.data.DaXacMinh;
                    ViewBag.TrangThaiXacMinh = checkXacThuc.data.TrangThai;
                    ViewBag.ThongBaoXacMinh = checkXacThuc.data.ThongBao;
                }
            }
            else
            {
                ViewBag.DaXacMinhSinhVien = true;
            }

            var ketQua = await _walletWebService.layViTheoUser(maUser.Value);
            var model = ketQua?.data ?? new WalletViewModel();

            // Tải lịch sử giao dịch
            var historyRes = await _walletWebService.layLichSuGiaoDich(maUser.Value);
            if (historyRes != null && historyRes.success && historyRes.data != null)
            {
                model.LichSuGiaoDichs = historyRes.data;
            }

            // Tải danh sách yêu cầu rút tiền
            var withdrawalRes = await _walletWebService.layDanhSachRutTien(maUser.Value);
            if (withdrawalRes != null && withdrawalRes.success && withdrawalRes.data != null)
            {
                model.DanhSachRutTiens = withdrawalRes.data;
            }

            return View(model);
        }

        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> TaoGiaoDich([FromBody] TaoGiaoDichNapTienRequest request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại."
                });
            }

            var role = HttpContext.Session.GetString("tenRole");
            if (role == "FreelancerStudent")
            {
                var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
                if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') trước khi nạp tiền vào ví!"
                    });
                }
            }

            var ketQua = await _giaoDichNapTienWebService.taoGiaoDichNapTien(maUser.Value, request);

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
        // KIỂM TRA TRẠNG THÁI GIAO DỊCH NẠP TIỀN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> TrangThai(string maGiaoDich)
        {
            if (string.IsNullOrEmpty(maGiaoDich))
            {
                return Json(new
                {
                    success = false,
                    message = "Mã giao dịch không hợp lệ."
                });
            }

            var ketQua = await _giaoDichNapTienWebService.layTrangThaiGiaoDich(maGiaoDich);

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
        // XÁC NHẬN NẠP TIỀN MÔ PHỎNG (DEMO / KIỂM THỬ)
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> XacNhanNapTienMoPhong([FromBody] Dictionary<string, string> request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện." });
            }

            if (request == null || !request.TryGetValue("maGiaoDich", out var maGiaoDich) || string.IsNullOrWhiteSpace(maGiaoDich))
            {
                return Json(new { success = false, message = "Thiếu mã giao dịch cần xác nhận." });
            }

            var ketQua = await _giaoDichNapTienWebService.xacNhanNapTienMoPhong(maGiaoDich, maUser.Value);

            return Json(new
            {
                success = ketQua.success,
                message = ketQua.message,
                data = ketQua.data
            });
        }

        // =====================================================
        // TẠO YÊU CẦU RÚT TIỀN MÔ PHỎNG
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> TaoYeuCauRutTien([FromBody] RutTienRequestModel request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện." });
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                });
            }

            var maRole = HttpContext.Session.GetInt32("maRole");
            if (maRole == 1)
            {
                var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
                if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái 'Đã xác minh') trước khi tạo yêu cầu rút tiền!"
                    });
                }
            }

            var ketQua = await _walletWebService.taoYeuCauRutTien(maUser.Value, request);

            return Json(new
            {
                success = ketQua.success,
                message = ketQua.message,
                data = ketQua.data
            });
        }

        // =====================================================
        // XÁC NHẬN DUYỆT HOẶC HỦY RÚT TIỀN MÔ PHỎNG
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> XacNhanRutTienMoPhong([FromBody] Dictionary<string, object> request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện." });
            }

            if (request == null || !request.TryGetValue("maRutTien", out var maRutTienObj) ||
                !int.TryParse(maRutTienObj?.ToString(), out int maRutTien))
            {
                return Json(new { success = false, message = "Mã yêu cầu rút tiền không hợp lệ." });
            }

            bool chapNhan = true;
            if (request.TryGetValue("chapNhan", out var chapNhanObj) && chapNhanObj != null)
            {
                bool.TryParse(chapNhanObj.ToString(), out chapNhan);
            }

            string? lyDo = null;
            if (request.TryGetValue("lyDo", out var lyDoObj) && lyDoObj != null)
            {
                lyDo = lyDoObj.ToString();
            }

            var ketQua = await _walletWebService.xacNhanRutTienMoPhong(maRutTien, maUser.Value, chapNhan, lyDo);

            return Json(new
            {
                success = ketQua.success,
                message = ketQua.message
            });
        }
    }
}
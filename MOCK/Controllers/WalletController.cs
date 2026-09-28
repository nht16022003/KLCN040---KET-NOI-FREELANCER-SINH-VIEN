using System;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;

namespace MOCK.Controllers
{
    public class WalletController : Controller
    {
        private int CurrentUserId => 1; // Default session demo user

        // =========================================================================
        // CUS-UC-07.01: Xem thông tin tài chính ví (Dashboard ví)
        // =========================================================================
        public IActionResult Index()
        {
            var model = MockDataStore.GetWalletView(CurrentUserId);
            if (model == null)
            {
                return NotFound("Không tìm thấy thông tin tài chính ví.");
            }
            return View(model);
        }

        // =========================================================================
        // CUS-UC-07.02: Xem lịch sử giao dịch (Lọc, tìm kiếm, chi tiết)
        // =========================================================================
        public IActionResult LichSuGiaoDich(string? keyword, string? loaiGiaoDich, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var model = MockDataStore.GetLichSuGiaoDich(CurrentUserId, keyword, loaiGiaoDich, trangThai, tuNgay, denNgay);
            return View(model);
        }

        // =========================================================================
        // CUS-UC-08.01: Nạp tiền vào ví (VietQR, VNPAY, MoMo)
        // =========================================================================
        [HttpGet]
        public IActionResult NapTien(decimal? soTien, string? phuongThuc)
        {
            var model = MockDataStore.GetNapTienView(CurrentUserId, soTien, phuongThuc);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanNapTien(decimal soTien, string phuongThuc, string noiDung, string? maThamChieu)
        {
            if (soTien < 10000)
            {
                TempData["ErrorMessage"] = "Số tiền nạp tối thiểu là 10.000 VNĐ.";
                return RedirectToAction(nameof(NapTien), new { soTien, phuongThuc });
            }

            bool result = MockDataStore.NapTienVaoVi(CurrentUserId, soTien, phuongThuc, noiDung, maThamChieu);
            if (result)
            {
                TempData["SuccessMessage"] = $"Nạp thành công {soTien:N0} VNĐ vào số dư ví khả dụng!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xử lý nạp tiền. Vui lòng thử lại.";
            return RedirectToAction(nameof(NapTien));
        }

        // =========================================================================
        // CUS-UC-08.02: Gửi yêu cầu hỗ trợ nạp tiền (Phiếu ticket tra soát)
        // =========================================================================
        [HttpGet]
        public IActionResult HoTroNapTien()
        {
            var model = MockDataStore.GetHoTroNapTienView(CurrentUserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuiYeuCauHoTroNapTien(decimal soTien, string phuongThuc, string? maGDNganHang, string? ghiChu, string? anhBienLai)
        {
            if (soTien <= 0)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập số tiền hợp lệ cần hỗ trợ tra soát.";
                return RedirectToAction(nameof(HoTroNapTien));
            }

            bool result = MockDataStore.GuiYeuCauHoTroNapTien(CurrentUserId, soTien, phuongThuc, maGDNganHang, ghiChu, anhBienLai);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã gửi phiếu yêu cầu hỗ trợ nạp tiền thành công! Đội ngũ Admin sẽ kiểm tra và cộng tiền trong vòng 15-30 phút.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể gửi yêu cầu hỗ trợ. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(HoTroNapTien));
        }

        // =========================================================================
        // FRL-UC-11: Rút tiền từ ví (YeuCauRutTien)
        // =========================================================================
        [HttpGet]
        public IActionResult RutTien()
        {
            var model = MockDataStore.GetRutTienViewModel(CurrentUserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RutTien(MOCK.Models.ViewModels.RutTienViewModel model)
        {
            if (model.SoTienRut < 50000)
            {
                ModelState.AddModelError("SoTienRut", "Số tiền rút tối thiểu là 50.000 VNĐ.");
                var reloaded = MockDataStore.GetRutTienViewModel(CurrentUserId);
                model.TaiKhoanDaLuus = reloaded.TaiKhoanDaLuus;
                model.LichSuRutTiens = reloaded.LichSuRutTiens;
                model.SoDuKhaDung = reloaded.SoDuKhaDung;
                return View(model);
            }

            bool result = MockDataStore.CreateYeuCauRutTien(model, CurrentUserId);
            if (result)
            {
                TempData["SuccessMessage"] = $"Yêu cầu rút {model.SoTienRut:N0} VNĐ về tài khoản {model.TenNganHang} ({model.SoTaiKhoan}) đã được tiếp nhận và đang chờ xử lý.";
                return RedirectToAction(nameof(RutTien));
            }

            TempData["ErrorMessage"] = "Số dư khả dụng trong ví không đủ để thực hiện yêu cầu rút tiền này.";
            return RedirectToAction(nameof(RutTien));
        }

        // =========================================================================
        // FRL-UC-11: Quản lý tài khoản ngân hàng (TaiKhoanNganHang)
        // =========================================================================
        [HttpGet]
        public IActionResult TaiKhoanNganHang()
        {
            var model = MockDataStore.GetTaiKhoanNganHangList(CurrentUserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTaiKhoanNganHang(MOCK.Models.ViewModels.TaiKhoanNganHangCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var fullModel = MockDataStore.GetTaiKhoanNganHangList(CurrentUserId);
                fullModel.NewAccount = model;
                return View(nameof(TaiKhoanNganHang), fullModel);
            }

            MockDataStore.AddTaiKhoanNganHang(model, CurrentUserId);
            TempData["SuccessMessage"] = $"Đã thêm tài khoản ngân hàng {model.TenNganHang} ({model.SoTaiKhoan}) thành công!";
            return RedirectToAction(nameof(TaiKhoanNganHang));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDefaultTaiKhoanNganHang(int id)
        {
            bool success = MockDataStore.SetDefaultTaiKhoanNganHang(id, CurrentUserId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã thay đổi tài khoản ngân hàng nhận tiền mặc định thành công.";
            }
            return RedirectToAction(nameof(TaiKhoanNganHang));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTaiKhoanNganHang(int id)
        {
            bool success = MockDataStore.DeleteTaiKhoanNganHang(id, CurrentUserId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa tài khoản ngân hàng khỏi danh sách.";
            }
            return RedirectToAction(nameof(TaiKhoanNganHang));
        }
    }
}

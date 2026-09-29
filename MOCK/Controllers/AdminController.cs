using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.Entities;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;

namespace MOCK.Controllers
{
    public class AdminController : Controller
    {
        // ==========================================================
        // 0. TỔNG QUAN QUẢN TRỊ (ADMIN DASHBOARD)
        // ==========================================================
        [HttpGet]
        public IActionResult Index()
        {
            var vm = MockDataStore.GetAdminDashboard();
            return View(vm);
        }

        // ==========================================================
        // AD-UC-01.01: QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG
        // ==========================================================
        [HttpGet]
        public IActionResult DanhSachNguoiDung(string? keyword, int? role, string? status)
        {
            var vm = MockDataStore.GetAdminUserManagement(keyword, role, status);
            return View(vm);
        }

        // ==========================================================
        // AD-UC-02.01: XỬ LÝ TRẠNG THÁI TÀI KHOẢN (KHÓA / MỞ KHÓA)
        // ==========================================================
        [HttpGet]
        public IActionResult XuLyTaiKhoan(int? maUser)
        {
            var allUsers = MockDataStore.GetAdminUserManagement(null, null, null).Users;
            var vm = new AdminAccountStatusViewModel
            {
                DanhSachTaiKhoan = allUsers,
                LichSuXuLys = MockDataStore.LichSuXuLyTaiKhoans.OrderByDescending(x => x.NgayXuLy).ToList(),
                XuLyModel = new AdminXuLyPostModel
                {
                    MaUser = maUser ?? (allUsers.FirstOrDefault()?.MaUser ?? 0),
                    HanhDong = "KHOA_TAI_KHOAN"
                }
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XuLyTaiKhoan(AdminXuLyPostModel model)
        {
            if (!ModelState.IsValid)
            {
                var allUsers = MockDataStore.GetAdminUserManagement(null, null, null).Users;
                var vm = new AdminAccountStatusViewModel
                {
                    DanhSachTaiKhoan = allUsers,
                    LichSuXuLys = MockDataStore.LichSuXuLyTaiKhoans.OrderByDescending(x => x.NgayXuLy).ToList(),
                    XuLyModel = model
                };
                return View(vm);
            }

            var success = MockDataStore.XuLyTrangThaiTaiKhoan(model.MaUser, 1, model.HanhDong, model.LyDo);
            if (success)
            {
                TempData["SuccessMessage"] = model.HanhDong == "KHOA_TAI_KHOAN" 
                    ? $"Đã khóa tài khoản ID #{model.MaUser} thành công!" 
                    : $"Đã mở khóa tài khoản ID #{model.MaUser} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản người dùng tương ứng.";
            }

            return RedirectToAction(nameof(XuLyTaiKhoan), new { maUser = model.MaUser });
        }

        // ==========================================================
        // AD-UC-03.01: XỬ LÝ YÊU CẦU NẠP TIỀN
        // ==========================================================
        [HttpGet]
        public IActionResult YeuCauNapTien(string? status)
        {
            var vm = MockDataStore.GetAdminDeposits(status);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuyetNapTien(int id)
        {
            var success = MockDataStore.DuyetYeuCauNap(id, 1);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã duyệt thành công yêu cầu nạp tiền #{id} và cộng số dư vào ví người dùng!";
            }
            else
            {
                TempData["ErrorMessage"] = "Yêu cầu nạp tiền không hợp lệ hoặc đã được xử lý trước đó.";
            }
            return RedirectToAction(nameof(YeuCauNapTien));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TuChoiNapTien(int id, string lyDo)
        {
            var success = MockDataStore.TuChoiYeuCauNap(id, 1, lyDo);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã từ chối yêu cầu nạp tiền #{id}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể từ chối yêu cầu nạp tiền này.";
            }
            return RedirectToAction(nameof(YeuCauNapTien));
        }

        // ==========================================================
        // AD-UC-04.01: ĐIỀU CHỈNH SỐ DƯ TÀI KHOẢN
        // ==========================================================
        [HttpGet]
        public IActionResult DieuChinhSoDu(int? maWallet)
        {
            var vm = MockDataStore.GetAdminBalanceAdjustment();
            if (maWallet.HasValue)
            {
                vm.FormModel.MaWallet = maWallet.Value;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DieuChinhSoDu(AdminAdjustmentPostModel formModel)
        {
            if (!ModelState.IsValid)
            {
                var vm = MockDataStore.GetAdminBalanceAdjustment();
                vm.FormModel = formModel;
                return View(vm);
            }

            var success = MockDataStore.ThucHienDieuChinhSoDu(formModel.MaWallet, 1, formModel.LoaiDieuChinh, formModel.SoTien, formModel.LyDo);
            if (success)
            {
                TempData["SuccessMessage"] = $"Điều chỉnh số dư ví #{formModel.MaWallet} thành công ({(formModel.LoaiDieuChinh == "CongTien" ? "+" : "-")}{formModel.SoTien:N0} VNĐ)!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thực hiện điều chỉnh số dư (ví không tồn tại hoặc số dư không đủ).";
            }

            return RedirectToAction(nameof(DieuChinhSoDu), new { maWallet = formModel.MaWallet });
        }

        // ==========================================================
        // AD-UC-05.01: XỬ LÝ THANH TOÁN GIAO DỊCH (ESCROW)
        // ==========================================================
        [HttpGet]
        public IActionResult QuanLyGiaoDich()
        {
            var vm = MockDataStore.GetAdminTransactionManagement();
            return View(vm);
        }

        // ==========================================================
        // AD-UC-06.01: QUẢN LÝ PHÍ, HOA HỒNG & DOANH THU
        // ==========================================================
        [HttpGet]
        public IActionResult CauHinhPhi()
        {
            var vm = MockDataStore.GetAdminFeeCommission();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatPhiVaHoaHong(decimal phiDangBai, decimal phanTramHoaHong, string? moTa)
        {
            MockDataStore.CapNhatPhiVaHoaHong(phiDangBai, phanTramHoaHong, 1, moTa ?? "Cập nhật phí và hoa hồng qua giao diện Quản trị");
            TempData["SuccessMessage"] = "Đã cập nhật biểu phí đăng bài và tỷ lệ hoa hồng mới thành công!";
            return RedirectToAction(nameof(CauHinhPhi));
        }

        // ==========================================================
        // AD-UC-07.01: XỬ LÝ TRANH CHẤP
        // ==========================================================
        [HttpGet]
        public IActionResult XuLyTranhChap(int? maTranhChap)
        {
            var vm = MockDataStore.GetAdminDisputes();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhanXuTranhChap(int maTranhChap, string ketQua, decimal hoanTienCus, decimal traFRL)
        {
            var success = MockDataStore.GiaiQuyetTranhChap(maTranhChap, 1, ketQua, hoanTienCus, traFRL);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã ban hành phán quyết và giải quyết tranh chấp #{maTranhChap} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xử lý tranh chấp (tranh chấp không tồn tại hoặc đã giải quyết).";
            }
            return RedirectToAction(nameof(XuLyTranhChap));
        }

        // ==========================================================
        // AD-UC-08.01: XỬ LÝ YÊU CẦU HỖ TRỢ
        // ==========================================================
        [HttpGet]
        public IActionResult XuLyHoTro(string? status)
        {
            var vm = MockDataStore.GetAdminSupportTickets(status);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhanHoiHoTro(int maYeuCau, string phanHoi, string trangThai)
        {
            var success = MockDataStore.PhanHoiYeuCauHoTro(maYeuCau, 1, phanHoi, trangThai);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã gửi phản hồi và cập nhật trạng thái yêu cầu #{maYeuCau} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu hỗ trợ.";
            }
            return RedirectToAction(nameof(XuLyHoTro));
        }

        // ==========================================================
        // AD-UC-09.01: GIÁM SÁT HOẠT ĐỘNG HỆ THỐNG
        // ==========================================================
        [HttpGet]
        public IActionResult GiamSatHeThong()
        {
            var vm = MockDataStore.GetAdminSystemMonitoring();
            return View(vm);
        }

        // ==========================================================
        // AD-UC-10: QUẢN LÝ HỢP ĐỒNG (MẪU HỢP ĐỒNG & CHÍNH SÁCH)
        // ==========================================================
        [HttpGet]
        public IActionResult QuanLyHopDong()
        {
            var vm = MockDataStore.GetAdminContractTemplate();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatMauHopDong(string tieuDe, string dieuKhoan, int thoiHan, decimal tyLe)
        {
            MockDataStore.CapNhatMauHopDong(tieuDe, dieuKhoan, thoiHan, tyLe);
            TempData["SuccessMessage"] = "Đã lưu và áp dụng bản mẫu hợp đồng cùng chính sách nghiệm thu mới!";
            return RedirectToAction(nameof(QuanLyHopDong));
        }

        // ==========================================================
        // AD: XEM CHI TIẾT HỢP ĐỒNG GIỮA FREELANCERSTUDENT VÀ NHATUYENDUNG
        // ==========================================================
        [HttpGet]
        public IActionResult ChiTietHopDong(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(QuanLyHopDong));
            }

            var contractDetail = MockDataStore.GetContractDetail(id);
            if (contractDetail == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy hợp đồng #{id} trong hệ thống.";
                return RedirectToAction(nameof(QuanLyHopDong));
            }

            return View(contractDetail);
        }

        // ==========================================================
        // AD-UC-11: QUẢN LÝ PHÍ ĐĂNG BÀI
        // ==========================================================
        [HttpGet]
        public IActionResult QuanLyPhiDangBai()
        {
            var vm = MockDataStore.GetAdminJobPostingFee();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatPhiDangBai(decimal phiDangBaiMoi, string? ghiChu)
        {
            MockDataStore.CapNhatPhiDangBai(phiDangBaiMoi, 1, ghiChu ?? "Cập nhật phí đăng bài dự án");
            TempData["SuccessMessage"] = $"Đã cập nhật mức phí đăng bài thành {phiDangBaiMoi:N0} VNĐ thành công!";
            return RedirectToAction(nameof(QuanLyPhiDangBai));
        }
    }
}

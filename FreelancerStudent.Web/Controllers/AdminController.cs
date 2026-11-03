using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminWebService _adminService;
        private readonly IMinhChungWebService _minhChungService;

        public AdminController(IAdminWebService adminService, IMinhChungWebService minhChungService)
        {
            _adminService = adminService;
            _minhChungService = minhChungService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var tenRole = HttpContext.Session.GetString("tenRole");
            bool isAdmin = string.Equals(tenRole, "Admin", StringComparison.OrdinalIgnoreCase) ||
                           (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"));

            if (!isAdmin)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào phân hệ Quản trị. Vui lòng đăng nhập với tài khoản Admin.";
                context.Result = new RedirectToActionResult("DangNhap", "Account", null);
            }
        }

        private int GetCurrentAdminId()
        {
            return HttpContext.Session.GetInt32("maUser") ?? 5;
        }

        // ==========================================================
        // 1. TỔNG QUAN QUẢN TRỊ
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var res = await _adminService.LayDashboardAsync();
            var vm = res.success && res.data != null ? res.data : new AdminDashboardViewModel();
            return View(vm);
        }

        // ==========================================================
        // 2. QUẢN LÝ NGƯỜI DÙNG
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> DanhSachNguoiDung(string? keyword, int? role, string? status)
        {
            var res = await _adminService.LayDanhSachUsersAsync(keyword, role, status);
            var users = res.success && res.data != null ? res.data : new List<AdminUserItemViewModel>();

            var vm = new AdminUserManagementViewModel
            {
                Users = users,
                Keyword = keyword,
                SelectedRole = role,
                SelectedStatus = status,
                SoSinhVien = users.Count(u => u.MaRole == 1),
                SoNhaTuyenDung = users.Count(u => u.MaRole == 2),
                SoBiKhoa = users.Count(u => u.Status == "LOCKED" || u.Status == "BiKhoa")
            };

            return View(vm);
        }

        // ==========================================================
        // 3. KHÓA VÀ MỞ KHÓA TÀI KHOẢN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> XuLyTaiKhoan(int? maUser)
        {
            var userRes = await _adminService.LayDanhSachUsersAsync(null, null, null);
            var allUsers = userRes.success && userRes.data != null ? userRes.data : new List<AdminUserItemViewModel>();

            var historyRes = await _adminService.LayLichSuXuLyUserAsync(maUser);
            var histories = historyRes.success && historyRes.data != null ? historyRes.data : new List<AdminLichSuXuLyViewModel>();

            var vm = new AdminAccountStatusViewModel
            {
                DanhSachTaiKhoan = allUsers,
                LichSuXuLys = histories,
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
        public async Task<IActionResult> XuLyTaiKhoan(AdminXuLyPostModel model)
        {
            if (!ModelState.IsValid)
            {
                var userRes = await _adminService.LayDanhSachUsersAsync(null, null, null);
                var allUsers = userRes.success && userRes.data != null ? userRes.data : new List<AdminUserItemViewModel>();

                var historyRes = await _adminService.LayLichSuXuLyUserAsync(model.MaUser);
                var histories = historyRes.success && historyRes.data != null ? historyRes.data : new List<AdminLichSuXuLyViewModel>();

                var vm = new AdminAccountStatusViewModel
                {
                    DanhSachTaiKhoan = allUsers,
                    LichSuXuLys = histories,
                    XuLyModel = model
                };
                return View(vm);
            }

            var adminId = GetCurrentAdminId();
            var res = await _adminService.XuLyTrangThaiUserAsync(model, adminId);
            if (res.success)
            {
                TempData["SuccessMessage"] = model.HanhDong == "KHOA_TAI_KHOAN"
                    ? $"Đã khóa tài khoản ID #{model.MaUser} thành công!"
                    : $"Đã mở khóa tài khoản ID #{model.MaUser} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể xử lý tài khoản này.";
            }

            return RedirectToAction(nameof(XuLyTaiKhoan), new { maUser = model.MaUser });
        }

        // ==========================================================
        // 4. ĐIỀU CHỈNH SỐ DƯ VÍ
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> DieuChinhSoDu(int? maWallet)
        {
            var walletsRes = await _adminService.LayDanhSachWalletsAsync();
            var wallets = walletsRes.success && walletsRes.data != null ? walletsRes.data : new List<AdminWalletItemViewModel>();

            var historyRes = await _adminService.LayLichSuDieuChinhViAsync(maWallet);
            var histories = historyRes.success && historyRes.data != null ? historyRes.data : new List<AdminLichSuDieuChinhViViewModel>();

            var vm = new AdminBalanceAdjustmentViewModel
            {
                Wallets = wallets,
                LichSuDieuChinhs = histories,
                FormModel = new AdminAdjustmentPostModel
                {
                    MaWallet = maWallet ?? (wallets.FirstOrDefault()?.MaWallet ?? 0),
                    LoaiDieuChinh = "CongTien",
                    SoTien = 100000
                }
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DieuChinhSoDu(AdminAdjustmentPostModel formModel)
        {
            if (!ModelState.IsValid)
            {
                var walletsRes = await _adminService.LayDanhSachWalletsAsync();
                var wallets = walletsRes.success && walletsRes.data != null ? walletsRes.data : new List<AdminWalletItemViewModel>();

                var historyRes = await _adminService.LayLichSuDieuChinhViAsync(formModel.MaWallet);
                var histories = historyRes.success && historyRes.data != null ? historyRes.data : new List<AdminLichSuDieuChinhViViewModel>();

                var vm = new AdminBalanceAdjustmentViewModel
                {
                    Wallets = wallets,
                    LichSuDieuChinhs = histories,
                    FormModel = formModel
                };
                return View(vm);
            }

            var adminId = GetCurrentAdminId();
            var res = await _adminService.DieuChinhSoDuViAsync(formModel, adminId);
            if (res.success)
            {
                TempData["SuccessMessage"] = $"Điều chỉnh số dư ví #{formModel.MaWallet} thành công ({(formModel.LoaiDieuChinh == "CongTien" ? "+" : "-")}{formModel.SoTien:N0} VNĐ)!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể thực hiện điều chỉnh số dư.";
            }

            return RedirectToAction(nameof(DieuChinhSoDu), new { maWallet = formModel.MaWallet });
        }

        // ==========================================================
        // 5. QUẢN LÝ HỢP ĐỒNG & MẪU HỢP ĐỒNG
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> QuanLyHopDong()
        {
            var templateRes = await _adminService.LayMauHopDongAsync();
            var contractsRes = await _adminService.LayDanhSachHopDongAsync();

            var vm = templateRes.success && templateRes.data != null
                ? templateRes.data
                : new AdminContractTemplateViewModel();

            vm.DanhSachHopDongHeThong = contractsRes.success && contractsRes.data != null
                ? contractsRes.data
                : new List<AdminHopDongItemViewModel>();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatMauHopDong(string tieuDe, string dieuKhoan, int thoiHan, decimal tyLe)
        {
            var model = new AdminContractTemplateViewModel
            {
                TenMauHopDong = tieuDe,
                DieuKhoanChung = dieuKhoan,
                ThoiHanNghiemThuNgay = thoiHan,
                TyLeGiaiNganMacDinh = tyLe
            };

            var res = await _adminService.CapNhatMauHopDongAsync(model);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Đã lưu và áp dụng bản mẫu hợp đồng cùng chính sách nghiệm thu mới!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Cập nhật mẫu hợp đồng thất bại.";
            }

            return RedirectToAction(nameof(QuanLyHopDong));
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietHopDong(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(QuanLyHopDong));
            }

            var res = await _adminService.LayChiTietHopDongAsync(id);
            if (!res.success || res.data == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy hợp đồng #{id} trong hệ thống.";
                return RedirectToAction(nameof(QuanLyHopDong));
            }

            return View(res.data);
        }

        // ==========================================================
        // 6. DUYỆT YÊU CẦU RÚT TIỀN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> DuyetRutTien(string? trangThai)
        {
            var filter = string.IsNullOrWhiteSpace(trangThai) ? "ChoXuLy" : trangThai.Trim();

            var allRes = await _adminService.LayDanhSachYeuCauRutTienAsync("TatCa");
            var allList = allRes.success && allRes.data != null ? allRes.data : new List<AdminYeuCauRutTienItemViewModel>();

            var filteredList = filter == "TatCa" 
                ? allList 
                : allList.Where(x => x.TrangThai == filter).ToList();

            var vm = new AdminDuyetRutTienViewModel
            {
                DanhSachYeuCau = filteredList,
                CurrentFilter = filter,
                TongSoChoXuLy = allList.Count(x => x.TrangThai == "ChoXuLy"),
                TongSoHoanThanh = allList.Count(x => x.TrangThai == "HoanThanh"),
                TongSoTuChoi = allList.Count(x => x.TrangThai == "TuChoi"),
                TongTienChoXuLy = allList.Where(x => x.TrangThai == "ChoXuLy").Sum(x => x.SoTienRut)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyRutTien(AdminXuLyRutTienPostModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu xử lý không hợp lệ.";
                return RedirectToAction(nameof(DuyetRutTien));
            }

            var adminId = GetCurrentAdminId();
            var res = await _adminService.XuLyRutTienAsync(model, adminId);
            if (res.success)
            {
                TempData["SuccessMessage"] = model.ChapNhan
                    ? $"Duyệt giải ngân thành công cho yêu cầu rút tiền #{model.MaRutTien}!"
                    : $"Đã từ chối yêu cầu rút tiền #{model.MaRutTien} và hoàn trả tiền về ví của người dùng!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể xử lý yêu cầu rút tiền này.";
            }

            return RedirectToAction(nameof(DuyetRutTien));
        }

        // ==========================================================
        // 7. DUYỆT MINH CHỨNG THẺ SINH VIÊN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> DuyetMinhChung()
        {
            var res = await _minhChungService.LayDanhSachChoDuyetAsync();
            var list = res.success && res.data != null ? res.data : new List<MinhChungItemViewModel>();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyMinhChung(PheDuyetMinhChungRequestViewModel model)
        {
            if (model.MaMinhChung <= 0)
            {
                TempData["ErrorMessage"] = "Mã minh chứng không hợp lệ.";
                return RedirectToAction(nameof(DuyetMinhChung));
            }

            model.NguoiXacMinh = GetCurrentAdminId();
            var res = await _minhChungService.PheDuyetMinhChungAsync(model);

            if (res.success)
            {
                TempData["SuccessMessage"] = model.ChapNhan
                    ? $"Đã phê duyệt thẻ sinh viên #{model.MaMinhChung} thành công và tự động đồng bộ hồ sơ sinh viên!"
                    : $"Đã từ chối minh chứng #{model.MaMinhChung}.";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể xử lý minh chứng này.";
            }

            return RedirectToAction(nameof(DuyetMinhChung));
        }

        // ==========================================================
        // 8. QUẢN LÝ CẤU HÌNH PHÍ SÀN & PHÍ ĐĂNG TIN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> CauHinhHeThong()
        {
            var res = await _adminService.LayDanhSachCauHinhPhiAsync();
            var list = res.success && res.data != null ? res.data : new List<AdminCauHinhPhiViewModel>();

            var phiDangBai = list.FirstOrDefault(c => c.TenCauHinh == "PhiDangBai")?.GiaTri ?? 0m;

            var dashRes = await _adminService.LayDashboardAsync();
            var tongDoanhThuPhi = dashRes.success && dashRes.data != null ? dashRes.data.TongDoanhThuPhiDangBai : 0m;

            var vm = new AdminQuanLyCauHinhViewModel
            {
                DanhSachCauHinh = list,
                FormCapNhat = new AdminCapNhatPhiDangBaiViewModel
                {
                    PhiDangBai = phiDangBai,
                    MaAdmin = GetCurrentAdminId()
                },
                TongDoanhThuPhiDangBai = tongDoanhThuPhi
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatPhiDangBai(AdminCapNhatPhiDangBaiViewModel model)
        {
            if (model.PhiDangBai < 0)
            {
                TempData["ErrorMessage"] = "Mức phí đăng bài không được nhỏ hơn 0 VNĐ.";
                return RedirectToAction(nameof(CauHinhHeThong));
            }

            model.MaAdmin = GetCurrentAdminId();
            var res = await _adminService.CapNhatPhiDangBaiAsync(model);

            if (res.success)
            {
                TempData["SuccessMessage"] = $"Cập nhật mức phí đăng bài thành công: {model.PhiDangBai:N0} VNĐ!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể cập nhật cấu hình phí.";
            }

            return RedirectToAction(nameof(CauHinhHeThong));
        }
    }
}
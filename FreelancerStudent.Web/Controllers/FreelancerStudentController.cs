using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class FreelancerStudentController : Controller
    {
        private readonly IFreelancerStudentWebService _freelancerStudentWebService;

        private readonly IJobPostWebService _jobPostWebService;

        private readonly IBaiDangTimViecWebService _baiDangWebService;

        public FreelancerStudentController(IFreelancerStudentWebService freelancerStudentWebService, IJobPostWebService jobPostWebService,
        IBaiDangTimViecWebService baiDangTimViecWebService)
        {
            _freelancerStudentWebService = freelancerStudentWebService;
            _jobPostWebService = jobPostWebService;
            _baiDangWebService = baiDangTimViecWebService;
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
        public async Task<IActionResult> Profile(int maUser)
        {
            if (maUser <= 0)
            {
                return NotFound();
            }

            var listResult = await _freelancerStudentWebService
                .layDanhSachFreelancerStudentAsync();
            var freelancer = listResult.data?.FirstOrDefault(x => x.maUser == maUser);

            if (freelancer == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ freelancer của tài khoản đang đăng nhập.";
                return NotFound();
            }

            var result = await _freelancerStudentWebService
                .layProfileFreelancerStudentAsync(freelancer.maFreelancerStudents);

            if (!result.success || result.data == null)
            {
                TempData["Error"] = result.message;
                return NotFound();
            }

            ViewBag.LaChinhMinh = HttpContext.Session.GetInt32("maUser") == maUser;

            return View(result.data);
        }


        //Tuấn
        [HttpGet]

        public async Task<IActionResult> DanhSachNopTuyen()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");


            }

            //Lấy danh sách các đơn ứng tuyển của user này
            var kq = await _jobPostWebService.layDSUngTuyenTheoMaUser(maUser.Value);
            var dsUngTuyen = kq?.data ?? new List<UngTuyenViewModel>();
            return View(dsUngTuyen);
        }

        //Tuấn
        //Mở trang đăng tin tìm việc
        [HttpGet]
        public IActionResult DangTinTimViec()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            return View(new DangTinTimViecViewModel { maUser = maUser.Value });
        }

        //Tuấn
        // Submit form Đăng tin tìm việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangTinTimViec(DangTinTimViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            model.maUser = maUser.Value;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var ketQua = await _baiDangWebService.taoBaiDangAsync(model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Đăng tin tìm việc thành công! Nhà tuyển dụng có thể xem hồ sơ dịch vụ của bạn.";
                return RedirectToAction("QuanLyTimViec");
            }
            ModelState.AddModelError(string.Empty, ketQua.message ?? "Đăng tin thất bại.");
            return View(model);
        }

        //Tuấn
        //Quản lý danh sách các bài đăng tìm việc của sinh viên
        [HttpGet]
        public async Task<IActionResult> QuanLyTimViec()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            var ketQua = await _baiDangWebService.layDanhSachCuaToiAsync(maUser.Value);
            var dsBaiDang = ketQua?.data ?? new List<BaiDangTimViecViewModel>();
            return View(dsBaiDang);
        }

        //Tuấn
        //Bật / Tắt trạng thái ẩn bài đăng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThaiBaiDang(string maBaiDang, string trangThai)
        {
            var ketQua = await _baiDangWebService.doiTrangThaiAsync(maBaiDang, trangThai);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái bài đăng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }
            return RedirectToAction("QuanLyTimViec");
        }


        //Tuấn
        // Xóa bài đăng tìm việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaBaiDang(string maBaiDang)
        {
            var ketQua = await _baiDangWebService.xoaBaiDangAsync(maBaiDang);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Đã xóa bài đăng tìm việc!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }
            return RedirectToAction("QuanLyTimViec");
        }


        //Tuấn
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var ketQua = await _baiDangWebService.layTatCaBaiDangAsync();
            var dsBaiDang = ketQua?.data ?? new List<BaiDangTimViecViewModel>();

            return View(dsBaiDang);
        }

        //Tuấn
        // Mở form chỉnh sửa bài tìm việc
        [HttpGet]
        public async Task<IActionResult> SuaTinTimViec(string maBaiDang)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var ketQua = await _baiDangWebService.layDanhSachCuaToiAsync(maUser.Value);
            var bai = ketQua.data?.FirstOrDefault(b => b.maBaiDang == maBaiDang);
            if (bai == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài đăng tìm việc!";
                return RedirectToAction("QuanLyTimViec");
            }

            var model = new DangTinTimViecViewModel
            {
                maUser = maUser.Value,
                tieude = bai.tieude,
                mota = bai.mota ?? string.Empty,
                kynang = bai.kynang ?? string.Empty,
                mucGiaTu = bai.mucGiaTu ?? 500000
            };

            ViewBag.MaBaiDang = maBaiDang;
            return View(model);
        }

        //Tuấn
        // Submit lưu chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTinTimViec(string maBaiDang, DangTinTimViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.maUser = maUser.Value;
            if (!ModelState.IsValid)
            {
                ViewBag.MaBaiDang = maBaiDang;
                return View(model);
            }

            var ketQua = await _baiDangWebService.capNhatBaiDangAsync(maBaiDang, model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật bài đăng tìm việc thành công!";
                return RedirectToAction("QuanLyTimViec");
            }

            ModelState.AddModelError(string.Empty, ketQua.message ?? "Cập nhật thất bại.");
            ViewBag.MaBaiDang = maBaiDang;
            return View(model);
        }




        // Tuấn - Hồ sơ cho freelancer xem
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            // 1. Lấy mã User từ Session khi đã đăng nhập
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var tenUser = HttpContext.Session.GetString("hovaten") ?? string.Empty;

            //  Gọi API lấy hồ sơ theo mã User
            var res = await _freelancerStudentWebService.layChiTietHoSoAsync(maUser.Value);

            //  Nếu đã có dữ liệu thì lấy, nếu chưa có (lần đầu tạo hồ sơ) thì tạo mới Model mặc định
            var model = res?.data ?? new ChiTietHoSoFreelancerViewModel
            {
                MaUser = maUser.Value,
                TenUser = tenUser,
                TrangthaiNhanViec = true
            };

            // Nếu NewSkillsInput chưa có nhưng đã có DanhSachKyNang thì gộp thành chuỗi cách nhau dấu phẩy
            if (string.IsNullOrEmpty(model.NewSkillsInput) && model.DanhSachKyNang != null && model.DanhSachKyNang.Any())
            {
                model.NewSkillsInput = string.Join(", ", model.DanhSachKyNang);
            }

            return View(model);
        }

        //Tuấn
        //Submit lưu hồ sơ khi update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo(ChiTietHoSoFreelancerViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.MaUser = maUser.Value;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi WebService gửi dữ liệu cập nhật xuống API
            var ketQua = await _freelancerStudentWebService.capNhatHoSoAsync(model);

            if (ketQua.success)
            {
                // Cập nhật lại tên trên Header nếu có đổi họ tên
                if (!string.IsNullOrEmpty(model.TenUser))
                {
                    HttpContext.Session.SetString("hovaten", model.TenUser);
                }

                TempData["SuccessMessage"] = "Cập nhật hồ sơ năng lực thành công!";
                return RedirectToAction("Profile", new { id = model.MaUser });
            }

            TempData["ErrorMessage"] = ketQua.message ?? "Có lỗi xảy ra khi lưu hồ sơ.";
            return View(model);
        }



    }
}
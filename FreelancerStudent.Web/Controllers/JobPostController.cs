using System.Security.Claims;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class JobPostController : Controller
    {
        private readonly IJobPostWebService _jobPostService; //Khai báo để sử dụng serviceswweb

        private readonly INhaTuyenDungWebService _nhaTuyenDungService;

        public JobPostController(IJobPostWebService jobPostWebService, INhaTuyenDungWebService nhaTuyenDungWebService)
        {
            _jobPostService = jobPostWebService;
            _nhaTuyenDungService = nhaTuyenDungWebService;
        }

        public async Task<IActionResult> Index()
        {
            var layDanhSachJobPost = await _jobPostService.layDanhSachJobPost();


            Console.WriteLine($"===> [SỐ LƯỢNG NTD NHẬN ĐƯỢC]: {layDanhSachJobPost.data?.Count ?? 0}");

            var dsJobPost = layDanhSachJobPost.data ?? new List<FreelancerStudent.Web.ViewModels.JobPostViewModel>();
            return View(dsJobPost);
        }

        //XS
        [HttpGet]
        public async Task<IActionResult> Detail(string maJob)
        {
            if (string.IsNullOrWhiteSpace(maJob))
            {
                return NotFound();
            }

            var ketQua = await _jobPostService.layChiTietJobPost(maJob);

            if (!ketQua.success || ketQua.data == null)
            {
                if (!string.IsNullOrWhiteSpace(ketQua.message))
                {
                    TempData["Error"] = ketQua.message;
                }

                return NotFound();
            }

            return View(ketQua.data);
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            //Nếu chưa đăng ký thì trả về View DangKy với
            //model là DangKyViewModel
            return View(new DangKyViewModel());
            //new DangKyViewModel() tức là tạo object để truyền dữ liệu từ Controller -> View
            //Trong view tương ứng sẽ @model DangKyViewModel
            //Sau đó có thể sửa dụng @Model.hovaten
        }



        //Tuấn Anh

        [HttpGet]
        public IActionResult Create()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var model = new JobPostViewModel
            {
                maUser = maUser.Value,

                // Mặc định 1 sinh viên
                soluongtuyen = 1,

                // Tạm thời phí đăng bài = 0
                phiDangBai = 0
            };

            return View(model);
        }


        //Tuấn Anh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPostViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Không lấy maUser từ HTML để tránh người dùng sửa ID
            model.maUser = maUser.Value;

            if (model.thoigiandukienhoanthanh <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.thoigiandukienhoanthanh),
                    "Hạn hoàn thành phải lớn hơn ngày hiện tại."
                );
            }

            if (!ModelState.IsValid)
            {
                model.phiDangBai = 0;
                return View(model);
            }

            var ketQua = await _jobPostService.taoJobPost(model);

            if (ketQua.success)
            {
                TempData["Success"] = "Đăng tin tuyển dụng thành công!";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(
                string.Empty,
                ketQua.message ?? "Không thể đăng tin tuyển dụng."
            );

            model.phiDangBai = 0;

            return View(model);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> DanhSachUngTuyen()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");

            }
            var ketqua = await _jobPostService.layDSUngTuyenTheoMaUser(maUser.Value);

            var dsUngTuyen = ketqua.data ?? new List<UngTuyenViewModel>();

            return View(dsUngTuyen);
        }

        //Tuấn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetUngTuyen(int maUngTuyen, string trangThai)
        {
            var ketQua = await _jobPostService.duyetUngTuyenAsync(maUngTuyen, trangThai);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = ketQua.message;
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }
            // Tải lại trang Danh sách ứng tuyển
            return RedirectToAction("DanhSachUngTuyen");
        }

        //Tuan
        [HttpGet]
        public async Task<IActionResult> UngTuyen(string maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            //Lay thong tin bai jobpost

            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc này!";
                return RedirectToAction("Index");
            }

            // Lấy danh sách Nhà tuyển dụng để tìm người đăng bài Job này
            var dsNTD = await _nhaTuyenDungService.layDanhSachNhaTuyenDungAsync();
            var ntd = dsNTD.data?.FirstOrDefault(n => n.maNhaTuyenDung == job.maNhaTuyenDung);

            var model = new UngTuyenViewModel
            {
                maJob = job.maJob,
                maUser = maUser.Value,
                tieude = job.tieude,
                mota = job.mota,
                thulao = job.thulao,
                kynangyeucau = job.kynangyeucau,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                soluongtuyen = job.soluongtuyen,

                // Điền sẵn giá gốc của bài Job vào ô đề xuất
                thulaoDeXuat = job.thulao,
                // Thông tin Nhà tuyển dụng
                maNhaTuyenDung = job.maNhaTuyenDung,
                tenCongTy = !string.IsNullOrWhiteSpace(ntd?.tencongty) ? ntd.tencongty : ntd?.hotenUser,
                avatarNhaTuyenDung = ntd?.logo ?? ntd?.avatar
            };
            return View(model);
        }


        //Tuan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UngTuyen(UngTuyenViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            model.maUser = maUser.Value;

            var ketQua = await _jobPostService.nopDonUngTuyenAsync(model);

            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Ứng tuyển thành công!";
                // Chuyển hướng về trang Danh sách bài đã nộp của sinh viên
                return RedirectToAction("DanhSachNopTuyen", "FreelancerStudent");
            }



            ModelState.AddModelError(string.Empty, ketQua.message ?? "Có lỗi xảy ra khi nộp đơn ứng tuyển.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> QuanLyTinTuyenDung()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var ketQua = await _jobPostService.layJobCuaToiAsync(maUser.Value);
            var dsJob = ketQua?.data ?? new List<JobPostViewModel>();

            return View(dsJob);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> Edit(string maJob)
        {
            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài tuyển dụng!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }
            return View(job);
        }
        //Tuấn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobPostViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ketQua = await _jobPostService.capNhatJobPostAsync(model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật bài tuyển dụng thành công!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            ModelState.AddModelError(string.Empty, ketQua.message ?? "Cập nhật thất bại.");
            return View(model);
        }

        // Đổi trạng thái tin tuyển dụng (Đang tuyển <-> Đã đóng / Tạm dừng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(string maJob, string trangThaiMoi)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(maJob))
            {
                TempData["ErrorMessage"] = "Mã bài tuyển dụng không hợp lệ.";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài tuyển dụng!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            job.status = trangThaiMoi;
            var ketQua = await _jobPostService.capNhatJobPostAsync(job);

            if (ketQua.success)
            {
                TempData["SuccessMessage"] = trangThaiMoi == "DangTuyen" 
                    ? "Đã mở lại tin tuyển dụng thành công!" 
                    : "Đã đóng tin tuyển dụng!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message ?? "Không thể cập nhật trạng thái tin.";
            }

            return RedirectToAction("QuanLyTinTuyenDung");
        }


    }
}
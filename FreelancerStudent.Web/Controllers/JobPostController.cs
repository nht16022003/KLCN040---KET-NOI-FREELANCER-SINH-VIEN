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

        public JobPostController(IJobPostWebService jobPostWebService)
        {
            _jobPostService = jobPostWebService;
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
    }
}
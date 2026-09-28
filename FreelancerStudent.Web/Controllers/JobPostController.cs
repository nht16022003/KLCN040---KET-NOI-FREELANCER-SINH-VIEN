using System.Security.Claims;
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


    }
}
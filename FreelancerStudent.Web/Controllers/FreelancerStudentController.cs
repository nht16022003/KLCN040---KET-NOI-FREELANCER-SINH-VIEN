using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class FreelancerStudentController : Controller
    {
        private readonly IFreelancerStudentWebService _freelancerStudentWebService;

        public FreelancerStudentController(IFreelancerStudentWebService freelancerStudentWebService)
        {
            _freelancerStudentWebService = freelancerStudentWebService;
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

            return View(result.data);
        }

        public async Task<IActionResult> DanhSachNopTuyen()
        {
            return View();
        }


    }
}
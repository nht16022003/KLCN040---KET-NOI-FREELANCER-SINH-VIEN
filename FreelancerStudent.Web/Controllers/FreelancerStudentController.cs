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


    }
}
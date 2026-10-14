using FreelancerStudent.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FreelancerStudent.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var tenRole = HttpContext.Session.GetString("tenRole");
            switch (tenRole)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "NhaTuyenDung":
                    return RedirectToAction("Index", "FreelancerStudent");
                case "FreelancerStudent":
                    return RedirectToAction("Index", "JobPost");
                default:
                    return RedirectToAction("DangNhap", "Account");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

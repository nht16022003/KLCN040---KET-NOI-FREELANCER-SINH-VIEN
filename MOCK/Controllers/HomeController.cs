using Microsoft.AspNetCore.Mvc;
using MOCK.Models;
using System.Diagnostics;

namespace MOCK.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Action tiện ích để chuyển đổi Role khi test giao diện
        public IActionResult SwitchRole(string role)
        {
            if (string.Equals(role, "NhaTuyenDung", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("UserRole", "NhaTuyenDung");
                HttpContext.Session.SetString("UserName", "tuan_depzai");
                HttpContext.Session.SetString("UserRoleTitle", "Nhà tuyển dụng");
            }
            else if (string.Equals(role, "FreelancerStudent", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("UserRole", "FreelancerStudent");
                HttpContext.Session.SetString("UserName", "Son_ko_thogminh");
                HttpContext.Session.SetString("UserRoleTitle", "Freelancer Student");
            }
            else
            {
                // Chưa đăng nhập (Guest)
                HttpContext.Session.Remove("UserRole");
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("UserRoleTitle");
            }

            return RedirectToAction(nameof(Index));
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

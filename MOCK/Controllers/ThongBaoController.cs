using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;

namespace MOCK.Controllers
{
    public class ThongBaoController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? loai, string? status)
        {
            // Mặc định người dùng hiện tại là Nhà tuyển dụng (MaUser = 4) nếu role là NhaTuyenDung, ngược lại là Freelancer (MaUser = 1)
            var role = HttpContext.Session.GetString("UserRole");
            int currentUserId = role == "FreelancerStudent" ? 1 : 4;

            var model = MockDataStore.GetThongBaos(currentUserId, loai, status);
            return View(model);
        }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            bool result = MockDataStore.MarkThongBaoAsRead(id);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead()
        {
            var role = HttpContext.Session.GetString("UserRole");
            int currentUserId = role == "FreelancerStudent" ? 1 : 4;

            MockDataStore.MarkAllThongBaoAsRead(currentUserId);
            TempData["SuccessMessage"] = "Đã đánh dấu tất cả thông báo là đã đọc.";
            return RedirectToAction("Index");
        }
    }
}

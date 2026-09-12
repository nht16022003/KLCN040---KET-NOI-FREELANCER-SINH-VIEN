using Microsoft.AspNetCore.Mvc;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Người dùng {Username} đang thử đăng nhập (Form nháp).", model.UsernameOrEmail);

            // TODO: Sau này sẽ gọi API Auth (FreelancerStudent.API/api/Auth/login) và thiết lập Cookie Authentication
            // Tạm thời xử lý nháp chuyển hướng về trang chủ hoặc returnUrl
            TempData["SuccessMessage"] = $"Đăng nhập thành công với tài khoản: {model.UsernameOrEmail} (Bản nháp)!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction("Login");
        }

        // GET: /Account/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // TODO: Xóa cookie authentication
            TempData["InfoMessage"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}

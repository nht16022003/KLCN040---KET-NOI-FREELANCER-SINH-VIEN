using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerStudent.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthApiService authApiService, ILogger<AccountController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập và là Nhà tuyển dụng thì vào thẳng Dashboard
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "KhachHang")
            {
                return RedirectToAction("Dashboard", "Customer");
            }

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

            _logger.LogInformation("Web Controller: Người dùng {Username} gửi form đăng nhập", model.UsernameOrEmail);

            // 1. Gọi sang Backend API (FreelancerStudent.API/api/Auth/login)
            var authResult = await _authApiService.LoginAsync(model.UsernameOrEmail, model.Password);

            if (!authResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, authResult.Message);
                return View(model);
            }

            // 2. Thiết lập Claims và Cookie Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResult.UserID ?? string.Empty),
                new Claim(ClaimTypes.Name, authResult.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, authResult.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, authResult.LoaiUser ?? string.Empty),
                new Claim("FullName", authResult.FullName ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? System.DateTimeOffset.UtcNow.AddDays(7) : System.DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // 3. Lưu thông tin vào Session để các Controller khác sử dụng
            HttpContext.Session.SetString("UserID", authResult.UserID ?? string.Empty);
            HttpContext.Session.SetString("UserName", authResult.UserName ?? string.Empty);
            HttpContext.Session.SetString("FullName", authResult.FullName ?? string.Empty);
            HttpContext.Session.SetString("UserEmail", authResult.Email ?? string.Empty);
            HttpContext.Session.SetString("UserRole", authResult.LoaiUser ?? string.Empty);

            TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {authResult.FullName} quay trở lại.";

            // 4. Nếu có ReturnUrl hợp lệ thì chuyển hướng về đó
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            // =========================================================================
            // 5. ĐIỀU HƯỚNG THEO PHÂN QUYỀN (loaiUser) TỪ CSDL:
            //    - Nếu là "KhachHang" (Nhà tuyển dụng) ➡️ Đến /Customer/Dashboard
            //    - Nếu là "FreelancerSV" (Sinh viên)   ➡️ Đến /Home/Index
            //    - Nếu là "Admin" (Quản trị viên)     ➡️ Đến /Home/Index
            // =========================================================================
            if (authResult.LoaiUser == "KhachHang")
            {
                return RedirectToAction("Dashboard", "Customer");
            }
            else if (authResult.LoaiUser == "FreelancerSV")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (authResult.LoaiUser == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET & POST: /Account/Logout
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Người dùng đang đăng xuất.");

            // 1. Gọi sang Backend API để ghi nhận kết thúc phiên đăng xuất
            await _authApiService.LogoutAsync();

            // 2. Xóa Cookie Authentication trên trình duyệt
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Xóa toàn bộ Session phía Web
            HttpContext.Session.Clear();

            // 4. Thông báo và chuyển hướng về màn hình Đăng nhập
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công khỏi hệ thống.";
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["SuccessMessage"] = $"Đăng ký tài khoản {model.LoaiUser} thành công cho {model.FullName}! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();
            return View(model);
        }
    }
}

using System.Security.Claims;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthWebService _authWebService; //Khai báo để sử dụng serviceswweb

        public AccountController(IAuthWebService authWebService)
        {
            _authWebService = authWebService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); //trả về DangKy và truyền vào DangKyViewModel
            }

            var ketqua_reponse_api_dangky = await _authWebService.DangKyAsync(model);

            if (ketqua_reponse_api_dangky.success)
            {
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "Account");
            }
            ModelState.AddModelError(string.Empty, ketqua_reponse_api_dangky.message!);


            return View(model);
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new DangNhapViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Gọi API Service để đăng nhập
            var ketqua_dangnhap = await _authWebService.DangNhapAsync(model);

            if (!ketqua_dangnhap.success || ketqua_dangnhap.data == null)
            {
                ModelState.AddModelError(string.Empty, ketqua_dangnhap.message);
                return View(model);
            }

            var user = ketqua_dangnhap.data;



            //Lưu thông tin session cho View/Layout
            HttpContext.Session.SetInt32("maUser", user.maUser);
            HttpContext.Session.SetString("hovaten", user.hovaten);
            HttpContext.Session.SetString("tenRole", user.tenrole);

            //Cấp Cookie claims dùng để phân quyền
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.maUser.ToString()),
                new Claim(ClaimTypes.Name, user.hovaten),
                new Claim(ClaimTypes.Email,user.email),
                new Claim(ClaimTypes.Role, user.tenrole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            /*
             var authProperties = new AuthenticationProperties
             {
                 IsPersistent = model.RememberMe,
                 ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
             };

            */
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = $"Chào mừng bạn trở lại, {user.hovaten}!";

            //Kiểm tra tên role để chuyển trang
            var tenRole_ = HttpContext.Session.GetString("tenRole");



            switch (tenRole_)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "NhaTuyenDung":
                    return RedirectToAction("Index", "FreelancerStudent");
                case "FreelancerStudent":
                    return RedirectToAction("Index", "JobPost");

                default:
                    return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost, HttpGet]


        public async Task<IActionResult> DangXuat()
        {
            //Xóa cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //Xóa toàn bộ dữ liệu trong session
            HttpContext.Session.Clear();

            //Chuyển hướng về login
            return RedirectToAction("DangNhap", "Account");
        }
    }
}
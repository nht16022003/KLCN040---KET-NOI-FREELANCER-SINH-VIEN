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
        [HttpGet]
        public async Task<IActionResult> ThietLapTaiKhoan(string tab = "thongtin")
        {
            var maUser =
                HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction(
                    "DangNhap",
                    "Account"
                );
            }

            var ketQua =
                await _authWebService
                    .LayThongTinTaiKhoanAsync(maUser.Value);

            if (!ketQua.success || ketQua.data == null)
            {
                TempData["ErrorMessage"] =
                    ketQua.message
                    ?? "Không lấy được thông tin tài khoản.";

                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }

            ViewBag.Tab = tab;

            return View(ketQua.data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTinTaiKhoan(CapNhatThongTinTaiKhoanViewModel model)
        {
            var maUser =
                HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap");
            }

            // Không tin maUser gửi từ HTML
            model.maUser = maUser.Value;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Thông tin nhập chưa hợp lệ.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            var ketQua =
                await _authWebService
                    .CapNhatThongTinTaiKhoanAsync(model);

            if (!ketQua.success)
            {
                TempData["ErrorMessage"] =
                    ketQua.message;

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            // Cập nhật tên trên Header luôn
            if (ketQua.data != null)
            {
                HttpContext.Session.SetString(
                    "hovaten",
                    ketQua.data.hovaten
                );
            }

            TempData["SuccessMessage"] =
                "Cập nhật thông tin thành công!";

            return RedirectToAction(
                "ThietLapTaiKhoan",
                new { tab = "thongtin" }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatAvatar(IFormFile avatar)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (avatar == null || avatar.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ảnh.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            // Giới hạn 5MB
            if (avatar.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] =
                    "Dung lượng ảnh không được vượt quá 5MB.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            var extension =
                Path.GetExtension(avatar.FileName).ToLower();

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] =
                    "Chỉ hỗ trợ ảnh JPG, JPEG, PNG hoặc WEBP.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "avatars"
            );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName =
                $"avatar_{maUser}_{Guid.NewGuid():N}{extension}";

            var filePath =
                Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            var avatarUrl =
                $"/uploads/avatars/{fileName}";

            // Bước tiếp theo sẽ gửi avatarUrl xuống API
            var ketQua =
                await _authWebService.CapNhatAvatarAsync(
                    maUser.Value,
                    avatarUrl
                );

            if (!ketQua.success)
            {
                // Nếu DB không cập nhật được thì xóa file vừa upload
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                TempData["ErrorMessage"] =
                    ketQua.message ?? "Cập nhật avatar thất bại.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "thongtin" }
                );
            }

            HttpContext.Session.SetString(
                "UserAvatar",
                avatarUrl
            );

            TempData["SuccessMessage"] =
                "Cập nhật ảnh đại diện thành công!";

            return RedirectToAction(
                "ThietLapTaiKhoan",
                new { tab = "thongtin" }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(
            DoiMatKhauViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Vui lòng nhập đầy đủ thông tin mật khẩu.";

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "baomat" }
                );
            }

            var ketQua = await _authWebService.DoiMatKhauAsync(
                maUser.Value,
                model
            );

            if (!ketQua.success)
            {
                TempData["ErrorMessage"] = ketQua.message;

                return RedirectToAction(
                    "ThietLapTaiKhoan",
                    new { tab = "baomat" }
                );
            }

            TempData["SuccessMessage"] =
                "Đổi mật khẩu thành công!";

            return RedirectToAction("Index", "Home");
        }
    }
}
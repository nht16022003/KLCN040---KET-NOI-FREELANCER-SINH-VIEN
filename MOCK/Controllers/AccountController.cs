using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.Entities;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MOCK.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        // ==================== CUS-UC-01: QUÊN MẬT KHẨU ====================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel { Step = 1 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailOrUsername))
            {
                ModelState.AddModelError("EmailOrUsername", "Vui lòng nhập Email hoặc Tên tài khoản đã đăng ký.");
                model.Step = 1;
                return View(model);
            }

            var user = MockDataStore.Users.FirstOrDefault(u =>
                u.TenTaiKhoanUser.Equals(model.EmailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                u.EmailUser.Equals(model.EmailOrUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ModelState.AddModelError("EmailOrUsername", "Không tìm thấy tài khoản với thông tin đã cung cấp.");
                model.Step = 1;
                return View(model);
            }

            // Bước 2: Chuyển sang bước xác thực OTP & đặt mật khẩu mới
            model.Step = 2;
            model.UserRoleName = user.MaRole switch
            {
                1 => "Freelancer Sinh viên",
                2 => "Nhà tuyển dụng / Khách hàng",
                3 => "Quản trị viên",
                _ => "Thành viên"
            };
            model.OtpCode = "888666"; // Mã demo gợi ý cho người dùng
            TempData["InfoMessage"] = $"Hệ thống đã gửi mã OTP xác nhận về email: {user.EmailUser}. (Mã demo: 888666)";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ForgotPasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OtpCode) || model.OtpCode.Trim() != "888666")
            {
                ModelState.AddModelError("OtpCode", "Mã xác thực OTP không đúng hoặc đã hết hạn. Vui lòng nhập mã: 888666");
                model.Step = 2;
                return View("ForgotPassword", model);
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu mới phải có tối thiểu 6 ký tự.");
                model.Step = 2;
                return View("ForgotPassword", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không trùng khớp.");
                model.Step = 2;
                return View("ForgotPassword", model);
            }

            bool success = MockDataStore.ResetPasswordByEmailOrUsername(model.EmailOrUsername, model.NewPassword);
            if (success)
            {
                model.Step = 3;
                TempData["SuccessMessage"] = "Khôi phục mật khẩu thành công! Bạn có thể đăng nhập ngay với mật khẩu mới.";
                return View("ForgotPassword", model);
            }

            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình cập nhật mật khẩu.");
            model.Step = 2;
            return View("ForgotPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm kiếm trong MockDataStore
            var user = MockDataStore.Users.FirstOrDefault(u =>
                (u.TenTaiKhoanUser.Equals(model.UsernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                 u.EmailUser.Equals(model.UsernameOrEmail, StringComparison.OrdinalIgnoreCase)));

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản hoặc email không tồn tại trong hệ thống demo.");
                return View(model);
            }

            // Gán thông tin vào Session
            string roleName = user.MaRole switch
            {
                1 => "FreelancerStudent",
                2 => "NhaTuyenDung",
                3 => "Admin",
                _ => "Guest"
            };

            string roleTitle = user.MaRole switch
            {
                1 => "Freelancer Student",
                2 => "Nhà tuyển dụng",
                3 => "Quản trị viên",
                _ => "Thành viên"
            };

            // Lấy avatar người dùng
            string avatarUrl = "uploads/avatar/default_student.png";
            var studentObj = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaUser == user.MaUser);
            if (studentObj != null && !string.IsNullOrEmpty(studentObj.Avatar))
            {
                avatarUrl = studentObj.Avatar;
            }
            else
            {
                var empObj = MockDataStore.NhaTuyenDungs.FirstOrDefault(e => e.MaUser == user.MaUser);
                if (empObj != null && !string.IsNullOrEmpty(empObj.Avatar))
                {
                    avatarUrl = empObj.Avatar;
                }
            }

            HttpContext.Session.SetString("UserRole", roleName);
            HttpContext.Session.SetString("UserName", user.TenTaiKhoanUser);
            HttpContext.Session.SetString("UserFullName", user.HotenUser);
            HttpContext.Session.SetString("UserRoleTitle", roleTitle);
            HttpContext.Session.SetString("UserAvatar", avatarUrl);
            HttpContext.Session.SetInt32("UserId", user.MaUser);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng email trong MockDataStore
            if (MockDataStore.Users.Any(u => u.EmailUser.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
                return View(model);
            }

            // Tạo tài khoản mới vào MockDataStore
            int newMaRole = model.RoleType == "NhaTuyenDung" ? 2 : 1;
            int newUserId = MockDataStore.Users.Max(u => u.MaUser) + 1;
            string username = model.Email.Split('@')[0];

            var newUser = new User
            {
                MaUser = newUserId,
                HotenUser = model.FullName,
                TenTaiKhoanUser = username,
                PashwordHash = model.Password,
                EmailUser = model.Email,
                SdtUser = model.PhoneNumber,
                Ngaysinh = DateTime.Now.AddYears(-20),
                Status = "ACTIVE",
                NgayTao = DateTime.Now,
                MaRole = newMaRole
            };

            MockDataStore.Users.Add(newUser);

            // Tự động gán Session đăng nhập
            string roleName = newMaRole == 2 ? "NhaTuyenDung" : "FreelancerStudent";
            string roleTitle = newMaRole == 2 ? "Nhà tuyển dụng" : "Freelancer Student";
            string defaultAvatar = "uploads/avatar/default_student.png";

            HttpContext.Session.SetString("UserRole", roleName);
            HttpContext.Session.SetString("UserName", newUser.TenTaiKhoanUser);
            HttpContext.Session.SetString("UserFullName", newUser.HotenUser);
            HttpContext.Session.SetString("UserRoleTitle", roleTitle);
            HttpContext.Session.SetString("UserAvatar", defaultAvatar);
            HttpContext.Session.SetInt32("UserId", newUser.MaUser);

            // Nếu là sinh viên -> Chuyển thẳng sang trang Nộp minh chứng sinh viên
            if (newMaRole == 1)
            {
                var newStudent = new FreelancerStudent
                {
                    MaFreelancerStudents = MockDataStore.FreelancerStudents.Any() ? MockDataStore.FreelancerStudents.Max(s => s.MaFreelancerStudents) + 1 : 1,
                    MaUser = newUser.MaUser,
                    MaChuyenNganh = "CNTT",
                    TenTruong = "Đại học Công nghệ Thông tin",
                    DiaDiemTruong = "TP.HCM",
                    DiaDiemFreelancerStudent = "TP.HCM",
                    NamThu = 2,
                    GPA = 3.5,
                    NienKhoa = "2024-2028",
                    Gioithieu = "Sinh viên mới tham gia nền tảng Freelancer Students.",
                    Avatar = defaultAvatar,
                    TrangthaiNhanViec = true,
                    ChiPhiTu = 100000
                };
                MockDataStore.FreelancerStudents.Add(newStudent);

                return RedirectToAction("MinhChung", "Account", new { isNew = true });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MinhChung(bool? isNew, int? userId)
        {
            // Lấy ID từ session hoặc tham số hoặc mặc định user 1 (hoặc user 3 có status Đang gửi)
            int currentUserId = userId ?? HttpContext.Session.GetInt32("UserId") ?? 1;
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? MockDataStore.Users.First();
            var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaUser == user.MaUser) ?? new FreelancerStudent
            {
                MaUser = user.MaUser,
                TenTruong = "Đại học Công nghệ Thông tin",
                MaChuyenNganh = "CNTT",
                NamThu = 2,
                NienKhoa = "2024-2028"
            };

            var minhChung = MockDataStore.MinhChungs.FirstOrDefault(m => m.MaUser == user.MaUser);
            if (minhChung == null)
            {
                minhChung = new MinhChungFreelancerStudent
                {
                    MaMinhChung = MockDataStore.MinhChungs.Any() ? MockDataStore.MinhChungs.Max(m => m.MaMinhChung) + 1 : 1,
                    MaUser = user.MaUser,
                    LoaiMinhChung = "Thẻ sinh viên",
                    FileMinhChung = "uploads/minhchung/sample_the_sv.jpg",
                    NgayNop = DateTime.Now,
                    TrangThaiGuiMinhChung = isNew == true ? "Chưa nộp" : "Đang gửi"
                };
            }

            var model = new MinhChungViewModel
            {
                User = user,
                FreelancerStudent = student,
                MinhChung = minhChung,
                ChuyenNganhs = MockDataStore.ChuyenNganhs,
                SelectedLoaiMinhChung = string.IsNullOrEmpty(minhChung.LoaiMinhChung) ? "Thẻ sinh viên" : minhChung.LoaiMinhChung,
                TenTruong = student.TenTruong ?? "Đại học Công nghệ Thông tin",
                MaTruong = student.MaTruong ?? "UIT01",
                DiaDiemTruong = student.DiaDiemTruong ?? "Thủ Đức, TP.HCM",
                MaChuyenNganh = student.MaChuyenNganh ?? "CNTT",
                NamThu = student.NamThu > 0 ? student.NamThu : 2,
                NienKhoa = student.NienKhoa ?? "2024-2028",
                GPA = student.GPA > 0 ? student.GPA : 3.5,
                FileMinhChungUrl = minhChung.FileMinhChung,
                IsFirstTimeAfterRegister = isNew ?? false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MinhChung(MinhChungViewModel model)
        {
            int currentUserId = model.User?.MaUser > 0 ? model.User.MaUser : (HttpContext.Session.GetInt32("UserId") ?? 1);
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? MockDataStore.Users.First();
            var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaUser == user.MaUser);

            if (student != null)
            {
                student.TenTruong = model.TenTruong;
                student.MaTruong = model.MaTruong ?? string.Empty;
                student.DiaDiemTruong = model.DiaDiemTruong ?? string.Empty;
                student.MaChuyenNganh = model.MaChuyenNganh;
                student.NamThu = model.NamThu;
                student.NienKhoa = model.NienKhoa;
                student.GPA = model.GPA ?? 3.5;
            }

            var existingMC = MockDataStore.MinhChungs.FirstOrDefault(m => m.MaUser == user.MaUser);
            if (existingMC != null)
            {
                existingMC.LoaiMinhChung = model.SelectedLoaiMinhChung;
                existingMC.FileMinhChung = !string.IsNullOrEmpty(model.FileMinhChungUrl) ? model.FileMinhChungUrl : "uploads/minhchung/user_card_upload.jpg";
                existingMC.NgayNop = DateTime.Now;
                existingMC.TrangThaiGuiMinhChung = "Đang gửi";
                existingMC.LydoTuChoi = null;
            }
            else
            {
                var newMC = new MinhChungFreelancerStudent
                {
                    MaMinhChung = MockDataStore.MinhChungs.Any() ? MockDataStore.MinhChungs.Max(m => m.MaMinhChung) + 1 : 1,
                    MaUser = user.MaUser,
                    LoaiMinhChung = model.SelectedLoaiMinhChung,
                    FileMinhChung = !string.IsNullOrEmpty(model.FileMinhChungUrl) ? model.FileMinhChungUrl : "uploads/minhchung/user_card_upload.jpg",
                    NgayNop = DateTime.Now,
                    TrangThaiGuiMinhChung = "Đang gửi",
                    LydoTuChoi = null
                };
                MockDataStore.MinhChungs.Add(newMC);
                existingMC = newMC;
            }

            model.User = user;
            model.FreelancerStudent = student ?? new FreelancerStudent();
            model.MinhChung = existingMC;
            model.ChuyenNganhs = MockDataStore.ChuyenNganhs;
            model.SuccessMessage = "Hồ sơ minh chứng của bạn đã được gửi thành công! Quản trị viên sẽ tiến hành đối soát và kích hoạt huy hiệu sinh viên xác thực trong vòng 24 giờ.";
            model.IsFirstTimeAfterRegister = false;

            return View(model);
        }

        // =========================================================================
        // USE CASE CUS-UC-03: QUẢN LÝ THÔNG TIN CÁ NHÂN & THIẾT LẬP BẢO MẬT
        // =========================================================================

        [HttpGet]
        public IActionResult Profile(string? tab = "profile", bool? edit = false)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? MockDataStore.Users.First();

            var viewModel = BuildProfileViewModel(user, tab ?? "profile", edit ?? false);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(UserProfileViewModel model)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? MockDataStore.Users.First();

            // Kiểm tra tính hợp lệ Họ và tên
            if (string.IsNullOrWhiteSpace(model.EditHotenUser))
            {
                ModelState.AddModelError("EditHotenUser", "Họ và tên không được để trống.");
            }

            // Ngoại lệ EXC 1: Kiểm tra tính hợp lệ Số điện thoại
            if (string.IsNullOrWhiteSpace(model.EditSdtUser))
            {
                ModelState.AddModelError("EditSdtUser", "Số điện thoại không được để trống.");
            }
            else if (!Regex.IsMatch(model.EditSdtUser.Trim(), @"^(0[3|5|7|8|9])[0-9]{8}$"))
            {
                ModelState.AddModelError("EditSdtUser", "Số điện thoại không đúng định dạng quy chuẩn (VD: 0901234567, bắt đầu bằng 03/05/07/08/09 và đủ 10 số).");
            }

            // Ngoại lệ EXC 2: Kiểm tra định dạng tệp ảnh đại diện
            if (model.AvatarUpload != null && model.AvatarUpload.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(model.AvatarUpload.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("AvatarUpload", "Tệp tải lên không phải định dạng ảnh hợp lệ (chỉ chấp nhận .jpg, .jpeg, .png, .webp).");
                }
                else if (model.AvatarUpload.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    ModelState.AddModelError("AvatarUpload", "Kích thước tệp ảnh quá lớn. Vui lòng chọn ảnh dung lượng dưới 5MB.");
                }
            }

            // Nếu có lỗi -> Trả về giao diện chỉnh sửa kèm thông báo lỗi
            if (!ModelState.IsValid)
            {
                var errorViewModel = BuildProfileViewModel(user, "profile", true);
                errorViewModel.EditHotenUser = model.EditHotenUser;
                errorViewModel.EditSdtUser = model.EditSdtUser;
                errorViewModel.ErrorMessage = "Cập nhật thông tin thất bại. Vui lòng kiểm tra lại các trường báo đỏ bên dưới.";
                return View("Profile", errorViewModel);
            }

            // Cập nhật Avatar nếu có tải lên mới
            if (model.AvatarUpload != null && model.AvatarUpload.Length > 0)
            {
                try
                {
                    string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string uploadsFolder = Path.Combine(webRootPath, "uploads", "avatar");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileExtension = Path.GetExtension(model.AvatarUpload.FileName).ToLowerInvariant();
                    string uniqueFileName = $"avatar_user_{user.MaUser}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.AvatarUpload.CopyTo(stream);
                    }

                    string relativeAvatarPath = $"uploads/avatar/{uniqueFileName}";

                    // Đồng bộ Avatar vào bảng FreelancerStudent hoặc NhaTuyenDung
                    var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaUser == user.MaUser);
                    if (student != null)
                    {
                        student.Avatar = relativeAvatarPath;
                    }

                    var employer = MockDataStore.NhaTuyenDungs.FirstOrDefault(e => e.MaUser == user.MaUser);
                    if (employer != null)
                    {
                        employer.Avatar = relativeAvatarPath;
                    }

                    // Cập nhật avatar trong Session
                    HttpContext.Session.SetString("UserAvatar", relativeAvatarPath);
                }
                catch (Exception)
                {
                    // Fallback an toàn nếu môi trường không ghi file được
                }
            }

            // Cập nhật thông tin vào MockDataStore
            user.HotenUser = model.EditHotenUser.Trim();
            user.SdtUser = model.EditSdtUser.Trim();
            HttpContext.Session.SetString("UserFullName", user.HotenUser);

            TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction("Profile", new { tab = "profile", edit = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? MockDataStore.Users.First();

            // Ngoại lệ EXC 1: Kiểm tra mật khẩu hiện tại
            if (string.IsNullOrEmpty(model.CurrentPassword) || !string.Equals(model.CurrentPassword, user.PashwordHash))
            {
                ModelState.AddModelError("ChangePasswordModel.CurrentPassword", "Mật khẩu hiện tại không chính xác.");
            }

            // Ngoại lệ EXC 3: Kiểm tra mật khẩu mới đạt chuẩn bảo mật
            if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Length < 8 ||
                !model.NewPassword.Any(char.IsLetter) || !model.NewPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("ChangePasswordModel.NewPassword", "Mật khẩu mới phải đạt tối thiểu 8 ký tự bao gồm chữ và số.");
            }

            // Ngoại lệ EXC 2: Kiểm tra mật khẩu xác nhận
            if (!string.Equals(model.NewPassword, model.ConfirmPassword))
            {
                ModelState.AddModelError("ChangePasswordModel.ConfirmPassword", "Mật khẩu xác nhận không trùng khớp.");
            }

            // Nếu có lỗi -> Trả về tab Bảo mật với lỗi
            if (!ModelState.IsValid)
            {
                var errorViewModel = BuildProfileViewModel(user, "security", false);
                errorViewModel.ChangePasswordModel = model;
                errorViewModel.ErrorMessage = "Đổi mật khẩu không thành công. Vui lòng kiểm tra lại thông tin bên dưới.";
                return View("Profile", errorViewModel);
            }

            // Hậu điều kiện CUS-UC-01.05: Cập nhật mật khẩu mới, vô hiệu hóa phiên làm việc và đăng xuất
            user.PashwordHash = model.NewPassword;
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Mật khẩu cũ đã bị vô hiệu hóa, vui lòng đăng nhập lại.";
            return RedirectToAction("Login");
        }

        private UserProfileViewModel BuildProfileViewModel(User user, string activeTab = "profile", bool isEditMode = false)
        {
            string roleTitle = user.MaRole switch
            {
                1 => "Freelancer Student",
                2 => "Nhà tuyển dụng",
                3 => "Quản trị viên",
                _ => "Thành viên"
            };

            string avatarUrl = "uploads/avatar/default_student.png";
            var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaUser == user.MaUser);
            if (student != null && !string.IsNullOrEmpty(student.Avatar))
            {
                avatarUrl = student.Avatar;
            }
            else
            {
                var emp = MockDataStore.NhaTuyenDungs.FirstOrDefault(e => e.MaUser == user.MaUser);
                if (emp != null && !string.IsNullOrEmpty(emp.Avatar))
                {
                    avatarUrl = emp.Avatar;
                }
            }

            string trangThaiXacThuc = "Chưa xác thực";
            var mc = MockDataStore.MinhChungs.FirstOrDefault(m => m.MaUser == user.MaUser);
            if (mc != null)
            {
                trangThaiXacThuc = mc.TrangThaiGuiMinhChung switch
                {
                    "Đã xác minh" => "Đã xác thực hồ sơ",
                    "Đang gửi" => "Đang chờ đối soát minh chứng",
                    "Đã từ chối" => "Từ chối xác thực",
                    _ => "Chưa xác thực hồ sơ"
                };
            }
            else if (user.MaRole == 2)
            {
                var emp = MockDataStore.NhaTuyenDungs.FirstOrDefault(e => e.MaUser == user.MaUser);
                if (emp != null && emp.Trangthai == "Active")
                {
                    trangThaiXacThuc = "Đã xác thực doanh nghiệp";
                }
            }

            return new UserProfileViewModel
            {
                User = user,
                MaUser = user.MaUser,
                HotenUser = user.HotenUser,
                TenTaiKhoanUser = user.TenTaiKhoanUser,
                EmailUser = user.EmailUser,
                SdtUser = user.SdtUser,
                NgayTao = user.NgayTao,
                Status = user.Status,
                RoleTitle = roleTitle,
                TrangThaiXacThuc = trangThaiXacThuc,
                AvatarUrl = avatarUrl,
                IsEditMode = isEditMode,
                ActiveTab = activeTab,
                EditHotenUser = user.HotenUser,
                EditSdtUser = user.SdtUser ?? string.Empty
            };
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

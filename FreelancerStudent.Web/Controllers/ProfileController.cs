using FreelancerStudent.Web.Models.Entities;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDBConText _context;

        // Tiêm DbContext vào Controller
        public ProfileController(ApplicationDBConText context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AccountInfor(string id = "USR001")
        {
            var freelancer = await _context.FreelancerSV
               .Include(f => f.Users)
               .FirstOrDefaultAsync(f => f.userID == id);

            // Nếu không tìm thấy thông tin
            if (freelancer == null)
            {
                return NotFound("Không tìm thấy thông tin tài khoản sinh viên.");
            }

            // Đưa dữ liệu từ 2 bảng SQL vào ViewModel
            var viewModel = new AccountInforViewModel
            {
                // Từ bảng Users
                userID = freelancer.Users.userID,
                userName = freelancer.Users.userName,
                email = freelancer.Users.email,
                phoneNumber = freelancer.Users.phoneNumber ?? "Chưa cập nhật",
                loaiUser = freelancer.Users.loaiUser,
                status = freelancer.Users.status,
                createdAt = freelancer.Users.createdAt,

                // Từ bảng FreelancerSV
                free_ID = freelancer.free_ID,
                birth = freelancer.birth,
                gender = freelancer.gender?.Trim() ?? "Chưa rõ",
                address = freelancer.address ?? "Chưa cập nhật",
                university = freelancer.university ?? "Chưa cập nhật",
                AcademicYearStart = freelancer.AcademicYearStart,
                AcademicYearEnd = freelancer.AcademicYearEnd,
                major = freelancer.major ?? "Chưa cập nhật",
                studentCardID = freelancer.studentCardID ?? "Chưa cập nhật",
                GPA = freelancer.GPA
            };

            return View(viewModel);
        }
    }
}

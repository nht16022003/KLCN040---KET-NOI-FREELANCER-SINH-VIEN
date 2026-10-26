using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class NhaTuyenDungController : Controller
    {

        private readonly INhaTuyenDungWebService _nhaTuyenDungWebService;
        private readonly IJobPostWebService _jobPostWebService;
        private readonly IDanhGiaWebService _danhGiaWebService;

        public NhaTuyenDungController(
            INhaTuyenDungWebService nhaTuyenDungWebService, 
            IJobPostWebService jobPostWebService,
            IDanhGiaWebService danhGiaWebService)
        {
            _nhaTuyenDungWebService = nhaTuyenDungWebService;
            _jobPostWebService = jobPostWebService;
            _danhGiaWebService = danhGiaWebService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Xem hồ sơ Nhà tuyển dụng (cá nhân hoặc công khai)
        /// Hỗ trợ cả route /NhaTuyenDung/Profile và /Employer/Profile
        /// </summary>
        [HttpGet]
        [Route("NhaTuyenDung/Profile")]
        [Route("NhaTuyenDung/Profile/{id:int}")]
        [Route("Employer/Profile")]
        [Route("Employer/Profile/{id:int}")]
        public async Task<IActionResult> Profile(int? id, int? maUser, int? maNhaTuyenDung)
        {
            var currentUserId = HttpContext.Session.GetInt32("maUser");

            // Xác định đối tượng cần truy vấn
            int? targetNtdId = id.HasValue && id > 0 ? id : (maNhaTuyenDung.HasValue && maNhaTuyenDung > 0 ? maNhaTuyenDung : null);
            int? targetUserId = maUser.HasValue && maUser > 0 ? maUser : null;

            // Nếu không truyền ID nào, dùng tài khoản đang đăng nhập
            if (!targetNtdId.HasValue && !targetUserId.HasValue)
            {
                if (currentUserId.HasValue && currentUserId > 0)
                {
                    targetUserId = currentUserId.Value;
                }
                else
                {
                    return RedirectToAction("DangNhap", "Account");
                }
            }

            ApiReponse<FreelancerStudent.Web.ViewModels.NhaTuyenDungProfileViewModel>? result = null;

            if (targetNtdId.HasValue)
            {
                result = await _nhaTuyenDungWebService.layProfileNhaTuyenDungAsync(targetNtdId.Value);
            }
            else if (targetUserId.HasValue)
            {
                result = await _nhaTuyenDungWebService.layProfileNhaTuyenDungTheoMaUserAsync(targetUserId.Value);
            }

            if (result == null || !result.success || result.data == null)
            {
                TempData["Error"] = result?.message ?? "Không tìm thấy hồ sơ Nhà tuyển dụng.";
                return RedirectToAction("Search", "NhaTuyenDung");
            }

            var profile = result.data;
            ViewData["IsProfileOwner"] = currentUserId.HasValue && profile.NhaTuyenDung != null && profile.NhaTuyenDung.MaUser == currentUserId.Value;

            if (profile.NhaTuyenDung != null)
            {
                try
                {
                    var stats = await _danhGiaWebService.LayNhaTuyenDungAsync(profile.NhaTuyenDung.MaNhaTuyenDung, 1, 10);
                    if (stats?.success == true && stats.data != null)
                    {
                        profile.ThongKeDanhGia = stats.data;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NhaTuyenDungController] Lỗi khi nạp đánh giá: {ex.Message}");
                }
            }

            return View(profile);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> DanhSachUngVien(string? maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            // Gọi API lấy danh sách các bạn sinh viên đã nộp đơn vào Job của NTD này
            var ketQua = await _jobPostWebService.layDSUngTuyenTheoMaUser(maUser.Value);

            var dsUngTuyen = ketQua?.data ?? new List<UngTuyenViewModel>();
            // Nếu có truyền maJob -> Chỉ lọc lấy các ứng viên nộp vào bài Job này
            if (!string.IsNullOrEmpty(maJob))
            {
                dsUngTuyen = dsUngTuyen.Where(u => u.maJob == maJob).ToList();
                ViewBag.CurrentMaJob = maJob;
            }

            return View(dsUngTuyen);
        }


        //Tuấn Anh
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            //Gọi service lấy danh sách từ API
            var layDanhSachNhaTuyenDung = await _nhaTuyenDungWebService.layDanhSachNhaTuyenDungAsync();

            //  IN XEM WEB ĐÃ NHẬN ĐƯỢC BAO NHIÊU PHẦN TỬ:
            Console.WriteLine($"===> [SỐ LƯỢNG NTD NHẬN ĐƯỢC]: {layDanhSachNhaTuyenDung.data?.Count ?? 0}");


            //Truyền ds sang View
            var danhsachNTD = layDanhSachNhaTuyenDung.data ?? new List<FreelancerStudent.Web.ViewModels.NhaTuyenDungViewModel>();

            return View(danhsachNTD);
        }
    }
}
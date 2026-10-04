using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class NhaTuyenDungController : Controller
    {

        private readonly INhaTuyenDungWebService _nhaTuyenDungWebService;

        private readonly IJobPostWebService _jobPostWebService;

        public NhaTuyenDungController(INhaTuyenDungWebService nhaTuyenDungWebService, IJobPostWebService jobPostWebService)
        {
            _nhaTuyenDungWebService = nhaTuyenDungWebService;
            _jobPostWebService = jobPostWebService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
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
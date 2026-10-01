using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class NhaTuyenDungController : Controller
    {

        private readonly INhaTuyenDungWebService _nhaTuyenDungWebService;

        public NhaTuyenDungController(INhaTuyenDungWebService nhaTuyenDungWebService)
        {
            _nhaTuyenDungWebService = nhaTuyenDungWebService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DanhSachUngVien()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Search()
        {
            //Gọi service lấy danh sách từ API
            var layDanhSachNhaTuyenDung = await _nhaTuyenDungWebService.layDanhSachNhaTuyenDungAsync();

            // 👉 IN XEM WEB ĐÃ NHẬN ĐƯỢC BAO NHIÊU PHẦN TỬ:
            Console.WriteLine($"===> [SỐ LƯỢNG NTD NHẬN ĐƯỢC]: {layDanhSachNhaTuyenDung.data?.Count ?? 0}");


            //Truyền ds sang View
            var danhsachNTD = layDanhSachNhaTuyenDung.data ?? new List<FreelancerStudent.Web.ViewModels.NhaTuyenDungViewModel>();

            return View(danhsachNTD);
        }
    }
}
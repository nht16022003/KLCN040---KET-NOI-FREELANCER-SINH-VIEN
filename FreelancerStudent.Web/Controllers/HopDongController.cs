
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    [Authorize]
    public class HopDongController : Controller
    {
        private readonly IHopDongWebService _hopDongService;
        private readonly INhaTuyenDungWebService _ntdService;

        public HopDongController(
            IHopDongWebService hopDongService,
            INhaTuyenDungWebService ntdService)
        {
            _hopDongService = hopDongService;
            _ntdService = ntdService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var model = new DanhSachHopDongViewModel
            {
                TuKhoa = tuKhoa,
                TrangThai = trangThai
            };

            try
            {
                // Tìm nhà tuyển dụng thuộc User đăng nhập
                var response = await _ntdService
                    .layDanhSachNhaTuyenDungAsync();

                if (!response.success || response.data == null)
                {
                    model.Loi = "Không lấy được thông tin nhà tuyển dụng.";
                    return View(model);
                }

                var nhaTuyenDung = response.data
                    .FirstOrDefault(x => x.maUser == maUser.Value);

                if (nhaTuyenDung == null)
                {
                    model.Loi = "Tài khoản không thuộc nhà tuyển dụng.";
                    return View(model);
                }

                // Gọi API lấy danh sách hợp đồng
                model.HopDongs = await _hopDongService
                    .LayDanhSachAsync(
                        nhaTuyenDung.maNhaTuyenDung,
                        tuKhoa,
                        trangThai);
            }
            catch (HttpRequestException)
            {
                model.Loi = "Không thể kết nối đến API hợp đồng.";
            }
            catch (TaskCanceledException)
            {
                model.Loi = "Yêu cầu quá thời gian xử lý.";
            }
            catch (Exception)
            {
                model.Loi = "Có lỗi xảy ra khi tải hợp đồng.";
            }

            return View(model);
        }
    }
}

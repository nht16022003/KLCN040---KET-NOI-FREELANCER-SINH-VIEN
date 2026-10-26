using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NhaTuyenDungController : ControllerBase
    {
        private readonly INhaTuyenDungService _nhaTuyenDungService;

        public NhaTuyenDungController(INhaTuyenDungService nhaTuyenDungService)
        {
            _nhaTuyenDungService = nhaTuyenDungService;
        }

        [HttpGet("DanhSachNhaTuyenDung")]

        public async Task<IActionResult> layDanhSachNTD()
        {
            try
            {
                //Gọi service
                var ketquads = await _nhaTuyenDungService.layDanhSachNhaTuyenDungAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách thành công!",
                    data = ketquads
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách: " + ex.Message
                });
            }
        }

        [HttpGet("Profile/{maNhaTuyenDung:int}")]
        public async Task<IActionResult> layProfile(int maNhaTuyenDung)
        {
            try
            {
                var profile = await _nhaTuyenDungService.layProfileNhaTuyenDung(maNhaTuyenDung);
                if (profile == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hồ sơ nhà tuyển dụng!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin hồ sơ nhà tuyển dụng thành công!",
                    data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi tải hồ sơ nhà tuyển dụng: " + ex.Message
                });
            }
        }

        [HttpGet("Profile/ByUser/{maUser:int}")]
        public async Task<IActionResult> layProfileByMaUser(int maUser)
        {
            try
            {
                var profile = await _nhaTuyenDungService.layProfileNhaTuyenDungTheoMaUser(maUser);
                if (profile == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hồ sơ nhà tuyển dụng của người dùng này!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin hồ sơ nhà tuyển dụng thành công!",
                    data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi tải hồ sơ nhà tuyển dụng: " + ex.Message
                });
            }
        }
    }
}
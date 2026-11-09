using FreelancerStudent.API.DTOs.RequestDTOs;
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

        [HttpPut("CapNhatThongTin")]
        public async Task<IActionResult> capNhatThongTin([FromBody] CapNhatThongTinNhaTuyenDung_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ!",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var ketQua = await _nhaTuyenDungService.capNhatThongTinNTDAsync(request);
                if (!ketQua)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hồ sơ nhà tuyển dụng để cập nhật!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thông tin nhà tuyển dụng thành công!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi cập nhật thông tin: " + ex.Message
                });
            }
        }

        [HttpPut("CapNhatGioiThieu")]
        public async Task<IActionResult> capNhatGioiThieu([FromBody] CapNhatGioiThieuNhaTuyenDung_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ!"
                });
            }

            try
            {
                var ketQua = await _nhaTuyenDungService.capNhatGioiThieuNTDAsync(request);
                if (!ketQua)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hồ sơ nhà tuyển dụng để cập nhật!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật giới thiệu đơn vị thành công!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi cập nhật giới thiệu: " + ex.Message
                });
            }
        }

        public class CapNhatLogoNTD_Request
        {
            public int maUser { get; set; }
            public string logoUrl { get; set; } = string.Empty;
        }

        [HttpPut("CapNhatLogo")]
        public async Task<IActionResult> capNhatLogo([FromBody] CapNhatLogoNTD_Request request)
        {
            if (request == null || request.maUser <= 0 || string.IsNullOrWhiteSpace(request.logoUrl))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu cập nhật logo không hợp lệ!"
                });
            }

            try
            {
                var ketQua = await _nhaTuyenDungService.capNhatLogoNTDAsync(request.maUser, request.logoUrl);
                if (!ketQua)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy hồ sơ nhà tuyển dụng để cập nhật!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật logo/avatar thành công!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi cập nhật logo: " + ex.Message
                });
            }
        }
    }
}
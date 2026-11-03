using FreelancerStudent.API.DTOs.RequestDTO;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YeuThichController : ControllerBase
    {
        private readonly IYeuThichService _yeuThichService;

        public YeuThichController(IYeuThichService yeuThichService)
        {
            _yeuThichService = yeuThichService;
        }

        #region Freelancer Lưu việc

        [HttpPost("luu-viec")]
        public async Task<IActionResult> LuuViec([FromBody] LuuViec_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ!" });
            }

            try
            {
                var result = await _yeuThichService.LuuViecAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Đã lưu công việc thành công!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("bo-luu-viec")]
        public async Task<IActionResult> BoLuuViec([FromQuery] int maUser, [FromQuery] string maJob)
        {
            if (maUser <= 0 || string.IsNullOrWhiteSpace(maJob))
            {
                return BadRequest(new { success = false, message = "Thông tin không hợp lệ!" });
            }

            try
            {
                var result = await _yeuThichService.BoLuuViecAsync(maUser, maJob);
                return Ok(new
                {
                    success = true,
                    message = "Đã bỏ lưu công việc!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("kiem-tra-luu-viec")]
        public async Task<IActionResult> KiemTraLuuViec([FromQuery] int maUser, [FromQuery] string maJob)
        {
            if (maUser <= 0 || string.IsNullOrWhiteSpace(maJob))
            {
                return Ok(new { success = true, data = false });
            }

            try
            {
                var daLuu = await _yeuThichService.KiemTraLuuViecAsync(maUser, maJob);
                return Ok(new { success = true, data = daLuu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("danh-sach-viec-da-luu/{maUser}")]
        public async Task<IActionResult> LayDanhSachViecDaLuu(int maUser)
        {
            if (maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Mã người dùng không hợp lệ!" });
            }

            try
            {
                var danhSach = await _yeuThichService.LayDanhSachViecDaLuuAsync(maUser);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách việc đã lưu thành công!",
                    data = danhSach
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Nhà tuyển dụng lưu Freelancer

        [HttpPost("luu-freelancer")]
        public async Task<IActionResult> LuuFreelancer([FromBody] LuuFreelancer_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ!" });
            }

            try
            {
                var result = await _yeuThichService.LuuFreelancerAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Đã lưu Freelancer vào danh sách yêu thích!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("bo-luu-freelancer")]
        public async Task<IActionResult> BoLuuFreelancer([FromQuery] int maUser, [FromQuery] int maFreelancerStudent)
        {
            if (maUser <= 0 || maFreelancerStudent <= 0)
            {
                return BadRequest(new { success = false, message = "Thông tin không hợp lệ!" });
            }

            try
            {
                var result = await _yeuThichService.BoLuuFreelancerAsync(maUser, maFreelancerStudent);
                return Ok(new
                {
                    success = true,
                    message = "Đã xóa Freelancer khỏi danh sách yêu thích!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("kiem-tra-luu-freelancer")]
        public async Task<IActionResult> KiemTraLuuFreelancer([FromQuery] int maUser, [FromQuery] int maFreelancerStudent)
        {
            if (maUser <= 0 || maFreelancerStudent <= 0)
            {
                return Ok(new { success = true, data = false });
            }

            try
            {
                var daLuu = await _yeuThichService.KiemTraLuuFreelancerAsync(maUser, maFreelancerStudent);
                return Ok(new { success = true, data = daLuu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("danh-sach-freelancer-yeu-thich/{maUser}")]
        public async Task<IActionResult> LayDanhSachFreelancerYeuThich(int maUser)
        {
            if (maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Mã người dùng không hợp lệ!" });
            }

            try
            {
                var danhSach = await _yeuThichService.LayDanhSachFreelancerYeuThichAsync(maUser);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách Freelancer yêu thích thành công!",
                    data = danhSach
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}


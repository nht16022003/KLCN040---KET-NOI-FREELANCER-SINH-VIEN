using System;
using System.Threading.Tasks;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCongViecController : ControllerBase
    {
        private readonly ITaskCongViecService _taskService;

        public TaskCongViecController(ITaskCongViecService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/TaskCongViec/hop-dong/{maHD}?maUser=...
        [HttpGet("hop-dong/{maHD}")]
        public async Task<IActionResult> LayTienDoDuAn(string maHD, [FromQuery] int maUser)
        {
            if (string.IsNullOrWhiteSpace(maHD) || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Thông tin yêu cầu không hợp lệ." });
            }

            try
            {
                var result = await _taskService.LayTienDoDuAnAsync(maHD, maUser);
                return Ok(new { success = true, data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: api/TaskCongViec?maUser=...
        [HttpPost]
        public async Task<IActionResult> ThemTask([FromBody] TaoTaskCongViec_RequestDTO request, [FromQuery] int maUser)
        {
            if (!ModelState.IsValid || request == null || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ." });
            }

            try
            {
                var result = await _taskService.ThemTaskAsync(request, maUser);
                return Ok(new { success = true, message = "Thêm đầu việc thành công!", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tạo đầu việc: " + ex.Message });
            }
        }

        // PUT: api/TaskCongViec/{maTask}?maUser=...
        [HttpPut("{maTask}")]
        public async Task<IActionResult> CapNhatTask(int maTask, [FromBody] CapNhatTaskCongViec_RequestDTO request, [FromQuery] int maUser)
        {
            if (!ModelState.IsValid || request == null || maTask <= 0 || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ." });
            }

            request.maTask = maTask;

            try
            {
                var result = await _taskService.CapNhatTaskAsync(request, maUser);
                return Ok(new { success = result, message = "Cập nhật đầu việc thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi cập nhật đầu việc: " + ex.Message });
            }
        }

        // PATCH: api/TaskCongViec/{maTask}/trang-thai?maUser=...
        [HttpPatch("{maTask}/trang-thai")]
        public async Task<IActionResult> CapNhatTrangThaiTask(int maTask, [FromBody] CapNhatTrangThaiTask_RequestDTO request, [FromQuery] int maUser)
        {
            if (!ModelState.IsValid || request == null || maTask <= 0 || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ." });
            }

            try
            {
                var result = await _taskService.CapNhatTrangThaiTaskAsync(maTask, request.trangThai, maUser);
                return Ok(new { success = result, message = "Cập nhật trạng thái đầu việc thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi cập nhật trạng thái: " + ex.Message });
            }
        }

        // DELETE: api/TaskCongViec/{maTask}?maUser=...
        [HttpDelete("{maTask}")]
        public async Task<IActionResult> XoaTask(int maTask, [FromQuery] int maUser)
        {
            if (maTask <= 0 || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Thông tin xóa không hợp lệ." });
            }

            try
            {
                var result = await _taskService.XoaTaskAsync(maTask, maUser);
                return Ok(new { success = result, message = "Đã xóa đầu việc thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi xóa đầu việc: " + ex.Message });
            }
        }

        // POST: api/TaskCongViec/hop-dong/{maHD}/danh-gia-giai-doan?maUser=...
        [HttpPost("hop-dong/{maHD}/danh-gia-giai-doan")]
        public async Task<IActionResult> DanhGiaGiaiDoan(string maHD, [FromBody] DanhGiaGiaiDoan_RequestDTO request, [FromQuery] int maUser)
        {
            if (string.IsNullOrWhiteSpace(maHD) || request == null || maUser <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ." });
            }

            try
            {
                var result = await _taskService.DanhGiaGiaiDoanAsync(maHD, request.maGiaiDoan, request.trangThai, maUser);
                var thongBao = string.Equals(request.trangThai, "Dat", StringComparison.OrdinalIgnoreCase)
                    ? "Đã xác nhận dự án đạt yêu cầu! Các mốc giai đoạn đã chuyển sang trạng thái Đạt."
                    : "Đã cập nhật trạng thái dự án sang Chưa đạt.";
                return Ok(new { success = result, message = thongBao });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi đánh giá giai đoạn: " + ex.Message });
            }
        }
    }
}

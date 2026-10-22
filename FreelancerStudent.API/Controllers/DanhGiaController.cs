using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhGiaController : ControllerBase
    {
        private readonly IDanhGiaService _service;

        public DanhGiaController(IDanhGiaService service)
        {
            _service = service;
        }

        [HttpGet("hop-dong/{maHD}")]
        public async Task<IActionResult> GetForm(string maHD, [FromQuery] int maUser)
        {
            try
            {
                var data = await _service.LayThongTinFormAsync(maHD, maUser);
                return Ok(new { success = true, message = "Lấy thông tin đánh giá thành công.", data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { success = false, message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải thông tin đánh giá." }); }
        }

        [HttpPost]
        public async Task<IActionResult> Gui([FromBody] DanhGia_RequestDTO request, [FromQuery] int maUser)
        {
            try
            {
                var data = await _service.GuiDanhGiaAsync(request, maUser);
                return Ok(new { success = true, message = "Gửi đánh giá thành công.", data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { success = false, message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi gửi đánh giá." }); }
        }

        [HttpPut("{maDanhGia:int}")]
        public async Task<IActionResult> CapNhat(int maDanhGia, [FromBody] DanhGia_RequestDTO request, [FromQuery] int maUser)
        {
            try
            {
                var data = await _service.CapNhatDanhGiaAsync(maDanhGia, request, maUser);
                return Ok(new { success = true, message = "Cập nhật đánh giá thành công.", data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { success = false, message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi cập nhật đánh giá." }); }
        }

        [HttpGet("freelancer/{maFreelancerStudent:int}")]
        public async Task<IActionResult> GetFreelancer(int maFreelancerStudent, [FromQuery] int trang = 1, [FromQuery] int kichThuocTrang = 10)
        {
            try
            {
                var data = await _service.LayThongKeFreelancerAsync(maFreelancerStudent, trang, kichThuocTrang);
                return Ok(new { success = true, message = "Lấy đánh giá Freelancer thành công.", data });
            }
            catch (ArgumentException ex) { return NotFound(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải đánh giá Freelancer." }); }
        }

        [HttpGet("nha-tuyen-dung/{maNhaTuyenDung:int}")]
        public async Task<IActionResult> GetNhaTuyenDung(int maNhaTuyenDung, [FromQuery] int trang = 1, [FromQuery] int kichThuocTrang = 10)
        {
            try
            {
                var data = await _service.LayThongKeNhaTuyenDungAsync(maNhaTuyenDung, trang, kichThuocTrang);
                return Ok(new { success = true, message = "Lấy đánh giá nhà tuyển dụng thành công.", data });
            }
            catch (ArgumentException ex) { return NotFound(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải đánh giá nhà tuyển dụng." }); }
        }

        [HttpGet("cong-viec-da-thue")]
        public async Task<IActionResult> GetCongViecDaThue([FromQuery] int maUser)
        {
            try
            {
                var data = await _service.LayCongViecDaThueAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách công việc đã thuê thành công.", data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { success = false, message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải danh sách công việc đã thuê." }); }
        }
    }
}

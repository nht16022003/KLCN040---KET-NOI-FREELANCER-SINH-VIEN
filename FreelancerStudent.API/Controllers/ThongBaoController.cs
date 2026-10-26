using FreelancerStudent.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoService _service;

        public ThongBaoController(IThongBaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int maUser, [FromQuery] int trang = 1, [FromQuery] int kichThuocTrang = 10)
        {
            try
            {
                var data = await _service.LayDanhSachAsync(maUser, trang, kichThuocTrang);
                return Ok(new { success = true, message = "Lấy thông báo thành công.", data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải thông báo." }); }
        }

        [HttpGet("chua-doc")]
        public async Task<IActionResult> GetChuaDoc([FromQuery] int maUser)
        {
            try
            {
                var data = await _service.DemChuaDocAsync(maUser);
                return Ok(new { success = true, data });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi đếm thông báo chưa đọc." }); }
        }

        [HttpPut("{maThongBao:int}/da-doc")]
        public async Task<IActionResult> DanhDauDaDoc(int maThongBao, [FromQuery] int maUser)
        {
            try
            {
                var result = await _service.DanhDauDaDocAsync(maThongBao, maUser);
                return result ? Ok(new { success = true, message = "Đã đánh dấu thông báo." }) : NotFound(new { success = false, message = "Không tìm thấy thông báo." });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi cập nhật thông báo." }); }
        }

        [HttpPut("da-doc-tat-ca")]
        public async Task<IActionResult> DanhDauTatCaDaDoc([FromQuery] int maUser)
        {
            try
            {
                var count = await _service.DanhDauTatCaDaDocAsync(maUser);
                return Ok(new { success = true, message = "Đã đánh dấu tất cả thông báo là đã đọc.", data = count });
            }
            catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
            catch { return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi cập nhật thông báo." }); }
        }
    }
}

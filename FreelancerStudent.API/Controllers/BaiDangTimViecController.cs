using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaiDangTimViecController : ControllerBase
    {
        private readonly IBaiDangTimViecService _service;

        public BaiDangTimViecController(IBaiDangTimViecService service)
        {
            _service = service;
        }

        //Tuấn
        [HttpPost("TaoBaiDang")]
        public async Task<IActionResult> TaoBaiDang([FromBody] BaiDangTimViec_RequestDTO request)
        {
            try
            {
                var ketQua = await _service.taoBaiDangAsync(request);
                return Ok(new { success = true, message = "Đăng tin tìm việc thành công!", data = ketQua });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        //Tuấn
        [HttpGet("CuaToi/{maUser}")]
        public async Task<IActionResult> LayDanhSachCuaToi(int maUser)
        {
            try
            {
                var ketQua = await _service.layDanhSachTheoMaUserAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách thành công!", data = ketQua });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        //Tuấn
        [HttpGet("TatCa")]
        public async Task<IActionResult> LayTatCa()
        {
            try
            {
                var ketQua = await _service.layTatCaBaiDangKhaDungAsync();
                return Ok(new { success = true, message = "Lấy danh sách thành công!", data = ketQua });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        //Tuấn
        [HttpPut("DoiTrangThai")]
        public async Task<IActionResult> DoiTrangThai(string maBaiDang, string trangThai)
        {
            try
            {
                var ok = await _service.doiTrangThaiAsync(maBaiDang, trangThai);
                return Ok(new { success = ok, message = ok ? "Cập nhật trạng thái thành công!" : "Không tìm thấy bài đăng!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        //Tuấn
        [HttpDelete("Xoa/{maBaiDang}")]
        public async Task<IActionResult> Xoa(string maBaiDang)
        {
            try
            {
                var ok = await _service.xoaBaiDangAsync(maBaiDang);
                return Ok(new { success = ok, message = ok ? "Xóa bài đăng thành công!" : "Không tìm thấy bài đăng!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        //Tuấn
        [HttpPut("CapNhat/{maBaiDang}")]
        public async Task<IActionResult> CapNhat(string maBaiDang, [FromBody] BaiDangTimViec_RequestDTO request)
        {
            try
            {
                var ok = await _service.capNhatBaiDangAsync(maBaiDang, request);
                return Ok(new { success = ok, message = "Cập nhật bài đăng tìm việc thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}

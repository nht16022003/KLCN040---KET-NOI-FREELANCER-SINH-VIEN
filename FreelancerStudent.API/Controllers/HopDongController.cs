
using Microsoft.AspNetCore.Mvc;
using FreelancerStudent.API.Services;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.DTOs.ReponseDTOs;

namespace FreelancerStudent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HopDongController : ControllerBase
    {
        private readonly IHopDongService _hopDongService;

        public HopDongController(
            IHopDongService hopDongService)
        {
            _hopDongService = hopDongService;
        }

        // GET: api/HopDong/danh-sach/1
        [HttpGet("danh-sach/{maNhaTuyenDung:int}")]
        public async Task<ActionResult<List<HopDongResponseDTO>>>
            GetDanhSachHopDong(
                int maNhaTuyenDung,
                [FromQuery] HopDongFilterRequestDTO request)
        {
            try
            {
                var danhSach = await _hopDongService
                    .GetDanhSachHopDongAsync(
                        maNhaTuyenDung,
                        request.TuKhoa,
                        request.TrangThai
                    );

                return Ok(danhSach);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra khi lấy danh sách hợp đồng."
                });
            }
        }

        // GET: api/HopDong/danh-sach-freelancer/1
        [HttpGet("danh-sach-freelancer/{maFreelancerStudent:int}")]
        public async Task<ActionResult<List<HopDongResponseDTO>>>
            GetDanhSachHopDongFreelancer(
                int maFreelancerStudent,
                [FromQuery] HopDongFilterRequestDTO request)
        {
            try
            {
                var danhSach = await _hopDongService
                    .GetDanhSachHopDongTheoFreelancerAsync(
                        maFreelancerStudent,
                        request.TuKhoa,
                        request.TrangThai
                    );

                return Ok(danhSach);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra khi lấy danh sách hợp đồng của Freelancer."
                });
            }
        }

        // POST: api/HopDong/tao-hop-dong
        [HttpPost("tao-hop-dong")]
        public async Task<ActionResult<HopDongResponseDTO>> TaoHopDong(
            [FromBody] TaoHopDong_RequestDTO request,
            [FromQuery] int? maUserNTD)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ketQua = await _hopDongService.TaoDuThaoHopDongAsync(request, maUserNTD);
                return Ok(ketQua);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tạo dự thảo hợp đồng: " + ex.Message });
            }
        }

        // GET: api/HopDong/theo-job/{maJob}
        [HttpGet("theo-job/{maJob}")]
        public async Task<ActionResult<HopDongResponseDTO>> GetHopDongTheoJob(string maJob)
        {
            try
            {
                var hopDong = await _hopDongService.LayChiTietHopDongTheoMaJobAsync(maJob);
                if (hopDong == null)
                {
                    return NotFound(new { message = "Không tìm thấy hợp đồng cho công việc này." });
                }
                return Ok(hopDong);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin hợp đồng: " + ex.Message });
            }
        }

        // GET: api/HopDong/chi-tiet/{maHD}
        [HttpGet("chi-tiet/{maHD}")]
        public async Task<ActionResult<HopDongResponseDTO>> GetChiTietHopDong(string maHD)
        {
            try
            {
                var hopDong = await _hopDongService.LayChiTietHopDongTheoMaAsync(maHD);
                if (hopDong == null)
                {
                    return NotFound(new { message = "Không tìm thấy hợp đồng." });
                }
                return Ok(hopDong);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy chi tiết hợp đồng: " + ex.Message });
            }
        }

        // POST: api/HopDong/xac-nhan
        [HttpPost("xac-nhan")]
        public async Task<IActionResult> XacNhanHopDong(
            [FromBody] XacNhanHopDong_RequestDTO request,
            [FromQuery] int? maUserFreelancer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ketQua = await _hopDongService.XacNhanHopDongAsync(request, maUserFreelancer);
                return Ok(new
                {
                    success = ketQua,
                    message = request.dongY ? "Đã xác nhận dự thảo hợp đồng thành công!" : "Đã từ chối dự thảo hợp đồng."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi xử lý hợp đồng: " + ex.Message });
            }
        }
    }
}
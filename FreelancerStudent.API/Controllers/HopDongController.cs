
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
    }
}

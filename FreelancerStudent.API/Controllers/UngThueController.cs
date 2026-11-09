using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UngThueController : ControllerBase
    {
        private readonly IUngThueService _ungThueService;

        public UngThueController(IUngThueService ungThueService)
        {
            _ungThueService = ungThueService;
        }

        // POST: api/UngThue/GuiYeuCau
        [HttpPost("GuiYeuCau")]
        public async Task<IActionResult> GuiYeuCau([FromBody] GuiYeuCauThue_RequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ" });

                var result = await _ungThueService.GuiYeuCauThueAsync(request);
                return Ok(new { success = true, message = "Gửi yêu cầu thuê Freelancer thành công!", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/UngThue/DanhSachTheoFreelancer/{maUser}
        [HttpGet("DanhSachTheoFreelancer/{maUser}")]
        public async Task<IActionResult> DanhSachTheoFreelancer(int maUser)
        {
            try
            {
                var ds = await _ungThueService.LayDanhSachTheoFreelancerAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách yêu cầu thuê thành công", data = ds });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/UngThue/DanhSachTheoNTD/{maUser}
        [HttpGet("DanhSachTheoNTD/{maUser}")]
        public async Task<IActionResult> DanhSachTheoNTD(int maUser)
        {
            try
            {
                var ds = await _ungThueService.LayDanhSachTheoNTDAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách yêu cầu thuê thành công", data = ds });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT: api/UngThue/PhanHoi
        [HttpPut("PhanHoi")]
        public async Task<IActionResult> PhanHoi([FromBody] PhanHoiYeuCauThue_RequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu phản hồi không hợp lệ" });

                var result = await _ungThueService.PhanHoiYeuCauThueAsync(request);
                string msg = request.chapNhan ? "Đã chấp nhận yêu cầu thuê!" : "Đã từ chối yêu cầu thuê.";
                return Ok(new { success = true, message = msg, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}


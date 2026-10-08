using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _service;

        public PortfolioController(IPortfolioService service)
        {
            _service = service;
        }

        [HttpGet("{maFreelancerStudents:int}")]
        public async Task<IActionResult> Get(int maFreelancerStudents)
        {
            var data = await _service.layPortfolioAsync(maFreelancerStudents);
            return data == null
                ? NotFound(new { success = false, message = "Portfolio không tồn tại" })
                : Ok(new { success = true, data });
        }

        [HttpPut("projects")]
        public async Task<IActionResult> UpdateProject([FromBody] DuAnTrongPortfolio_UpdateRequestDTO request)
        {
            var data = await _service.suaDuAnAsync(request);
            return data == null ? NotFound(new { success = false, message = "Không tìm thấy dự án" }) : Ok(new { success = true, data });
        }

        [HttpDelete("projects/{maDA}")]
        public async Task<IActionResult> DeleteProject(string maDA, [FromQuery] int maFreelancerStudents)
        {
            var data = await _service.xoaDuAnAsync(maDA, maFreelancerStudents);
            return data == null ? NotFound(new { success = false, message = "Không tìm thấy dự án" }) : Ok(new { success = true, data });
        }

        [HttpPut("featured")]
        public async Task<IActionResult> UpdateFeatured([FromBody] DuAnNoiBat_RequestDTO request)
        {
            var data = await _service.capNhatDuAnNoiBatAsync(request);
            return data == null ? NotFound(new { success = false, message = "Portfolio không tồn tại" }) : Ok(new { success = true, data });
        }

        [HttpPost("projects")]
        public async Task<IActionResult> AddProject([FromBody] DuAnTrongPortfolio_RequestDTO request)
        {
            var data = await _service.themDuAnAsync(request);
            return data == null
                ? NotFound(new { success = false, message = "Portfolio không tồn tại cho freelancer này" })
                : Ok(new { success = true, data });
        }
    }
}
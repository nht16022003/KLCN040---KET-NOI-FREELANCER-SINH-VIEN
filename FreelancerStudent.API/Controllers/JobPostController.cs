using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }

        [HttpGet("DanhSachJobPost")]

        public async Task<IActionResult> layDanhSachJobPost()
        {
            try
            {
                //Gọi service
                var ketquads = await _jobPostService.layDanhSachJobPostAsync();

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

        [HttpPost("TaoJob")]
        public async Task<IActionResult> TaoJob([FromBody] JobPost_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ketqua = await _jobPostService.taoJobPostAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Tạo Job Thành Công !!",
                    data = ketqua
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }


        [HttpGet("DanhSachUngTuyen/{maUser}")]
        public async Task<IActionResult> layDSUngTuyen(int maUser)
        {
            try
            {
                var dsResult = await _jobPostService.layDanhSachUngTuyen_JobPost_TheoMaUser(maUser);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách ứng tuyển thành công!",
                    data = dsResult
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
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

        [HttpPost("DuyetUngTuyen")]
        public async Task<IActionResult> DuyetUngTuyen([FromBody] DuyetUngTuyen_RequestDTO request)
        {
            try
            {
                var ketQua = await _jobPostService.duyetUngTuyenAsync(request);
                string thongBao = request.trangThai == "ChapNhan"
                    ? "Đã chấp nhận đơn ứng tuyển thành công!"
                    : "Đã từ chối đơn ứng tuyển!";
                return Ok(new
                {
                    success = true,
                    message = thongBao
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

        //Tuấn
        [HttpPost("UngTuyen")]
        public async Task<IActionResult> NopDonUngTuyen([FromBody] UngTuyen_RequestDTO request)
        {
            try
            {
                await _jobPostService.nopDonUngTuyenAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Nộp đơn ứng tuyển thành công! Nhà tuyển dụng sẽ sớm phản hồi."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        //Tuan
        [HttpGet("CuaToi/{maUser}")]
        public async Task<IActionResult> LayJobCuaToi(int maUser)
        {
            try
            {
                var data = await _jobPostService.layJobPostTheoMaUserAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách thành công!", data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("CapNhat")]
        public async Task<IActionResult> CapNhatJob([FromBody] JobPost_RequestDTO request)
        {
            try
            {
                var ok = await _jobPostService.capNhatJobPostAsync(request);
                return Ok(new { success = ok, message = "Cập nhật bài tuyển dụng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}
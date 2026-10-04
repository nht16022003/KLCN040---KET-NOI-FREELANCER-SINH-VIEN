using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FreelancerStudentController : ControllerBase
    {
        private readonly IFreelancerStudentService _freelancerStudentService;

        public FreelancerStudentController(IFreelancerStudentService freelancerStudentService)
        {
            _freelancerStudentService = freelancerStudentService;
        }
        //XS
        [HttpGet("Profile/{maFreelancerStudents:int}")]
        public async Task<IActionResult> layProfile(int maFreelancerStudents)
        {
            var profile = await _freelancerStudentService
                .layProfileFreelancerStudent(maFreelancerStudents);

            if (profile == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy hồ sơ freelancer."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Lấy hồ sơ freelancer thành công.",
                data = profile
            });
        }

        [HttpGet("DanhSachFreelancerStudent")]

        public async Task<IActionResult> layDanhSachFreelancerStudents()
        {
            try
            {
                //Gọi service
                var ketquads = await _freelancerStudentService.layDanhSachFreelancerStudent();

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


    }
}
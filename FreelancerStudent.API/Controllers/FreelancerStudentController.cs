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
        [HttpGet("maUser")]
        public async Task<IActionResult> layFreelancerTheoMaUSer(int maUser)
        {
            try
            {
                //Gọi service
                var ketquads = await _freelancerStudentService.layFreelancerStudent_TheoMaUser(maUser);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thành công!",
                    data = ketquads
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy: " + ex.Message
                });
            }
        }

        //Tuấn
        [HttpGet("ChiTiet/{id}")]
        public async Task<IActionResult> layChiTietHoSo(int id)
        {
            try
            {
                var ketqua = await _freelancerStudentService.layChiTietHoSoAsync(id);
                if (ketqua == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy hồ sơ Freelancer này." });
                }

                return Ok(new { success = true, data = ketqua });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy hồ sơ: " + ex.Message });
            }
        }

        //Tuấn
        [HttpPut("CapNhatHoSo")]
        public async Task<IActionResult> CapNhatHoSo([FromBody] DTOs.ReponseDTOs.ChiTietHoSoFreelancer_ReponseDTO dto)
        {
            try
            {
                var ketqua = await _freelancerStudentService.capNhatHoSoAsync(dto);
                if (!ketqua)
                {
                    return BadRequest(new { success = false, message = "Cập nhật hồ sơ thất bại!" });
                }

                return Ok(new { success = true, message = "Cập nhật hồ sơ thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi cập nhật hồ sơ: " + ex.Message });
            }
        }

        // Cập nhật thông tin cơ bản: Địa điểm, Ngôn ngữ, Kỹ năng cơ bản, Mức giá tham khảo
        [HttpPut("CapNhatThongTinCoBan")]
        public async Task<IActionResult> CapNhatThongTinCoBan([FromBody] DTOs.RequestDTOs.CapNhatThongTinCoBan_RequestDTO dto)
        {
            try
            {
                if (dto == null || dto.maUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu cập nhật không hợp lệ." });
                }

                var ketqua = await _freelancerStudentService.capNhatThongTinCoBanAsync(dto);
                if (!ketqua)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy hồ sơ sinh viên để cập nhật." });
                }

                return Ok(new { success = true, message = "Cập nhật thông tin cơ bản thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi cập nhật thông tin cơ bản: " + ex.Message });
            }
        }

        // Cập nhật điểm GPA học tập
        [HttpPut("CapNhatGPA")]
        public async Task<IActionResult> CapNhatGPA([FromBody] DTOs.RequestDTOs.CapNhatGPA_RequestDTO dto)
        {
            try
            {
                if (dto == null || dto.maUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu cập nhật không hợp lệ." });
                }

                var ketqua = await _freelancerStudentService.capNhatGPAAsync(dto);
                if (!ketqua)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy hồ sơ sinh viên để cập nhật GPA." });
                }

                return Ok(new { success = true, message = "Cập nhật điểm GPA thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi cập nhật GPA: " + ex.Message });
            }
        }

        //Xuân Sơn
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


    }
}
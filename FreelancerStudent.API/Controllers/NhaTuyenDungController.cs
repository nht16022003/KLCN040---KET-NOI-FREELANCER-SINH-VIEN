using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NhaTuyenDungController : ControllerBase
    {
        private readonly INhaTuyenDungService _nhaTuyenDungService;

        public NhaTuyenDungController(INhaTuyenDungService nhaTuyenDungService)
        {
            _nhaTuyenDungService = nhaTuyenDungService;
        }

        [HttpGet("DanhSachNhaTuyenDung")]

        public async Task<IActionResult> layDanhSachNTD()
        {
            try
            {
                //Gọi service
                var ketquads = await _nhaTuyenDungService.layDanhSachNhaTuyenDungAsync();

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
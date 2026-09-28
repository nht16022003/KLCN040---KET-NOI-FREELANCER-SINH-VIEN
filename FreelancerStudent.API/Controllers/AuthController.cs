using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //api/auth/dangky

        [HttpPost("dangky")]
        public async Task<IActionResult> DangKy([FromBody] DangKy_RequestDTO request) //Nhận vào DangKy_RequestDTO
        {
            //Kiểm tra ràng buộc 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //Gọi service xử lý nghiệp vụ Đăng ký
                var ketqua = await _authService.DangKyTaiKhoanAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Đăng ký thành công !",
                    data = ketqua
                });
            }
            catch (Exception ex)
            {
                //Trả về HTTP BAD REQUEST nếu có lỗi logic
                return BadRequest(
                   new
                   {
                       success = false,
                       message = ex.Message
                   }
                );
            }
        }

        [HttpPost("DangNhap")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhap_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //Gọi service đăng nhập
                var ketqua = await _authService.DangNhapTaiKhoanAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công",
                    data = ketqua
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                  new
                  {
                      success = false,
                      message = ex.Message
                  }
               );
            }
        }


    }
}
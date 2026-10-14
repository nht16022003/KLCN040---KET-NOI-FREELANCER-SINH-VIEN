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



        //Tuấn Anh

        [HttpGet("ThongTinTaiKhoan/{maUser}")]
        public async Task<IActionResult> ThongTinTaiKhoan(int maUser)
        {
            try
            {
                var ketQua =
                    await _authService
                        .layThongTinTaiKhoanAsync(maUser);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin tài khoản thành công!",
                    data = ketQua
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


        //Tuấn Anh

        [HttpPut("CapNhatThongTinTaiKhoan")]
        public async Task<IActionResult> CapNhatThongTinTaiKhoan([FromBody] CapNhatThongTinTaiKhoan_RequestDTO request)
        {
            try
            {
                var ketQua =
                    await _authService
                        .capNhatThongTinTaiKhoanAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thông tin thành công!",
                    data = ketQua
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


        //Tuấn Anh

        [HttpPut("CapNhatAvatar")]
        public async Task<IActionResult> CapNhatAvatar([FromBody] CapNhatAvatar_RequestDTO request)
        {
            try
            {
                await _authService.capNhatAvatarAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật ảnh đại diện thành công!"
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


        //Tuấn Anh
        [HttpPut("DoiMatKhau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhau_RequestDTO request)
        {
            try
            {
                await _authService.doiMatKhauAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công!"
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
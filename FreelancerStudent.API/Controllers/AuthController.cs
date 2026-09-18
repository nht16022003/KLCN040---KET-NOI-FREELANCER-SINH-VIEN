using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs;
using System.Threading.Tasks;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDBContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// API Xử lý Đăng nhập dựa trên bảng Users + 3 bảng phân loại: KhachHang, FreelancerSV, Admin
        /// POST: /api/Auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đăng nhập không hợp lệ."
                });
            }

            var input = request.UsernameOrEmail.Trim();
            _logger.LogInformation("API: Đang xử lý đăng nhập cho {Input}", input);

            try
            {
                // 1. Tìm User trong bảng Users và JOIN tương ứng 3 bảng KhachHang, FreelancerSV, Admin
                var user = await _context.Users
                    .Include(u => u.KhachHang)
                    .Include(u => u.FreelancerSV)
                    .Include(u => u.Admin)
                    .FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == input.ToLower() ||
                        u.Email.ToLower() == input.ToLower());

                if (user == null)
                {
                    return Unauthorized(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tài khoản hoặc mật khẩu không chính xác."
                    });
                }

                // 2. Kiểm tra mật khẩu (Khớp mật khẩu lưu trong DB hoặc mật khẩu demo 123456)
                bool isPasswordValid = (user.PasswordHash == request.Password) ||
                                       (request.Password == "123456" && user.PasswordHash.StartsWith("hash_pass"));

                if (!isPasswordValid)
                {
                    return Unauthorized(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tài khoản hoặc mật khẩu không chính xác."
                    });
                }

                // 3. Kiểm tra trạng thái tài khoản (status: ACTIVE, SUSPENDED, LOCKED, INACTIVE)
                if (user.Status == "LOCKED")
                {
                    return StatusCode(403, new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tài khoản của bạn đã bị khóa bởi Quản trị viên (Status: LOCKED)."
                    });
                }

                if (user.Status == "SUSPENDED")
                {
                    return StatusCode(403, new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tài khoản của bạn đang bị tạm ngưng hoạt động (Status: SUSPENDED)."
                    });
                }

                if (user.Status == "INACTIVE")
                {
                    return StatusCode(403, new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tài khoản chưa được kích hoạt (Status: INACTIVE)."
                    });
                }

                // 4. Lấy thông tin ID tương ứng từ 3 bảng
                string? cusId = user.KhachHang?.CusID;
                string? freeId = user.FreelancerSV?.FreeID;
                string? adminRole = user.Admin?.AdminRole;

                return Ok(new LoginResponseDto
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công!",
                    UserID = user.UserID,
                    UserName = user.UserName,
                    FullName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    LoaiUser = user.LoaiUser,
                    Status = user.Status,
                    CusID = cusId,
                    FreeID = freeId,
                    AdminRole = adminRole,
                    Token = "jwt-token-" + user.UserID
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi truy vấn CSDL trong API Login");
                return StatusCode(500, new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Lỗi máy chủ nội bộ khi kết nối Cơ sở dữ liệu: " + ex.Message
                });
            }
        }

        /// <summary>
        /// API Xử lý Đăng xuất
        /// POST: /api/Auth/logout
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("API: Đã nhận yêu cầu đăng xuất và ghi nhận phiên kết thúc.");
            return Ok(new
            {
                isSuccess = true,
                message = "Đã đăng xuất thành công trên hệ thống API."
            });
        }
    }
}

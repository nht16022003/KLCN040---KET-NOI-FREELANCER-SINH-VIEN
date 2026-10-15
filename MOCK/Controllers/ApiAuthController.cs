using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MOCK.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Yêu cầu đăng ký không hợp lệ."
            });
        }

        var fullName = (request.FullName ?? string.Empty).Trim();
        var userName = (request.UserName ?? string.Empty).Trim();
        var email = (request.Email ?? string.Empty).Trim();
        var password = request.Password ?? string.Empty;
        var roleValue = (request.Role ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            password.Length < 6)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Vui lòng nhập đầy đủ thông tin, tên tài khoản và mật khẩu tối thiểu 6 ký tự."
            });
        }

        if (!IsValidEmail(email))
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Email không hợp lệ."
            });
        }

        var normalizedRole = (roleValue.Equals("NhaTuyenDung", StringComparison.OrdinalIgnoreCase) ||
                              roleValue.Equals("Employer", StringComparison.OrdinalIgnoreCase))
            ? "NhaTuyenDung"
            : "FreelancerStudent";

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new
            {
                isSuccess = false,
                message = "Chuỗi kết nối database chưa được cấu hình."
            });
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var checkSql = @"SELECT 1 FROM Users WHERE LOWER(emailUser) = LOWER(@Email) OR LOWER(tenTaiKhoanUser) = LOWER(@UserName)";
        await using (var checkCommand = new SqlCommand(checkSql, connection))
        {
            checkCommand.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email;
            checkCommand.Parameters.Add("@UserName", SqlDbType.VarChar, 30).Value = userName;

            var exists = await checkCommand.ExecuteScalarAsync();
            if (exists != null)
            {
                return Conflict(new
                {
                    isSuccess = false,
                    message = "Email hoặc tên tài khoản đã tồn tại."
                });
            }
        }

        var roleId = normalizedRole == "NhaTuyenDung" ? 2 : 1;
        var insertSql = @"
            INSERT INTO Users (hotenUser, tenTaiKhoanUser, pashwordHash, emailUser, status, ngayTao, maRole)
            VALUES (@FullName, @UserName, @Password, @Email, N'ACTIVE', @NgayTao, @RoleId);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        int newUserId;
        await using (var insertCommand = new SqlCommand(insertSql, connection))
        {
            insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName;
            insertCommand.Parameters.Add("@UserName", SqlDbType.VarChar, 30).Value = userName;
            insertCommand.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = password;
            insertCommand.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email;
            insertCommand.Parameters.Add("@NgayTao", SqlDbType.DateTime2).Value = DateTime.Now;
            insertCommand.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;

            var insertedId = await insertCommand.ExecuteScalarAsync();
            newUserId = insertedId == null ? 0 : Convert.ToInt32(insertedId);
        }

        if (newUserId <= 0)
        {
            return StatusCode(500, new
            {
                isSuccess = false,
                message = "Đăng ký thất bại khi lưu vào database."
            });
        }

        return Ok(new
        {
            isSuccess = true,
            data = new
            {
                token = $"token_{Guid.NewGuid():N}",
                email = email,
                role = normalizedRole
            },
            message = "Đăng ký thành công."
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Yêu cầu đăng nhập không hợp lệ."
            });
        }

        var email = (request.Email ?? string.Empty).Trim();
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Vui lòng nhập email và mật khẩu."
            });
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(500, new
            {
                isSuccess = false,
                message = "Chuỗi kết nối database chưa được cấu hình."
            });
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT maUser, hotenUser, emailUser, tenTaiKhoanUser, pashwordHash, maRole
            FROM Users
            WHERE LOWER(emailUser) = LOWER(@Email) OR LOWER(tenTaiKhoanUser) = LOWER(@Email)";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email;

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return Unauthorized(new
            {
                isSuccess = false,
                message = "Email hoặc mật khẩu không hợp lệ."
            });
        }

        var storedPassword = reader["pashwordHash"]?.ToString() ?? string.Empty;
        var storedRoleId = reader["maRole"] as int? ?? Convert.ToInt32(reader["maRole"]);
        var userEmail = reader["emailUser"]?.ToString() ?? email;

        if (!string.Equals(storedPassword, password, StringComparison.Ordinal))
        {
            return Unauthorized(new
            {
                isSuccess = false,
                message = "Email hoặc mật khẩu không hợp lệ."
            });
        }

        var role = storedRoleId == 2 ? "NhaTuyenDung" : "FreelancerStudent";

        return Ok(new
        {
            isSuccess = true,
            data = new
            {
                token = $"token_{Guid.NewGuid():N}",
                email = userEmail,
                role = role
            },
            message = "Đăng nhập thành công."
        });
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class RegisterRequest
{
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

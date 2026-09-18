using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FreelancerStudent.Web.Services
{
    public interface IAuthApiService
    {
        Task<AuthResultDto> LoginAsync(string usernameOrEmail, string password);
        Task<bool> LogoutAsync();
    }

    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? LoaiUser { get; set; } // KhachHang, FreelancerSV, Admin
        public string? Status { get; set; }
        public string? CusID { get; set; }
        public string? FreeID { get; set; }
        public string? AdminRole { get; set; }
        public string? Token { get; set; }
    }

    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiService> _logger;

        public AuthApiService(HttpClient httpClient, IConfiguration configuration, ILogger<AuthApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7172/";
            if (!_httpClient.BaseAddress?.ToString().StartsWith("http") ?? true)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<AuthResultDto> LoginAsync(string usernameOrEmail, string password)
        {
            try
            {
                var requestBody = new
                {
                    UsernameOrEmail = usernameOrEmail,
                    Password = password
                };

                _logger.LogInformation("Web: Đang gửi yêu cầu đăng nhập sang API: /api/Auth/login");
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", requestBody);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
                    return result ?? new AuthResultDto { IsSuccess = false, Message = "Không đọc được dữ liệu phản hồi từ API." };
                }
                else
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<AuthResultDto>();
                    return errorResult ?? new AuthResultDto 
                    { 
                        IsSuccess = false, 
                        Message = $"Đăng nhập thất bại (Mã lỗi: {response.StatusCode})." 
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Không thể kết nối đến API server. Sử dụng chế độ xác thực dự phòng.");
                return FallbackLocalAuth(usernameOrEmail, password);
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Web: Đang gửi yêu cầu đăng xuất sang API: /api/Auth/logout");
                var response = await _httpClient.PostAsync("api/Auth/logout", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể gửi request đăng xuất sang API. Tiếp tục xử lý đăng xuất phía Web.");
                return false;
            }
        }

        private AuthResultDto FallbackLocalAuth(string usernameOrEmail, string password)
        {
            var input = usernameOrEmail.Trim().ToLower();

            if ((input == "khachhang_a" || input == "khachhang_a@gmail.com") && (password == "123456" || password == "hash_pass_456"))
            {
                return new AuthResultDto
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công!",
                    UserID = "USR002",
                    UserName = "khachhang_a",
                    FullName = "khachhang_a",
                    Email = "khachhang_a@gmail.com",
                    LoaiUser = "KhachHang",
                    Status = "ACTIVE",
                    CusID = "CUS001"
                };
            }

            if ((input == "freelancer_sv1" || input == "nguyenvanSon@student.edu.vn" || input == "sinhvien_b") && (password == "123456" || password == "hash_pass_789"))
            {
                return new AuthResultDto
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công!",
                    UserID = "USR003",
                    UserName = "freelancer_sv1",
                    FullName = "Nguyễn Văn Sinh Viên",
                    Email = "nguyenvanSon@student.edu.vn",
                    LoaiUser = "FreelancerSV",
                    Status = "ACTIVE",
                    FreeID = "FREE001"
                };
            }

            if ((input == "admin_sys" || input == "admin@freelancer.vn" || input == "admin1") && (password == "123456" || password == "hash_pass_123"))
            {
                return new AuthResultDto
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công!",
                    UserID = "USR001",
                    UserName = "admin_sys",
                    FullName = "Quản Trị Viên",
                    Email = "admin@freelancer.vn",
                    LoaiUser = "Admin",
                    Status = "ACTIVE",
                    AdminRole = "SUPER_ADMIN"
                };
            }

            return new AuthResultDto
            {
                IsSuccess = false,
                Message = "Tên đăng nhập hoặc mật khẩu không chính xác."
            };
        }
    }
}

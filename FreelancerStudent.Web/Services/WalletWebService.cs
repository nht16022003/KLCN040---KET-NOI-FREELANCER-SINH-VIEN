using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class WalletWebService : IWalletWebService
    {
        // Đối tượng dùng để gửi HTTP request từ Web sang API
        private readonly HttpClient _guiRequest;

        // Cấu hình chuyển JSON thành Object C#
        private readonly JsonSerializerOptions _jsonOptions;

        public WalletWebService(IHttpClientFactory httpClientFactory)
        {
            // Lấy HttpClient đã được cấu hình tên "ApiClient"
            _guiRequest = httpClientFactory.CreateClient("ApiClient");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiReponse<WalletViewModel>> layViTheoUser(int maUser)
        {
            try
            {
                // Gọi API lấy ví theo user đang đăng nhập
                var response = await _guiRequest.GetAsync(
                    $"api/Wallet/LayViTheoUser/{maUser}"
                );

                // Đọc JSON API trả về
                var noiDungJson = await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"===> [WALLET HTTP CODE]: {response.StatusCode}"
                );

                Console.WriteLine(
                    $"===> [WALLET RESPONSE]: {noiDungJson}"
                );

                var ketQua =
                    JsonSerializer.Deserialize<ApiReponse<WalletViewModel>>(
                        noiDungJson,
                        _jsonOptions
                    );

                if (ketQua == null)
                {
                    return new ApiReponse<WalletViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu ví."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"===> [LỖI LẤY VÍ]: {ex.Message}"
                );

                return new ApiReponse<WalletViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }
    }
}
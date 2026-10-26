
using System.Net.Http.Json;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

namespace FreelancerStudent.Web.Services
{
    public class BanGiaoSanPhamWebService
        : IBanGiaoSanPhamWebService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BanGiaoSanPhamWebService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = factory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<ApiReponse<T>> GuiAsync<T>(
            HttpMethod method,
            string url,
            object? payload = null)
        {
            var result = new ApiReponse<T>();

            try
            {
                using var request = new HttpRequestMessage(method, url);
                var context = _httpContextAccessor.HttpContext;

                if (context == null ||
                    context.User.Identity?.IsAuthenticated != true)
                {
                    result.success = false;
                    result.message = "Vui lòng đăng nhập lại.";
                    return result;
                }

                var cookieName = "FreelancerStudent.Cookie";

                if (!context.Request.Cookies.TryGetValue(
                        cookieName, out var cookieValue) ||
                    string.IsNullOrWhiteSpace(cookieValue))
                {
                    result.success = false;
                    result.message = "Không tìm thấy Cookie đăng nhập.";
                    return result;
                }

                request.Headers.TryAddWithoutValidation(
                    "Cookie", $"{cookieName}={cookieValue}");
                if (payload != null)
                    request.Content =
                        JsonContent.Create(payload);

                using var response =
                    await _httpClient.SendAsync(request);

                var json =
                    await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var apiResult =
                        JsonSerializer.Deserialize<ApiReponse<T>>(
                            json, options);

                    if (apiResult != null)
                        return apiResult;

                    result.success = false;
                    result.message =
                        "API trả về dữ liệu không hợp lệ.";
                    return result;
                }

                result.success = false;
                result.message =
                    $"Lỗi API ({(int)response.StatusCode}).";

                try
                {
                    using var document =
                        JsonDocument.Parse(json);

                    if (document.RootElement.TryGetProperty(
                        "message", out var message))
                    {
                        result.message =
                            message.GetString()
                            ?? result.message;
                    }
                }
                catch (JsonException)
                {
                    // Giữ thông báo mã HTTP.
                }
            }
            catch (HttpRequestException ex)
            {
                result.success = false;
                result.message =
                    "Không thể kết nối API: " + ex.Message;
            }
            catch (TaskCanceledException)
            {
                result.success = false;
                result.message = "Kết nối API quá thời gian.";
            }

            return result;
        }

        // 1. Freelancer bàn giao sản phẩm
        public Task<ApiReponse<BanGiaoResponseViewModel>>
            TaoBanGiaoAsync(TaoBanGiaoViewModel model)
        {
            return GuiAsync<BanGiaoResponseViewModel>(
                HttpMethod.Post,
                "api/BanGiaoSanPham",
                model);
        }

        // 2. Lấy lịch sử bàn giao
        public Task<ApiReponse<List<BanGiaoResponseViewModel>>>
            LayDanhSachAsync(string maHD)
        {
            return GuiAsync<List<BanGiaoResponseViewModel>>(
                HttpMethod.Get,
                "api/BanGiaoSanPham/hopdong/"
                    + Uri.EscapeDataString(maHD));
        }

        // 3. Khách hàng gửi yêu cầu chỉnh sửa
        public Task<ApiReponse<YeuCauChinhSuaResponseViewModel>>
            GuiYeuCauAsync(TaoYeuCauChinhSuaViewModel model)
        {
            return GuiAsync<YeuCauChinhSuaResponseViewModel>(
                HttpMethod.Post,
                "api/BanGiaoSanPham/yeucauchinhsua",
                model);
        }

        // 4. Lấy lịch sử yêu cầu chỉnh sửa
        public Task<ApiReponse<List<YeuCauChinhSuaResponseViewModel>>>
            LayYeuCauAsync(string maHD)
        {
            return GuiAsync<
                List<YeuCauChinhSuaResponseViewModel>>(
                HttpMethod.Get,
                "api/BanGiaoSanPham/yeucauchinhsua/"
                    + Uri.EscapeDataString(maHD));
        }

        // 5. Khách hàng nghiệm thu sản phẩm
        public Task<ApiReponse<BanGiaoResponseViewModel>>
            NghiemThuAsync(int maBanGiao)
        {
            return GuiAsync<BanGiaoResponseViewModel>(
                HttpMethod.Put,
                $"api/BanGiaoSanPham/nghiemthu/{maBanGiao}");
        }
    }
}

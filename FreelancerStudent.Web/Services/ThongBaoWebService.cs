using System.Net.Http.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class ThongBaoWebService : IThongBaoWebService
    {
        private readonly HttpClient _httpClient;

        public ThongBaoWebService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public Task<ApiReponse<ThongBaoDanhSachViewModel>> LayDanhSachAsync(int maUser, int trang)
            => GetAsync<ThongBaoDanhSachViewModel>($"api/ThongBao?maUser={maUser}&trang={trang}");

        public Task<ApiReponse<int>> DemChuaDocAsync(int maUser)
            => GetAsync<int>($"api/ThongBao/chua-doc?maUser={maUser}");

        public Task<ApiReponse<bool>> DanhDauDaDocAsync(int maThongBao, int maUser)
            => PutAsync<bool>($"api/ThongBao/{maThongBao}/da-doc?maUser={maUser}");

        public Task<ApiReponse<int>> DanhDauTatCaDaDocAsync(int maUser)
            => PutAsync<int>($"api/ThongBao/da-doc-tat-ca?maUser={maUser}");

        private async Task<ApiReponse<T>> GetAsync<T>(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);
                return await ReadAsync<T>(response);
            }
            catch (Exception ex) { return new ApiReponse<T> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        private async Task<ApiReponse<T>> PutAsync<T>(string url)
        {
            try
            {
                using var response = await _httpClient.PutAsync(url, null);
                return await ReadAsync<T>(response);
            }
            catch (Exception ex) { return new ApiReponse<T> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        private static async Task<ApiReponse<T>> ReadAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ApiReponse<T>>();
                if (result != null)
                    return result;
            }
            catch { }
            return new ApiReponse<T> { success = false, message = $"Lỗi từ máy chủ ({(int)response.StatusCode})." };
        }
    }
}

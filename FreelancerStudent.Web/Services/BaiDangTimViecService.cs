using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class BaiDangTimViecWebService : IBaiDangTimViecWebService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public BaiDangTimViecWebService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        //Tuấn
        public async Task<ApiReponse<BaiDangTimViecViewModel>> taoBaiDangAsync(DangTinTimViecViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/BaiDangTimViec/TaoBaiDang", content);
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<BaiDangTimViecViewModel>>(data, _jsonOptions)
                    ?? new ApiReponse<BaiDangTimViecViewModel> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<BaiDangTimViecViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }
        //Tuấn

        public async Task<ApiReponse<List<BaiDangTimViecViewModel>>> layDanhSachCuaToiAsync(int maUser)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/BaiDangTimViec/CuaToi/{maUser}");
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<BaiDangTimViecViewModel>>>(data, _jsonOptions)
                    ?? new ApiReponse<List<BaiDangTimViecViewModel>> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<BaiDangTimViecViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }
        //Tuấn

        public async Task<ApiReponse<List<BaiDangTimViecViewModel>>> layTatCaBaiDangAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/BaiDangTimViec/TatCa");
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<BaiDangTimViecViewModel>>>(data, _jsonOptions)
                    ?? new ApiReponse<List<BaiDangTimViecViewModel>> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<BaiDangTimViecViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }
        //Tuấn

        public async Task<ApiReponse<bool>> doiTrangThaiAsync(string maBaiDang, string trangThai)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/BaiDangTimViec/DoiTrangThai?maBaiDang={maBaiDang}&trangThai={trangThai}", null);
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(data, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }
        //Tuấn

        public async Task<ApiReponse<bool>> xoaBaiDangAsync(string maBaiDang)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/BaiDangTimViec/Xoa/{maBaiDang}");
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(data, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        //Tuấn
        public async Task<ApiReponse<bool>> capNhatBaiDangAsync(string maBaiDang, DangTinTimViecViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/BaiDangTimViec/CapNhat/{maBaiDang}", content);
                var data = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(data, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Lỗi đọc dữ liệu từ API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

    }
}

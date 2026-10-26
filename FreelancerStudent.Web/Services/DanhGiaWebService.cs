using System.Net.Http.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class DanhGiaWebService : IDanhGiaWebService
    {
        private readonly HttpClient _httpClient;

        public DanhGiaWebService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public Task<ApiReponse<DanhGiaFormViewModel>> LayFormAsync(string maHD, int maUser)
            => GetAsync<DanhGiaFormViewModel>($"api/DanhGia/hop-dong/{Uri.EscapeDataString(maHD)}?maUser={maUser}");

        public Task<ApiReponse<List<HopDongDanhGiaViewModel>>> LayCongViecDaThueAsync(int maUser)
            => GetAsync<List<HopDongDanhGiaViewModel>>($"api/DanhGia/cong-viec-da-thue?maUser={maUser}");

        public Task<ApiReponse<DanhGiaThongKeViewModel>> LayFreelancerAsync(int id, int trang, int kichThuocTrang)
            => GetAsync<DanhGiaThongKeViewModel>($"api/DanhGia/freelancer/{id}?trang={trang}&kichThuocTrang={kichThuocTrang}");

        public Task<ApiReponse<DanhGiaThongKeViewModel>> LayNhaTuyenDungAsync(int id, int trang, int kichThuocTrang)
            => GetAsync<DanhGiaThongKeViewModel>($"api/DanhGia/nha-tuyen-dung/{id}?trang={trang}&kichThuocTrang={kichThuocTrang}");

        public async Task<ApiReponse<DanhGiaItemViewModel>> GuiAsync(DanhGiaRequestViewModel request, int maUser)
            => await PostAsync<DanhGiaItemViewModel>($"api/DanhGia?maUser={maUser}", request);

        public async Task<ApiReponse<DanhGiaItemViewModel>> CapNhatAsync(int maDanhGia, DanhGiaRequestViewModel request, int maUser)
            => await PutAsync<DanhGiaItemViewModel>($"api/DanhGia/{maDanhGia}?maUser={maUser}", request);

        private async Task<ApiReponse<T>> GetAsync<T>(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);
                return await ReadAsync<T>(response);
            }
            catch (Exception ex) { return new ApiReponse<T> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        private async Task<ApiReponse<T>> PostAsync<T>(string url, object payload)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(url, payload);
                return await ReadAsync<T>(response);
            }
            catch (Exception ex) { return new ApiReponse<T> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        private async Task<ApiReponse<T>> PutAsync<T>(string url, object payload)
        {
            try
            {
                using var response = await _httpClient.PutAsJsonAsync(url, payload);
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

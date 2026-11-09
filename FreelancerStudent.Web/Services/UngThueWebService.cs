using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class UngThueWebService : IUngThueWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public UngThueWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiReponse<YeuCauThueViewModel>> GuiYeuCauThueAsync(GuiYeuCauThueRequestModel model, int maUserNTD)
        {
            try
            {
                var payload = new
                {
                    maUserNTD = maUserNTD,
                    maFreelancerStudent = model.maFreelancerStudent,
                    maJob = model.maJob,
                    tieuDeCongViec = model.tieuDeCongViec,
                    moTaCongViec = model.moTaCongViec,
                    nganSachDeNghi = model.nganSachDeNghi,
                    thoiHanDuKien = model.thoiHanDuKien
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/UngThue/GuiYeuCau", content);
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<YeuCauThueViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<YeuCauThueViewModel>>> LayDanhSachYeuCauThueTheoNhaTuyenDungAsync(int maUserNTD)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/UngThue/DanhSachTheoNTD/{maUserNTD}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<YeuCauThueViewModel>>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<List<YeuCauThueViewModel>> { success = false, message = "Lỗi phản hồi từ máy chủ", data = new List<YeuCauThueViewModel>() };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<YeuCauThueViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message, data = new List<YeuCauThueViewModel>() };
            }
        }

        public async Task<ApiReponse<List<YeuCauThueViewModel>>> LayDanhSachYeuCauThueTheoFreelancerAsync(int maUserFreelancer)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/UngThue/DanhSachTheoFreelancer/{maUserFreelancer}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<YeuCauThueViewModel>>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<List<YeuCauThueViewModel>> { success = false, message = "Lỗi phản hồi từ máy chủ", data = new List<YeuCauThueViewModel>() };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<YeuCauThueViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message, data = new List<YeuCauThueViewModel>() };
            }
        }

        public async Task<ApiReponse<YeuCauThueViewModel>> LayChiTietYeuCauThueAsync(int maYeuCau)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/UngThue/{maYeuCau}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<YeuCauThueViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<YeuCauThueViewModel>> PhanHoiYeuCauThueAsync(int maYeuCau, bool chapNhan, string? lyDoTuChoi, int maUserFreelancer)
        {
            try
            {
                var payload = new
                {
                    maYeuCau = maYeuCau,
                    maUserFreelancer = maUserFreelancer,
                    chapNhan = chapNhan,
                    lyDoTuChoi = lyDoTuChoi
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PutAsync("api/UngThue/PhanHoi", content);
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<YeuCauThueViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<YeuCauThueViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }
    }
}


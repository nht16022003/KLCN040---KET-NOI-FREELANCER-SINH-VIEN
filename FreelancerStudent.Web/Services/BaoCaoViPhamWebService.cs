using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class BaoCaoViPhamWebService : IBaoCaoViPhamWebService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public BaoCaoViPhamWebService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _client = factory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiReponse<List<BaoCaoViPhamViewModel>>> LayDanhSachAsync() => GetAsync<List<BaoCaoViPhamViewModel>>("api/BaoCaoViPham");
        public Task<ApiReponse<BaoCaoViPhamChiTietViewModel>> LayChiTietAsync(int maBaoCao) => GetAsync<BaoCaoViPhamChiTietViewModel>($"api/BaoCaoViPham/{maBaoCao}");
        public Task<ApiReponse<LichSuDiemUyTinViewModel>> LayLichSuDiemUyTinAsync() => GetAsync<LichSuDiemUyTinViewModel>("api/BaoCaoViPham/lich-su-diem");
        public Task<ApiReponse<List<KhieuNaiViPhamViewModel>>> LayDanhSachKhieuNaiAsync() => GetAsync<List<KhieuNaiViPhamViewModel>>("api/BaoCaoViPham/khieu-nai");

        public Task<ApiReponse<bool>> TaoBaoCaoAsync(TaoBaoCaoViPhamViewModel model) => PostFormAsync("api/BaoCaoViPham/tao", form =>
        {
            form.Add(new StringContent(model.MaHD), "MaHD");
            form.Add(new StringContent(model.LoaiViPham), "LoaiViPham");
            form.Add(new StringContent(model.MoTa), "MoTa");
            if (model.ThoiGianViPham.HasValue) form.Add(new StringContent(model.ThoiGianViPham.Value.ToString("O")), "ThoiGianViPham");
            AddFiles(form, model.TepMinhChungs);
        });

        public Task<ApiReponse<bool>> GuiGiaiTrinhAsync(GiaiTrinhViPhamViewModel model) => PostFormAsync($"api/BaoCaoViPham/{model.MaBaoCao}/giai-trinh", form =>
        {
            form.Add(new StringContent(model.DongYNoiDung.ToString().ToLowerInvariant()), "DongYNoiDung");
            form.Add(new StringContent(model.NoiDung), "NoiDung");
            form.Add(new StringContent(model.DongYGiaHan.ToString().ToLowerInvariant()), "DongYGiaHan");
            AddFiles(form, model.TepMinhChungs);
        });

        public Task<ApiReponse<bool>> PhanHoiAsync(PhanHoiViPhamViewModel model) => PostFormAsync($"api/BaoCaoViPham/{model.MaBaoCao}/phan-hoi", form =>
        {
            form.Add(new StringContent(model.NoiDung), "NoiDung");
            form.Add(new StringContent(model.DongYGiaHan.ToString().ToLowerInvariant()), "DongYGiaHan");
            AddFiles(form, model.TepMinhChungs);
        });

        public Task<ApiReponse<bool>> GuiKhieuNaiAsync(GuiKhieuNaiViPhamViewModel model) => PostFormAsync($"api/BaoCaoViPham/{model.MaBaoCao}/khieu-nai", form =>
        {
            form.Add(new StringContent(model.NoiDung), "NoiDung");
            AddFiles(form, model.TepMinhChungs);
        });

        public Task<ApiReponse<bool>> KetLuanAsync(KetLuanViPhamViewModel model) => PostJsonAsync($"api/BaoCaoViPham/{model.MaBaoCao}/ket-luan", new
        {
            model.KetLuan,
            model.MucDoViPham,
            model.GhiChu,
            model.SoNgayGiaHan
        });

        public Task<ApiReponse<bool>> KetLuanKhieuNaiAsync(KetLuanKhieuNaiViewModel model) => PostJsonAsync($"api/BaoCaoViPham/khieu-nai/{model.MaKhieuNai}/ket-luan", new
        {
            model.ChapNhan,
            model.KetQua
        });

        private async Task<ApiReponse<T>> GetAsync<T>(string url)
        {
            using var request = CreateRequest(HttpMethod.Get, url);
            using var response = await _client.SendAsync(request);
            return await ReadResponseAsync<T>(response);
        }

        private async Task<ApiReponse<bool>> PostFormAsync(string url, Action<MultipartFormDataContent> addFields)
        {
            using var request = CreateRequest(HttpMethod.Post, url);
            using var form = new MultipartFormDataContent();
            addFields(form);
            request.Content = form;
            using var response = await _client.SendAsync(request);
            return await ReadActionResponseAsync(response);
        }

        private async Task<ApiReponse<bool>> PostJsonAsync<TBody>(string url, TBody body)
        {
            using var request = CreateRequest(HttpMethod.Post, url);
            request.Content = JsonContent.Create(body);
            using var response = await _client.SendAsync(request);
            return await ReadActionResponseAsync(response);
        }

        private async Task<ApiReponse<bool>> ReadActionResponseAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;
                var success = root.TryGetProperty("success", out var successValue) && successValue.GetBoolean();
                var message = root.TryGetProperty("message", out var messageValue) ? messageValue.GetString() : null;
                return new ApiReponse<bool> { success = success && response.IsSuccessStatusCode, message = message, data = success && response.IsSuccessStatusCode };
            }
            catch (JsonException)
            {
                return new ApiReponse<bool> { success = false, message = response.IsSuccessStatusCode ? "Dữ liệu API không hợp lệ." : $"Lỗi API ({(int)response.StatusCode})." };
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            var cookie = _httpContextAccessor.HttpContext?.Request.Headers.Cookie.ToString();
            if (!string.IsNullOrWhiteSpace(cookie)) request.Headers.TryAddWithoutValidation("Cookie", cookie);
            return request;
        }

        private async Task<ApiReponse<T>> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<ApiReponse<T>>(content, _jsonOptions);
                return result ?? new ApiReponse<T> { success = false, message = "Không đọc được phản hồi từ API." };
            }
            catch (JsonException)
            {
                return new ApiReponse<T> { success = false, message = response.IsSuccessStatusCode ? "Dữ liệu API không hợp lệ." : $"Lỗi API ({(int)response.StatusCode})." };
            }
        }

        private static void AddFiles(MultipartFormDataContent form, IEnumerable<IFormFile> files)
        {
            foreach (var file in files.Where(x => x.Length > 0))
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                if (!string.IsNullOrWhiteSpace(file.ContentType))
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                form.Add(streamContent, "TepMinhChungs", Path.GetFileName(file.FileName));
            }
        }
    }
}

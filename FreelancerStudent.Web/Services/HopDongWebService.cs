
using System.Net.Http.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class HopDongWebService : IHopDongWebService
    {
        private readonly HttpClient _httpClient;

        public HopDongWebService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public async Task<List<HopDongViewModel>>
            LayDanhSachAsync(
                int maNhaTuyenDung,
                string? tuKhoa,
                string? trangThai)
        {
            var url =
                $"api/HopDong/danh-sach/{maNhaTuyenDung}";

            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                parameters.Add(
                    "TuKhoa=" + Uri.EscapeDataString(tuKhoa));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                parameters.Add(
                    "TrangThai=" + Uri.EscapeDataString(trangThai));
            }

            if (parameters.Count > 0)
            {
                url += "?" + string.Join("&", parameters);
            }

            using var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<HopDongViewModel>>()
                ?? new List<HopDongViewModel>();
        }
    }
}

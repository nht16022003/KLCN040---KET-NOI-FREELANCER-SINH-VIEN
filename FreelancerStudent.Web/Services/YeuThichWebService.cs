using System.Text.Json;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.Services.Interfaces;

namespace FreelancerStudent.Web.Services
{
    public class YeuThichWebService : IYeuThichWebService
    {
        private readonly HttpClient _guiRequest;

        public YeuThichWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");
        }

        #region Freelancer Lưu việc

        public async Task<(bool success, string message)> LuuViecAsync(int maUser, string maJob, string? ghiChu = null)
        {
            try
            {
                var body = new { maUser, maJob, ghiChu };
                var response = await _guiRequest.PostAsJsonAsync("api/YeuThich/luu-viec", body);
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                bool success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                string message = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi kết nối: " + ex.Message);
            }
        }

        public async Task<(bool success, string message)> BoLuuViecAsync(int maUser, string maJob)
        {
            try
            {
                var response = await _guiRequest.DeleteAsync($"api/YeuThich/bo-luu-viec?maUser={maUser}&maJob={Uri.EscapeDataString(maJob)}");
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                bool success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                string message = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi kết nối: " + ex.Message);
            }
        }

        public async Task<bool> KiemTraLuuViecAsync(int maUser, string maJob)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/YeuThich/kiem-tra-luu-viec?maUser={maUser}&maJob={Uri.EscapeDataString(maJob)}");
                if (!response.IsSuccessStatusCode) return false;
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var d))
                {
                    return d.GetBoolean();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CongViecDaLuuViewModel>> LayDanhSachViecDaLuuAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/YeuThich/danh-sach-viec-da-luu/{maUser}");
                if (!response.IsSuccessStatusCode) return new List<CongViecDaLuuViewModel>();
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<CongViecDaLuuViewModel>>(data.GetRawText(), options) ?? new();
                }
                return new List<CongViecDaLuuViewModel>();
            }
            catch
            {
                return new List<CongViecDaLuuViewModel>();
            }
        }

        #endregion

        #region Nhà tuyển dụng lưu Freelancer

        public async Task<(bool success, string message)> LuuFreelancerAsync(int maUser, int maFreelancerStudent, string? ghiChu = null)
        {
            try
            {
                var body = new { maUser, maFreelancerStudent, ghiChu };
                var response = await _guiRequest.PostAsJsonAsync("api/YeuThich/luu-freelancer", body);
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                bool success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                string message = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi kết nối: " + ex.Message);
            }
        }

        public async Task<(bool success, string message)> BoLuuFreelancerAsync(int maUser, int maFreelancerStudent)
        {
            try
            {
                var response = await _guiRequest.DeleteAsync($"api/YeuThich/bo-luu-freelancer?maUser={maUser}&maFreelancerStudent={maFreelancerStudent}");
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                bool success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                string message = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, "Lỗi kết nối: " + ex.Message);
            }
        }

        public async Task<bool> KiemTraLuuFreelancerAsync(int maUser, int maFreelancerStudent)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/YeuThich/kiem-tra-luu-freelancer?maUser={maUser}&maFreelancerStudent={maFreelancerStudent}");
                if (!response.IsSuccessStatusCode) return false;
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var d))
                {
                    return d.GetBoolean();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<FreelancerYeuThichViewModel>> LayDanhSachFreelancerYeuThichAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/YeuThich/danh-sach-freelancer-yeu-thich/{maUser}");
                if (!response.IsSuccessStatusCode) return new List<FreelancerYeuThichViewModel>();
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<FreelancerYeuThichViewModel>>(data.GetRawText(), options) ?? new();
                }
                return new List<FreelancerYeuThichViewModel>();
            }
            catch
            {
                return new List<FreelancerYeuThichViewModel>();
            }
        }

        #endregion
    }
}


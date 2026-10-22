
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

        public async Task<List<HopDongViewModel>>
            LayDanhSachTheoFreelancerAsync(
                int maFreelancerStudent,
                string? tuKhoa,
                string? trangThai)
        {
            var url =
                $"api/HopDong/danh-sach-freelancer/{maFreelancerStudent}";

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

        public async Task<ApiReponse<HopDongViewModel>> TaoDuThaoHopDongAsync(
            TaoHopDongViewModel model,
            int maUserNTD)
        {
            var result = new ApiReponse<HopDongViewModel>();

            try
            {
                var payload = new
                {
                    maJob = model.maJob,
                    maFreelancerStudent = model.maFreelancerStudent,
                    sotienkyquy = model.sotienkyquy,
                    ngayBatDau = model.ngayBatDau ?? DateTime.Now,
                    ngayKetThuc = model.ngayKetThuc,
                    hinhthuclamviec = model.hinhthuclamviec,
                    dieuKhoan = model.dieuKhoan,
                    maUngTuyen = model.maUngTuyen,
                    giaiDoans = model.giaiDoans?.Where(g => !string.IsNullOrWhiteSpace(g.tenGiaiDoan)).Select(g => new
                    {
                        tenGiaiDoan = g.tenGiaiDoan.Trim(),
                        moTa = g.moTa,
                        hanChot = g.hanChot ?? model.ngayKetThuc,
                        tyLeGiaiNgan = g.tyLeGiaiNgan,
                        soTien = g.soTien
                    }).ToList()
                };

                var url = $"api/HopDong/tao-hop-dong?maUserNTD={maUserNTD}";
                using var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<HopDongViewModel>();
                    result.success = true;
                    result.data = data;
                    result.message = "Khởi tạo dự thảo hợp đồng thành công!";
                    return result;
                }

                // Đọc lỗi từ API
                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    if (errorObj != null && errorObj.TryGetValue("message", out var msg))
                    {
                        result.message = msg.ToString() ?? "Có lỗi xảy ra khi tạo hợp đồng.";
                    }
                    else
                    {
                        result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode})";
                    }
                }
                catch
                {
                    result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode})";
                }

                result.success = false;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
            }

            return result;
        }

        public async Task<ApiReponse<bool>> NghiemThuAsync(string maHD, int maUserNTD)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync($"api/HopDong/nghiem-thu?maUser={maUserNTD}", new { maHD });
                return await ReadApiResponseAsync<bool>(response);
            }
            catch (Exception ex) { return new ApiReponse<bool> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        public async Task<ApiReponse<HopDongThanhToanViewModel>> LayThongTinThanhToanAsync(string maHD, int maUserNTD)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"api/HopDong/thanh-toan/{Uri.EscapeDataString(maHD)}?maUser={maUserNTD}");
                return await ReadApiResponseAsync<HopDongThanhToanViewModel>(response);
            }
            catch (Exception ex) { return new ApiReponse<HopDongThanhToanViewModel> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        public async Task<ApiReponse<bool>> ThanhToanAsync(string maHD, int maUserNTD)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync($"api/HopDong/thanh-toan?maUser={maUserNTD}", new { maHD });
                return await ReadApiResponseAsync<bool>(response);
            }
            catch (Exception ex) { return new ApiReponse<bool> { success = false, message = "Không thể kết nối đến máy chủ: " + ex.Message }; }
        }

        private static async Task<ApiReponse<T>> ReadApiResponseAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ApiReponse<T>>();
                if (result != null)
                    return result;
            }
            catch
            {
            }
            return new ApiReponse<T> { success = false, message = $"Lỗi từ máy chủ ({(int)response.StatusCode})." };
        }

        public async Task<HopDongViewModel?> LayChiTietTheoJobAsync(string maJob)
        {
            try
            {
                var url = $"api/HopDong/theo-job/{Uri.EscapeDataString(maJob)}";
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<HopDongViewModel>();
                }
            }
            catch
            {
                // Bỏ qua lỗi và trả về null
            }

            return null;
        }

        public async Task<HopDongViewModel?> LayChiTietTheoMaAsync(string maHD)
        {
            try
            {
                var url = $"api/HopDong/chi-tiet/{Uri.EscapeDataString(maHD)}";
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<HopDongViewModel>();
                }
            }
            catch
            {
                // Bỏ qua lỗi và trả về null
            }

            return null;
        }

        public async Task<ApiReponse<bool>> XacNhanHopDongAsync(
            string maHD,
            bool dongY,
            string? lyDoTuChoi,
            int maUserFreelancer)
        {
            var result = new ApiReponse<bool>();

            try
            {
                var payload = new
                {
                    maHD = maHD,
                    dongY = dongY,
                    lyDoTuChoi = lyDoTuChoi
                };

                var url = $"api/HopDong/xac-nhan?maUserFreelancer={maUserFreelancer}";
                using var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    result.success = true;
                    result.data = true;
                    result.message = dongY ? "Xác nhận dự thảo hợp đồng thành công!" : "Đã từ chối dự thảo hợp đồng.";
                    return result;
                }

                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    if (errorObj != null && errorObj.TryGetValue("message", out var msg))
                    {
                        result.message = msg.ToString() ?? "Có lỗi xảy ra khi xử lý hợp đồng.";
                    }
                    else
                    {
                        result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode})";
                    }
                }
                catch
                {
                    result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode})";
                }

                result.success = false;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
            }

            return result;
        }

        public async Task<ApiReponse<bool>> KyQuyHopDongAsync(
            string maHD,
            int maUserNhaTuyenDung)
        {
            var result = new ApiReponse<bool>();

            try
            {
                var payload = new
                {
                    maHD = maHD
                };

                var url = $"api/HopDong/KyQuy?maUserNhaTuyenDung={maUserNhaTuyenDung}";
                using var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    result.success = true;
                    result.data = true;
                    result.message = "Ký quỹ hợp đồng thành công! Hợp đồng đã được kích hoạt sang trạng thái Đang thực hiện.";
                    return result;
                }

                var errorBody = await response.Content.ReadAsStringAsync();
                try
                {
                    using var document = System.Text.Json.JsonDocument.Parse(errorBody);
                    if (document.RootElement.TryGetProperty("message", out var messageElement))
                        result.message = messageElement.GetString() ?? $"Lỗi từ máy chủ ({(int)response.StatusCode}).";
                    else
                        result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode}).";
                }
                catch (System.Text.Json.JsonException)
                {
                    result.message = $"Lỗi từ máy chủ ({(int)response.StatusCode}).";
                }

                result.success = false;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến API ký quỹ. Vui lòng kiểm tra API đang chạy và nhật ký máy chủ.";
            }

            return result;
        }
    }
}
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class MinhChungWebService : IMinhChungWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<MinhChungWebService> _logger;

        public MinhChungWebService(IHttpClientFactory httpClientFactory, ILogger<MinhChungWebService> logger)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiReponse<QuetTheSinhVienResultViewModel>> QuetTheSinhVienAsync(IFormFile file, int maUser)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType);
                formData.Add(streamContent, "file", file.FileName);

                var response = await _guiRequest.PostAsync($"api/MinhChung/quet-the-sinh-vien?maUser={maUser}", formData);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errObj = JsonSerializer.Deserialize<ApiReponse<QuetTheSinhVienResultViewModel>>(content, _jsonOptions);
                    return new ApiReponse<QuetTheSinhVienResultViewModel>
                    {
                        success = false,
                        message = errObj?.message ?? $"Lỗi máy chủ ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<QuetTheSinhVienResultViewModel>>(content, _jsonOptions)
                    ?? new ApiReponse<QuetTheSinhVienResultViewModel> { success = false, message = "Không thể đọc dữ liệu phản hồi từ OCR." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API quét thẻ sinh viên");
                return new ApiReponse<QuetTheSinhVienResultViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối tới hệ thống OCR: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<bool>> XacNhanTheSinhVienAsync(XacNhanTheSinhVienViewModel model)
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/MinhChung/xac-nhan-the-sinh-vien", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errObj = JsonSerializer.Deserialize<ApiReponse<bool>>(content, _jsonOptions);
                    return new ApiReponse<bool>
                    {
                        success = false,
                        message = errObj?.message ?? $"Lỗi cập nhật hồ sơ ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<bool>>(content, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Không nhận được phản hồi từ hệ thống." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi xác nhận thẻ sinh viên");
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<List<MinhChungItemViewModel>>> LayLichSuMinhChungAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/MinhChung/lich-su/{maUser}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<List<MinhChungItemViewModel>>
                    {
                        success = false,
                        message = $"Không thể tải lịch sử minh chứng ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<List<MinhChungItemViewModel>>>(content, _jsonOptions)
                    ?? new ApiReponse<List<MinhChungItemViewModel>> { success = false, message = "Không thể phân tích dữ liệu lịch sử." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử minh chứng");
                return new ApiReponse<List<MinhChungItemViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<List<ChuyenNganhOptionViewModel>>> LayDanhSachChuyenNganhAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/MinhChung/chuyen-nganh");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<List<ChuyenNganhOptionViewModel>>
                    {
                        success = false,
                        message = $"Không thể tải danh sách chuyên ngành ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<List<ChuyenNganhOptionViewModel>>>(content, _jsonOptions)
                    ?? new ApiReponse<List<ChuyenNganhOptionViewModel>> { success = false, message = "Không thể phân tích danh sách chuyên ngành." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách chuyên ngành");
                return new ApiReponse<List<ChuyenNganhOptionViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<ThongTinHienTaiViewModel>> LayThongTinHienTaiAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/MinhChung/thong-tin-hien-tai/{maUser}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<ThongTinHienTaiViewModel>
                    {
                        success = false,
                        message = $"Không thể tải thông tin sinh viên ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<ThongTinHienTaiViewModel>>(content, _jsonOptions)
                    ?? new ApiReponse<ThongTinHienTaiViewModel> { success = false, message = "Không thể phân tích thông tin sinh viên." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sinh viên");
                return new ApiReponse<ThongTinHienTaiViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<KiemTraXacThucViewModel>> KiemTraTrangThaiXacThucAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/MinhChung/kiem-tra-xac-thuc/{maUser}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<KiemTraXacThucViewModel>
                    {
                        success = false,
                        message = $"Lỗi kiểm tra xác thực ({response.StatusCode})",
                        data = new KiemTraXacThucViewModel { DaXacMinh = false, TrangThai = "Loi", ThongBao = "Không thể kiểm tra trạng thái xác thực." }
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<KiemTraXacThucViewModel>>(content, _jsonOptions)
                    ?? new ApiReponse<KiemTraXacThucViewModel> { success = false, message = "Không thể đọc dữ liệu phản hồi." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra trạng thái xác thực minh chứng sinh viên");
                return new ApiReponse<KiemTraXacThucViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message,
                    data = new KiemTraXacThucViewModel { DaXacMinh = false, TrangThai = "Loi", ThongBao = "Lỗi kết nối máy chủ." }
                };
            }
        }

        public async Task<ApiReponse<bool>> PheDuyetMinhChungAsync(int maMinhChung, bool chapNhan, string? lyDoTuChoi)
        {
            return await PheDuyetMinhChungAsync(new PheDuyetMinhChungRequestViewModel
            {
                MaMinhChung = maMinhChung,
                ChapNhan = chapNhan,
                LyDoTuChoi = lyDoTuChoi
            });
        }

        public async Task<ApiReponse<bool>> PheDuyetMinhChungAsync(PheDuyetMinhChungRequestViewModel request)
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(request, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/MinhChung/phe-duyet", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<bool>
                    {
                        success = false,
                        message = $"Lỗi phê duyệt ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<bool>>(content, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Không nhận được phản hồi từ hệ thống." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi phê duyệt minh chứng");
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<List<MinhChungItemViewModel>>> LayDanhSachChoDuyetAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/MinhChung/danh-sach-cho-duyet");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<List<MinhChungItemViewModel>>
                    {
                        success = false,
                        message = $"Lỗi lấy danh sách chờ duyệt ({response.StatusCode})"
                    };
                }

                return JsonSerializer.Deserialize<ApiReponse<List<MinhChungItemViewModel>>>(content, _jsonOptions)
                    ?? new ApiReponse<List<MinhChungItemViewModel>> { success = false, message = "Không đọc được danh sách chờ duyệt." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách minh chứng chờ duyệt");
                return new ApiReponse<List<MinhChungItemViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ: " + ex.Message
                };
            }
        }
    }
}



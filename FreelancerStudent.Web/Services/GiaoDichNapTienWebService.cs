using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class GiaoDichNapTienWebService : IGiaoDichNapTienWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public GiaoDichNapTienWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<ApiReponse<GiaoDichNapTienViewModel>> taoGiaoDichNapTien(
            int maUser,
            TaoGiaoDichNapTienRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _guiRequest.PostAsync(
                    $"api/GiaoDichNapTien/TaoGiaoDich/{maUser}",
                    content
                );

                var noiDungJson =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"===> [NAP TIEN HTTP CODE]: {response.StatusCode}"
                );

                Console.WriteLine(
                    $"===> [NAP TIEN RESPONSE]: {noiDungJson}"
                );

                var ketQua =
                    JsonSerializer.Deserialize<
                        ApiReponse<GiaoDichNapTienViewModel>
                    >(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<GiaoDichNapTienViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu giao dịch."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<GiaoDichNapTienViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }


        // =====================================================
        // LẤY TRẠNG THÁI GIAO DỊCH NẠP TIỀN
        // =====================================================
        public async Task<ApiReponse<GiaoDichNapTienViewModel>> layTrangThaiGiaoDich(
            string maGiaoDich)
        {
            try
            {
                var response = await _guiRequest.GetAsync(
                    $"api/GiaoDichNapTien/TrangThai/{maGiaoDich}"
                );

                var noiDungJson =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"===> [TRANG THAI NAP TIEN HTTP CODE]: {response.StatusCode}"
                );

                Console.WriteLine(
                    $"===> [TRANG THAI NAP TIEN RESPONSE]: {noiDungJson}"
                );

                var ketQua =
                    JsonSerializer.Deserialize<
                        ApiReponse<GiaoDichNapTienViewModel>
                    >(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<GiaoDichNapTienViewModel>
                    {
                        success = false,
                        message = "Không đọc được trạng thái giao dịch."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<GiaoDichNapTienViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        // =====================================================
        // XÁC NHẬN NẠP TIỀN MÔ PHỎNG (DEMO)
        // =====================================================
        public async Task<ApiReponse<GiaoDichNapTienViewModel>> xacNhanNapTienMoPhong(string maGiaoDich, int maUser)
        {
            try
            {
                var response = await _guiRequest.PostAsync(
                    $"api/GiaoDichNapTien/XacNhanMoPhong/{maGiaoDich}?maUser={maUser}",
                    null
                );

                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<GiaoDichNapTienViewModel>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<GiaoDichNapTienViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu phản hồi xác nhận nạp tiền."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<GiaoDichNapTienViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<string>> uploadBienLai(string maGiaoDich, IFormFile file)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType);
                formData.Add(streamContent, "file", file.FileName);

                var response = await _guiRequest.PostAsync($"api/GiaoDichNapTien/UploadBienLai/{maGiaoDich}", formData);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiReponse<string>
                    {
                        success = false,
                        message = !string.IsNullOrWhiteSpace(content) ? content : $"Máy chủ API trả về mã lỗi: HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    return new ApiReponse<string>
                    {
                        success = false,
                        message = "Phản hồi từ API rỗng."
                    };
                }

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                bool success = root.TryGetProperty("success", out var s) && s.GetBoolean();
                string message = root.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                string fileUrl = root.TryGetProperty("fileUrl", out var f) ? f.GetString() ?? "" : "";

                return new ApiReponse<string>
                {
                    success = success,
                    message = message,
                    data = fileUrl
                };
            }
            catch (Exception ex)
            {
                return new ApiReponse<string>
                {
                    success = false,
                    message = "Lỗi tải ảnh biên lai: " + ex.Message
                };
            }
        }
    }
}
using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class WalletWebService : IWalletWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public WalletWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ==========================================
        // 1. LẤY THÔNG TIN VÍ
        // ==========================================
        public async Task<ApiReponse<WalletViewModel>> layViTheoUser(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Wallet/LayViTheoUser/{maUser}");
                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<WalletViewModel>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<WalletViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu ví."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<WalletViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        // ==========================================
        // 2. LẤY LỊCH SỬ GIAO DỊCH
        // ==========================================
        public async Task<ApiReponse<List<LichSuGiaoDichItemViewModel>>> layLichSuGiaoDich(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Wallet/LichSuGiaoDich/{maUser}");
                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<List<LichSuGiaoDichItemViewModel>>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<List<LichSuGiaoDichItemViewModel>>
                    {
                        success = false,
                        message = "Không đọc được lịch sử giao dịch."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<LichSuGiaoDichItemViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        // ==========================================
        // 3. TẠO YÊU CẦU RÚT TIỀN MÔ PHỎNG
        // ==========================================
        public async Task<ApiReponse<YeuCauRutTienItemViewModel>> taoYeuCauRutTien(int maUser, RutTienRequestModel request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _guiRequest.PostAsync($"api/Wallet/TaoYeuCauRutTien/{maUser}", content);
                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<YeuCauRutTienItemViewModel>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<YeuCauRutTienItemViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu phản hồi rút tiền."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<YeuCauRutTienItemViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        // ==========================================
        // 4. LẤY DANH SÁCH YÊU CẦU RÚT TIỀN
        // ==========================================
        public async Task<ApiReponse<List<YeuCauRutTienItemViewModel>>> layDanhSachRutTien(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Wallet/DanhSachRutTien/{maUser}");
                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<List<YeuCauRutTienItemViewModel>>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<List<YeuCauRutTienItemViewModel>>
                    {
                        success = false,
                        message = "Không đọc được danh sách yêu cầu rút tiền."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<YeuCauRutTienItemViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        // ==========================================
        // 5. XÁC NHẬN DUYỆT HOẶC HỦY RÚT TIỀN MÔ PHỎNG
        // ==========================================
        public async Task<ApiReponse<bool>> xacNhanRutTienMoPhong(int maRutTien, int maUser, bool chapNhan, string? lyDo = null)
        {
            try
            {
                string url = $"api/Wallet/XacNhanRutTienMoPhong/{maRutTien}?maUser={maUser}&chapNhan={chapNhan}";
                if (!string.IsNullOrWhiteSpace(lyDo))
                {
                    url += $"&lyDo={Uri.EscapeDataString(lyDo)}";
                }

                var response = await _guiRequest.PostAsync(url, null);
                var noiDungJson = await response.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<bool>>(noiDungJson, _jsonOptions);

                if (ketQua == null)
                {
                    return new ApiReponse<bool>
                    {
                        success = response.IsSuccessStatusCode,
                        message = response.IsSuccessStatusCode ? "Xử lý thành công." : "Thao tác thất bại."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }
    }
}
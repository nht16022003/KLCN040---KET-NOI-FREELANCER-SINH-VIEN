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
    }
}
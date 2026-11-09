using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing.Tree;

namespace FreelancerStudent.Web.Services
{
    public class NhaTuyenDungWebService : INhaTuyenDungWebService
    {
        //Đối tượng dùng để gửi http request từ web sang api
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public NhaTuyenDungWebService(IHttpClientFactory httpClientFactory)
        {
            //Láy HttpClient đã được cấu hình với tên "ApiClient" để gọi API
            _guiRequest = httpClientFactory.CreateClient("ApiClient");

            //Cấu hình cách chuyển đổi dữ liệu giữa JSON và Object C#
            _jsonOptions = new JsonSerializerOptions
            {
                //Không phân biệt chữ hoa và chữ thường của
                //tên thuộc tính JSON khi chuyển sang C#
                PropertyNameCaseInsensitive = true
            };


        }

        public async Task<ApiReponse<List<NhaTuyenDungViewModel>>> layDanhSachNhaTuyenDungAsync()
        {
            try
            {
                //Gửi resquet đến API
                var reponse = await _guiRequest.GetAsync("api/NhaTuyenDung/DanhSachNhaTuyenDung");
                var noiDung_JSON = await reponse.Content.ReadAsByteArrayAsync();

                // In chuỗi máy chủ trả về
                Console.WriteLine($"===> [HTTP CODE]: {reponse.StatusCode}");
                Console.WriteLine($"===> [NỘI DUNG API TRẢ VỀ]: {noiDung_JSON}");

                return JsonSerializer.Deserialize<ApiReponse<List<NhaTuyenDungViewModel>>>(noiDung_JSON, _jsonOptions)
                     ?? new ApiReponse<List<NhaTuyenDungViewModel>> { success = false, message = "Không thể đọc dữ liệu" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("================ LỖI CHI TIẾT ================");
                Console.WriteLine($"LỖI: {ex.Message}");
                Console.WriteLine("==============================================");

                return new ApiReponse<List<NhaTuyenDungViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<NhaTuyenDungProfileViewModel>> layProfileNhaTuyenDungAsync(int maNhaTuyenDung)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/NhaTuyenDung/Profile/{maNhaTuyenDung}");
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<NhaTuyenDungProfileViewModel>>(json, _jsonOptions)
                    ?? new ApiReponse<NhaTuyenDungProfileViewModel>
                    {
                        success = false,
                        message = "Không thể đọc dữ liệu hồ sơ nhà tuyển dụng."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<NhaTuyenDungProfileViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<NhaTuyenDungProfileViewModel>> layProfileNhaTuyenDungTheoMaUserAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/NhaTuyenDung/Profile/ByUser/{maUser}");
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<NhaTuyenDungProfileViewModel>>(json, _jsonOptions)
                    ?? new ApiReponse<NhaTuyenDungProfileViewModel>
                    {
                        success = false,
                        message = "Không thể đọc dữ liệu hồ sơ nhà tuyển dụng."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<NhaTuyenDungProfileViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<bool>> capNhatThongTinNTDAsync(CapNhatThongTinNhaTuyenDungViewModel model)
        {
            try
            {
                var payload = new
                {
                    maUser = model.MaUser,
                    tencongty = model.Tencongty,
                    linhvuc = model.Linhvuc,
                    diachi = model.Diachi,
                    link = model.Link,
                    hotenNguoiDaiDien = model.HotenNguoiDaiDien,
                    sdtLienHe = model.SdtLienHe
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _guiRequest.PutAsync("api/NhaTuyenDung/CapNhatThongTin", content);
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions)
                    ?? new ApiReponse<bool>
                    {
                        success = false,
                        message = "Không nhận được phản hồi hợp lệ từ máy chủ."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<bool>> capNhatGioiThieuNTDAsync(CapNhatGioiThieuNhaTuyenDungViewModel model)
        {
            try
            {
                var payload = new
                {
                    maUser = model.MaUser,
                    gioithieu = model.Gioithieu
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _guiRequest.PutAsync("api/NhaTuyenDung/CapNhatGioiThieu", content);
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions)
                    ?? new ApiReponse<bool>
                    {
                        success = false,
                        message = "Không nhận được phản hồi hợp lệ từ máy chủ."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<bool>> capNhatLogoNTDAsync(int maUser, string logoUrl)
        {
            try
            {
                var payload = new
                {
                    maUser = maUser,
                    logoUrl = logoUrl
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _guiRequest.PutAsync("api/NhaTuyenDung/CapNhatLogo", content);
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions)
                    ?? new ApiReponse<bool>
                    {
                        success = false,
                        message = "Không nhận được phản hồi hợp lệ từ máy chủ."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }
    }
}
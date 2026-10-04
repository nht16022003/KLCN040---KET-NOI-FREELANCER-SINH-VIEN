using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing.Tree;

namespace FreelancerStudent.Web.Services
{
    public class JobPostWebService : IJobPostWebService
    {
        //Đối tượng dùng để gửi http request từ web sang api
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public JobPostWebService(IHttpClientFactory httpClientFactory)
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

        public async Task<ApiReponse<List<JobPostViewModel>>> layDanhSachJobPost()
        {
            try
            {
                //Gửi resquet đến API
                var reponse = await _guiRequest.GetAsync("api/JobPost/DanhSachJobPost");
                var noiDung_JSON = await reponse.Content.ReadAsByteArrayAsync();

                // In chuỗi máy chủ trả về
                Console.WriteLine($"===> [HTTP CODE]: {reponse.StatusCode}");
                Console.WriteLine($"===> [NỘI DUNG API TRẢ VỀ]: {noiDung_JSON}");

                return JsonSerializer.Deserialize<ApiReponse<List<JobPostViewModel>>>(noiDung_JSON, _jsonOptions)
                     ?? new ApiReponse<List<JobPostViewModel>> { success = false, message = "Không thể đọc dữ liệu" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("================ LỖI CHI TIẾT ================");
                Console.WriteLine($"LỖI: {ex.Message}");
                Console.WriteLine("==============================================");

                return new ApiReponse<List<JobPostViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }


        //Tuấn Anh
        public async Task<ApiReponse<JobPostViewModel>> taoJobPost(JobPostViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _guiRequest.PostAsync(
                    "api/JobPost/TaoJob",
                    content
                );

                var noiDungJson =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"===> [TAO JOB HTTP CODE]: {response.StatusCode}"
                );

                Console.WriteLine(
                    $"===> [TAO JOB RESPONSE]: {noiDungJson}"
                );

                var ketQua =
                    JsonSerializer.Deserialize<ApiReponse<JobPostViewModel>>(
                        noiDungJson,
                        _jsonOptions
                    );

                if (ketQua == null)
                {
                    return new ApiReponse<JobPostViewModel>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu API trả về."
                    };
                }

                return ketQua;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"===> [LỖI TẠO JOB]: {ex.Message}"
                );

                return new ApiReponse<JobPostViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }


        //Tuấn
        //Tuấn
        public async Task<ApiReponse<List<UngTuyenViewModel>>> layDSUngTuyenTheoMaUser(int maUser)
        {
            try
            {
                var respnse = await _guiRequest.GetAsync($"api/JobPost/DanhSachUngTuyen/{maUser}");

                var noiDungJson = await respnse.Content.ReadAsStringAsync();

                var ketQua = JsonSerializer.Deserialize<ApiReponse<List<UngTuyenViewModel>>>(noiDungJson, _jsonOptions);
                if (ketQua == null)
                {
                    return new ApiReponse<List<UngTuyenViewModel>>
                    {
                        success = false,
                        message = "Không đọc được dữ liệu từ API."
                    };
                }
                return ketQua;
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<UngTuyenViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối API" + ex.Message
                };
            }
        }

        public async Task<ApiReponse<bool>> duyetUngTuyenAsync(int maUngTuyen, string trangThai)
        {
            try
            {
                var request = new
                {
                    maUngTuyen = maUngTuyen,
                    trangThai = trangThai
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/JobPost/DuyetUngTuyen", content);
                var noiDung = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ApiReponse<bool>>(noiDung, _jsonOptions) ??
                new ApiReponse<bool>
                {
                    success = false,
                    message = "Không đọc được dữ liệu trả về."
                };
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

        public async Task<ApiReponse<bool>> nopDonUngTuyenAsync(UngTuyenViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/JobPost/UngTuyen", content);
                var noiDung = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ApiReponse<bool>>(noiDung, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Lỗi đọc dữ liệu API." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = ex.Message };
            }
        }


        //Tuấn
        public async Task<ApiReponse<List<JobPostViewModel>>> layJobCuaToiAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/JobPost/CuaToi/{maUser}");
                var data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ApiReponse<List<JobPostViewModel>>>(data, _jsonOptions)
                    ?? new ApiReponse<List<JobPostViewModel>> { success = false, message = "Lỗi đọc dữ liệu." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<JobPostViewModel>> { success = false, message = ex.Message };
            }
        }


        //Tuấn
        public async Task<ApiReponse<bool>> capNhatJobPostAsync(JobPostViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _guiRequest.PutAsync("api/JobPost/CapNhat", content);
                var data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ApiReponse<bool>>(data, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Lỗi đọc dữ liệu." };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = ex.Message };
            }
        }

    }
}
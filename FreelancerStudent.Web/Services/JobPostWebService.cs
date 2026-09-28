using System.Text;
using System.Text.Json;
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
    }
}
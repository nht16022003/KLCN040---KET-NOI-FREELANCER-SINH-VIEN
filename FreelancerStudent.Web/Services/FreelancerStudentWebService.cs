using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing.Tree;

namespace FreelancerStudent.Web.Services
{
    public class FreelancerStudentWebService : IFreelancerStudentWebService
    {
        //Đối tượng dùng để gửi http request từ web sang api
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public FreelancerStudentWebService(IHttpClientFactory httpClientFactory)
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
        //XS
        public async Task<ApiReponse<FreelancerStudentProfileViewModel>> layProfileFreelancerStudentAsync(int maFreelancerStudents)
        {
            try
            {
                var response = await _guiRequest.GetAsync(
                    $"api/FreelancerStudent/Profile/{maFreelancerStudents}");
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<FreelancerStudentProfileViewModel>>(
                           json, _jsonOptions)
                       ?? new ApiReponse<FreelancerStudentProfileViewModel>
                       {
                           success = false,
                           message = "Không thể đọc dữ liệu hồ sơ freelancer."
                       };
            }
            catch (Exception ex)
            {
                return new ApiReponse<FreelancerStudentProfileViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối API: " + ex.Message
                };
            }
        }

        public async Task<ApiReponse<List<FreelacerStudentViewModel>>> layDanhSachFreelancerStudentAsync()
        {
            try
            {
                //Gửi resquet đến API
                var reponse = await _guiRequest.GetAsync("api/FreelancerStudent/DanhSachFreelancerStudent");
                var noiDung_JSON = await reponse.Content.ReadAsByteArrayAsync();

                // In chuỗi máy chủ trả về
                Console.WriteLine($"===> [HTTP CODE]: {reponse.StatusCode}");
                Console.WriteLine($"===> [NỘI DUNG API TRẢ VỀ]: {noiDung_JSON}");

                return JsonSerializer.Deserialize<ApiReponse<List<FreelacerStudentViewModel>>>(noiDung_JSON, _jsonOptions)
                     ?? new ApiReponse<List<FreelacerStudentViewModel>> { success = false, message = "Không thể đọc dữ liệu" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("================ LỖI CHI TIẾT ================");
                Console.WriteLine($"LỖI: {ex.Message}");
                Console.WriteLine("==============================================");

                return new ApiReponse<List<FreelacerStudentViewModel>>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }


        //Tuấn
        public async Task<ApiReponse<ChiTietHoSoFreelancerViewModel>> layChiTietHoSoAsync(int maFreelancerStudents)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/FreelancerStudent/ChiTiet/{maFreelancerStudents}");
                var json = await response.Content.ReadAsByteArrayAsync();

                return JsonSerializer.Deserialize<ApiReponse<ChiTietHoSoFreelancerViewModel>>(json, _jsonOptions)
                    ?? new ApiReponse<ChiTietHoSoFreelancerViewModel>
                    {
                        success = false,
                        message = "Không thể đọc dữ liệu từ máy chủ API."
                    };
            }
            catch (Exception ex)
            {
                return new ApiReponse<ChiTietHoSoFreelancerViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API: " + ex.Message
                };
            }
        }

        //Tuấn
        public async Task<ApiReponse<bool>> capNhatHoSoAsync(ChiTietHoSoFreelancerViewModel model)
        {
            try
            {
                // Tách chuỗi NewSkillsInput thành danh sách kỹ năng
                if (!string.IsNullOrWhiteSpace(model.NewSkillsInput))
                {
                    model.DanhSachKyNang = model.NewSkillsInput
                        .Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PutAsync("api/FreelancerStudent/CapNhatHoSo", jsonContent);
                var noiDung = await response.Content.ReadAsByteArrayAsync();

                return JsonSerializer.Deserialize<ApiReponse<bool>>(noiDung, _jsonOptions)
                    ?? new ApiReponse<bool> { success = false, message = "Không thể cập nhật hồ sơ." };
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
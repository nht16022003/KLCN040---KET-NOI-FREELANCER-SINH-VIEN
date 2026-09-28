using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing.Tree;

namespace FreelancerStudent.Web.Services
{
    public class AuthWebService : IAuthWebService
    {
        //Đối tượng dùng để gửi http request từ web sang api
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthWebService(IHttpClientFactory httpClientFactory)
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

        //Gọi API đăng ký
        public async Task<ApiReponse<UserSessionViewModel>> DangKyAsync(DangKyViewModel model)
        {
            var api_request = new
            {
                hovaten = model.hovaten,
                tentaikhoan = model.tentaikhoan,
                email = model.email,
                sodienthoai = model.sodienthoai,
                password = model.password,
                xacnhanmatkhau = model.xacnhanmatkhau,
                marole = model.marole
            };

            var noiDung_Json_Degui = new StringContent(JsonSerializer.Serialize(api_request), Encoding.UTF8, "application/json");

            //JsonSerializer.Serialize(api_request) -> chuyển object C# thành JSON
            /*
            
                C# : 
                {
                    hovaten = "Nguyễn Văn A"
                }

                JSON
                {
                    "hovaten":"Nguyễn Văn A"
                }
            
                và cuối cùng dùng Encoding.UTF8
            */

            //"application/json" -> báo cho API biết Body là JSON

            var reponse = await _guiRequest.PostAsync("api/Auth/dangky", noiDung_Json_Degui);

            /*
                Gửi noiDung_Json_Degui đến API
                sau khi xử lý xong sẽ trả kết quả về reponse
            
            */
            var noiDung_API_TraVe = await reponse.Content.ReadAsStringAsync();

            try
            {
                //Chuyển JSon sang Object C# để Controller sử dụng
                return JsonSerializer.Deserialize<ApiReponse<UserSessionViewModel>>(noiDung_API_TraVe, _jsonOptions)

                //Nếu Deserialize trả về null thì tạo respnse lỗi
                ?? new ApiReponse<UserSessionViewModel>
                {
                    success = false,
                    message = "Không thể phân tích phản hồi từ máy chủ!"
                };

            }
            catch
            {
                /*
                
                Nếu xảy ra exception trong quá trình Deserialize thì trả về response lỗi thay vì làm chương trình bị crash.
                */
                return new ApiReponse<UserSessionViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API"
                };
            }
        }

        //Đăng nhập

        public async Task<ApiReponse<UserSessionViewModel>> DangNhapAsync(DangNhapViewModel model)
        {
            var api_request = new DangNhapViewModel
            {
                tentaikhoan_email = model.tentaikhoan_email,
                password = model.password
            };

            var noiDung_Json_Degui = new StringContent(JsonSerializer.Serialize(api_request), Encoding.UTF8, "application/json");

            var reponse = await _guiRequest.PostAsync("api/Auth/DangNhap", noiDung_Json_Degui);

            var noiDung_API_TraVe = await reponse.Content.ReadAsStringAsync();

            try
            {
                //Chuyển JSon sang Object C# để Controller sử dụng
                return JsonSerializer.Deserialize<ApiReponse<UserSessionViewModel>>(noiDung_API_TraVe, _jsonOptions)

                //Nếu Deserialize trả về null thì tạo respnse lỗi
                ?? new ApiReponse<UserSessionViewModel>
                {
                    success = false,
                    message = "Không thể phân tích phản hồi từ máy chủ!"
                };

            }
            catch
            {
                /*
                
                Nếu xảy ra exception trong quá trình Deserialize thì trả về response lỗi thay vì làm chương trình bị crash.
                */
                return new ApiReponse<UserSessionViewModel>
                {
                    success = false,
                    message = "Lỗi kết nối máy chủ API"
                };
            }
        }
    }
}
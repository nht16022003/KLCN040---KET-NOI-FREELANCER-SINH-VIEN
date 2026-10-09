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

        public async Task<ApiReponse<PortfolioViewModel>> layPortfolioAsync(int maFreelancerStudents)
        {
            var response = await _guiRequest.GetAsync($"api/Portfolio/{maFreelancerStudents}");
            var json = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ApiReponse<PortfolioViewModel>
                {
                    success = true,
                    data = new PortfolioViewModel
                    {
                        maFreelancerStudents = maFreelancerStudents
                    },
                    message = "Portfolio chưa có dữ liệu."
                };
            }

            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể đọc Portfolio." };
        }

        public async Task<ApiReponse<PortfolioViewModel>> capNhatPortfolioAsync(PortfolioViewModel portfolio)
        {
            var request = new
            {
                maFreelancerStudents = portfolio.maFreelancerStudents,
                moTaBanThan = portfolio.moTaBanThan,
                url_video = portfolio.url_video
            };
            var response = await _guiRequest.PutAsJsonAsync("api/Portfolio", request);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể cập nhật Portfolio." };
        }

        public async Task<ApiReponse<PortfolioViewModel>> themDuAnAsync(
            DuAnTrongPortfolioViewModel project, int maFreelancerStudents)
        {
            var request = new
            {
                maFreelancerStudents,
                tenDuAn = project.tenDuAn,
                vaiTro = project.vaiTro,
                moTa = project.moTa,
                congnghe = project.congnghe,
                linkGithub = project.linkGithub,
                linkDemo = project.linkDemo,
                link_file = project.link_file
            };
            var response = await _guiRequest.PostAsJsonAsync("api/Portfolio/projects", request);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể thêm dự án." };
        }

        public async Task<ApiReponse<PortfolioViewModel>> suaDuAnAsync(
            DuAnTrongPortfolioViewModel project, int maFreelancerStudents)
        {
            var request = new
            {
                maDA = project.maDA,
                maFreelancerStudents,
                tenDuAn = project.tenDuAn,
                vaiTro = project.vaiTro,
                moTa = project.moTa,
                congnghe = project.congnghe,
                linkGithub = project.linkGithub,
                linkDemo = project.linkDemo,
                link_file = project.link_file
            };
            var response = await _guiRequest.PutAsJsonAsync("api/Portfolio/projects", request);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể sửa dự án." };
        }

        public async Task<ApiReponse<PortfolioViewModel>> xoaDuAnAsync(string maDA, int maFreelancerStudents)
        {
            var response = await _guiRequest.DeleteAsync($"api/Portfolio/projects/{maDA}?maFreelancerStudents={maFreelancerStudents}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể xóa dự án." };
        }

        public async Task<ApiReponse<PortfolioViewModel>> capNhatDuAnNoiBatAsync(int maFreelancerStudents, List<string> maDAs)
        {
            var response = await _guiRequest.PutAsJsonAsync("api/Portfolio/featured", new { maFreelancerStudents, maDAs });
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiReponse<PortfolioViewModel>>(json, _jsonOptions)
                ?? new ApiReponse<PortfolioViewModel> { success = false, message = "Không thể cập nhật dự án nổi bật." };
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


    }
}
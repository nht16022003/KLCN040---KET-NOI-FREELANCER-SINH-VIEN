using System.Text;
using System.Text.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Chat;

namespace FreelancerStudent.Web.Services
{
    public class ChatWebService : IChatWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // 1. Tạo hoặc lấy phòng chat
        public async Task<ApiReponse<PhongChatViewModel>> TaoHoacLayPhongChatAsync(int maUserClient, int maFreelancerStudent, string? maJob)
        {
            try
            {
                var payload = new
                {
                    maUserClient = maUserClient,
                    maFreelancerStudent = maFreelancerStudent,
                    maJob = maJob
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/Chat/TaoHoacLayPhong", content);
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<PhongChatViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<PhongChatViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<PhongChatViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        // 2. Lấy danh sách phòng chat theo User
        public async Task<ApiReponse<List<PhongChatViewModel>>> LayDanhSachPhongChatAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Chat/DanhSachPhong/{maUser}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<PhongChatViewModel>>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<List<PhongChatViewModel>> { success = false, message = "Lỗi phản hồi từ máy chủ", data = new List<PhongChatViewModel>() };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<PhongChatViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message, data = new List<PhongChatViewModel>() };
            }
        }

        // 3. Lấy lịch sử tin nhắn
        public async Task<ApiReponse<List<TinNhanViewModel>>> LayLichSuTinNhanAsync(int maPhongChat, int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Chat/TinNhan/{maPhongChat}?maUser={maUser}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<TinNhanViewModel>>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<List<TinNhanViewModel>> { success = false, message = "Lỗi phản hồi từ máy chủ", data = new List<TinNhanViewModel>() };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<TinNhanViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message, data = new List<TinNhanViewModel>() };
            }
        }

        // 4. Gửi tin nhắn
        public async Task<ApiReponse<TinNhanViewModel>> GuiTinNhanAsync(int maPhongChat, int maNguoiGui, string noiDung, string? fileDinhKem, string loaiTinNhan = "Text")
        {
            try
            {
                var payload = new
                {
                    maPhongChat = maPhongChat,
                    maNguoiGui = maNguoiGui,
                    noiDung = noiDung,
                    fileDinhKem = fileDinhKem,
                    loaiTinNhan = loaiTinNhan
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _guiRequest.PostAsync("api/Chat/GuiTinNhan", content);
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<TinNhanViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<TinNhanViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<TinNhanViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        // 5. Tải file lên API
        public async Task<ApiReponse<FileDinhKemChatViewModel>> UploadFileAsync(IFormFile file)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
                formData.Add(streamContent, "file", file.FileName);

                var response = await _guiRequest.PostAsync("api/Chat/UploadFile", formData);
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<FileDinhKemChatViewModel>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<FileDinhKemChatViewModel> { success = false, message = "Lỗi phản hồi từ máy chủ khi tải tệp" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<FileDinhKemChatViewModel> { success = false, message = "Lỗi kết nối API tải tệp: " + ex.Message };
            }
        }

        // 6. Lấy danh sách file đính kèm trong phòng chat
        public async Task<ApiReponse<List<FileDinhKemChatViewModel>>> LayDanhSachFileAsync(int maPhongChat)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Chat/DanhSachFile/{maPhongChat}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<List<FileDinhKemChatViewModel>>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<List<FileDinhKemChatViewModel>> { success = false, message = "Lỗi phản hồi từ máy chủ", data = new List<FileDinhKemChatViewModel>() };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<FileDinhKemChatViewModel>> { success = false, message = "Lỗi kết nối API lấy tệp: " + ex.Message, data = new List<FileDinhKemChatViewModel>() };
            }
        }

        // 7. Đếm tổng số tin nhắn chưa đọc
        public async Task<ApiReponse<int>> DemTinNhanChuaDocAsync(int maUser)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Chat/chua-doc?maUser={maUser}");
                var jsonStr = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiReponse<int>>(jsonStr, _jsonOptions)
                       ?? new ApiReponse<int> { success = false, message = "Lỗi phản hồi từ máy chủ", data = 0 };
            }
            catch (Exception ex)
            {
                return new ApiReponse<int> { success = false, message = "Lỗi kết nối API đếm tin nhắn: " + ex.Message, data = 0 };
            }
        }
    }
}

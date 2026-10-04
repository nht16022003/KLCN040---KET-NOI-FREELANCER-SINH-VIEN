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
        public async Task<ApiReponse<TinNhanViewModel>> GuiTinNhanAsync(int maPhongChat, int maNguoiGui, string noiDung, string? fileDinhKem)
        {
            try
            {
                var payload = new
                {
                    maPhongChat = maPhongChat,
                    maNguoiGui = maNguoiGui,
                    noiDung = noiDung,
                    fileDinhKem = fileDinhKem,
                    loaiTinNhan = "Text"
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
    }
}

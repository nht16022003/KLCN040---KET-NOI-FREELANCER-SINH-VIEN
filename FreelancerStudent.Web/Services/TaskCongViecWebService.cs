using System.Net.Http.Json;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class TaskCongViecWebService : ITaskCongViecWebService
    {
        private readonly HttpClient _httpClient;

        public TaskCongViecWebService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public async Task<ApiReponse<TienDoDuAnViewModel>> LayTienDoDuAnAsync(string maHD, int maUser)
        {
            var result = new ApiReponse<TienDoDuAnViewModel>();
            try
            {
                var url = $"api/TaskCongViec/hop-dong/{Uri.EscapeDataString(maHD)}?maUser={maUser}";
                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ApiReponse<TienDoDuAnViewModel>>();
                    return data ?? new ApiReponse<TienDoDuAnViewModel> { success = false, message = "Dữ liệu trả về rỗng." };
                }

                var errObj = await response.Content.ReadFromJsonAsync<ApiReponse<TienDoDuAnViewModel>>();
                return errObj ?? new ApiReponse<TienDoDuAnViewModel>
                {
                    success = false,
                    message = $"Lỗi từ máy chủ ({(int)response.StatusCode})"
                };
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
                return result;
            }
        }

        public async Task<ApiReponse<TaskCongViecViewModel>> TaoTaskAsync(TaoTaskCongViecViewModel model, int maUser)
        {
            var result = new ApiReponse<TaskCongViecViewModel>();
            try
            {
                var url = $"api/TaskCongViec?maUser={maUser}";
                using var response = await _httpClient.PostAsJsonAsync(url, model);

                var data = await response.Content.ReadFromJsonAsync<ApiReponse<TaskCongViecViewModel>>();
                return data ?? new ApiReponse<TaskCongViecViewModel>
                {
                    success = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Thêm đầu việc thành công." : $"Lỗi từ máy chủ ({(int)response.StatusCode})"
                };
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
                return result;
            }
        }

        public async Task<ApiReponse<TaskCongViecViewModel>> CapNhatTaskAsync(CapNhatTaskCongViecViewModel model, int maUser)
        {
            var result = new ApiReponse<TaskCongViecViewModel>();
            try
            {
                var url = $"api/TaskCongViec/{model.maTask}?maUser={maUser}";
                using var response = await _httpClient.PutAsJsonAsync(url, model);

                var data = await response.Content.ReadFromJsonAsync<ApiReponse<TaskCongViecViewModel>>();
                return data ?? new ApiReponse<TaskCongViecViewModel>
                {
                    success = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Cập nhật đầu việc thành công." : $"Lỗi từ máy chủ ({(int)response.StatusCode})"
                };
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
                return result;
            }
        }

        public async Task<ApiReponse<TaskCongViecViewModel>> CapNhatTrangThaiTaskAsync(CapNhatTrangThaiTaskViewModel model, int maUser)
        {
            var result = new ApiReponse<TaskCongViecViewModel>();
            try
            {
                var url = $"api/TaskCongViec/{model.maTask}/trang-thai?maUser={maUser}";
                using var response = await _httpClient.PatchAsJsonAsync(url, model);

                var data = await response.Content.ReadFromJsonAsync<ApiReponse<TaskCongViecViewModel>>();
                return data ?? new ApiReponse<TaskCongViecViewModel>
                {
                    success = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Cập nhật trạng thái thành công." : $"Lỗi từ máy chủ ({(int)response.StatusCode})"
                };
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
                return result;
            }
        }

        public async Task<ApiReponse<bool>> XoaTaskAsync(int maTask, int maUser)
        {
            var result = new ApiReponse<bool>();
            try
            {
                var url = $"api/TaskCongViec/{maTask}?maUser={maUser}";
                using var response = await _httpClient.DeleteAsync(url);

                var data = await response.Content.ReadFromJsonAsync<ApiReponse<bool>>();
                return data ?? new ApiReponse<bool>
                {
                    success = response.IsSuccessStatusCode,
                    data = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode ? "Xóa đầu việc thành công." : $"Lỗi từ máy chủ ({(int)response.StatusCode})"
                };
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không thể kết nối đến máy chủ: " + ex.Message;
                return result;
            }
        }

        public async Task<ApiReponse<bool>> DanhGiaGiaiDoanAsync(string maHD, int? maGiaiDoan, string trangThai, int maUser)
        {
            try
            {
                var url = $"api/TaskCongViec/hop-dong/{Uri.EscapeDataString(maHD)}/danh-gia-giai-doan?maUser={maUser}";
                using var response = await _httpClient.PostAsJsonAsync(url, new
                {
                    maHD,
                    maGiaiDoan,
                    trangThai
                });

                var apiResult = await response.Content.ReadFromJsonAsync<ApiReponse<bool>>();
                return apiResult ?? new ApiReponse<bool>
                {
                    success = response.IsSuccessStatusCode,
                    data = response.IsSuccessStatusCode,
                    message = response.IsSuccessStatusCode
                        ? "Cập nhật nghiệm thu giai đoạn thành công."
                        : $"Lỗi từ máy chủ ({(int)response.StatusCode})."
                };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool>
                {
                    success = false,
                    message = "Không thể kết nối đến máy chủ: " + ex.Message
                };
            }
        }
    }
}

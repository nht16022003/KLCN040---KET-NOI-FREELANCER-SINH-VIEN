using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services
{
    public class AdminWebService : IAdminWebService
    {
        private readonly HttpClient _guiRequest;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminWebService(IHttpClientFactory httpClientFactory)
        {
            _guiRequest = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiReponse<AdminDashboardViewModel>> LayDashboardAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/Admin/Dashboard");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<AdminDashboardViewModel>>(json, _jsonOptions);
                return result ?? new ApiReponse<AdminDashboardViewModel> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<AdminDashboardViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminUserItemViewModel>>> LayDanhSachUsersAsync(string? keyword, int? role, string? status)
        {
            try
            {
                var url = $"api/Admin/Users?keyword={Uri.EscapeDataString(keyword ?? "")}&role={role}&status={Uri.EscapeDataString(status ?? "")}";
                var response = await _guiRequest.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminUserItemViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminUserItemViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminUserItemViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<bool>> XuLyTrangThaiUserAsync(AdminXuLyPostModel model, int maAdmin)
        {
            try
            {
                var reqObj = new
                {
                    MaUser = model.MaUser,
                    MaAdmin = maAdmin,
                    HanhDong = model.HanhDong,
                    LyDo = model.LyDo
                };

                var response = await _guiRequest.PostAsJsonAsync("api/Admin/XuLyTrangThaiUser", reqObj);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions);
                return result ?? new ApiReponse<bool> { success = response.IsSuccessStatusCode, message = response.IsSuccessStatusCode ? "Thành công" : "Thao tác thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminLichSuXuLyViewModel>>> LayLichSuXuLyUserAsync(int? maUser)
        {
            try
            {
                var url = maUser.HasValue ? $"api/Admin/LichSuXuLyUser?maUser={maUser.Value}" : "api/Admin/LichSuXuLyUser";
                var response = await _guiRequest.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminLichSuXuLyViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminLichSuXuLyViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminLichSuXuLyViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminWalletItemViewModel>>> LayDanhSachWalletsAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/Admin/Wallets");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminWalletItemViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminWalletItemViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminWalletItemViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<bool>> DieuChinhSoDuViAsync(AdminAdjustmentPostModel model, int maAdmin)
        {
            try
            {
                var reqObj = new
                {
                    MaWallet = model.MaWallet,
                    MaAdmin = maAdmin,
                    LoaiDieuChinh = model.LoaiDieuChinh,
                    SoTien = model.SoTien,
                    LyDo = model.LyDo
                };

                var response = await _guiRequest.PostAsJsonAsync("api/Admin/DieuChinhSoDu", reqObj);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions);
                return result ?? new ApiReponse<bool> { success = response.IsSuccessStatusCode, message = response.IsSuccessStatusCode ? "Điều chỉnh ví thành công" : "Thao tác thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminLichSuDieuChinhViViewModel>>> LayLichSuDieuChinhViAsync(int? maWallet)
        {
            try
            {
                var url = maWallet.HasValue ? $"api/Admin/LichSuDieuChinhVi?maWallet={maWallet.Value}" : "api/Admin/LichSuDieuChinhVi";
                var response = await _guiRequest.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminLichSuDieuChinhViViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminLichSuDieuChinhViViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminLichSuDieuChinhViViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminHopDongItemViewModel>>> LayDanhSachHopDongAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/Admin/HopDongs");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminHopDongItemViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminHopDongItemViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminHopDongItemViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<AdminHopDongItemViewModel>> LayChiTietHopDongAsync(string maHD)
        {
            try
            {
                var response = await _guiRequest.GetAsync($"api/Admin/HopDongs/{Uri.EscapeDataString(maHD)}");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<AdminHopDongItemViewModel>>(json, _jsonOptions);
                return result ?? new ApiReponse<AdminHopDongItemViewModel> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<AdminHopDongItemViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<AdminContractTemplateViewModel>> LayMauHopDongAsync()
        {
            try
            {
                var response = await _guiRequest.GetAsync("api/Admin/MauHopDong");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<AdminContractTemplateViewModel>>(json, _jsonOptions);
                return result ?? new ApiReponse<AdminContractTemplateViewModel> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<AdminContractTemplateViewModel> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<bool>> CapNhatMauHopDongAsync(AdminContractTemplateViewModel model)
        {
            try
            {
                var reqObj = new
                {
                    TenMauHopDong = model.TenMauHopDong,
                    DieuKhoanChung = model.DieuKhoanChung,
                    ThoiHanNghiemThuNgay = model.ThoiHanNghiemThuNgay,
                    TyLeGiaiNganMacDinh = model.TyLeGiaiNganMacDinh
                };

                var response = await _guiRequest.PostAsJsonAsync("api/Admin/CapNhatMauHopDong", reqObj);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions);
                return result ?? new ApiReponse<bool> { success = response.IsSuccessStatusCode, message = response.IsSuccessStatusCode ? "Cập nhật thành công" : "Thao tác thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminYeuCauRutTienItemViewModel>>> LayDanhSachYeuCauRutTienAsync(string? trangThai)
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(trangThai) || trangThai == "TatCa"
                    ? "api/Admin/YeuCauRutTien"
                    : $"api/Admin/YeuCauRutTien?trangThai={Uri.EscapeDataString(trangThai)}";

                var response = await _guiRequest.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<List<AdminYeuCauRutTienItemViewModel>>>(json, _jsonOptions);
                return result ?? new ApiReponse<List<AdminYeuCauRutTienItemViewModel>> { success = false, message = "Không phân tích được phản hồi từ API" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminYeuCauRutTienItemViewModel>> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<bool>> XuLyRutTienAsync(AdminXuLyRutTienPostModel model, int maAdmin)
        {
            try
            {
                var reqObj = new
                {
                    MaRutTien = model.MaRutTien,
                    ChapNhan = model.ChapNhan,
                    LyDoTuChoi = model.LyDoTuChoi,
                    MaAdmin = maAdmin
                };

                var response = await _guiRequest.PostAsJsonAsync("api/Admin/XuLyRutTien", reqObj);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions);
                return result ?? new ApiReponse<bool> { success = response.IsSuccessStatusCode, message = response.IsSuccessStatusCode ? "Thành công" : "Thao tác thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = "Lỗi kết nối API: " + ex.Message };
            }
        }

        public async Task<ApiReponse<List<AdminCauHinhPhiViewModel>>> LayDanhSachCauHinhPhiAsync()
        {
            try
            {
                var res = await _guiRequest.GetAsync("api/Admin/CauHinhPhi");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var apiRes = JsonSerializer.Deserialize<ApiReponse<List<AdminCauHinhPhiViewModel>>>(json, _jsonOptions);
                    return apiRes ?? new ApiReponse<List<AdminCauHinhPhiViewModel>> { success = false, message = "Không nhận được phản hồi từ máy chủ." };
                }
                return new ApiReponse<List<AdminCauHinhPhiViewModel>> { success = false, message = $"Lỗi máy chủ (HTTP {(int)res.StatusCode})" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<List<AdminCauHinhPhiViewModel>> { success = false, message = ex.Message };
            }
        }

        public async Task<ApiReponse<bool>> CapNhatPhiDangBaiAsync(AdminCapNhatPhiDangBaiViewModel model)
        {
            try
            {
                var res = await _guiRequest.PostAsJsonAsync("api/Admin/CapNhatPhiDangBai", model);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var apiRes = JsonSerializer.Deserialize<ApiReponse<bool>>(json, _jsonOptions);
                    return apiRes ?? new ApiReponse<bool> { success = false, message = "Không nhận được phản hồi từ máy chủ." };
                }
                return new ApiReponse<bool> { success = false, message = $"Lỗi máy chủ (HTTP {(int)res.StatusCode})" };
            }
            catch (Exception ex)
            {
                return new ApiReponse<bool> { success = false, message = ex.Message };
            }
        }
    }
}
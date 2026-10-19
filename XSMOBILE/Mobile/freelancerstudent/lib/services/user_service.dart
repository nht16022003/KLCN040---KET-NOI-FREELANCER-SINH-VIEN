import '../core/constants/api_endpoints.dart';
import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/user_model.dart';

class UserService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy thông tin tài khoản (AuthController.ThongTinTaiKhoan - GET api/Auth/ThongTinTaiKhoan/{maUser})
  Future<ApiResponse<UserModel>> layThongTinTaiKhoan(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.thongTinTaiKhoan}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<UserModel>(
        isSuccess: true,
        data: UserModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<UserModel>(
        isSuccess: false,
        message: 'Lỗi khi tải thông tin tài khoản: ${e.toString()}',
      );
    }
  }

  /// Cập nhật thông tin tài khoản (AuthController.CapNhatThongTinTaiKhoan - PUT api/Auth/CapNhatThongTinTaiKhoan)
  Future<ApiResponse<UserModel>> capNhatThongTinTaiKhoan(
    Map<String, dynamic> duLieu,
  ) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatThongTinTaiKhoan,
        duLieu,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<UserModel>(
        isSuccess: true,
        data: UserModel.fromJson(json),
        message: body['message']?.toString() ?? 'Cập nhật thông tin thành công!',
      );
    } catch (e) {
      return ApiResponse<UserModel>(
        isSuccess: false,
        message: 'Lỗi cập nhật thông tin: ${e.toString()}',
      );
    }
  }

  /// Cập nhật ảnh đại diện (AuthController.CapNhatAvatar - PUT api/Auth/CapNhatAvatar)
  Future<ApiResponse<bool>> capNhatAvatar(int maUser, String avatarUrl) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatAvatar,
        {'maUser': maUser, 'avatarUrl': avatarUrl},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật avatar thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Đổi mật khẩu (AuthController.DoiMatKhau - PUT api/Auth/DoiMatKhau)
  Future<ApiResponse<bool>> doiMatKhau(
    int maUser,
    String matKhauCu,
    String matKhauMoi,
  ) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.doiMatKhau,
        {
          'maUser': maUser,
          'matKhauCu': matKhauCu,
          'matKhauMoi': matKhauMoi,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Đổi mật khẩu thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<ApiResponse<UserModel>> getProfile({int maUser = 1}) =>
      layThongTinTaiKhoan(maUser);

  Future<ApiResponse<UserModel>> updateProfile(Map<String, dynamic> payload) =>
      capNhatThongTinTaiKhoan(payload);
}

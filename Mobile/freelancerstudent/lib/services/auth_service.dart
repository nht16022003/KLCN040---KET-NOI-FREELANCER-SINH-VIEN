import 'package:shared_preferences/shared_preferences.dart';

import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../core/storage/pref_keys.dart';
import '../models/auth_response_model.dart';
import '../core/constants/api_endpoints.dart';

class AuthService {
  final ApiClient _apiClient = ApiClient();

  String _normalizeRole(String role) {
    final value = role.trim();
    if (value.toLowerCase() == 'nhatuyendung' ||
        value.toLowerCase() == 'employer') {
      return 'NhaTuyenDung';
    }
    return 'FreelancerStudent';
  }

  Future<void> _saveSession(AuthResponseModel user) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(PrefKeys.userRole, user.role);
    await prefs.setString(PrefKeys.accessToken, user.token);
    await prefs.setString('user_email', user.email);
    await prefs.setInt(PrefKeys.userId, user.userId);
    await prefs.setString('user_full_name', user.fullName);
  }

  Future<AuthResponseModel?> getStoredSession() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString(PrefKeys.accessToken) ?? '';
    final email = prefs.getString('user_email') ?? '';
    final role = prefs.getString(PrefKeys.userRole) ?? 'FreelancerStudent';
    final userId = prefs.getInt(PrefKeys.userId) ?? 0;
    final fullName = prefs.getString('user_full_name') ?? '';

    if (email.isEmpty) {
      return null;
    }

    return AuthResponseModel(
      token: token.isEmpty ? 'local_session' : token,
      email: email,
      role: role,
      userId: userId,
      fullName: fullName,
    );
  }

  Future<ApiResponse<AuthResponseModel>> login(
    String email,
    String password,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.authLogin,
        {'tentaikhoan_email': email.trim(), 'password': password},
      );

      final handled = await _apiClient.handleResponse(
        response,
        (json) => AuthResponseModel.fromJson(json as Map<String, dynamic>),
      );

      if (handled.isSuccess && handled.data != null) {
        await _saveSession(handled.data!);
        return handled;
      }

      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: handled.message.isEmpty
            ? 'Email hoặc mật khẩu không hợp lệ.'
            : handled.message,
      );
    } catch (e) {
      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: 'Không thể kết nối tới máy chủ. Chi tiết: ${e.toString()}',
      );
    }
  }

  Future<ApiResponse<AuthResponseModel>> registerAccount({
    required String fullName,
    required String userName,
    required String email,
    required String password,
    required String role,
  }) async {
    final normalizedRole = _normalizeRole(role);
    final normalizedEmail = email.trim();
    final normalizedUserName = userName.trim();

    if (fullName.trim().isEmpty ||
        normalizedUserName.isEmpty ||
        normalizedEmail.isEmpty ||
        password.trim().length < 6) {
      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: 'Vui lòng nhập đầy đủ thông tin, tên tài khoản và mật khẩu tối thiểu 6 ký tự.',
      );
    }

    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.authRegister,
        {
          'hovaten': fullName.trim(),
          'tentaikhoan': normalizedUserName,
          'email': normalizedEmail,
          'password': password.trim(),
          'xacnhanmatkhau': password.trim(),
          'marole': normalizedRole == 'NhaTuyenDung' ? 2 : 1,
        },
      );

      final handled = await _apiClient.handleResponse(
        response,
        (json) => AuthResponseModel.fromJson(json as Map<String, dynamic>),
      );

      if (handled.isSuccess && handled.data != null) {
        await _saveSession(handled.data!);
        return handled;
      }

      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: handled.message.isEmpty
            ? 'Đăng ký thất bại. Vui lòng thử lại.'
            : handled.message,
      );
    } catch (e) {
      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message:
            'Không thể kết nối tới máy chủ. Tài khoản chưa được lưu vào database. Chi tiết: ${e.toString()}',
      );
    }
  }

  Future<ApiResponse<AuthResponseModel>> registerStudent(
    String fullName,
    String userName,
    String email,
    String password,
  ) async {
    return registerAccount(
      fullName: fullName,
      userName: userName,
      email: email,
      password: password,
      role: 'FreelancerStudent',
    );
  }

  Future<ApiResponse<AuthResponseModel>> registerEmployer(
    String companyName,
    String userName,
    String email,
    String password,
  ) async {
    return registerAccount(
      fullName: companyName,
      userName: userName,
      email: email,
      password: password,
      role: 'NhaTuyenDung',
    );
  }

  Future<ApiResponse<bool>> forgotPassword(String email) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/Auth/forgot-password',
        {'email': email},
      );
      return await _apiClient.handleResponse(response, (_) => true);
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }
}

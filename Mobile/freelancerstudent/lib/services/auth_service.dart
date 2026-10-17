import 'package:shared_preferences/shared_preferences.dart';

import '../core/constants/api_endpoints.dart';
import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../core/storage/pref_keys.dart';
import '../models/auth_response_model.dart';

class AuthService {
  final ApiClient _apiClient = ApiClient();

  String _chuanHoaVaiTro(String vaiTro) {
    final value = vaiTro.trim();
    if (value.toLowerCase() == 'nhatuyendung' ||
        value.toLowerCase() == 'employer' ||
        value == '2') {
      return 'NhaTuyenDung';
    }
    return 'FreelancerStudent';
  }

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lưu phiên đăng nhập an toàn vào thiết bị
  Future<void> luuPhienDangNhap(AuthResponseModel nguoiDung) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(PrefKeys.userRole, nguoiDung.role);
    await prefs.setString(PrefKeys.accessToken, nguoiDung.token);
    await prefs.setString('user_email', nguoiDung.email);
    await prefs.setInt(PrefKeys.userId, nguoiDung.userId);
    await prefs.setString('user_full_name', nguoiDung.fullName);
  }

  /// Lấy phiên đăng nhập đã lưu trong máy
  Future<AuthResponseModel?> layPhienDangNhapDaLuu() async {
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

  /// Đăng nhập tài khoản (AuthController.DangNhap)
  Future<ApiResponse<AuthResponseModel>> dangNhap(
    String tenTaiKhoanHoacEmail,
    String matKhau,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.dangNhap,
        {'tentaikhoan_email': tenTaiKhoanHoacEmail.trim(), 'password': matKhau},
      );

      final handled = await _apiClient.handleResponse(
        response,
        (json) => AuthResponseModel.fromJson(json as Map<String, dynamic>),
      );

      if (handled.isSuccess && handled.data != null) {
        await luuPhienDangNhap(handled.data!);
        return handled;
      }

      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: handled.message.isEmpty
            ? 'Tên tài khoản hoặc mật khẩu không chính xác.'
            : handled.message,
      );
    } catch (e) {
      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: 'Không thể kết nối tới máy chủ API: ${e.toString()}',
      );
    }
  }

  /// Đăng ký tài khoản (AuthController.dangky)
  Future<ApiResponse<AuthResponseModel>> dangKyTaiKhoan({
    required String hoVaTen,
    required String tenTaiKhoan,
    required String email,
    required String matKhau,
    required String vaiTro,
  }) async {
    final vaiTroChuan = _chuanHoaVaiTro(vaiTro);
    final emailChuan = email.trim();
    final taiKhoanChuan = tenTaiKhoan.trim();

    if (hoVaTen.trim().isEmpty ||
        taiKhoanChuan.isEmpty ||
        emailChuan.isEmpty ||
        matKhau.trim().length < 6) {
      return ApiResponse<AuthResponseModel>(
        isSuccess: false,
        message: 'Vui lòng nhập đầy đủ họ tên, tên tài khoản và mật khẩu tối thiểu 6 ký tự.',
      );
    }

    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.dangKy,
        {
          'hovaten': hoVaTen.trim(),
          'tentaikhoan': taiKhoanChuan,
          'email': emailChuan,
          'password': matKhau.trim(),
          'xacnhanmatkhau': matKhau.trim(),
          'marole': vaiTroChuan == 'NhaTuyenDung' ? 2 : 1,
        },
      );

      final handled = await _apiClient.handleResponse(
        response,
        (json) => AuthResponseModel.fromJson(json as Map<String, dynamic>),
      );

      if (handled.isSuccess && handled.data != null) {
        await luuPhienDangNhap(handled.data!);
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
        message: 'Không thể kết nối tới máy chủ. Chi tiết: ${e.toString()}',
      );
    }
  }

  /// Đăng ký dành cho Sinh viên Freelancer
  Future<ApiResponse<AuthResponseModel>> dangKySinhVien(
    String hoVaTen,
    String tenTaiKhoan,
    String email,
    String matKhau,
  ) async {
    return dangKyTaiKhoan(
      hoVaTen: hoVaTen,
      tenTaiKhoan: tenTaiKhoan,
      email: email,
      matKhau: matKhau,
      vaiTro: 'FreelancerStudent',
    );
  }

  /// Đăng ký dành cho Nhà tuyển dụng
  Future<ApiResponse<AuthResponseModel>> dangKyNhaTuyenDung(
    String tenCongTy,
    String tenTaiKhoan,
    String email,
    String matKhau,
  ) async {
    return dangKyTaiKhoan(
      hoVaTen: tenCongTy,
      tenTaiKhoan: tenTaiKhoan,
      email: email,
      matKhau: matKhau,
      vaiTro: 'NhaTuyenDung',
    );
  }

  /// Quên mật khẩu
  Future<ApiResponse<bool>> quenMatKhau(String email) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.quenMatKhau,
        {'email': email},
      );
      return await _apiClient.handleResponse(response, (_) => true);
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<AuthResponseModel?> getStoredSession() => layPhienDangNhapDaLuu();

  Future<ApiResponse<AuthResponseModel>> login(String email, String password) =>
      dangNhap(email, password);

  Future<ApiResponse<AuthResponseModel>> registerAccount({
    required String fullName,
    required String userName,
    required String email,
    required String password,
    required String role,
  }) =>
      dangKyTaiKhoan(
        hoVaTen: fullName,
        tenTaiKhoan: userName,
        email: email,
        matKhau: password,
        vaiTro: role,
      );

  Future<ApiResponse<AuthResponseModel>> registerStudent(
    String fullName,
    String userName,
    String email,
    String password,
  ) =>
      dangKySinhVien(fullName, userName, email, password);

  Future<ApiResponse<AuthResponseModel>> registerEmployer(
    String companyName,
    String userName,
    String email,
    String password,
  ) =>
      dangKyNhaTuyenDung(companyName, userName, email, password);

  Future<ApiResponse<bool>> forgotPassword(String email) => quenMatKhau(email);
}

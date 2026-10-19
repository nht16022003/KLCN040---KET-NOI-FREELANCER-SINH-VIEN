import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/storage/khoa_luu_tru.dart';
import '../models/mo_hinh_phan_hoi_xac_thuc.dart';
import '../services/dich_vu_xac_thuc.dart';

class AuthViewModel extends ChangeNotifier {
  final AuthService _authService = AuthService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  String? _thongBaoLoi;
  AuthResponseModel? _nguoiDungHienTai;

  bool get dangTai => _dangTai;
  String? get thongBaoLoi => _thongBaoLoi;
  AuthResponseModel? get nguoiDungHienTai => _nguoiDungHienTai;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  String? get errorMessage => _thongBaoLoi;
  AuthResponseModel? get currentUser => _nguoiDungHienTai;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Đăng nhập tài khoản
  Future<bool> dangNhap(String tenTaiKhoanHoacEmail, String matKhau) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _authService.dangNhap(tenTaiKhoanHoacEmail, matKhau);
    if (response.isSuccess && response.data != null) {
      _nguoiDungHienTai = response.data;
      _dangTai = false;
      notifyListeners();
      return true;
    }

    _thongBaoLoi = response.message;
    _dangTai = false;
    notifyListeners();
    return false;
  }

  /// Đăng ký tài khoản mới
  Future<bool> dangKyTaiKhoan({
    required String hoVaTen,
    required String tenTaiKhoan,
    required String email,
    required String soDienThoai,
    required String matKhau,
    required String vaiTro,
  }) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _authService.dangKyTaiKhoan(
      hoVaTen: hoVaTen,
      tenTaiKhoan: tenTaiKhoan,
      email: email,
      soDienThoai: soDienThoai,
      matKhau: matKhau,
      vaiTro: vaiTro,
    );

    if (response.isSuccess && response.data != null) {
      _nguoiDungHienTai = response.data;
      _dangTai = false;
      notifyListeners();
      return true;
    }

    _thongBaoLoi = response.message;
    _dangTai = false;
    notifyListeners();
    return false;
  }

  /// Đăng ký nhanh tài khoản sinh viên
  Future<void> dangKySinhVien(
    String hoVaTen,
    String tenTaiKhoan,
    String email,
    String soDienThoai,
    String matKhau,
  ) async {
    await dangKyTaiKhoan(
      hoVaTen: hoVaTen,
      tenTaiKhoan: tenTaiKhoan,
      email: email,
      soDienThoai: soDienThoai,
      matKhau: matKhau,
      vaiTro: 'FreelancerStudent',
    );
  }

  /// Tải phiên đăng nhập đã lưu trong máy
  Future<void> taiPhienDangNhap() async {
    final user = await _authService.layPhienDangNhapDaLuu();
    _nguoiDungHienTai = user;
    _thongBaoLoi = null;
    notifyListeners();
  }

  /// Đăng xuất tài khoản
  Future<void> dangXuat() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(PrefKeys.accessToken);
    await prefs.remove(PrefKeys.userRole);
    await prefs.remove(PrefKeys.userId);
    await prefs.remove('user_email');
    await prefs.remove('user_full_name');

    _nguoiDungHienTai = null;
    _thongBaoLoi = null;
    notifyListeners();
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<bool> login(String email, String password) => dangNhap(email, password);

  Future<bool> registerAccount({
    required String fullName,
    required String userName,
    required String email,
    required String phoneNumber,
    required String password,
    required String role,
  }) =>
      dangKyTaiKhoan(
        hoVaTen: fullName,
        tenTaiKhoan: userName,
        email: email,
        soDienThoai: phoneNumber,
        matKhau: password,
        vaiTro: role,
      );

  Future<void> registerStudent(
    String fullName,
    String userName,
    String email,
    String phoneNumber,
    String password,
  ) =>
      dangKySinhVien(fullName, userName, email, phoneNumber, password);

  Future<void> loadSession() => taiPhienDangNhap();

  Future<void> logout() => dangXuat();
}

import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/storage/pref_keys.dart';
import '../models/auth_response_model.dart';
import '../services/auth_service.dart';

class AuthViewModel extends ChangeNotifier {
  final AuthService _authService = AuthService();

  bool _isLoading = false;
  String? _errorMessage;
  AuthResponseModel? _currentUser;

  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  AuthResponseModel? get currentUser => _currentUser;

  Future<bool> login(String email, String password) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _authService.login(email, password);
    if (response.isSuccess && response.data != null) {
      _currentUser = response.data;
      _isLoading = false;
      notifyListeners();
      return true;
    }

    _errorMessage = response.message;
    _isLoading = false;
    notifyListeners();
    return false;
  }

  Future<bool> registerAccount({
    required String fullName,
    required String userName,
    required String email,
    required String password,
    required String role,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _authService.registerAccount(
      fullName: fullName,
      userName: userName,
      email: email,
      password: password,
      role: role,
    );

    if (response.isSuccess && response.data != null) {
      _currentUser = response.data;
      _isLoading = false;
      notifyListeners();
      return true;
    }

    _errorMessage = response.message;
    _isLoading = false;
    notifyListeners();
    return false;
  }

  Future<void> registerStudent(
    String fullName,
    String userName,
    String email,
    String password,
  ) async {
    await registerAccount(
      fullName: fullName,
      userName: userName,
      email: email,
      password: password,
      role: 'FreelancerStudent',
    );
  }

  Future<void> loadSession() async {
    final user = await _authService.getStoredSession();
    _currentUser = user;
    _errorMessage = null;
    notifyListeners();
  }

  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(PrefKeys.accessToken);
    await prefs.remove(PrefKeys.userRole);
    await prefs.remove(PrefKeys.userId);
    await prefs.remove('user_email');
    await prefs.remove('user_full_name');

    _currentUser = null;
    _errorMessage = null;
    notifyListeners();
  }
}

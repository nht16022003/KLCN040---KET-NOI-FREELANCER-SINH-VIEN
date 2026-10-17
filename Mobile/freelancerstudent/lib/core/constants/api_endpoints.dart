class ApiEndpoints {
  static const String baseUrl = 'http://10.0.2.2:5152/api';

  static const String authLogin = '$baseUrl/Auth/DangNhap';
  static const String authRegister = '$baseUrl/Auth/dangky';
  static const String authForgotPassword = '$baseUrl/Auth/forgot-password';

  static const String jobs = '$baseUrl/JobPost';
  static const String jobDetail = '$baseUrl/JobPost';
  static const String postJob = '$baseUrl/JobPost';

  static const String studentPosts = '$baseUrl/BaiDangTimViec';
  static const String applications = '$baseUrl/UngTuyen';
  static const String wallet = '$baseUrl/Wallet';
  static const String depositTransactions = '$baseUrl/GiaoDichNapTien';

  static const String chatHubUrl = '$baseUrl/chatHub';
}

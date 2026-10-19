import '../core/constants/api_endpoints.dart';

class AuthResponseModel {
  final String token;
  final String email;
  final String role;
  final int userId;
  final String fullName;

  // Thuộc tính bổ sung từ DangNhap_ReponseDTO
  final String tentaikhoan;
  final String? sodienthoai;
  final String? avatarUrl;
  final int marole;

  const AuthResponseModel({
    required this.token,
    required this.email,
    required this.role,
    this.userId = 0,
    this.fullName = '',
    this.tentaikhoan = '',
    this.sodienthoai,
    this.avatarUrl,
    this.marole = 1,
  });

  // Tên biến/getter thuần tiếng Việt
  int get maUser => userId;
  String get hoVaTen => fullName;
  String get tenTaiKhoan => tentaikhoan;
  String get soDienThoai => sodienthoai ?? '';
  String get vaiTro => role;
  String get maToken => token;
  String? get anhDaiDien => avatarUrl;
  String get anhDaiDienHienThi => ApiEndpoints.dinhDangUrlAnh(
        avatarUrl,
        tenMacDinh: fullName.isNotEmpty ? fullName : 'User',
      );

  factory AuthResponseModel.fromJson(Map<String, dynamic> json) {
    final payload = json['data'] is Map<String, dynamic>
        ? json['data'] as Map<String, dynamic>
        : json;

    final rawRole = payload['role'] ?? payload['tenrole'] ?? payload['vaiTro'] ?? payload['marole'];
    String normalizedRole = 'FreelancerStudent';

    if (rawRole is int) {
      normalizedRole = rawRole == 2 ? 'NhaTuyenDung' : 'FreelancerStudent';
    } else if (rawRole is String) {
      final value = rawRole.trim();
      if (value.toLowerCase().contains('nhatuyendung') ||
          value.toLowerCase().contains('employer') ||
          value.contains('2')) {
        normalizedRole = 'NhaTuyenDung';
      } else {
        normalizedRole = 'FreelancerStudent';
      }
    }

    final rawUserId = payload['maUser'] ?? payload['userId'] ?? 0;
    final parsedUserId = rawUserId is int
        ? rawUserId
        : int.tryParse(rawUserId.toString()) ?? 0;

    final rawMaRole = payload['marole'] ?? (normalizedRole == 'NhaTuyenDung' ? 2 : 1);

    return AuthResponseModel(
      token: payload['token']?.toString() ??
          'local_session_${DateTime.now().millisecondsSinceEpoch}',
      email: (payload['email'] ?? '').toString(),
      role: normalizedRole,
      userId: parsedUserId,
      fullName: (payload['hovaten'] ?? payload['hoTen'] ?? payload['fullName'] ?? '').toString(),
      tentaikhoan: (payload['tentaikhoan'] ?? payload['userName'] ?? '').toString(),
      sodienthoai: payload['sodienthoai']?.toString() ?? payload['soDienThoai']?.toString(),
      avatarUrl: payload['avatarUrl']?.toString() ?? payload['avatar']?.toString(),
      marole: rawMaRole is int ? rawMaRole : int.tryParse(rawMaRole.toString()) ?? 1,
    );
  }

  Map<String, dynamic> toJson() => {
    'token': token,
    'email': email,
    'role': role,
    'userId': userId,
    'fullName': fullName,
    'tentaikhoan': tentaikhoan,
    'sodienthoai': sodienthoai,
    'avatarUrl': avatarUrl,
    'marole': marole,
    // Tiếng Việt
    'maUser': userId,
    'hoTen': fullName,
    'vaiTro': role,
  };
}

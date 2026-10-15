class AuthResponseModel {
  final String token;
  final String email;
  final String role;
  final int userId;
  final String fullName;

  const AuthResponseModel({
    required this.token,
    required this.email,
    required this.role,
    this.userId = 0,
    this.fullName = '',
  });

  factory AuthResponseModel.fromJson(Map<String, dynamic> json) {
    final payload = json['data'] is Map<String, dynamic>
        ? json['data'] as Map<String, dynamic>
        : json;

    final rawRole = payload['role'] ?? payload['tenrole'] ?? payload['marole'];
    String normalizedRole = 'FreelancerStudent';

    if (rawRole is int) {
      normalizedRole = rawRole == 2 ? 'NhaTuyenDung' : 'FreelancerStudent';
    } else if (rawRole is String) {
      final value = rawRole.trim();
      if (value.toLowerCase().contains('nhatuyendung') ||
          value.toLowerCase().contains('employer')) {
        normalizedRole = 'NhaTuyenDung';
      } else {
        normalizedRole = 'FreelancerStudent';
      }
    }

    final rawUserId = payload['maUser'] ?? payload['userId'] ?? 0;
    final parsedUserId = rawUserId is int
        ? rawUserId
        : int.tryParse(rawUserId.toString()) ?? 0;

    return AuthResponseModel(
      token:
          payload['token'] ??
          'local_session_${DateTime.now().millisecondsSinceEpoch}',
      email: (payload['email'] ?? '').toString(),
      role: normalizedRole,
      userId: parsedUserId,
      fullName: (payload['hovaten'] ?? payload['fullName'] ?? '').toString(),
    );
  }

  Map<String, dynamic> toJson() => {
    'token': token,
    'email': email,
    'role': role,
    'userId': userId,
    'fullName': fullName,
  };
}

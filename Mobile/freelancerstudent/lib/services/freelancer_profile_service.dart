import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/freelancer_profile_model.dart';

class FreelancerProfileService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<FreelancerProfileModel>> getFreelancerProfile(
    int maUser,
  ) async {
    try {
      final listResponse = await _apiClient.get<Map<String, dynamic>>(
        '/FreelancerStudent/DanhSachFreelancerStudent',
      );

      final listPayload = listResponse.data is Map<String, dynamic>
          ? Map<String, dynamic>.from(listResponse.data as Map<String, dynamic>)
          : <String, dynamic>{};

      final listData = listPayload['data'] is List
          ? listPayload['data'] as List
          : const <dynamic>[];

      final matchedFreelancer = listData.cast<dynamic>().firstWhereOrNull(
        (item) => item is Map && (item['maUser'] as num?) == maUser,
      );

      final freelancerId = matchedFreelancer is Map
          ? (matchedFreelancer['maFreelancerStudents'] is num
                ? matchedFreelancer['maFreelancerStudents'] as num
                : int.tryParse(
                    matchedFreelancer['maFreelancerStudents']?.toString() ?? '',
                  ))
          : null;

      if (freelancerId == null) {
        return ApiResponse<FreelancerProfileModel>(
          isSuccess: false,
          message: 'Không tìm thấy hồ sơ freelancer cho tài khoản này.',
        );
      }

      final response = await _apiClient.get<Map<String, dynamic>>(
        '/FreelancerStudent/Profile/$freelancerId',
      );

      final payload = response.data is Map<String, dynamic>
          ? Map<String, dynamic>.from(response.data as Map<String, dynamic>)
          : <String, dynamic>{};

      final data = payload['data'] is Map<String, dynamic>
          ? Map<String, dynamic>.from(payload['data'] as Map<String, dynamic>)
          : <String, dynamic>{};

      if (payload['success'] == true || data.isNotEmpty) {
        return ApiResponse<FreelancerProfileModel>(
          isSuccess: true,
          data: FreelancerProfileModel.fromJson(data),
          message: payload['message'] ?? 'Tải thông tin freelancer thành công',
        );
      }
    } catch (_) {
      return ApiResponse<FreelancerProfileModel>(
        isSuccess: false,
        message: 'Không thể tải hồ sơ người dùng từ máy chủ.',
      );
    }

    return ApiResponse<FreelancerProfileModel>(
      isSuccess: false,
      message: 'Không có dữ liệu hồ sơ người dùng.',
    );
  }
}

extension _ListExt<T> on Iterable<T> {
  T? firstWhereOrNull(bool Function(T element) test) {
    for (final element in this) {
      if (test(element)) {
        return element;
      }
    }
    return null;
  }
}

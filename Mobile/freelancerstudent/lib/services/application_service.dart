import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/application_model.dart';

class ApplicationService {
  final ApiClient _apiClient;

  ApplicationService({ApiClient? apiClient})
    : _apiClient = apiClient ?? ApiClient();

  Future<ApiResponse<ApplicationModel>> applyJob(
    String jobId,
    Map<String, dynamic> data,
  ) async {
    final userId = data['maFreelancerStudents'] ?? data['maFreelancerStudent'];
    final payload = <String, dynamic>{
      ...data,
      'maJob': data['maJob'] ?? jobId,
      'maFreelancerStudent': userId ?? 0,
      'maFreelancerStudents': userId ?? 0,
      'thuGioiThieu': data['thuGioiThieu'] ?? '',
      'status': data['status'] ?? 'ChoDuyet',
      'thulaoDeXuat': data['thulaoDeXuat'] ?? 0,
    };

    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/UngTuyen',
        payload,
      );

      if (response.statusCode == null ||
          response.statusCode! < 200 ||
          response.statusCode! >= 300) {
        final message = (response.data is Map)
            ? (response.data as Map)['message']?.toString() ??
                  'Ứng tuyển thất bại.'
            : 'Ứng tuyển thất bại.';
        return ApiResponse<ApplicationModel>(
          isSuccess: false,
          message: message,
        );
      }

      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawData = body['data'] ?? body;
      final json = rawData is Map<String, dynamic>
          ? rawData
          : (rawData is Map
                ? Map<String, dynamic>.from(rawData)
                : <String, dynamic>{});

      if (json.isEmpty) {
        return ApiResponse<ApplicationModel>(
          isSuccess: false,
          message: 'Backend không trả về dữ liệu đơn ứng tuyển.',
        );
      }

      return ApiResponse<ApplicationModel>(
        isSuccess: true,
        message: body['message']?.toString() ?? 'Ứng tuyển thành công!',
        data: ApplicationModel.fromJson(json),
      );
    } on DioException catch (e) {
      final backendMessage = (e.response?.data is Map)
          ? (e.response?.data as Map)['message']?.toString() ?? ''
          : '';
      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: backendMessage.isNotEmpty
            ? backendMessage
            : 'Ứng tuyển thất bại. Vui lòng thử lại.',
      );
    }
  }

  Future<ApiResponse<ApplicationModel>> updateApplicationStatus(
    String applicationId,
    String status,
  ) async {
    final normalizedStatus = status.trim();
    if (applicationId.trim().isEmpty) {
      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: 'Không tìm thấy mã đơn ứng tuyển để cập nhật trạng thái.',
      );
    }

    final candidatePayload = <String, dynamic>{
      'trangThaiUngTuyen': normalizedStatus,
    };
    final candidates = <String>[
      '/JobPost/CapNhatTrangThaiUngTuyen/$applicationId',
      '/UngTuyen/$applicationId/status',
      '/UngTuyen/$applicationId',
      '/UngTuyen',
    ];

    for (final endpoint in candidates) {
      try {
        final response = await _apiClient.put<Map<String, dynamic>>(
          endpoint,
          endpoint == '/UngTuyen' ? candidatePayload : candidatePayload,
        );

        if (response.statusCode == null ||
            response.statusCode! < 200 ||
            response.statusCode! >= 300) {
          continue;
        }

        final body = response.data is Map<String, dynamic>
            ? response.data as Map<String, dynamic>
            : <String, dynamic>{};
        final rawData = body['data'] ?? body;
        final json = rawData is Map<String, dynamic>
            ? rawData
            : (rawData is Map
                  ? Map<String, dynamic>.from(rawData)
                  : <String, dynamic>{});

        if (json.isEmpty) {
          return ApiResponse<ApplicationModel>(
            isSuccess: false,
            message: 'Cập nhật trạng thái chưa được backend hỗ trợ trên API hiện có.',
          );
        }

        return ApiResponse<ApplicationModel>(
          isSuccess: true,
          message:
              body['message']?.toString() ?? 'Cập nhật trạng thái thành công!',
          data: ApplicationModel.fromJson(json),
        );
      } on DioException catch (e) {
        final statusCode = e.response?.statusCode;
        if (statusCode != 404 && statusCode != 405 && statusCode != 400) {
          final backendMessage = (e.response?.data is Map)
              ? (e.response?.data as Map)['message']?.toString() ?? ''
              : '';
          return ApiResponse<ApplicationModel>(
            isSuccess: false,
            message: backendMessage.isNotEmpty
                ? backendMessage
                : 'Cập nhật trạng thái thất bại.',
          );
        }
      }
    }

    return ApiResponse<ApplicationModel>(
      isSuccess: false,
      message: 'Cập nhật trạng thái chưa được backend hỗ trợ trên API hiện có. Không có dữ liệu mẫu được tạo ở mobile.',
    );
  }

  Future<ApiResponse<List<ApplicationModel>>> getMyApplications({
    int? userId,
  }) async {
    try {
      final targetUserId = userId ?? 0;
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/UngTuyen/my-applications',
        queryParameters: targetUserId > 0 ? {'maUser': targetUserId} : null,
      );

      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawData = body['data'] ?? <dynamic>[];
      final items = rawData is List ? rawData : <dynamic>[];

      return ApiResponse<List<ApplicationModel>>(
        isSuccess: true,
        data: items
            .map(
              (item) => ApplicationModel.fromJson(item as Map<String, dynamic>),
            )
            .toList(),
        message: items.isEmpty ? 'Bạn chưa có đơn ứng tuyển nào.' : '',
      );
    } on DioException catch (e) {
      final status = e.response?.statusCode;
      if (status == 404 || status == 400) {
        return ApiResponse<List<ApplicationModel>>(
          isSuccess: true,
          data: const <ApplicationModel>[],
          message: 'Bạn chưa có đơn ứng tuyển nào.',
        );
      }

      return ApiResponse<List<ApplicationModel>>(
        isSuccess: false,
        message: 'Không thể tải danh sách đơn ứng tuyển. Vui lòng thử lại sau.',
      );
    } catch (e) {
      return ApiResponse<List<ApplicationModel>>(
        isSuccess: false,
        message: 'Không thể tải danh sách đơn ứng tuyển. Vui lòng thử lại sau.',
      );
    }
  }
}

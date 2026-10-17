import 'package:dio/dio.dart';

import '../core/constants/api_endpoints.dart';
import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/application_model.dart';

class ApplicationService {
  final ApiClient _apiClient;

  ApplicationService({ApiClient? apiClient})
      : _apiClient = apiClient ?? ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Sinh viên nộp đơn ứng tuyển (JobPostController.NopDonUngTuyen - POST api/JobPost/UngTuyen)
  Future<ApiResponse<ApplicationModel>> nopDonUngTuyen({
    required String maJob,
    required int maFreelancerStudents,
    required String thuGioiThieu,
    required double thulaoDeXuat,
    String? thoiGianHoanThanhDeXuat,
  }) async {
    final payload = <String, dynamic>{
      'maJob': maJob,
      'maFreelancerStudents': maFreelancerStudents,
      'thuGioiThieu': thuGioiThieu,
      'thulaoDeXuat': thulaoDeXuat,
      'thoiGianHoanThanhDeXuat': thoiGianHoanThanhDeXuat ?? '14 ngày',
    };

    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.nopDonUngTuyen,
        payload,
      );

      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      if (body['success'] == true || response.statusCode == 200) {
        return ApiResponse<ApplicationModel>(
          isSuccess: true,
          message: body['message']?.toString() ?? 'Nộp đơn ứng tuyển thành công!',
          data: ApplicationModel.fromJson(payload),
        );
      }

      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: body['message']?.toString() ?? 'Ứng tuyển thất bại.',
      );
    } on DioException catch (e) {
      final backendMessage = (e.response?.data is Map)
          ? (e.response?.data as Map)['message']?.toString() ?? ''
          : '';
      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: backendMessage.isNotEmpty
            ? backendMessage
            : 'Ứng tuyển thất bại: ${e.message}',
      );
    } catch (e) {
      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: 'Lỗi ứng tuyển: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng duyệt đơn ứng tuyển (JobPostController.DuyetUngTuyen - POST api/JobPost/DuyetUngTuyen)
  /// trangThai: 'ChapNhan' hoặc 'TuChoi'
  Future<ApiResponse<bool>> duyetUngTuyen({
    required int maUngTuyen,
    required String trangThai, // 'ChapNhan' hoặc 'TuChoi'
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.duyetUngTuyen,
        {
          'maUngTuyen': maUngTuyen,
          'trangThai': trangThai,
        },
      );

      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật trạng thái thành công!',
      );
    } on DioException catch (e) {
      final backendMessage = (e.response?.data is Map)
          ? (e.response?.data as Map)['message']?.toString() ?? ''
          : '';
      return ApiResponse<bool>(
        isSuccess: false,
        message: backendMessage.isNotEmpty
            ? backendMessage
            : 'Duyệt ứng tuyển thất bại.',
      );
    }
  }

  /// Lấy danh sách ứng tuyển theo người dùng (JobPostController.layDSUngTuyen - GET api/JobPost/DanhSachUngTuyen/{maUser})
  Future<ApiResponse<List<ApplicationModel>>> layDanhSachUngTuyenTheoUser(
    int maUser,
  ) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhSachUngTuyen}/$maUser',
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
        message: items.isEmpty ? 'Chưa có đơn ứng tuyển nào.' : '',
      );
    } on DioException catch (e) {
      return ApiResponse<List<ApplicationModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải danh sách đơn ứng tuyển: ${e.message}',
      );
    } catch (e) {
      return ApiResponse<List<ApplicationModel>>(
        isSuccess: false,
        message: 'Không thể tải danh sách đơn ứng tuyển.',
      );
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<ApiResponse<ApplicationModel>> applyJob(
    String jobId,
    Map<String, dynamic> data,
  ) {
    final rawUserId = data['maFreelancerStudents'] ?? data['maFreelancerStudent'] ?? 0;
    final parsedUserId = rawUserId is int ? rawUserId : int.tryParse(rawUserId.toString()) ?? 0;
    final rawBudget = data['thulaoDeXuat'] ?? data['proposedBudget'] ?? 0;
    final parsedBudget = rawBudget is num ? rawBudget.toDouble() : 0.0;

    return nopDonUngTuyen(
      maJob: jobId,
      maFreelancerStudents: parsedUserId,
      thuGioiThieu: (data['thuGioiThieu'] ?? data['coverLetter'] ?? '').toString(),
      thulaoDeXuat: parsedBudget,
    );
  }

  Future<ApiResponse<ApplicationModel>> updateApplicationStatus(
    String applicationId,
    String status,
  ) async {
    final parsedId = int.tryParse(applicationId) ?? 0;
    final res = await duyetUngTuyen(
      maUngTuyen: parsedId,
      trangThai: status == 'Accepted' ? 'ChapNhan' : (status == 'Rejected' ? 'TuChoi' : status),
    );
    return ApiResponse<ApplicationModel>(
      isSuccess: res.isSuccess,
      message: res.message,
    );
  }

  Future<ApiResponse<List<ApplicationModel>>> getMyApplications({
    int? userId,
  }) => layDanhSachUngTuyenTheoUser(userId ?? 0);
}

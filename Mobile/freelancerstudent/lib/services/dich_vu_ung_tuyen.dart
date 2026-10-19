import 'package:dio/dio.dart';

import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../core/utils/tien_ich_loi_api.dart';
import '../models/mo_hinh_ung_tuyen.dart';

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
    int? maUser,
    int? maFreelancerStudents,
    required String thuGioiThieu,
    double? thulaoDeXuat,
    String? thoiGianHoanThanhDeXuat,
  }) async {
    final effectiveUserId = maUser ?? maFreelancerStudents ?? 0;
    final payload = <String, dynamic>{
      'maJob': maJob,
      'maUser': effectiveUserId,
      'thuGioiThieu': thuGioiThieu,
      if (thulaoDeXuat != null && thulaoDeXuat > 0)
        'thulaoDeXuat': thulaoDeXuat,
      if (thoiGianHoanThanhDeXuat != null &&
          thoiGianHoanThanhDeXuat.trim().isNotEmpty)
        'thoiGianHoanThanhDeXuat': thoiGianHoanThanhDeXuat.trim(),
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
          message:
              body['message']?.toString() ?? 'Nộp đơn ứng tuyển thành công!',
          data: ApplicationModel.fromJson(payload),
        );
      }

      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: body['message']?.toString() ?? 'Ứng tuyển thất bại.',
      );
    } on DioException catch (e) {
      return ApiResponse<ApplicationModel>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Ứng tuyển thất bại.'),
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
        data: body['success'] == true,
        message:
            body['message']?.toString() ?? 'Cập nhật trạng thái thành công!',
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
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi cập nhật trạng thái đơn ứng tuyển: $e',
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
        isSuccess: body['success'] != false,
        data: items
            .map(
              (item) => ApplicationModel.fromJson(item as Map<String, dynamic>),
            )
            .toList(),
        message: body['success'] == false
            ? body['message']?.toString() ?? 'Không thể tải danh sách đơn.'
            : items.isEmpty
            ? 'Chưa có đơn ứng tuyển nào.'
            : '',
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
    final rawUserId =
        data['maUser'] ??
        data['userId'] ??
        data['maFreelancerStudents'] ??
        data['maFreelancerStudent'] ??
        0;
    final parsedUserId =
        rawUserId is int ? rawUserId : int.tryParse(rawUserId.toString()) ?? 0;
    final rawBudget = data['thulaoDeXuat'] ?? data['proposedBudget'] ?? 0;
    final parsedBudget = rawBudget is num ? rawBudget.toDouble() : 0.0;

    return nopDonUngTuyen(
      maJob: jobId,
      maUser: parsedUserId,
      maFreelancerStudents: parsedUserId,
      thuGioiThieu:
          (data['thuGioiThieu'] ?? data['coverLetter'] ?? '').toString(),
      thulaoDeXuat: parsedBudget > 0 ? parsedBudget : null,
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

import '../core/constants/api_endpoints.dart';
import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/job_post_model.dart';

class JobService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy danh sách việc làm (tương ứng JobPostController.layDanhSachJobPost)
  Future<ApiResponse<List<JobPostModel>>> layDanhSachJobPost({
    int page = 1,
    String tuKhoa = '',
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.danhSachJobPost,
        queryParameters: {'page': page, 'tuKhoa': tuKhoa},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List
          ? body['data'] as List
          : body['items'] is List
              ? body['items'] as List
              : <dynamic>[];

      return ApiResponse<List<JobPostModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => JobPostModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<JobPostModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải danh sách việc làm: ${e.toString()}',
      );
    }
  }

  /// Lấy chi tiết công việc theo mã Job
  Future<ApiResponse<JobPostModel>> layChiTietJobPost(String maJob) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.chiTietJobPost}/$maJob',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;
      return ApiResponse<JobPostModel>(
        isSuccess: true,
        data: JobPostModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<JobPostModel>(
        isSuccess: false,
        message: 'Lỗi khi lấy chi tiết công việc: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng tạo Job mới (JobPostController.TaoJob)
  Future<ApiResponse<JobPostModel>> taoJob(
    Map<String, dynamic> duLieuJob,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.taoJob,
        duLieuJob,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;
      return ApiResponse<JobPostModel>(
        isSuccess: true,
        message: body['message']?.toString() ?? 'Tạo Job thành công!',
        data: JobPostModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<JobPostModel>(
        isSuccess: false,
        message: 'Lỗi khi tạo công việc: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng lấy các bài đăng của mình (JobPostController.LayJobCuaToi)
  Future<ApiResponse<List<JobPostModel>>> layJobCuaToi(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.jobCuaToi}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<JobPostModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => JobPostModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<JobPostModel>>(
        isSuccess: false,
        message: 'Lỗi khi lấy danh sách việc làm của tôi: ${e.toString()}',
      );
    }
  }

  /// Cập nhật bài tuyển dụng (JobPostController.CapNhat)
  Future<ApiResponse<bool>> capNhatJob(Map<String, dynamic> duLieu) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatJob,
        duLieu,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<ApiResponse<List<JobPostModel>>> getJobs({
    int page = 1,
    String keyword = '',
  }) => layDanhSachJobPost(page: page, tuKhoa: keyword);

  Future<ApiResponse<JobPostModel>> getJobDetail(String id) =>
      layChiTietJobPost(id);

  Future<ApiResponse<JobPostModel>> createJob(Map<String, dynamic> payload) =>
      taoJob(payload);
}

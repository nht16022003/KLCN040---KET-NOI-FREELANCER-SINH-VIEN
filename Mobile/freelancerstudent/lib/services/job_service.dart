import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/job_post_model.dart';

class JobService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<List<JobPostModel>>> getJobs({
    int page = 1,
    String keyword = '',
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/JobPost/DanhSachJobPost',
        queryParameters: {'page': page, 'keyword': keyword},
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
        message: e.toString(),
      );
    }
  }

  Future<ApiResponse<JobPostModel>> getJobDetail(String id) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/JobPost/ChiTietJobPost/$id',
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
      return ApiResponse<JobPostModel>(isSuccess: false, message: e.toString());
    }
  }

  Future<ApiResponse<JobPostModel>> createJob(
    Map<String, dynamic> payload,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/JobPost/TaoJob',
        payload,
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
      return ApiResponse<JobPostModel>(isSuccess: false, message: e.toString());
    }
  }
}

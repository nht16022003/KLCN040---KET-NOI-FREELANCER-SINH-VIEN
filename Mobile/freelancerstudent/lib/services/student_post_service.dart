import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/student_post_model.dart';

class StudentPostService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<List<StudentPostModel>>> getStudentPosts() async {
    try {
      final response = await _apiClient.get<List<dynamic>>('/BaiDangTimViec');
      final items = response.data ?? <dynamic>[];
      return ApiResponse<List<StudentPostModel>>(
        isSuccess: true,
        data: items
            .map(
              (item) => StudentPostModel.fromJson(item as Map<String, dynamic>),
            )
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<StudentPostModel>>(
        isSuccess: false,
        message: e.toString(),
      );
    }
  }

  Future<ApiResponse<StudentPostModel>> createStudentPost(
    Map<String, dynamic> payload,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/BaiDangTimViec',
        payload,
      );
      final json = response.data ?? <String, dynamic>{};
      return ApiResponse<StudentPostModel>(
        isSuccess: true,
        data: StudentPostModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<StudentPostModel>(
        isSuccess: false,
        message: e.toString(),
      );
    }
  }
}

import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/user_model.dart';

class UserService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<UserModel>> getProfile() async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/User/profile',
      );
      final json = response.data ?? <String, dynamic>{};
      return ApiResponse<UserModel>(
        isSuccess: true,
        data: UserModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<UserModel>(isSuccess: false, message: e.toString());
    }
  }

  Future<ApiResponse<UserModel>> updateProfile(
    Map<String, dynamic> payload,
  ) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        '/User/profile',
        payload,
      );
      final json = response.data ?? <String, dynamic>{};
      return ApiResponse<UserModel>(
        isSuccess: true,
        data: UserModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<UserModel>(isSuccess: false, message: e.toString());
    }
  }
}

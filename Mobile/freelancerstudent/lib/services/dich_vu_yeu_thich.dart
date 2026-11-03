import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_luu_viec.dart';
import '../models/mo_hinh_freelancer_yeu_thich.dart';

class YeuThichService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // FREELANCER LƯU CÔNG VIỆC
  // ==========================================

  /// Kiểm tra công việc đã được freelancer lưu chưa
  Future<ApiResponse<bool>> kiemTraLuuViec({
    required int maUser,
    required String maJob,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.kiemTraLuuViec,
        queryParameters: {'maUser': maUser, 'maJob': maJob},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final daLuu = body['daLuu'] == true;
      return ApiResponse<bool>(isSuccess: true, data: daLuu);
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Freelancer lưu công việc
  Future<ApiResponse<bool>> luuViec({
    required int maUser,
    required String maJob,
    String? ghiChu,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.luuViec,
        {
          'maUser': maUser,
          'maJob': maJob,
          'ghiChu': ghiChu,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Freelancer bỏ lưu công việc
  Future<ApiResponse<bool>> boLuuViec({
    required int maUser,
    required String maJob,
  }) async {
    try {
      final response = await _apiClient.delete<Map<String, dynamic>>(
        ApiEndpoints.boLuuViec,
        queryParameters: {'maUser': maUser, 'maJob': maJob},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Lấy danh sách công việc đã lưu của Freelancer
  Future<ApiResponse<List<CongViecDaLuuModel>>> layDanhSachViecDaLuu(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhSachViecDaLuu}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];
      final items = rawItems
          .map((item) => CongViecDaLuuModel.fromJson(item as Map<String, dynamic>))
          .toList();
      return ApiResponse<List<CongViecDaLuuModel>>(isSuccess: true, data: items);
    } catch (e) {
      return ApiResponse<List<CongViecDaLuuModel>>(
        isSuccess: false,
        message: 'Lỗi tải danh sách việc đã lưu: ${e.toString()}',
      );
    }
  }

  // ==========================================
  // NHÀ TUYỂN DỤNG LƯU FREELANCER YÊU THÍCH
  // ==========================================

  /// Kiểm tra freelancer đã được nhà tuyển dụng lưu chưa
  Future<ApiResponse<bool>> kiemTraLuuFreelancer({
    required int maUser,
    required int maFreelancerStudent,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.kiemTraLuuFreelancer,
        queryParameters: {
          'maUser': maUser,
          'maFreelancerStudent': maFreelancerStudent,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final daLuu = body['daLuu'] == true;
      return ApiResponse<bool>(isSuccess: true, data: daLuu);
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Nhà tuyển dụng lưu freelancer
  Future<ApiResponse<bool>> luuFreelancer({
    required int maUser,
    required int maFreelancerStudent,
    String? ghiChu,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.luuFreelancer,
        {
          'maUser': maUser,
          'maFreelancerStudent': maFreelancerStudent,
          'ghiChu': ghiChu,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Nhà tuyển dụng bỏ lưu freelancer
  Future<ApiResponse<bool>> boLuuFreelancer({
    required int maUser,
    required int maFreelancerStudent,
  }) async {
    try {
      final response = await _apiClient.delete<Map<String, dynamic>>(
        ApiEndpoints.boLuuFreelancer,
        queryParameters: {
          'maUser': maUser,
          'maFreelancerStudent': maFreelancerStudent,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Lấy danh sách Freelancer yêu thích của Nhà tuyển dụng
  Future<ApiResponse<List<FreelancerYeuThichModel>>> layDanhSachFreelancerYeuThich(
      int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhSachFreelancerYeuThich}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];
      final items = rawItems
          .map((item) =>
              FreelancerYeuThichModel.fromJson(item as Map<String, dynamic>))
          .toList();
      return ApiResponse<List<FreelancerYeuThichModel>>(
          isSuccess: true, data: items);
    } catch (e) {
      return ApiResponse<List<FreelancerYeuThichModel>>(
        isSuccess: false,
        message: 'Lỗi tải danh sách freelancer yêu thích: ${e.toString()}',
      );
    }
  }
}


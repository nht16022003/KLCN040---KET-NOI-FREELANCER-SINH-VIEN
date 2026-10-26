import 'dart:io';
import 'package:dio/dio.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';

class MinhChungService {
  final ApiClient _apiClient = ApiClient();

  /// Tải lên ảnh Thẻ sinh viên, gọi OCR trích xuất thông tin
  Future<ApiResponse<Map<String, dynamic>>> quetTheSinhVien(File imageFile, int maUser) async {
    try {
      final fileName = imageFile.path.split('/').last.split('\\').last;
      final formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          imageFile.path,
          filename: fileName,
        ),
      });

      final response = await _apiClient.post<Map<String, dynamic>>(
        '/MinhChung/quet-the-sinh-vien',
        formData,
        queryParameters: {'maUser': maUser},
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: success,
        data: data['data'] is Map<String, dynamic> ? data['data'] as Map<String, dynamic> : null,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: false,
        message: 'Lỗi nhận diện thẻ sinh viên: $e',
      );
    }
  }

  /// Sinh viên xác nhận thông tin thẻ, cập nhật hồ sơ và lưu minh chứng
  Future<ApiResponse<bool>> xacNhanTheSinhVien(Map<String, dynamic> payload) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/MinhChung/xac-nhan-the-sinh-vien',
        payload,
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      return ApiResponse<bool>(
        isSuccess: success,
        data: success,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi xác nhận hồ sơ: $e',
      );
    }
  }

  /// Lấy lịch sử minh chứng đã gửi của sinh viên
  Future<ApiResponse<List<Map<String, dynamic>>>> layLichSuMinhChung(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/MinhChung/lich-su/$maUser',
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      final rawList = data['data'] is List ? data['data'] as List : [];
      final list = rawList.map((e) => Map<String, dynamic>.from(e as Map)).toList();

      return ApiResponse<List<Map<String, dynamic>>>(
        isSuccess: success,
        data: list,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<List<Map<String, dynamic>>>(
        isSuccess: false,
        message: 'Lỗi tải lịch sử minh chứng: $e',
      );
    }
  }

  /// Lấy danh sách các chuyên ngành đào tạo
  Future<ApiResponse<List<Map<String, dynamic>>>> layDanhSachChuyenNganh() async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/MinhChung/chuyen-nganh',
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      final rawList = data['data'] is List ? data['data'] as List : [];
      final list = rawList.map((e) => Map<String, dynamic>.from(e as Map)).toList();

      return ApiResponse<List<Map<String, dynamic>>>(
        isSuccess: success,
        data: list,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<List<Map<String, dynamic>>>(
        isSuccess: false,
        message: 'Lỗi tải danh sách chuyên ngành: $e',
      );
    }
  }

  /// Lấy thông tin hồ sơ hiện tại của sinh viên để đối chiếu
  Future<ApiResponse<Map<String, dynamic>>> layThongTinHienTai(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/MinhChung/thong-tin-hien-tai/$maUser',
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: success,
        data: data['data'] is Map<String, dynamic> ? data['data'] as Map<String, dynamic> : null,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: false,
        message: 'Lỗi tải thông tin sinh viên: $e',
      );
    }
  }

  /// Kiểm tra xem sinh viên đã được xác thực minh chứng (trạng thái 'Đã xác minh') hay chưa
  Future<ApiResponse<Map<String, dynamic>>> kiemTraTrangThaiXacThuc(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/MinhChung/kiem-tra-xac-thuc/$maUser',
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: success,
        data: data['data'] is Map<String, dynamic> ? data['data'] as Map<String, dynamic> : null,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: false,
        message: 'Lỗi kiểm tra trạng thái xác thực: $e',
      );
    }
  }

  /// Phê duyệt minh chứng (Dành cho Admin hoặc Demo kiểm thử)
  Future<ApiResponse<bool>> pheDuyetMinhChung(int maMinhChung, bool chapNhan, {String? lyDoTuChoi}) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/MinhChung/phe-duyet',
        {
          'maMinhChung': maMinhChung,
          'chapNhan': chapNhan,
          'lyDoTuChoi': lyDoTuChoi,
        },
      );

      final data = response.data ?? {};
      final bool success = data['success'] ?? data['isSuccess'] ?? false;
      return ApiResponse<bool>(
        isSuccess: success,
        data: success,
        message: data['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi duyệt minh chứng: $e',
      );
    }
  }
}


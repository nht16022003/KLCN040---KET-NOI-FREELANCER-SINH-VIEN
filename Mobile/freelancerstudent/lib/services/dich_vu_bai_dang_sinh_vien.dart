import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_bai_dang_sinh_vien.dart';

class StudentPostService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy tất cả bài đăng tìm việc khả dụng (BaiDangTimViecController.LayTatCa - GET api/BaiDangTimViec/TatCa)
  Future<ApiResponse<List<StudentPostModel>>> layTatCaBaiDangTimViec() async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.tatCaBaiDangTimViec,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<StudentPostModel>>(
        isSuccess: true,
        data: rawItems
            .map(
              (item) => StudentPostModel.fromJson(item as Map<String, dynamic>),
            )
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<StudentPostModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải danh sách bài tìm việc: ${e.toString()}',
      );
    }
  }

  /// Sinh viên tạo bài đăng tìm việc mới (BaiDangTimViecController.TaoBaiDang - POST api/BaiDangTimViec/TaoBaiDang)
  Future<ApiResponse<StudentPostModel>> taoBaiDangTimViec(
    Map<String, dynamic> duLieu,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.taoBaiDangTimViec,
        duLieu,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<StudentPostModel>(
        isSuccess: true,
        data: StudentPostModel.fromJson(json),
        message: body['message']?.toString() ?? 'Đăng tin tìm việc thành công!',
      );
    } catch (e) {
      return ApiResponse<StudentPostModel>(
        isSuccess: false,
        message: 'Lỗi khi tạo bài đăng tìm việc: ${e.toString()}',
      );
    }
  }

  /// Lấy các bài đăng tìm việc của tôi (BaiDangTimViecController.LayDanhSachCuaToi - GET api/BaiDangTimViec/CuaToi/{maUser})
  Future<ApiResponse<List<StudentPostModel>>> layBaiDangTimViecCuaToi(
    int maUser,
  ) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.baiDangTimViecCuaToi}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<StudentPostModel>>(
        isSuccess: true,
        data: rawItems
            .map(
              (item) => StudentPostModel.fromJson(item as Map<String, dynamic>),
            )
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<StudentPostModel>>(
        isSuccess: false,
        message: 'Lỗi khi lấy danh sách bài đăng: ${e.toString()}',
      );
    }
  }

  /// Đổi trạng thái bài đăng tìm việc (BaiDangTimViecController.DoiTrangThai)
  Future<ApiResponse<bool>> doiTrangThaiBaiDang(
    String maBaiDang,
    String trangThai,
  ) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        '${ApiEndpoints.doiTrangThaiBaiDangTimViec}?maBaiDang=$maBaiDang&trangThai=$trangThai',
        {},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật trạng thái thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  /// Xóa bài đăng tìm việc (BaiDangTimViecController.Xoa)
  Future<ApiResponse<bool>> xoaBaiDangTimViec(String maBaiDang) async {
    try {
      final response = await _apiClient.delete<Map<String, dynamic>>(
        '${ApiEndpoints.xoaBaiDangTimViec}/$maBaiDang',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Xóa bài đăng thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<ApiResponse<List<StudentPostModel>>> getStudentPosts() =>
      layTatCaBaiDangTimViec();

  Future<ApiResponse<StudentPostModel>> createStudentPost(
    Map<String, dynamic> payload,
  ) => taoBaiDangTimViec(payload);
}

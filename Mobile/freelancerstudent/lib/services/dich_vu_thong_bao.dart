import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_thong_bao.dart';

class ThongBaoService {
  final ApiClient _apiClient = ApiClient();

  /// Lấy danh sách thông báo phân trang (GET api/ThongBao?maUser=...&trang=...&kichThuocTrang=...)
  Future<ApiResponse<ThongBaoDanhSachModel>> layDanhSachThongBao({
    required int maUser,
    int trang = 1,
    int kichThuocTrang = 10,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.thongBao,
        queryParameters: {
          'maUser': maUser,
          'trang': trang,
          'kichThuocTrang': kichThuocTrang,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final data = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<ThongBaoDanhSachModel>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: ThongBaoDanhSachModel.fromJson(data),
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<ThongBaoDanhSachModel>(
        isSuccess: false,
        message: 'Lỗi tải danh sách thông báo: ${e.toString()}',
      );
    }
  }

  /// Đếm số lượng thông báo chưa đọc (GET api/ThongBao/chua-doc?maUser=...)
  Future<ApiResponse<int>> demChuaDoc(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.thongBaoChuaDoc,
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final count = int.tryParse(body['data']?.toString() ?? '0') ?? 0;

      return ApiResponse<int>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: count,
      );
    } catch (e) {
      return ApiResponse<int>(
        isSuccess: false,
        message: 'Lỗi đếm thông báo: ${e.toString()}',
      );
    }
  }

  /// Đánh dấu 1 thông báo là đã đọc (PUT api/ThongBao/{maThongBao}/da-doc?maUser=...)
  Future<ApiResponse<bool>> danhDauDaDoc({
    required int maThongBao,
    required int maUser,
  }) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        '${ApiEndpoints.thongBao}/$maThongBao/da-doc',
        {},
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi cập nhật thông báo: ${e.toString()}',
      );
    }
  }

  /// Đánh dấu tất cả thông báo là đã đọc (PUT api/ThongBao/da-doc-tat-ca?maUser=...)
  Future<ApiResponse<bool>> danhDauTatCaDaDoc(int maUser) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.thongBaoDaDocTatCa,
        {},
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi cập nhật thông báo: ${e.toString()}',
      );
    }
  }
}

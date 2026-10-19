import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_nha_tuyen_dung.dart';

class NhaTuyenDungService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy danh sách nhà tuyển dụng (NhaTuyenDungController.layDanhSachNTD - GET api/NhaTuyenDung/DanhSachNhaTuyenDung)
  Future<ApiResponse<List<NhaTuyenDungModel>>> layDanhSachNhaTuyenDung() async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.danhSachNhaTuyenDung,
      );

      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      final rawData = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<NhaTuyenDungModel>>(
        isSuccess: true,
        data: rawData
            .map((item) => NhaTuyenDungModel.fromJson(item as Map<String, dynamic>))
            .toList(),
        message: body['message']?.toString() ?? 'Lấy danh sách nhà tuyển dụng thành công!',
      );
    } catch (e) {
      return ApiResponse<List<NhaTuyenDungModel>>(
        isSuccess: false,
        message: 'Lỗi khi lấy danh sách nhà tuyển dụng: ${e.toString()}',
      );
    }
  }
}

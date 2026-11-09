import '../core/constants/diem_cuoi_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/utils/tien_ich_loi_api.dart';
import '../models/mo_hinh_ung_thue.dart';

class UngThueService {
  final ApiClient _apiClient = ApiClient();

  Map<String, dynamic> _body(dynamic data) {
    if (data is Map<String, dynamic>) return data;
    return <String, dynamic>{};
  }

  /// Gửi yêu cầu thuê Freelancer từ Nhà tuyển dụng (POST api/UngThue/GuiYeuCau)
  Future<ApiResponse<UngThueModel>> guiYeuCauThue(
      GuiYeuCauThueRequest request) async {
    try {
      final res = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.ungThueGuiYeuCau,
        request.toJson(),
      );
      final body = _body(res.data);
      final data = body['data'];
      if (data is Map<String, dynamic>) {
        return ApiResponse<UngThueModel>(
          isSuccess: true,
          data: UngThueModel.fromJson(data),
          message: body['message']?.toString() ??
              'Gửi yêu cầu thuê Freelancer thành công!',
        );
      }
      return ApiResponse<UngThueModel>(
        isSuccess: body['success'] == true,
        message:
            body['message']?.toString() ?? 'Gửi yêu cầu thuê thất bại.',
      );
    } catch (e) {
      return ApiResponse<UngThueModel>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể gửi yêu cầu thuê.'),
      );
    }
  }

  /// Lấy danh sách yêu cầu thuê gửi đến Freelancer (GET api/UngThue/DanhSachTheoFreelancer/{maUser})
  Future<ApiResponse<List<UngThueModel>>> layDanhSachTheoFreelancer(
      int maUser) async {
    try {
      final res = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.ungThueDanhSachTheoFreelancer}/$maUser',
      );
      final body = _body(res.data);
      final rawList = body['data'] is List ? body['data'] as List : <dynamic>[];
      final list = rawList
          .whereType<Map<String, dynamic>>()
          .map((e) => UngThueModel.fromJson(e))
          .toList();

      return ApiResponse<List<UngThueModel>>(
        isSuccess: true,
        data: list,
        message: body['message']?.toString() ??
            'Lấy danh sách yêu cầu thuê thành công',
      );
    } catch (e) {
      return ApiResponse<List<UngThueModel>>(
        isSuccess: false,
        message: layThongBaoLoiApi(e,
            macDinh: 'Không thể tải danh sách yêu cầu thuê.'),
      );
    }
  }

  /// Lấy danh sách yêu cầu thuê đã gửi của Nhà tuyển dụng (GET api/UngThue/DanhSachTheoNTD/{maUser})
  Future<ApiResponse<List<UngThueModel>>> layDanhSachTheoNTD(int maUser) async {
    try {
      final res = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.ungThueDanhSachTheoNTD}/$maUser',
      );
      final body = _body(res.data);
      final rawList = body['data'] is List ? body['data'] as List : <dynamic>[];
      final list = rawList
          .whereType<Map<String, dynamic>>()
          .map((e) => UngThueModel.fromJson(e))
          .toList();

      return ApiResponse<List<UngThueModel>>(
        isSuccess: true,
        data: list,
        message: body['message']?.toString() ??
            'Lấy danh sách yêu cầu thuê thành công',
      );
    } catch (e) {
      return ApiResponse<List<UngThueModel>>(
        isSuccess: false,
        message: layThongBaoLoiApi(e,
            macDinh: 'Không thể tải danh sách yêu cầu thuê.'),
      );
    }
  }

  /// Freelancer phản hồi yêu cầu thuê (Chấp nhận hoặc Từ chối) (PUT api/UngThue/PhanHoi)
  Future<ApiResponse<UngThueModel>> phanHoiYeuCauThue(
      PhanHoiYeuCauThueRequest request) async {
    try {
      final res = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.ungThuePhanHoi,
        request.toJson(),
      );
      final body = _body(res.data);
      final data = body['data'];
      if (data is Map<String, dynamic>) {
        return ApiResponse<UngThueModel>(
          isSuccess: true,
          data: UngThueModel.fromJson(data),
          message: body['message']?.toString() ??
              (request.chapNhan
                  ? 'Đã chấp nhận yêu cầu thuê!'
                  : 'Đã từ chối yêu cầu thuê.'),
        );
      }
      return ApiResponse<UngThueModel>(
        isSuccess: body['success'] == true,
        message: body['message']?.toString() ?? 'Phản hồi yêu cầu thất bại.',
      );
    } catch (e) {
      return ApiResponse<UngThueModel>(
        isSuccess: false,
        message:
            layThongBaoLoiApi(e, macDinh: 'Lỗi khi phản hồi yêu cầu thuê.'),
      );
    }
  }
}


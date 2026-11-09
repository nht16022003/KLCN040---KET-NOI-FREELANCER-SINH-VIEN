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

  /// Lấy hồ sơ chi tiết Nhà tuyển dụng theo maNhaTuyenDung (GET api/NhaTuyenDung/Profile/{maNhaTuyenDung:int})
  Future<ApiResponse<NhaTuyenDungProfileModel>> layProfileNhaTuyenDung(
    int maNhaTuyenDung,
  ) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.profileNhaTuyenDung}/$maNhaTuyenDung',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawData = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<NhaTuyenDungProfileModel>(
        isSuccess: body['success'] != false,
        data: NhaTuyenDungProfileModel.fromJson(rawData),
        message: body['message']?.toString() ?? 'Lấy hồ sơ nhà tuyển dụng thành công!',
      );
    } catch (e) {
      return ApiResponse<NhaTuyenDungProfileModel>(
        isSuccess: false,
        message: 'Lỗi khi tải hồ sơ nhà tuyển dụng: ${e.toString()}',
      );
    }
  }

  /// Lấy hồ sơ chi tiết Nhà tuyển dụng theo maUser (GET api/NhaTuyenDung/Profile/ByUser/{maUser:int})
  Future<ApiResponse<NhaTuyenDungProfileModel>> layProfileNhaTuyenDungTheoUser(
    int maUser,
  ) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.profileNhaTuyenDungTheoUser}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawData = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<NhaTuyenDungProfileModel>(
        isSuccess: body['success'] != false,
        data: NhaTuyenDungProfileModel.fromJson(rawData),
        message: body['message']?.toString() ?? 'Lấy hồ sơ nhà tuyển dụng thành công!',
      );
    } catch (e) {
      return ApiResponse<NhaTuyenDungProfileModel>(
        isSuccess: false,
        message: 'Lỗi khi tải hồ sơ nhà tuyển dụng: ${e.toString()}',
      );
    }
  }

  /// Cập nhật thông tin cơ bản Nhà tuyển dụng (PUT api/NhaTuyenDung/CapNhatThongTin)
  Future<ApiResponse<bool>> capNhatThongTinNTD({
    required int maUser,
    String? tencongty,
    String? linhvuc,
    String? diachi,
    String? link,
    String? hotenNguoiDaiDien,
    String? sdtLienHe,
  }) async {
    try {
      final res = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatThongTinNTD,
        {
          'maUser': maUser,
          if (tencongty != null) 'tencongty': tencongty,
          if (linhvuc != null) 'linhvuc': linhvuc,
          if (diachi != null) 'diachi': diachi,
          if (link != null) 'link': link,
          if (hotenNguoiDaiDien != null) 'hotenNguoiDaiDien': hotenNguoiDaiDien,
          if (sdtLienHe != null) 'sdtLienHe': sdtLienHe,
        },
      );
      final body = res.data is Map<String, dynamic>
          ? res.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] != false,
        data: true,
        message: body['message']?.toString() ??
            'Cập nhật thông tin thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        data: false,
        message:
            'Lỗi khi cập nhật thông tin nhà tuyển dụng: ${e.toString()}',
      );
    }
  }

  /// Cập nhật phần giới thiệu Nhà tuyển dụng (PUT api/NhaTuyenDung/CapNhatGioiThieu)
  Future<ApiResponse<bool>> capNhatGioiThieuNTD({
    required int maUser,
    String? gioithieu,
  }) async {
    try {
      final res = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatGioiThieuNTD,
        {
          'maUser': maUser,
          'gioithieu': gioithieu,
        },
      );
      final body = res.data is Map<String, dynamic>
          ? res.data as Map<String, dynamic>
          : <String, dynamic>{};
      return ApiResponse<bool>(
        isSuccess: body['success'] != false,
        data: true,
        message: body['message']?.toString() ??
            'Cập nhật giới thiệu thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        data: false,
        message:
            'Lỗi khi cập nhật giới thiệu nhà tuyển dụng: ${e.toString()}',
      );
    }
  }
}

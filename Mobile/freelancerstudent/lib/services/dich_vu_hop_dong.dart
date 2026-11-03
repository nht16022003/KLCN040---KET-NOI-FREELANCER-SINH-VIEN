import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_hop_dong.dart';

class HopDongService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy danh sách hợp đồng của Nhà tuyển dụng (HopDongController.GetDanhSachHopDong)
  Future<ApiResponse<List<HopDongModel>>> layDanhSachHopDongNTD(
    int maNhaTuyenDung, {
    String tuKhoa = '',
    String trangThai = '',
  }) async {
    try {
      final response = await _apiClient.get<List<dynamic>>(
        '${ApiEndpoints.danhSachHopDongNTD}/$maNhaTuyenDung',
        queryParameters: {
          if (tuKhoa.isNotEmpty) 'TuKhoa': tuKhoa,
          if (trangThai.isNotEmpty) 'TrangThai': trangThai,
        },
      );
      final rawItems = response.data ?? <dynamic>[];

      return ApiResponse<List<HopDongModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => HopDongModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<HopDongModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải danh sách hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Lấy danh sách hợp đồng của Freelancer (HopDongController.GetDanhSachHopDongFreelancer)
  Future<ApiResponse<List<HopDongModel>>> layDanhSachHopDongFreelancer(
    int maFreelancerStudent, {
    String tuKhoa = '',
    String trangThai = '',
  }) async {
    try {
      final response = await _apiClient.get<List<dynamic>>(
        '${ApiEndpoints.danhSachHopDongFreelancer}/$maFreelancerStudent',
        queryParameters: {
          if (tuKhoa.isNotEmpty) 'TuKhoa': tuKhoa,
          if (trangThai.isNotEmpty) 'TrangThai': trangThai,
        },
      );
      final rawItems = response.data ?? <dynamic>[];

      return ApiResponse<List<HopDongModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => HopDongModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<HopDongModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải danh sách hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Xem chi tiết hợp đồng (HopDongController.GetChiTietHopDong)
  Future<ApiResponse<HopDongModel>> layChiTietHopDong(String maHD) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.chiTietHopDong}/$maHD',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<HopDongModel>(
        isSuccess: true,
        data: HopDongModel.fromJson(body),
      );
    } catch (e) {
      return ApiResponse<HopDongModel>(
        isSuccess: false,
        message: 'Lỗi khi lấy chi tiết hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Xem hợp đồng theo mã Job (HopDongController.GetHopDongTheoJob)
  Future<ApiResponse<HopDongModel>> layHopDongTheoJob(String maJob) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.hopDongTheoJob}/$maJob',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<HopDongModel>(
        isSuccess: true,
        data: HopDongModel.fromJson(body),
      );
    } catch (e) {
      return ApiResponse<HopDongModel>(
        isSuccess: false,
        message: 'Lỗi khi lấy thông tin hợp đồng của công việc: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng tạo dự thảo hợp đồng (HopDongController.TaoHopDong)
  Future<ApiResponse<HopDongModel>> taoHopDong(
    Map<String, dynamic> duLieu, {
    int? maUserNTD,
  }) async {
    try {
      final url = maUserNTD != null
          ? '${ApiEndpoints.taoHopDong}?maUserNTD=$maUserNTD'
          : ApiEndpoints.taoHopDong;
      final response = await _apiClient.post<Map<String, dynamic>>(
        url,
        duLieu,
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<HopDongModel>(
        isSuccess: true,
        data: HopDongModel.fromJson(body),
        message: 'Tạo dự thảo hợp đồng thành công!',
      );
    } catch (e) {
      return ApiResponse<HopDongModel>(
        isSuccess: false,
        message: 'Lỗi tạo dự thảo hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Sinh viên xác nhận hoặc từ chối hợp đồng (HopDongController.XacNhanHopDong)
  Future<ApiResponse<bool>> xacNhanHopDong({
    required String maHD,
    required bool dongY,
    int? maUserFreelancer,
    String? lyDoTuChoi,
  }) async {
    try {
      final url = maUserFreelancer != null
          ? '${ApiEndpoints.xacNhanHopDong}?maUserFreelancer=$maUserFreelancer'
          : ApiEndpoints.xacNhanHopDong;
      final response = await _apiClient.post<Map<String, dynamic>>(
        url,
        {
          'maHD': maHD,
          'dongY': dongY,
          'lyDoTuChoi': ?lyDoTuChoi,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ??
            (dongY ? 'Đã xác nhận hợp đồng!' : 'Đã từ chối hợp đồng!'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi xác nhận hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng ký quỹ tiền hợp đồng (HopDongController.KyQuy)
  Future<ApiResponse<bool>> kyQuyHopDong({
    required String maHD,
    int? maUserNhaTuyenDung,
  }) async {
    try {
      final url = maUserNhaTuyenDung != null
          ? '${ApiEndpoints.kyQuyHopDong}?maUserNhaTuyenDung=$maUserNhaTuyenDung'
          : ApiEndpoints.kyQuyHopDong;
      final response = await _apiClient.post<Map<String, dynamic>>(
        url,
        {'maHD': maHD},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Ký quỹ hợp đồng thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi ký quỹ hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng nghiệm thu hợp đồng (HopDongController.NghiemThu)
  Future<ApiResponse<bool>> nghiemThuHopDong({
    required String maHD,
    int? maUser,
  }) async {
    try {
      final url = maUser != null
          ? '${ApiEndpoints.nghiemThuHopDong}?maUser=$maUser'
          : ApiEndpoints.nghiemThuHopDong;
      final response = await _apiClient.post<Map<String, dynamic>>(
        url,
        {'maHD': maHD},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Nghiệm thu hợp đồng thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi nghiệm thu hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng giải ngân / thanh toán hợp đồng cho Freelancer (HopDongController.ThanhToan)
  Future<ApiResponse<bool>> thanhToanHopDong({
    required String maHD,
    int? maUser,
  }) async {
    try {
      final url = maUser != null
          ? '${ApiEndpoints.thanhToanHopDong}?maUser=$maUser'
          : ApiEndpoints.thanhToanHopDong;
      final response = await _apiClient.post<Map<String, dynamic>>(
        url,
        {'maHD': maHD},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Giải ngân thanh toán thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi thanh toán hợp đồng: ${e.toString()}',
      );
    }
  }

  /// Lấy thông tin thanh toán giải ngân hợp đồng (HopDongController.GetThongTinThanhToan)
  Future<ApiResponse<HopDongThanhToanModel>> layThongTinThanhToan(
    String maHD, {
    int? maUser,
  }) async {
    try {
      final url = maUser != null
          ? '${ApiEndpoints.thanhToanHopDong}/$maHD?maUser=$maUser'
          : '${ApiEndpoints.thanhToanHopDong}/$maHD';
      final response = await _apiClient.get<Map<String, dynamic>>(url);
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final dataMap = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<HopDongThanhToanModel>(
        isSuccess: true,
        data: HopDongThanhToanModel.fromJson(dataMap),
      );
    } catch (e) {
      return ApiResponse<HopDongThanhToanModel>(
        isSuccess: false,
        message: 'Lỗi tải thông tin thanh toán: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng xóa hợp đồng dự thảo hoặc đã hủy (HopDongController.XoaHopDong)
  Future<ApiResponse<bool>> xoaHopDong(
    String maHD, {
    int? maUserNTD,
  }) async {
    try {
      final url = maUserNTD != null
          ? '${ApiEndpoints.baseUrl}/HopDong/$maHD?maUserNTD=$maUserNTD'
          : '${ApiEndpoints.baseUrl}/HopDong/$maHD';
      final response = await _apiClient.delete<Map<String, dynamic>>(url);
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Đã xóa hợp đồng thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi xóa hợp đồng: ${e.toString()}',
      );
    }
  }
}

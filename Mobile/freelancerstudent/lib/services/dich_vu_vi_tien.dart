import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_vi_tien.dart';

class WalletService {
  final ApiClient _apiClient = ApiClient();

  // ==========================================
  // HÀM TIẾNG VIỆT ĐỒNG BỘ VỚI BACKEND API
  // ==========================================

  /// Lấy thông tin ví theo mã User (WalletController.LayViTheoUser - GET api/Wallet/LayViTheoUser/{maUser})
  Future<ApiResponse<WalletModel>> layViTheoUser(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.layViTheoUser}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final json = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<WalletModel>(
        isSuccess: true,
        data: WalletModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<WalletModel>(
        isSuccess: false,
        message: 'Lỗi khi lấy thông tin ví: ${e.toString()}',
      );
    }
  }

  /// Tạo giao dịch nạp tiền VietQR SePay (GiaoDichNapTienController.TaoGiaoDich - POST api/GiaoDichNapTien/TaoGiaoDich/{maUser})
  Future<ApiResponse<Map<String, dynamic>>> taoGiaoDichNapTien(
    int maUser,
    double soTien,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '${ApiEndpoints.taoGiaoDichNapTien}/$maUser',
        {
          'soTien': soTien,
          'phuongThuc': 'ChuyenKhoanNganHang',
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final data = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<Map<String, dynamic>>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: data,
        message: body['message']?.toString() ?? 'Tạo giao dịch nạp tiền thành công',
      );
    } catch (e) {
      return ApiResponse<Map<String, dynamic>>(
        isSuccess: false,
        message: 'Lỗi tạo giao dịch nạp tiền: ${e.toString()}',
      );
    }
  }

  /// Xác nhận nạp tiền mô phỏng (GiaoDichNapTienController.XacNhanMoPhong - POST api/GiaoDichNapTien/XacNhanMoPhong/{maGiaoDich}?maUser={maUser})
  Future<ApiResponse<bool>> xacNhanNapTienMoPhong(
    String maGiaoDich,
    int maUser,
  ) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '${ApiEndpoints.xacNhanNapTienMoPhong}/$maGiaoDich?maUser=$maUser',
        {},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final isOk = body['success'] == true || response.statusCode == 200;
      return ApiResponse<bool>(
        isSuccess: isOk,
        data: isOk,
        message: body['message']?.toString() ??
            'Xác nhận nạp tiền mô phỏng thành công! Số dư đã được cộng vào ví.',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi xác nhận nạp tiền mô phỏng: ${e.toString()}',
      );
    }
  }

  /// Kiểm tra trạng thái giao dịch nạp tiền (GiaoDichNapTienController.TrangThai - GET api/GiaoDichNapTien/TrangThai/{maGiaoDich})
  Future<ApiResponse<String>> layTrangThaiGiaoDich(String maGiaoDich) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.trangThaiGiaoDichNapTien}/$maGiaoDich',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final status = (body['data']?['trangThai'] ?? body['trangThai'] ?? '').toString();

      return ApiResponse<String>(
        isSuccess: true,
        data: status,
      );
    } catch (e) {
      return ApiResponse<String>(
        isSuccess: false,
        message: 'Lỗi kiểm tra trạng thái giao dịch: ${e.toString()}',
      );
    }
  }

  /// Lấy lịch sử giao dịch ví (WalletController.LayLichSuGiaoDich - GET api/Wallet/LichSuGiaoDich/{maUser})
  Future<ApiResponse<List<LichSuGiaoDichModel>>> layLichSuGiaoDich(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.lichSuGiaoDichVi}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<LichSuGiaoDichModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => LichSuGiaoDichModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<LichSuGiaoDichModel>>(
        isSuccess: false,
        message: 'Lỗi khi lấy lịch sử giao dịch: ${e.toString()}',
      );
    }
  }

  /// Tạo yêu cầu rút tiền mô phỏng (WalletController.TaoYeuCauRutTien - POST api/Wallet/TaoYeuCauRutTien/{maUser})
  Future<ApiResponse<YeuCauRutTienModel>> taoYeuCauRutTien(
    int maUser, {
    required double soTienRut,
    required String tenNganHang,
    required String soTaiKhoan,
    required String tenChuTaiKhoan,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '${ApiEndpoints.taoYeuCauRutTien}/$maUser',
        {
          'soTienRut': soTienRut,
          'tenNganHang': tenNganHang,
          'soTaiKhoan': soTaiKhoan,
          'tenChuTaiKhoan': tenChuTaiKhoan,
        },
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final dataJson = body['data'] is Map<String, dynamic> ? body['data'] as Map<String, dynamic> : body;

      return ApiResponse<YeuCauRutTienModel>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: YeuCauRutTienModel.fromJson(dataJson),
        message: body['message']?.toString() ?? 'Tạo yêu cầu rút tiền thành công!',
      );
    } catch (e) {
      return ApiResponse<YeuCauRutTienModel>(
        isSuccess: false,
        message: 'Lỗi tạo yêu cầu rút tiền: ${e.toString()}',
      );
    }
  }

  /// Lấy danh sách yêu cầu rút tiền của người dùng (WalletController.LayDanhSachRutTien - GET api/Wallet/DanhSachRutTien/{maUser})
  Future<ApiResponse<List<YeuCauRutTienModel>>> layDanhSachRutTien(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhSachRutTien}/$maUser',
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<YeuCauRutTienModel>>(
        isSuccess: true,
        data: rawItems
            .map((item) => YeuCauRutTienModel.fromJson(item as Map<String, dynamic>))
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<YeuCauRutTienModel>>(
        isSuccess: false,
        message: 'Lỗi khi lấy danh sách yêu cầu rút tiền: ${e.toString()}',
      );
    }
  }

  /// Xác nhận duyệt hoặc hủy yêu cầu rút tiền mô phỏng (POST api/Wallet/XacNhanRutTienMoPhong/{maRutTien})
  Future<ApiResponse<bool>> xacNhanRutTienMoPhong(
    int maRutTien,
    int maUser, {
    required bool chapNhan,
    String? lyDo,
  }) async {
    try {
      final queryLyDo = (lyDo != null && lyDo.trim().isNotEmpty)
          ? '&lyDo=${Uri.encodeQueryComponent(lyDo.trim())}'
          : '';
      final response = await _apiClient.post<Map<String, dynamic>>(
        '${ApiEndpoints.xacNhanRutTienMoPhong}/$maRutTien?maUser=$maUser&chapNhan=$chapNhan$queryLyDo',
        {},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final isOk = body['success'] == true || response.statusCode == 200;
      return ApiResponse<bool>(
        isSuccess: isOk,
        data: isOk,
        message: body['message']?.toString() ??
            (chapNhan
                ? 'Đã xác nhận duyệt yêu cầu rút tiền mô phỏng thành công!'
                : 'Đã hủy yêu cầu rút tiền và hoàn tiền về số dư khả dụng.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi xử lý yêu cầu rút tiền mô phỏng: ${e.toString()}',
      );
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<ApiResponse<WalletModel>> getWalletBalance({int maUser = 1}) =>
      layViTheoUser(maUser);

  Future<ApiResponse<bool>> createDepositTransaction(int amount, {int maUser = 1}) async {
    final res = await taoGiaoDichNapTien(maUser, amount.toDouble());
    return ApiResponse<bool>(
      isSuccess: res.isSuccess,
      data: res.isSuccess,
      message: res.message,
    );
  }
}

import '../core/constants/api_endpoints.dart';
import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/wallet_model.dart';

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

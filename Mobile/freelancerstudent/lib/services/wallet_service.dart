import '../core/network/api_client.dart';
import '../core/network/api_response.dart';
import '../models/wallet_model.dart';

class WalletService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<WalletModel>> getWalletBalance() async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '/Wallet/balance',
      );
      final json = response.data ?? <String, dynamic>{};
      return ApiResponse<WalletModel>(
        isSuccess: true,
        data: WalletModel.fromJson(json),
      );
    } catch (e) {
      return ApiResponse<WalletModel>(isSuccess: false, message: e.toString());
    }
  }

  Future<ApiResponse<bool>> createDepositTransaction(int amount) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        '/Wallet/deposit',
        {'amount': amount},
      );
      return ApiResponse<bool>(
        isSuccess: response.statusCode != null && response.statusCode! < 400,
        data: true,
        message: 'Tạo giao dịch nạp tiền thành công',
      );
    } catch (e) {
      return ApiResponse<bool>(isSuccess: false, message: e.toString());
    }
  }
}

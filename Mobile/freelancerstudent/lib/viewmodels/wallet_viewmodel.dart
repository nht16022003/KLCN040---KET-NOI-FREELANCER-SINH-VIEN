import 'package:flutter/foundation.dart';

import '../models/wallet_model.dart';
import '../services/wallet_service.dart';

class WalletViewModel extends ChangeNotifier {
  final WalletService _walletService = WalletService();

  bool _isLoading = false;
  WalletModel? _wallet;
  String? _errorMessage;

  bool get isLoading => _isLoading;
  WalletModel? get wallet => _wallet;
  String? get errorMessage => _errorMessage;

  Future<void> fetchWalletBalance() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _walletService.getWalletBalance();
    if (response.isSuccess) {
      _wallet = response.data;
    } else {
      _errorMessage = response.message;
    }

    _isLoading = false;
    notifyListeners();
  }
}

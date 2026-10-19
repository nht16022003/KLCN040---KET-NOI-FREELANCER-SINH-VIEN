import 'package:flutter/foundation.dart';

import '../models/mo_hinh_vi_tien.dart';
import '../services/dich_vu_vi_tien.dart';

class WalletViewModel extends ChangeNotifier {
  final WalletService _walletService = WalletService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  WalletModel? _vi;
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  WalletModel? get vi => _vi;
  String? get thongBaoLoi => _thongBaoLoi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  WalletModel? get wallet => _vi;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải thông tin số dư ví từ API
  Future<void> taiThongTinVi([int? maUser]) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _walletService.layViTheoUser(maUser ?? 1);
    if (response.isSuccess) {
      _vi = response.data;
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<void> fetchWalletBalance() => taiThongTinVi();
}

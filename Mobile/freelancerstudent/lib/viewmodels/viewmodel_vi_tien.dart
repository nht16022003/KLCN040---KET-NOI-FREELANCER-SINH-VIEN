import 'package:flutter/foundation.dart';

import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_vi_tien.dart';
import '../services/dich_vu_vi_tien.dart';

class WalletViewModel extends ChangeNotifier {
  final WalletService _walletService = WalletService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  WalletModel? _vi;
  List<LichSuGiaoDichModel> _danhSachGiaoDich = [];
  List<YeuCauRutTienModel> _danhSachRutTien = [];
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  WalletModel? get vi => _vi;
  List<LichSuGiaoDichModel> get danhSachGiaoDich => _danhSachGiaoDich;
  List<YeuCauRutTienModel> get danhSachRutTien => _danhSachRutTien;
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

  /// Tải lịch sử biến động số dư ví
  Future<void> taiLichSuGiaoDich(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _walletService.layLichSuGiaoDich(maUser);
    if (response.isSuccess) {
      _danhSachGiaoDich = response.data ?? [];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải danh sách yêu cầu rút tiền của người dùng
  Future<void> taiDanhSachRutTien(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _walletService.layDanhSachRutTien(maUser);
    if (response.isSuccess) {
      _danhSachRutTien = response.data ?? [];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Gửi yêu cầu rút tiền mô phỏng
  Future<ApiResponse<YeuCauRutTienModel>> taoYeuCauRutTien(
    int maUser, {
    required double soTienRut,
    required String tenNganHang,
    required String soTaiKhoan,
    required String tenChuTaiKhoan,
  }) async {
    _dangTai = true;
    notifyListeners();

    final response = await _walletService.taoYeuCauRutTien(
      maUser,
      soTienRut: soTienRut,
      tenNganHang: tenNganHang,
      soTaiKhoan: soTaiKhoan,
      tenChuTaiKhoan: tenChuTaiKhoan,
    );

    if (response.isSuccess) {
      // Cập nhật lại số dư ví và danh sách rút tiền
      await taiThongTinVi(maUser);
      await taiDanhSachRutTien(maUser);
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
    return response;
  }

  /// Xác nhận nạp tiền mô phỏng
  Future<ApiResponse<bool>> xacNhanNapTienMoPhong(
    String maGiaoDich,
    int maUser,
  ) async {
    final res = await _walletService.xacNhanNapTienMoPhong(maGiaoDich, maUser);
    if (res.isSuccess) {
      await taiThongTinVi(maUser);
      await taiLichSuGiaoDich(maUser);
    } else {
      _thongBaoLoi = res.message;
      notifyListeners();
    }
    return res;
  }

  /// Xác nhận duyệt hoặc hủy yêu cầu rút tiền mô phỏng
  Future<ApiResponse<bool>> xacNhanRutTienMoPhong(
    int maRutTien,
    int maUser, {
    required bool chapNhan,
    String? lyDo,
  }) async {
    final res = await _walletService.xacNhanRutTienMoPhong(
      maRutTien,
      maUser,
      chapNhan: chapNhan,
      lyDo: lyDo,
    );
    if (res.isSuccess) {
      await taiThongTinVi(maUser);
      await taiDanhSachRutTien(maUser);
      await taiLichSuGiaoDich(maUser);
    } else {
      _thongBaoLoi = res.message;
      notifyListeners();
    }
    return res;
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<void> fetchWalletBalance() => taiThongTinVi();
}

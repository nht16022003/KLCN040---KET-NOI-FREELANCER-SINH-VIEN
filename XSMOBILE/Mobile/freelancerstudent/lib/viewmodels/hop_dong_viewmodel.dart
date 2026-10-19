import 'package:flutter/foundation.dart';

import '../models/hop_dong_model.dart';
import '../services/hop_dong_service.dart';

class HopDongViewModel extends ChangeNotifier {
  final HopDongService _hopDongService = HopDongService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  List<HopDongModel> _danhSachHopDong = <HopDongModel>[];
  HopDongModel? _hopDongHienTai;
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  List<HopDongModel> get danhSachHopDong => _danhSachHopDong;
  HopDongModel? get hopDongHienTai => _hopDongHienTai;
  String? get thongBaoLoi => _thongBaoLoi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  List<HopDongModel> get contracts => _danhSachHopDong;
  HopDongModel? get currentContract => _hopDongHienTai;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách hợp đồng của Nhà tuyển dụng
  Future<void> taiDanhSachHopDongNTD(int maNhaTuyenDung, {String tuKhoa = '', String trangThai = ''}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _hopDongService.layDanhSachHopDongNTD(
      maNhaTuyenDung,
      tuKhoa: tuKhoa,
      trangThai: trangThai,
    );

    if (response.isSuccess) {
      _danhSachHopDong = response.data ?? <HopDongModel>[];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải danh sách hợp đồng của Sinh viên Freelancer
  Future<void> taiDanhSachHopDongFreelancer(int maFreelancerStudent, {String tuKhoa = '', String trangThai = ''}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _hopDongService.layDanhSachHopDongFreelancer(
      maFreelancerStudent,
      tuKhoa: tuKhoa,
      trangThai: trangThai,
    );

    if (response.isSuccess) {
      _danhSachHopDong = response.data ?? <HopDongModel>[];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải chi tiết 1 hợp đồng
  Future<void> taiChiTietHopDong(String maHD) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _hopDongService.layChiTietHopDong(maHD);
    if (response.isSuccess) {
      _hopDongHienTai = response.data;
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Sinh viên xác nhận / từ chối hợp đồng
  Future<bool> xacNhanHopDong(String maHD, bool dongY, {int? maUserFreelancer, String? lyDoTuChoi}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _hopDongService.xacNhanHopDong(
      maHD: maHD,
      dongY: dongY,
      maUserFreelancer: maUserFreelancer,
      lyDoTuChoi: lyDoTuChoi,
    );

    _dangTai = false;
    if (!response.isSuccess) {
      _thongBaoLoi = response.message;
    }
    notifyListeners();
    return response.isSuccess;
  }

  /// Nhà tuyển dụng ký quỹ hợp đồng
  Future<bool> kyQuyHopDong(String maHD, {int? maUserNhaTuyenDung}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _hopDongService.kyQuyHopDong(
      maHD: maHD,
      maUserNhaTuyenDung: maUserNhaTuyenDung,
    );

    _dangTai = false;
    if (!response.isSuccess) {
      _thongBaoLoi = response.message;
    }
    notifyListeners();
    return response.isSuccess;
  }
}

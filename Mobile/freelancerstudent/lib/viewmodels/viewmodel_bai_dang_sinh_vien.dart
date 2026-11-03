import 'package:flutter/foundation.dart';

import '../models/mo_hinh_bai_dang_sinh_vien.dart';
import '../services/dich_vu_bai_dang_sinh_vien.dart';

class StudentPostViewModel extends ChangeNotifier {
  final StudentPostService _service = StudentPostService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  List<StudentPostModel> _danhSachBaiDang = <StudentPostModel>[];
  List<StudentPostModel> _danhSachBaiDangCuaToi = <StudentPostModel>[];
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  List<StudentPostModel> get danhSachBaiDang => _danhSachBaiDang;
  List<StudentPostModel> get danhSachBaiDangCuaToi => _danhSachBaiDangCuaToi;
  String? get thongBaoLoi => _thongBaoLoi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  List<StudentPostModel> get posts => _danhSachBaiDang;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách tất cả bài đăng tìm việc khả dụng của sinh viên
  Future<void> taiDanhSachBaiDangTimViec() async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _service.layTatCaBaiDangTimViec();
    if (response.isSuccess) {
      _danhSachBaiDang = response.data ?? <StudentPostModel>[];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải danh sách các bài đăng tìm việc của sinh viên hiện tại
  Future<void> taiDanhSachBaiDangCuaToi(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _service.layBaiDangTimViecCuaToi(maUser);
    if (response.isSuccess) {
      _danhSachBaiDangCuaToi = response.data ?? <StudentPostModel>[];
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tạo bài đăng tìm việc mới
  Future<bool> taoBaiDang(Map<String, dynamic> duLieu) async {
    _thongBaoLoi = null;
    final response = await _service.taoBaiDangTimViec(duLieu);
    if (response.isSuccess) {
      final rawUser = duLieu['maUser'];
      final maUser = rawUser is int ? rawUser : int.tryParse(rawUser?.toString() ?? '') ?? 0;
      if (maUser > 0) {
        await taiDanhSachBaiDangCuaToi(maUser);
      }
      await taiDanhSachBaiDangTimViec();
      return true;
    }
    _thongBaoLoi = response.message;
    notifyListeners();
    return false;
  }

  /// Cập nhật bài đăng tìm việc (PUT api/BaiDangTimViec/CapNhat/{maBaiDang})
  Future<bool> capNhatBaiDang(
    String maBaiDang,
    Map<String, dynamic> duLieu,
    int maUser,
  ) async {
    _thongBaoLoi = null;
    final response = await _service.capNhatBaiDangTimViec(maBaiDang, duLieu);
    if (response.isSuccess) {
      if (maUser > 0) {
        await taiDanhSachBaiDangCuaToi(maUser);
      }
      await taiDanhSachBaiDangTimViec();
      return true;
    }
    _thongBaoLoi = response.message;
    notifyListeners();
    return false;
  }

  /// Đổi trạng thái Ẩn / Hiển thị của bài đăng tìm việc
  Future<bool> doiTrangThaiBaiDang(
    String maBaiDang,
    String trangThai,
    int maUser,
  ) async {
    _thongBaoLoi = null;
    final response = await _service.doiTrangThaiBaiDang(maBaiDang, trangThai);
    if (response.isSuccess) {
      if (maUser > 0) {
        await taiDanhSachBaiDangCuaToi(maUser);
      }
      await taiDanhSachBaiDangTimViec();
      return true;
    }
    _thongBaoLoi = response.message;
    notifyListeners();
    return false;
  }

  /// Xóa bài đăng tìm việc
  Future<bool> xoaBaiDang(String maBaiDang, int maUser) async {
    _thongBaoLoi = null;
    final response = await _service.xoaBaiDangTimViec(maBaiDang);
    if (response.isSuccess) {
      if (maUser > 0) {
        await taiDanhSachBaiDangCuaToi(maUser);
      }
      await taiDanhSachBaiDangTimViec();
      return true;
    }
    _thongBaoLoi = response.message;
    notifyListeners();
    return false;
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<void> fetchStudentPosts() => taiDanhSachBaiDangTimViec();
}

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
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  List<StudentPostModel> get danhSachBaiDang => _danhSachBaiDang;
  String? get thongBaoLoi => _thongBaoLoi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  List<StudentPostModel> get posts => _danhSachBaiDang;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách các bài đăng tìm việc của sinh viên
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

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<void> fetchStudentPosts() => taiDanhSachBaiDangTimViec();
}

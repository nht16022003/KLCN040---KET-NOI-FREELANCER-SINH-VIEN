import 'package:flutter/foundation.dart';

import '../models/mo_hinh_ung_tuyen.dart';
import '../services/dich_vu_ung_tuyen.dart';

class ApplicationViewModel extends ChangeNotifier {
  final ApplicationService _service = ApplicationService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  List<ApplicationModel> _danhSachUngTuyen = <ApplicationModel>[];
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  List<ApplicationModel> get danhSachUngTuyen => _danhSachUngTuyen;
  String? get thongBaoLoi => _thongBaoLoi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  List<ApplicationModel> get applications => _danhSachUngTuyen;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách đơn ứng tuyển
  Future<void> taiDanhSachUngTuyen({int? maUser}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _service.layDanhSachUngTuyenTheoUser(maUser ?? 0);
    if (response.isSuccess) {
      _danhSachUngTuyen = response.data ?? <ApplicationModel>[];
      if (_danhSachUngTuyen.isEmpty &&
          response.message.isNotEmpty &&
          !response.message.contains('Chưa có đơn ứng tuyển nào')) {
        _thongBaoLoi = response.message;
      }
    } else {
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Thêm đơn ứng tuyển vào danh sách hiển thị
  void themUngTuyenCucBo(ApplicationModel ungTuyen) {
    final tonTai = _danhSachUngTuyen.any((item) => item.maUngTuyen == ungTuyen.maUngTuyen);
    if (!tonTai) {
      _danhSachUngTuyen = [ungTuyen, ..._danhSachUngTuyen];
      _thongBaoLoi = null;
      notifyListeners();
    }
  }

  // ==========================================
  // TƯƠNG THÍCH NGƯỢC (BACKWARD COMPATIBILITY)
  // ==========================================
  Future<void> fetchMyApplications({int? userId}) =>
      taiDanhSachUngTuyen(maUser: userId);

  void addLocalApplication(ApplicationModel application) =>
      themUngTuyenCucBo(application);
}

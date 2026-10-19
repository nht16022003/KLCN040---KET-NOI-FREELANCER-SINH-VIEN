import 'package:flutter/foundation.dart';

import '../models/mo_hinh_task_cong_viec.dart';
import '../services/dich_vu_task_cong_viec.dart';

class TaskViewModel extends ChangeNotifier {
  final TaskCongViecService _service = TaskCongViecService();

  bool _dangTai = false;
  bool _dangGhi = false;
  TienDoDuAnModel? _tienDo;
  String? _thongBaoLoi;

  bool get dangTai => _dangTai;
  bool get dangGhi => _dangGhi;
  TienDoDuAnModel? get tienDo => _tienDo;
  String? get thongBaoLoi => _thongBaoLoi;

  Future<void> taiTienDo(String maHD, int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    if (_tienDo?.maHD != maHD) _tienDo = null;
    notifyListeners();

    final res = await _service.layTienDoDuAn(maHD, maUser);
    if (res.isSuccess) {
      _tienDo = res.data;
    } else {
      _thongBaoLoi = res.message;
    }
    _dangTai = false;
    notifyListeners();
  }

  /// Thực hiện thao tác ghi; trả về thông báo lỗi (null nếu thành công) rồi tải lại tiến độ.
  Future<String?> _ghi(
    Future<dynamic> Function() thaoTac,
    String maHD,
    int maUser,
  ) async {
    _dangGhi = true;
    notifyListeners();
    final res = await thaoTac();
    _dangGhi = false;
    if (res.isSuccess == true) {
      await taiTienDo(maHD, maUser);
      return null;
    }
    notifyListeners();
    return res.message as String? ?? 'Thao tác thất bại.';
  }

  Future<String?> themTask({
    required String maHD,
    required int maUser,
    required String tenTask,
    required String moTa,
    int? maGiaiDoan,
    String? linkSanPham,
    DateTime? hanHoanThanh,
  }) => _ghi(
        () => _service.themTask(
          maHD: maHD,
          maUser: maUser,
          tenTask: tenTask,
          moTa: moTa,
          maGiaiDoan: maGiaiDoan,
          linkSanPham: linkSanPham,
          hanHoanThanh: hanHoanThanh,
        ),
        maHD,
        maUser,
      );

  Future<String?> suaTask({
    required TaskCongViecModel task,
    required int maUser,
    required String tenTask,
    required String moTa,
    int? maGiaiDoan,
    String? linkSanPham,
    DateTime? hanHoanThanh,
  }) => _ghi(
        () => _service.capNhatTask(
          task: task,
          maUser: maUser,
          tenTask: tenTask,
          moTa: moTa,
          maGiaiDoan: maGiaiDoan,
          linkSanPham: linkSanPham,
          hanHoanThanh: hanHoanThanh,
        ),
        task.maHD,
        maUser,
      );

  Future<String?> doiTrangThai({
    required TaskCongViecModel task,
    required int maUser,
    required String trangThai,
  }) => _ghi(
        () => _service.capNhatTrangThai(
          maTask: task.maTask,
          maUser: maUser,
          trangThai: trangThai,
        ),
        task.maHD,
        maUser,
      );

  Future<String?> xoaTask({
    required TaskCongViecModel task,
    required int maUser,
  }) => _ghi(
        () => _service.xoaTask(maTask: task.maTask, maUser: maUser),
        task.maHD,
        maUser,
      );

  Future<String?> danhGiaGiaiDoan({
    required String maHD,
    required int maUser,
    int? maGiaiDoan,
    required String trangThai,
  }) => _ghi(
        () => _service.danhGiaGiaiDoan(
          maHD: maHD,
          maUser: maUser,
          maGiaiDoan: maGiaiDoan,
          trangThai: trangThai,
        ),
        maHD,
        maUser,
      );
}

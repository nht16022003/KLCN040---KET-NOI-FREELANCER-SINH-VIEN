import 'package:flutter/foundation.dart';

import '../models/mo_hinh_nha_tuyen_dung.dart';
import '../services/dich_vu_nha_tuyen_dung.dart';

class NhaTuyenDungViewModel extends ChangeNotifier {
  final NhaTuyenDungService _service = NhaTuyenDungService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  List<NhaTuyenDungModel> _danhSachNhaTuyenDung = <NhaTuyenDungModel>[];
  List<NhaTuyenDungModel> _danhSachLoc = <NhaTuyenDungModel>[];
  NhaTuyenDungProfileModel? _profileChiTiet;
  String? _thongBaoLoi;
  String _tuKhoa = '';
  String _linhVucDaChon = '';

  bool get dangTai => _dangTai;
  List<NhaTuyenDungModel> get danhSachNhaTuyenDung => _danhSachLoc;
  NhaTuyenDungProfileModel? get profileChiTiet => _profileChiTiet;
  int get tongSoNhaTuyenDung => _danhSachNhaTuyenDung.length;
  String? get thongBaoLoi => _thongBaoLoi;
  String get tuKhoa => _tuKhoa;
  String get linhVucDaChon => _linhVucDaChon;

  // Lấy danh sách các lĩnh vực hoạt động độc nhất để làm bộ lọc
  List<String> get danhSachLinhVuc {
    final setLinhVuc = <String>{};
    for (final item in _danhSachNhaTuyenDung) {
      if (item.linhvuc != null && item.linhvuc!.trim().isNotEmpty) {
        setLinhVuc.add(item.linhvuc!.trim());
      }
    }
    final list = setLinhVuc.toList()..sort();
    return ['Tất cả', ...list];
  }

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách nhà tuyển dụng từ API
  Future<void> taiDanhSachNhaTuyenDung() async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _service.layDanhSachNhaTuyenDung();

    if (response.isSuccess) {
      _danhSachNhaTuyenDung = response.data ?? <NhaTuyenDungModel>[];
      _apDungBoLoc();
      if (_danhSachNhaTuyenDung.isEmpty) {
        _thongBaoLoi = 'Hiện tại chưa có nhà tuyển dụng nào trong hệ thống.';
      }
    } else {
      _danhSachNhaTuyenDung = <NhaTuyenDungModel>[];
      _danhSachLoc = <NhaTuyenDungModel>[];
      _thongBaoLoi = response.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải hồ sơ chi tiết Nhà tuyển dụng theo maNhaTuyenDung
  Future<NhaTuyenDungProfileModel?> taiProfileNhaTuyenDung(int maNhaTuyenDung) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layProfileNhaTuyenDung(maNhaTuyenDung);
    if (res.isSuccess && res.data != null) {
      _profileChiTiet = res.data;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
    return _profileChiTiet;
  }

  /// Tải hồ sơ chi tiết Nhà tuyển dụng theo maUser
  Future<NhaTuyenDungProfileModel?> taiProfileNhaTuyenDungTheoUser(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layProfileNhaTuyenDungTheoUser(maUser);
    if (res.isSuccess && res.data != null) {
      _profileChiTiet = res.data;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
    return _profileChiTiet;
  }

  /// Tìm kiếm theo từ khóa (tên công ty, người liên hệ, địa chỉ)
  void timKiem(String tuKhoa) {
    _tuKhoa = tuKhoa.trim();
    _apDungBoLoc();
    notifyListeners();
  }

  /// Lọc theo lĩnh vực hoạt động
  void locTheoLinhVuc(String linhVuc) {
    if (linhVuc == 'Tất cả') {
      _linhVucDaChon = '';
    } else {
      _linhVucDaChon = linhVuc;
    }
    _apDungBoLoc();
    notifyListeners();
  }

  /// Áp dụng bộ lọc kết hợp từ khóa và lĩnh vực (tương tự Search.cshtml bên Web)
  void _apDungBoLoc() {
    final kw = _tuKhoa.toLowerCase();
    _danhSachLoc = _danhSachNhaTuyenDung.where((item) {
      final tenCongTy = (item.tencongty ?? '').toLowerCase();
      final nguoiDaiDien = (item.hotenUser ?? '').toLowerCase();
      final diaChi = (item.diachi ?? '').toLowerCase();
      final linhVuc = item.linhvuc ?? '';

      final khopTuKhoa = kw.isEmpty ||
          tenCongTy.contains(kw) ||
          nguoiDaiDien.contains(kw) ||
          diaChi.contains(kw);

      final khopLinhVuc = _linhVucDaChon.isEmpty || linhVuc == _linhVucDaChon;

      return khopTuKhoa && khopLinhVuc;
    }).toList();
  }
}

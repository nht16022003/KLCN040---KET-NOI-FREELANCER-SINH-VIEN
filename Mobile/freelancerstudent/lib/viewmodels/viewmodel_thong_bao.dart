import 'package:flutter/foundation.dart';
import '../models/mo_hinh_thong_bao.dart';
import '../services/dich_vu_thong_bao.dart';

class ThongBaoViewModel extends ChangeNotifier {
  final ThongBaoService _service = ThongBaoService();

  bool _dangTai = false;
  String? _thongBaoLoi;
  List<ThongBaoModel> _danhSachThongBao = [];
  int _soLuongChuaDoc = 0;
  int _trangHienTai = 1;
  int _tongSoThongBao = 0;

  bool get dangTai => _dangTai;
  String? get thongBaoLoi => _thongBaoLoi;
  List<ThongBaoModel> get danhSachThongBao => _danhSachThongBao;
  int get soLuongChuaDoc => _soLuongChuaDoc;
  int get trangHienTai => _trangHienTai;
  int get tongSoThongBao => _tongSoThongBao;

  /// Đếm thông báo chưa đọc (gọi định kỳ hoặc khi mở app)
  Future<void> demChuaDoc(int maUser) async {
    final res = await _service.demChuaDoc(maUser);
    if (res.isSuccess) {
      _soLuongChuaDoc = res.data ?? 0;
      notifyListeners();
    }
  }

  /// Tải danh sách thông báo
  Future<void> taiDanhSachThongBao(int maUser, {int trang = 1}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layDanhSachThongBao(maUser: maUser, trang: trang);
    if (res.isSuccess && res.data != null) {
      _trangHienTai = res.data!.trang;
      _tongSoThongBao = res.data!.tongSoThongBao;
      _soLuongChuaDoc = res.data!.soThongBaoChuaDoc;
      if (trang == 1) {
        _danhSachThongBao = res.data!.danhSach;
      } else {
        _danhSachThongBao.addAll(res.data!.danhSach);
      }
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Đánh dấu 1 thông báo là đã đọc
  Future<void> danhDauDaDoc(int maThongBao, int maUser) async {
    final res = await _service.danhDauDaDoc(maThongBao: maThongBao, maUser: maUser);
    if (res.isSuccess) {
      final index = _danhSachThongBao.indexWhere((t) => t.maThongBao == maThongBao);
      if (index != -1 && !_danhSachThongBao[index].daDoc) {
        final item = _danhSachThongBao[index];
        _danhSachThongBao[index] = ThongBaoModel(
          maThongBao: item.maThongBao,
          tieuDe: item.tieuDe,
          noiDung: item.noiDung,
          loaiThongBao: item.loaiThongBao,
          linkDieuHuong: item.linkDieuHuong,
          daDoc: true,
          ngayTao: item.ngayTao,
        );
        if (_soLuongChuaDoc > 0) _soLuongChuaDoc--;
        notifyListeners();
      }
    }
  }

  /// Đánh dấu tất cả thông báo là đã đọc
  Future<void> danhDauTatCaDaDoc(int maUser) async {
    final res = await _service.danhDauTatCaDaDoc(maUser);
    if (res.isSuccess) {
      _danhSachThongBao = _danhSachThongBao.map((item) {
        return ThongBaoModel(
          maThongBao: item.maThongBao,
          tieuDe: item.tieuDe,
          noiDung: item.noiDung,
          loaiThongBao: item.loaiThongBao,
          linkDieuHuong: item.linkDieuHuong,
          daDoc: true,
          ngayTao: item.ngayTao,
        );
      }).toList();
      _soLuongChuaDoc = 0;
      notifyListeners();
    }
  }
}

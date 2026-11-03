import 'package:flutter/foundation.dart';
import '../models/mo_hinh_danh_gia.dart';
import '../services/dich_vu_danh_gia.dart';

class DanhGiaViewModel extends ChangeNotifier {
  final DanhGiaService _service = DanhGiaService();

  bool _dangTai = false;
  String? _thongBaoLoi;
  List<HopDongDanhGiaModel> _danhSachCongViec = [];
  FormDanhGiaModel? _formHienTai;
  ThongKeDanhGiaModel? _thongKeDoiTac;

  bool get dangTai => _dangTai;
  String? get thongBaoLoi => _thongBaoLoi;
  List<HopDongDanhGiaModel> get danhSachCongViec => _danhSachCongViec;
  FormDanhGiaModel? get formHienTai => _formHienTai;
  ThongKeDanhGiaModel? get thongKeDoiTac => _thongKeDoiTac;

  /// Tải danh sách hợp đồng chờ/đã đánh giá
  Future<void> taiDanhSachCongViecDaThue(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layCongViecDaThue(maUser);
    if (res.isSuccess) {
      _danhSachCongViec = res.data ?? [];
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải chi tiết form đánh giá cho 1 hợp đồng
  Future<void> taiFormDanhGia(String maHD, int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layFormDanhGia(maHD: maHD, maUser: maUser);
    if (res.isSuccess) {
      _formHienTai = res.data;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Gửi đánh giá
  Future<bool> guiDanhGia({
    required int maUser,
    required String maHD,
    required int soSaoTongThe,
    int? soSaoChatLuong,
    int? soSaoDungHan,
    int? soSaoGiaoTiep,
    String? nhanXet,
  }) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.guiDanhGia(
      maUser: maUser,
      maHD: maHD,
      soSaoTongThe: soSaoTongThe,
      soSaoChatLuong: soSaoChatLuong,
      soSaoDungHan: soSaoDungHan,
      soSaoGiaoTiep: soSaoGiaoTiep,
      nhanXet: nhanXet,
    );

    _dangTai = false;
    if (!res.isSuccess) {
      _thongBaoLoi = res.message;
    }
    notifyListeners();
    return res.isSuccess;
  }

  /// Cập nhật đánh giá
  Future<bool> capNhatDanhGia({
    required int maDanhGia,
    required int maUser,
    required String maHD,
    required int soSaoTongThe,
    int? soSaoChatLuong,
    int? soSaoDungHan,
    int? soSaoGiaoTiep,
    String? nhanXet,
  }) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.capNhatDanhGia(
      maDanhGia: maDanhGia,
      maUser: maUser,
      maHD: maHD,
      soSaoTongThe: soSaoTongThe,
      soSaoChatLuong: soSaoChatLuong,
      soSaoDungHan: soSaoDungHan,
      soSaoGiaoTiep: soSaoGiaoTiep,
      nhanXet: nhanXet,
    );

    _dangTai = false;
    if (!res.isSuccess) {
      _thongBaoLoi = res.message;
    }
    notifyListeners();
    return res.isSuccess;
  }

  /// Tải thông kê đánh giá Freelancer
  Future<void> taiDanhGiaFreelancer(int maFS, {int trang = 1}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layDanhGiaFreelancer(maFreelancerStudent: maFS, trang: trang);
    if (res.isSuccess) {
      _thongKeDoiTac = res.data;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  /// Tải thống kê đánh giá Nhà tuyển dụng
  Future<void> taiDanhGiaNhaTuyenDung(int maNTD, {int trang = 1}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layDanhGiaNhaTuyenDung(maNhaTuyenDung: maNTD, trang: trang);
    if (res.isSuccess) {
      _thongKeDoiTac = res.data;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }
}

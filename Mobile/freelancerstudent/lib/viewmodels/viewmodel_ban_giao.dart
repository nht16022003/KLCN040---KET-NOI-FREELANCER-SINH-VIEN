import 'package:flutter/foundation.dart';
import '../models/mo_hinh_ban_giao.dart';
import '../services/dich_vu_ban_giao.dart';

class BanGiaoViewModel extends ChangeNotifier {
  final BanGiaoService _service = BanGiaoService();

  List<BanGiaoModel> _danhSachBanGiao = [];
  List<YeuCauChinhSuaModel> _danhSachYeuCau = [];
  bool _dangTai = false;
  bool _dangXuLy = false;
  String? _thongBaoLoi;

  List<BanGiaoModel> get danhSachBanGiao => _danhSachBanGiao;
  List<YeuCauChinhSuaModel> get danhSachYeuCau => _danhSachYeuCau;
  bool get dangTai => _dangTai;
  bool get dangXuLy => _dangXuLy;
  String? get thongBaoLoi => _thongBaoLoi;

  /// Phiên bản bàn giao mới nhất
  BanGiaoModel? get moiNhat =>
      _danhSachBanGiao.isNotEmpty ? _danhSachBanGiao.first : null;

  /// Điều kiện cho phép Freelancer nộp bài bàn giao:
  /// - Là Freelancer
  /// - Hợp đồng ở trạng thái DangThucHien
  /// - Chưa nộp bản nào HOẶC bản mới nhất có trạng thái YeuCauChinhSua
  bool coTheNop(bool laFreelancer, String trangThaiHopDong) {
    if (!laFreelancer) return false;
    if (trangThaiHopDong != 'DangThucHien') return false;
    return moiNhat == null || moiNhat!.trangThai == 'YeuCauChinhSua';
  }

  /// Điều kiện cho phép Khách hàng (NTD) đánh giá / nghiệm thu:
  /// - Là Nhà tuyển dụng
  /// - Hợp đồng ở trạng thái DangThucHien
  /// - Bản mới nhất có trạng thái ChoNghiemThu
  bool coTheDanhGia(bool laKhachHang, String trangThaiHopDong) {
    if (!laKhachHang) return false;
    if (trangThaiHopDong != 'DangThucHien') return false;
    return moiNhat?.trangThai == 'ChoNghiemThu';
  }

  /// Tải dữ liệu các phiên bản bàn giao và yêu cầu chỉnh sửa
  Future<void> taiDuLieu(String maHD) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final resBanGiao = await _service.layDanhSachBanGiao(maHD);
      final resYeuCau = await _service.layDanhSachYeuCau(maHD);

      if (resBanGiao.isSuccess) {
        _danhSachBanGiao = (resBanGiao.data ?? [])
          ..sort((a, b) => b.maBanGiao.compareTo(a.maBanGiao));
      } else {
        _thongBaoLoi = resBanGiao.message;
      }

      if (resYeuCau.isSuccess) {
        _danhSachYeuCau = resYeuCau.data ?? [];
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối: ${e.toString()}';
    } finally {
      _dangTai = false;
      notifyListeners();
    }
  }

  /// Freelancer gửi bàn giao sản phẩm
  Future<bool> taoBanGiao({
    required String maHD,
    required String fileSanPhamUrl,
    String? linkDemo,
    required String moTaBanGiao,
  }) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final res = await _service.taoBanGiao(
        maHD: maHD,
        fileSanPhamUrl: fileSanPhamUrl,
        linkDemo: linkDemo,
        moTaBanGiao: moTaBanGiao,
      );

      if (res.isSuccess) {
        await taiDuLieu(maHD);
        return true;
      } else {
        _thongBaoLoi = res.message;
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối: ${e.toString()}';
      return false;
    } finally {
      _dangXuLy = false;
      notifyListeners();
    }
  }

  /// Khách hàng gửi yêu cầu chỉnh sửa
  Future<bool> guiYeuCauChinhSua({
    required String maHD,
    required int maBanGiao,
    required String noiDungYeuCau,
  }) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final res = await _service.guiYeuCauChinhSua(
        maBanGiao: maBanGiao,
        noiDungYeuCau: noiDungYeuCau,
      );

      if (res.isSuccess) {
        await taiDuLieu(maHD);
        return true;
      } else {
        _thongBaoLoi = res.message;
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối: ${e.toString()}';
      return false;
    } finally {
      _dangXuLy = false;
      notifyListeners();
    }
  }

  /// Khách hàng nghiệm thu sản phẩm
  Future<bool> nghiemThu({
    required String maHD,
    required int maBanGiao,
  }) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final res = await _service.nghiemThu(maBanGiao);

      if (res.isSuccess) {
        await taiDuLieu(maHD);
        return true;
      } else {
        _thongBaoLoi = res.message;
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối: ${e.toString()}';
      return false;
    } finally {
      _dangXuLy = false;
      notifyListeners();
    }
  }
}

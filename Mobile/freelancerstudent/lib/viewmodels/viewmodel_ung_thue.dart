import 'package:flutter/foundation.dart';
import '../models/mo_hinh_ung_thue.dart';
import '../services/dich_vu_ung_thue.dart';

class UngThueViewModel extends ChangeNotifier {
  final UngThueService _service = UngThueService();

  List<UngThueModel> _danhSachFreelancer = [];
  List<UngThueModel> _danhSachNTD = [];
  bool _dangTai = false;
  bool _dangXuLy = false;
  String? _thongBaoLoi;
  String? _thongBaoThanhCong;

  List<UngThueModel> get danhSachFreelancer => _danhSachFreelancer;
  List<UngThueModel> get danhSachNTD => _danhSachNTD;
  bool get dangTai => _dangTai;
  bool get dangXuLy => _dangXuLy;
  String? get thongBaoLoi => _thongBaoLoi;
  String? get thongBaoThanhCong => _thongBaoThanhCong;

  void xoaThongBao() {
    _thongBaoLoi = null;
    _thongBaoThanhCong = null;
    notifyListeners();
  }

  /// Tải danh sách yêu cầu thuê gửi đến Freelancer
  Future<void> taiDanhSachTheoFreelancer(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final res = await _service.layDanhSachTheoFreelancer(maUser);
      if (res.isSuccess && res.data != null) {
        _danhSachFreelancer = res.data!;
      } else {
        _thongBaoLoi = res.message;
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối khi tải yêu cầu thuê.';
    } finally {
      _dangTai = false;
      notifyListeners();
    }
  }

  /// Tải danh sách yêu cầu thuê đã gửi của NTD
  Future<void> taiDanhSachTheoNTD(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    try {
      final res = await _service.layDanhSachTheoNTD(maUser);
      if (res.isSuccess && res.data != null) {
        _danhSachNTD = res.data!;
      } else {
        _thongBaoLoi = res.message;
      }
    } catch (e) {
      _thongBaoLoi = 'Lỗi kết nối khi tải danh sách yêu cầu thuê.';
    } finally {
      _dangTai = false;
      notifyListeners();
    }
  }

  /// Gửi yêu cầu thuê Freelancer
  Future<bool> guiYeuCauThue(GuiYeuCauThueRequest req) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    _thongBaoThanhCong = null;
    notifyListeners();

    try {
      final res = await _service.guiYeuCauThue(req);
      if (res.isSuccess) {
        _thongBaoThanhCong = res.message;
        await taiDanhSachTheoNTD(req.maUserNTD);
        _dangXuLy = false;
        notifyListeners();
        return true;
      } else {
        _thongBaoLoi = res.message;
        _dangXuLy = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Không thể gửi yêu cầu thuê.';
      _dangXuLy = false;
      notifyListeners();
      return false;
    }
  }

  /// Freelancer chấp nhận yêu cầu thuê
  Future<bool> chapNhanYeuCau(int maYeuCau, int maUserFreelancer) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    _thongBaoThanhCong = null;
    notifyListeners();

    try {
      final res = await _service.phanHoiYeuCauThue(PhanHoiYeuCauThueRequest(
        maYeuCau: maYeuCau,
        maUserFreelancer: maUserFreelancer,
        chapNhan: true,
      ));
      if (res.isSuccess) {
        _thongBaoThanhCong = res.message;
        await taiDanhSachTheoFreelancer(maUserFreelancer);
        _dangXuLy = false;
        notifyListeners();
        return true;
      } else {
        _thongBaoLoi = res.message;
        _dangXuLy = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Không thể phản hồi yêu cầu thuê.';
      _dangXuLy = false;
      notifyListeners();
      return false;
    }
  }

  /// Freelancer từ chối yêu cầu thuê
  Future<bool> tuChoiYeuCau(
      int maYeuCau, int maUserFreelancer, String lyDo) async {
    _dangXuLy = true;
    _thongBaoLoi = null;
    _thongBaoThanhCong = null;
    notifyListeners();

    try {
      final res = await _service.phanHoiYeuCauThue(PhanHoiYeuCauThueRequest(
        maYeuCau: maYeuCau,
        maUserFreelancer: maUserFreelancer,
        chapNhan: false,
        lyDoTuChoi: lyDo,
      ));
      if (res.isSuccess) {
        _thongBaoThanhCong = res.message;
        await taiDanhSachTheoFreelancer(maUserFreelancer);
        _dangXuLy = false;
        notifyListeners();
        return true;
      } else {
        _thongBaoLoi = res.message;
        _dangXuLy = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _thongBaoLoi = 'Không thể từ chối yêu cầu thuê.';
      _dangXuLy = false;
      notifyListeners();
      return false;
    }
  }
}


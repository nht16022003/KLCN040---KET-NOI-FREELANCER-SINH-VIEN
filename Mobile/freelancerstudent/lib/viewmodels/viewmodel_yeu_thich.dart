import 'package:flutter/foundation.dart';
import '../models/mo_hinh_luu_viec.dart';
import '../models/mo_hinh_freelancer_yeu_thich.dart';
import '../services/dich_vu_yeu_thich.dart';

class YeuThichViewModel extends ChangeNotifier {
  final YeuThichService _service = YeuThichService();

  bool _dangTai = false;
  String? _thongBaoLoi;

  List<CongViecDaLuuModel> _danhSachViecDaLuu = [];
  List<FreelancerYeuThichModel> _danhSachFreelancerYeuThich = [];

  final Set<String> _setMaJobDaLuu = {};
  final Set<int> _setMaFreelancerDaLuu = {};

  bool get dangTai => _dangTai;
  String? get thongBaoLoi => _thongBaoLoi;
  List<CongViecDaLuuModel> get danhSachViecDaLuu => _danhSachViecDaLuu;
  List<FreelancerYeuThichModel> get danhSachFreelancerYeuThich =>
      _danhSachFreelancerYeuThich;

  bool daLuuViec(String maJob) => _setMaJobDaLuu.contains(maJob);
  bool daLuuFreelancer(int maFreelancer) =>
      _setMaFreelancerDaLuu.contains(maFreelancer);

  // ==========================================
  // XỬ LÝ VIỆC ĐÃ LƯU
  // ==========================================

  Future<void> taiDanhSachViecDaLuu(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layDanhSachViecDaLuu(maUser);
    if (res.isSuccess) {
      _danhSachViecDaLuu = res.data ?? [];
      _setMaJobDaLuu.clear();
      for (var item in _danhSachViecDaLuu) {
        _setMaJobDaLuu.add(item.maJob);
      }
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  Future<bool> kiemTraLuuViec(int maUser, String maJob) async {
    final res = await _service.kiemTraLuuViec(maUser: maUser, maJob: maJob);
    if (res.isSuccess && res.data == true) {
      _setMaJobDaLuu.add(maJob);
      notifyListeners();
      return true;
    } else {
      _setMaJobDaLuu.remove(maJob);
      notifyListeners();
      return false;
    }
  }

  Future<bool> toggleLuuViec(int maUser, String maJob, {String? ghiChu}) async {
    if (_setMaJobDaLuu.contains(maJob)) {
      final res = await _service.boLuuViec(maUser: maUser, maJob: maJob);
      if (res.isSuccess) {
        _setMaJobDaLuu.remove(maJob);
        _danhSachViecDaLuu.removeWhere((item) => item.maJob == maJob);
        notifyListeners();
        return false;
      }
    } else {
      final res =
          await _service.luuViec(maUser: maUser, maJob: maJob, ghiChu: ghiChu);
      if (res.isSuccess) {
        _setMaJobDaLuu.add(maJob);
        await taiDanhSachViecDaLuu(maUser);
        return true;
      }
    }
    return _setMaJobDaLuu.contains(maJob);
  }

  // ==========================================
  // XỬ LÝ FREELANCER YÊU THÍCH
  // ==========================================

  Future<void> taiDanhSachFreelancerYeuThich(int maUser) async {
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _service.layDanhSachFreelancerYeuThich(maUser);
    if (res.isSuccess) {
      _danhSachFreelancerYeuThich = res.data ?? [];
      _setMaFreelancerDaLuu.clear();
      for (var item in _danhSachFreelancerYeuThich) {
        _setMaFreelancerDaLuu.add(item.maFreelancerStudent);
      }
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTai = false;
    notifyListeners();
  }

  Future<bool> kiemTraLuuFreelancer(int maUser, int maFreelancer) async {
    final res = await _service.kiemTraLuuFreelancer(
      maUser: maUser,
      maFreelancerStudent: maFreelancer,
    );
    if (res.isSuccess && res.data == true) {
      _setMaFreelancerDaLuu.add(maFreelancer);
      notifyListeners();
      return true;
    } else {
      _setMaFreelancerDaLuu.remove(maFreelancer);
      notifyListeners();
      return false;
    }
  }

  Future<bool> toggleLuuFreelancer(int maUser, int maFreelancer,
      {String? ghiChu}) async {
    if (_setMaFreelancerDaLuu.contains(maFreelancer)) {
      final res = await _service.boLuuFreelancer(
          maUser: maUser, maFreelancerStudent: maFreelancer);
      if (res.isSuccess) {
        _setMaFreelancerDaLuu.remove(maFreelancer);
        _danhSachFreelancerYeuThich
            .removeWhere((item) => item.maFreelancerStudent == maFreelancer);
        notifyListeners();
        return false;
      }
    } else {
      final res = await _service.luuFreelancer(
        maUser: maUser,
        maFreelancerStudent: maFreelancer,
        ghiChu: ghiChu,
      );
      if (res.isSuccess) {
        _setMaFreelancerDaLuu.add(maFreelancer);
        await taiDanhSachFreelancerYeuThich(maUser);
        return true;
      }
    }
    return _setMaFreelancerDaLuu.contains(maFreelancer);
  }
}


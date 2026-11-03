import 'package:flutter/foundation.dart';

import '../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../services/dich_vu_cong_viec.dart';

class JobViewModel extends ChangeNotifier {
  final JobService _jobService = JobService();

  // ==========================================
  // BIẾN TRẠNG THÁI TIẾNG VIỆT
  // ==========================================
  bool _dangTai = false;
  List<JobPostModel> _danhSachJobPost = <JobPostModel>[];
  String? _thongBaoLoi;
  List<JobPostModel> _tatCaJobPost = <JobPostModel>[];
  bool _daTaiDanhSach = false;
  int _requestId = 0;

  // Bộ lọc nâng cao đồng bộ Web Views/JobPost/Index.cshtml
  String _tuKhoaHienTai = '';
  String _mucThuLaoLoc = 'tat-ca'; // tat-ca, duoi-1tr, 1tr-3tr, 3tr-10tr, tren-10tr
  String _trangThaiLoc = 'tat-ca'; // tat-ca, DangTuyen, DaHoanThanh
  String _thuTuSapXep = 'moi-nhat'; // moi-nhat, luong-cao, luong-thap

  bool get dangTai => _dangTai;
  List<JobPostModel> get danhSachJobPost => _danhSachJobPost;
  String? get thongBaoLoi => _thongBaoLoi;
  String get mucThuLaoLoc => _mucThuLaoLoc;
  String get trangThaiLoc => _trangThaiLoc;
  String get thuTuSapXep => _thuTuSapXep;

  bool get dangCoBoLocNangCao =>
      _mucThuLaoLoc != 'tat-ca' ||
      _trangThaiLoc != 'tat-ca' ||
      _thuTuSapXep != 'moi-nhat';

  // Trạng thái gợi ý việc làm
  bool _dangTaiGoiY = false;
  List<JobPostModel> _danhSachJobGoiY = <JobPostModel>[];
  String? _thongBaoLoiGoiY;

  bool get dangTaiGoiY => _dangTaiGoiY;
  List<JobPostModel> get danhSachJobGoiY => _danhSachJobGoiY;
  String? get thongBaoLoiGoiY => _thongBaoLoiGoiY;

  // Trạng thái việc làm của Nhà tuyển dụng (Job của tôi)
  bool _dangTaiJobCuaToi = false;
  List<JobPostModel> _danhSachJobCuaToi = <JobPostModel>[];
  String? _thongBaoLoiJobCuaToi;

  bool get dangTaiJobCuaToi => _dangTaiJobCuaToi;
  List<JobPostModel> get danhSachJobCuaToi => _danhSachJobCuaToi;
  String? get thongBaoLoiJobCuaToi => _thongBaoLoiJobCuaToi;

  // Tương thích ngược
  bool get isLoading => _dangTai;
  List<JobPostModel> get jobs => _danhSachJobPost;
  String? get errorMessage => _thongBaoLoi;

  // ==========================================
  // HÀM XỬ LÝ TIẾNG VIỆT
  // ==========================================

  /// Tải danh sách việc làm từ API
  Future<void> taiDanhSachJobPost({
    int trang = 1,
    String tuKhoa = '',
    bool lamMoi = false,
  }) async {
    _tuKhoaHienTai = tuKhoa.trim().toLowerCase();
    if (_daTaiDanhSach && _tuKhoaHienTai.isNotEmpty && !lamMoi) {
      _requestId++;
      _dangTai = false;
      _thongBaoLoi = null;
      _danhSachJobPost = _locCongViec(_tatCaJobPost, _tuKhoaHienTai);
      if (_danhSachJobPost.isEmpty) {
        _thongBaoLoi = 'Chưa có công việc nào phù hợp.';
      }
      notifyListeners();
      return;
    }

    final requestId = ++_requestId;
    _dangTai = true;
    _thongBaoLoi = null;
    notifyListeners();

    final response = await _jobService.layDanhSachJobPost(
      page: trang,
      tuKhoa: tuKhoa,
    );
    if (requestId != _requestId) return;

    if (response.isSuccess) {
      _tatCaJobPost = response.data ?? <JobPostModel>[];
      _daTaiDanhSach = true;
      _danhSachJobPost = _locCongViec(_tatCaJobPost, _tuKhoaHienTai);
      if (_danhSachJobPost.isEmpty) {
        _thongBaoLoi = 'Chưa có công việc nào phù hợp.';
      }
    } else {
      _danhSachJobPost = <JobPostModel>[];
      _thongBaoLoi = response.message.isNotEmpty
          ? response.message
          : 'Không thể tải danh sách việc làm.';
    }

    _dangTai = false;
    notifyListeners();
  }

  void datBoLocThuLao(String mucThuLao) {
    _mucThuLaoLoc = mucThuLao;
    _apDungLaiBoLoc();
  }

  void datBoLocTrangThai(String trangThai) {
    _trangThaiLoc = trangThai;
    _apDungLaiBoLoc();
  }

  void datThuTuSapXep(String thuTu) {
    _thuTuSapXep = thuTu;
    _apDungLaiBoLoc();
  }

  void xoaBoLocNangCao() {
    _mucThuLaoLoc = 'tat-ca';
    _trangThaiLoc = 'tat-ca';
    _thuTuSapXep = 'moi-nhat';
    _apDungLaiBoLoc();
  }

  void _apDungLaiBoLoc() {
    _thongBaoLoi = null;
    _danhSachJobPost = _locCongViec(_tatCaJobPost, _tuKhoaHienTai);
    if (_danhSachJobPost.isEmpty) {
      _thongBaoLoi = 'Chưa có công việc nào phù hợp với bộ lọc.';
    }
    notifyListeners();
  }

  // Tương thích ngược
  Future<void> fetchJobs({int page = 1, String keyword = ''}) =>
      taiDanhSachJobPost(trang: page, tuKhoa: keyword);

  Future<void> refreshJobs({String keyword = ''}) =>
      taiDanhSachJobPost(tuKhoa: keyword, lamMoi: true);

  /// Tải danh sách việc làm gợi ý cho Freelancer từ API
  Future<void> taiDanhSachJobGoiY(int maUser) async {
    _dangTaiGoiY = true;
    _thongBaoLoiGoiY = null;
    notifyListeners();

    final response = await _jobService.layDanhSachJobGoiY(maUser);

    if (response.isSuccess) {
      _danhSachJobGoiY = response.data ?? <JobPostModel>[];
    } else {
      _danhSachJobGoiY = <JobPostModel>[];
      _thongBaoLoiGoiY = response.message;
    }

    _dangTaiGoiY = false;
    notifyListeners();
  }

  /// Tải danh sách việc làm do chính Nhà tuyển dụng đăng (Job của tôi)
  Future<void> taiDanhSachJobCuaToi(int maUser) async {
    _dangTaiJobCuaToi = true;
    _thongBaoLoiJobCuaToi = null;
    notifyListeners();

    final response = await _jobService.layJobCuaToi(maUser);

    if (response.isSuccess) {
      _danhSachJobCuaToi = response.data ?? <JobPostModel>[];
    } else {
      _danhSachJobCuaToi = <JobPostModel>[];
      _thongBaoLoiJobCuaToi = response.message;
    }

    _dangTaiJobCuaToi = false;
    notifyListeners();
  }

  /// Đổi trạng thái tin tuyển dụng (Đang tuyển / Đã đóng)
  Future<bool> doiTrangThaiJob(JobPostModel job, String trangThaiMoi) async {
    final payload = {
      'maJob': job.maJob,
      'maUser': job.maUser,
      'tieude': job.tieude,
      'mota': job.mota,
      'thulao': job.thulao,
      'thoigiandukienhoanthanh': job.hanHoanThanh.toIso8601String(),
      'soluongtuyen': job.soluongtuyen,
      'kynangyeucau': job.kynangyeucau,
      'status': trangThaiMoi,
      'phiDangBai': job.phiDangBai,
    };

    final response = await _jobService.capNhatJob(payload);
    if (response.isSuccess) {
      final index = _danhSachJobCuaToi.indexWhere((j) => j.maJob == job.maJob);
      if (index != -1) {
        _danhSachJobCuaToi[index] = job.copyWith(status: trangThaiMoi);
        notifyListeners();
      }
      return true;
    }
    return false;
  }

  List<JobPostModel> _locCongViec(List<JobPostModel> jobs, String query) {
    final ketQua = jobs.where((job) {
      final khopTuKhoa = query.isEmpty ||
          job.tieude.toLowerCase().contains(query) ||
          job.kynangyeucau.toLowerCase().contains(query) ||
          (job.tenNhaTuyenDung ?? '').toLowerCase().contains(query);

      if (!khopTuKhoa) return false;

      // Lọc theo mức thù lao
      final thuLao = job.budget;
      bool khopThuLao = true;
      switch (_mucThuLaoLoc) {
        case 'duoi-1tr':
          khopThuLao = thuLao < 1000000;
          break;
        case '1tr-3tr':
          khopThuLao = thuLao >= 1000000 && thuLao <= 3000000;
          break;
        case '3tr-10tr':
          khopThuLao = thuLao > 3000000 && thuLao <= 10000000;
          break;
        case 'tren-10tr':
          khopThuLao = thuLao > 10000000;
          break;
      }
      if (!khopThuLao) return false;

      // Lọc theo trạng thái
      if (_trangThaiLoc == 'DangTuyen') {
        if (job.isDaHoanThanh || job.isDaTuyenDu || job.status != 'DangTuyen') {
          return false;
        }
      } else if (_trangThaiLoc == 'DaHoanThanh') {
        if (!job.isDaHoanThanh) return false;
      }

      return true;
    }).toList();

    // Sắp xếp
    if (_thuTuSapXep == 'luong-cao') {
      ketQua.sort((a, b) => b.budget.compareTo(a.budget));
    } else if (_thuTuSapXep == 'luong-thap') {
      ketQua.sort((a, b) => a.budget.compareTo(b.budget));
    }

    return ketQua;
  }
}

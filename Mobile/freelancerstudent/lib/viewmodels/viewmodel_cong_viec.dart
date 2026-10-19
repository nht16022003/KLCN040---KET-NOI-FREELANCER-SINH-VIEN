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

  bool get dangTai => _dangTai;
  List<JobPostModel> get danhSachJobPost => _danhSachJobPost;
  String? get thongBaoLoi => _thongBaoLoi;

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
    final query = tuKhoa.trim().toLowerCase();
    if (_daTaiDanhSach && query.isNotEmpty && !lamMoi) {
      _requestId++;
      _dangTai = false;
      _thongBaoLoi = null;
      _danhSachJobPost = _locCongViec(_tatCaJobPost, query);
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
      _danhSachJobPost = _locCongViec(_tatCaJobPost, query);
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
    if (query.isEmpty) return jobs;
    return jobs.where((job) {
      return job.tieude.toLowerCase().contains(query) ||
          job.kynangyeucau.toLowerCase().contains(query);
    }).toList();
  }
}

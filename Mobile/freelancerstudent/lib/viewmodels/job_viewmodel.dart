import 'package:flutter/foundation.dart';

import '../models/job_post_model.dart';
import '../services/job_service.dart';

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

  List<JobPostModel> _locCongViec(List<JobPostModel> jobs, String query) {
    if (query.isEmpty) return jobs;
    return jobs.where((job) {
      return job.tieude.toLowerCase().contains(query) ||
          job.kynangyeucau.toLowerCase().contains(query);
    }).toList();
  }
}

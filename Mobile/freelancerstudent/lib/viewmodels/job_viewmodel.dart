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
  Future<void> taiDanhSachJobPost({int trang = 1, String tuKhoa = ''}) async {
    _dangTai = true;
    _thongBaoLoi = null;
    _danhSachJobPost = <JobPostModel>[];
    notifyListeners();

    final response = await _jobService.layDanhSachJobPost(page: trang, tuKhoa: tuKhoa);

    if (response.isSuccess) {
      _danhSachJobPost = response.data ?? <JobPostModel>[];
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
}

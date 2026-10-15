import 'package:flutter/foundation.dart';

import '../models/job_post_model.dart';
import '../services/job_service.dart';

class JobViewModel extends ChangeNotifier {
  final JobService _jobService = JobService();

  bool _isLoading = false;
  List<JobPostModel> _jobs = <JobPostModel>[];
  String? _errorMessage;

  bool get isLoading => _isLoading;
  List<JobPostModel> get jobs => _jobs;
  String? get errorMessage => _errorMessage;

  Future<void> fetchJobs({int page = 1, String keyword = ''}) async {
    _isLoading = true;
    _errorMessage = null;
    _jobs = <JobPostModel>[];
    notifyListeners();

    final response = await _jobService.getJobs(page: page, keyword: keyword);

    if (response.isSuccess) {
      _jobs = response.data ?? <JobPostModel>[];
      if (_jobs.isEmpty) {
        _errorMessage = 'Chưa có công việc nào được đăng.';
      }
    } else {
      _jobs = <JobPostModel>[];
      _errorMessage = response.message.isNotEmpty
          ? response.message
          : 'Không thể tải danh sách việc làm.';
    }

    _isLoading = false;
    notifyListeners();
  }
}

import 'package:flutter/foundation.dart';

import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_chinh_sua_ho_so_freelancer.dart';
import '../models/mo_hinh_ho_so_freelancer.dart';
import '../services/dich_vu_ho_so_freelancer.dart';

class FreelancerProfileViewModel extends ChangeNotifier {
  final FreelancerProfileService _service = FreelancerProfileService();

  FreelancerProfileModel? _profile;
  bool _isLoading = false;
  String? _errorMessage;

  FreelancerProfileModel? get profile => _profile;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadProfile(int maUser) async {
    await _load(() => _service.getFreelancerProfile(maUser));
  }

  Future<void> loadProfileById(int freelancerId) async {
    if (_profile?.maFreelancerStudents != freelancerId) {
      _profile = null;
      notifyListeners();
    }
    await _load(() => _service.getFreelancerProfileById(freelancerId));
  }

  Future<void> _load(
    Future<ApiResponse<FreelancerProfileModel>> Function() fetch,
  ) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await fetch();
    if (response.isSuccess && response.data != null) {
      _profile = response.data;
    } else {
      _errorMessage = response.message;
    }

    _isLoading = false;
    notifyListeners();
  }

  Future<bool> saveProfile(FreelancerProfileEditModel profile) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.updateFreelancerProfile(profile);
    _isLoading = false;
    if (!response.isSuccess) {
      _errorMessage = response.message;
    }
    notifyListeners();
    return response.isSuccess;
  }

  Future<bool> updatePortfolioInfo({
    required int freelancerId,
    String? moTaBanThan,
    String? urlVideo,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.updatePortfolioInfo(
      freelancerId: freelancerId,
      moTaBanThan: moTaBanThan,
      urlVideo: urlVideo,
    );
    if (!response.isSuccess) {
      _errorMessage = response.message;
      _isLoading = false;
      notifyListeners();
      return false;
    }

    final refreshed = await _service.getFreelancerProfileById(freelancerId);
    if (refreshed.isSuccess && refreshed.data != null) {
      _profile = refreshed.data;
    } else {
      _errorMessage = refreshed.message;
    }
    _isLoading = false;
    notifyListeners();
    return true;
  }

  Future<bool> addPortfolioProject({
    required int freelancerId,
    required String projectName,
    String? role,
    String? description,
    String? technologies,
    String? githubUrl,
    String? demoUrl,
    String? fileUrl,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.addPortfolioProject(
      freelancerId: freelancerId,
      projectName: projectName,
      role: role,
      description: description,
      technologies: technologies,
      githubUrl: githubUrl,
      demoUrl: demoUrl,
      fileUrl: fileUrl,
    );
    if (!response.isSuccess) {
      _errorMessage = response.message;
      _isLoading = false;
      notifyListeners();
      return false;
    }

    final refreshed = await _service.getFreelancerProfileById(freelancerId);
    if (refreshed.isSuccess && refreshed.data != null) {
      _profile = refreshed.data;
    } else {
      _errorMessage = refreshed.message;
    }
    _isLoading = false;
    notifyListeners();
    return true;
  }

  Future<bool> updatePortfolioProject({
    required String maDA,
    required int freelancerId,
    required String projectName,
    String? role,
    String? description,
    String? technologies,
    String? githubUrl,
    String? demoUrl,
    String? fileUrl,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.updatePortfolioProject(
      maDA: maDA,
      freelancerId: freelancerId,
      projectName: projectName,
      role: role,
      description: description,
      technologies: technologies,
      githubUrl: githubUrl,
      demoUrl: demoUrl,
      fileUrl: fileUrl,
    );
    if (!response.isSuccess) {
      _errorMessage = response.message;
      _isLoading = false;
      notifyListeners();
      return false;
    }

    final refreshed = await _service.getFreelancerProfileById(freelancerId);
    if (refreshed.isSuccess && refreshed.data != null) {
      _profile = refreshed.data;
    } else {
      _errorMessage = refreshed.message;
    }
    _isLoading = false;
    notifyListeners();
    return true;
  }

  Future<bool> deletePortfolioProject({
    required String maDA,
    required int freelancerId,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.deletePortfolioProject(
      maDA: maDA,
      freelancerId: freelancerId,
    );
    if (!response.isSuccess) {
      _errorMessage = response.message;
      _isLoading = false;
      notifyListeners();
      return false;
    }

    final refreshed = await _service.getFreelancerProfileById(freelancerId);
    if (refreshed.isSuccess && refreshed.data != null) {
      _profile = refreshed.data;
    } else {
      _errorMessage = refreshed.message;
    }
    _isLoading = false;
    notifyListeners();
    return true;
  }

  Future<bool> updateFeaturedProjects({
    required int freelancerId,
    required List<String> maDAs,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.updateFeaturedProjects(
      freelancerId: freelancerId,
      maDAs: maDAs,
    );
    if (!response.isSuccess) {
      _errorMessage = response.message;
      _isLoading = false;
      notifyListeners();
      return false;
    }

    final refreshed = await _service.getFreelancerProfileById(freelancerId);
    if (refreshed.isSuccess && refreshed.data != null) {
      _profile = refreshed.data;
    } else {
      _errorMessage = refreshed.message;
    }
    _isLoading = false;
    notifyListeners();
    return true;
  }
}

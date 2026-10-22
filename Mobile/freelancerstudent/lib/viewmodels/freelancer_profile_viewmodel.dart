import 'package:flutter/foundation.dart';

import '../core/network/api_response.dart';
import '../models/freelancer_profile_edit_model.dart';
import '../models/freelancer_profile_model.dart';
import '../services/freelancer_profile_service.dart';

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

  Future<bool> addPortfolioProject({
    required int freelancerId,
    required String projectName,
    String? role,
    String? description,
    String? technologies,
    String? githubUrl,
    String? demoUrl,
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

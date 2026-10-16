import 'package:flutter/foundation.dart';

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
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.getFreelancerProfile(maUser);
    if (response.isSuccess && response.data != null) {
      _profile = response.data;
    } else {
      _errorMessage = response.message;
    }

    _isLoading = false;
    notifyListeners();
  }
}

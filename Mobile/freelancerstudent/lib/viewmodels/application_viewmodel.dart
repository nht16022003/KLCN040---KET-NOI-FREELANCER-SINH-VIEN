import 'package:flutter/foundation.dart';

import '../models/application_model.dart';
import '../services/application_service.dart';

class ApplicationViewModel extends ChangeNotifier {
  final ApplicationService _service = ApplicationService();

  bool _isLoading = false;
  List<ApplicationModel> _applications = <ApplicationModel>[];
  String? _errorMessage;

  bool get isLoading => _isLoading;
  List<ApplicationModel> get applications => _applications;
  String? get errorMessage => _errorMessage;

  Future<void> fetchMyApplications({int? userId}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.getMyApplications(userId: userId);
    if (response.isSuccess) {
      _applications = response.data ?? <ApplicationModel>[];
      if (_applications.isEmpty &&
          response.message.isNotEmpty &&
          !response.message.contains('Bạn chưa có đơn ứng tuyển nào')) {
        _errorMessage = response.message;
      }
    } else {
      _errorMessage = response.message;
    }

    _isLoading = false;
    notifyListeners();
  }

  void addLocalApplication(ApplicationModel application) {
    final exists = _applications.any((item) => item.id == application.id);
    if (!exists) {
      _applications = [application, ..._applications];
      _errorMessage = null;
      notifyListeners();
    }
  }
}

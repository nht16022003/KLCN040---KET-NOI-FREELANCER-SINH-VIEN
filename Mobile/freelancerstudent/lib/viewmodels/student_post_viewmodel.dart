import 'package:flutter/foundation.dart';

import '../models/student_post_model.dart';
import '../services/student_post_service.dart';

class StudentPostViewModel extends ChangeNotifier {
  final StudentPostService _service = StudentPostService();

  bool _isLoading = false;
  List<StudentPostModel> _posts = <StudentPostModel>[];
  String? _errorMessage;

  bool get isLoading => _isLoading;
  List<StudentPostModel> get posts => _posts;
  String? get errorMessage => _errorMessage;

  Future<void> fetchStudentPosts() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final response = await _service.getStudentPosts();
    if (response.isSuccess) {
      _posts = response.data ?? <StudentPostModel>[];
    } else {
      _errorMessage = response.message;
    }

    _isLoading = false;
    notifyListeners();
  }
}

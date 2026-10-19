// This is a basic Flutter widget test.
//
// To perform an interaction with a widget in your test, use the WidgetTester
// utility in the flutter_test package. For example, you can send tap and scroll
// gestures. You can also use WidgetTester to find child widgets in the widget
// tree, read text, and verify that the values of widget properties are correct.

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'package:freelancerstudent/main.dart';
import 'package:freelancerstudent/models/application_model.dart';
import 'package:freelancerstudent/models/auth_response_model.dart';
import 'package:freelancerstudent/viewmodels/application_viewmodel.dart';
import 'package:freelancerstudent/viewmodels/auth_viewmodel.dart';
import 'package:freelancerstudent/views/applications/my_applications_screen.dart';
import 'package:freelancerstudent/views/auth/login_screen.dart';
import 'package:freelancerstudent/views/auth/register_screen.dart';
import 'package:freelancerstudent/views/auth/splash_screen.dart';

class FakeAuthViewModel extends AuthViewModel {
  @override
  bool get isLoading => false;

  @override
  String? get errorMessage => null;

  @override
  AuthResponseModel? get currentUser => AuthResponseModel(
    token: 'fake-token',
    email: 'test@example.com',
    role: 'FreelancerStudent',
  );

  @override
  Future<bool> registerAccount({
    required String fullName,
    required String userName,
    required String email,
    required String phoneNumber,
    required String password,
    required String role,
  }) async {
    return true;
  }
}

class FakeApplicationViewModel extends ApplicationViewModel {
  @override
  bool get isLoading => false;

  @override
  List<ApplicationModel> get applications => [
    const ApplicationModel(
      id: 'app-1',
      jobId: 'JOB-123',
      applicantId: 'user-1',
      coverLetter: 'Tôi có kinh nghiệm Flutter.',
      proposedBudget: 2500000,
      status: 'Pending',
    ),
  ];

  @override
  String? get errorMessage => null;

  @override
  Future<void> fetchMyApplications({int? userId}) async {}
}

void main() {
  testWidgets('app loads splash screen', (WidgetTester tester) async {
    await tester.pumpWidget(const FreelancerStudentApp());

    expect(find.byType(SplashScreen), findsOneWidget);
    await tester.pump(const Duration(milliseconds: 1300));
  });

  testWidgets('successful registration returns to login screen', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(
      ChangeNotifierProvider<AuthViewModel>.value(
        value: FakeAuthViewModel(),
        child: const MaterialApp(home: RegisterScreen()),
      ),
    );

    await tester.enterText(find.byType(TextField).at(0), 'Nguyen Van A');
    await tester.enterText(find.byType(TextField).at(1), 'nguyenvana');
    await tester.enterText(find.byType(TextField).at(2), 'test@example.com');
    await tester.enterText(find.byType(TextField).at(3), '0912345678');
    await tester.enterText(find.byType(TextField).at(4), '123456');
    await tester.enterText(find.byType(TextField).at(5), '123456');

    final submitButton = find.widgetWithText(ElevatedButton, 'Tạo tài khoản');
    await tester.ensureVisible(submitButton);
    await tester.tap(submitButton);
    await tester.pumpAndSettle();

    expect(find.byType(LoginScreen), findsOneWidget);
  });

  testWidgets('my applications screen uses API-backed application data', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(
      MultiProvider(
        providers: [
          ChangeNotifierProvider<AuthViewModel>.value(
            value: FakeAuthViewModel(),
          ),
          ChangeNotifierProvider<ApplicationViewModel>.value(
            value: FakeApplicationViewModel(),
          ),
        ],
        child: const MaterialApp(home: MyApplicationsScreen()),
      ),
    );

    expect(find.text('Công việc #JOB-123'), findsOneWidget);
    expect(find.text('Trạng thái: Đang chờ duyệt'), findsOneWidget);
  });
}

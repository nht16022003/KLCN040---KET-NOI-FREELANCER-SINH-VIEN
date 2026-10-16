import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'viewmodels/application_viewmodel.dart';
import 'viewmodels/auth_viewmodel.dart';
import 'viewmodels/chat_viewmodel.dart';
import 'viewmodels/freelancer_profile_viewmodel.dart';
import 'viewmodels/job_viewmodel.dart';
import 'viewmodels/student_post_viewmodel.dart';
import 'viewmodels/wallet_viewmodel.dart';
import 'views/auth/splash_screen.dart';

void main() {
  runApp(const FreelancerStudentApp());
}

class FreelancerStudentApp extends StatelessWidget {
  const FreelancerStudentApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthViewModel()),
        ChangeNotifierProvider(create: (_) => JobViewModel()),
        ChangeNotifierProvider(create: (_) => StudentPostViewModel()),
        ChangeNotifierProvider(create: (_) => ApplicationViewModel()),
        ChangeNotifierProvider(create: (_) => WalletViewModel()),
        ChangeNotifierProvider(create: (_) => ChatViewModel()),
        ChangeNotifierProvider(create: (_) => FreelancerProfileViewModel()),
      ],
      child: MaterialApp(
        title: 'FLsv',
        debugShowCheckedModeBanner: false,
        theme: ThemeData(
          useMaterial3: true,
          primarySwatch: Colors.amber,
          scaffoldBackgroundColor: const Color(0xFFF8F7F2),
          colorScheme: ColorScheme.fromSeed(
            seedColor: const Color(0xFFF2C94C),
            primary: const Color(0xFFF2C94C),
            secondary: const Color(0xFFE9B72F),
          ),
        ),
        home: const SplashScreen(),
      ),
    );
  }
}

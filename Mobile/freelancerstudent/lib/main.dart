import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'core/theme/chu_de_ung_dung.dart';

import 'viewmodels/viewmodel_ung_tuyen.dart';
import 'viewmodels/viewmodel_xac_thuc.dart';
import 'viewmodels/viewmodel_chat.dart';
import 'viewmodels/viewmodel_ho_so_freelancer.dart';
import 'viewmodels/viewmodel_hop_dong.dart';
import 'viewmodels/viewmodel_cong_viec.dart';
import 'viewmodels/viewmodel_nha_tuyen_dung.dart';
import 'viewmodels/viewmodel_bai_dang_sinh_vien.dart';
import 'viewmodels/viewmodel_task_cong_viec.dart';
import 'viewmodels/viewmodel_vi_tien.dart';
import 'views/auth/man_hinh_khoi_dong.dart';

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
        ChangeNotifierProvider(create: (_) => HopDongViewModel()),
        ChangeNotifierProvider(create: (_) => TaskViewModel()),
        ChangeNotifierProvider(create: (_) => NhaTuyenDungViewModel()),
      ],
      child: MaterialApp(
        title: 'FLsv',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.lightTheme,
        home: const SplashScreen(),
      ),
    );
  }
}

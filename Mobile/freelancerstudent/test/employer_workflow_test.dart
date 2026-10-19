import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'package:freelancerstudent/models/mo_hinh_ung_tuyen.dart';
import 'package:freelancerstudent/models/mo_hinh_hop_dong.dart';
import 'package:freelancerstudent/models/mo_hinh_phan_hoi_xac_thuc.dart';
import 'package:freelancerstudent/models/mo_hinh_bai_dang_tuyen_dung.dart';
import 'package:freelancerstudent/viewmodels/viewmodel_xac_thuc.dart';
import 'package:freelancerstudent/viewmodels/viewmodel_cong_viec.dart';
import 'package:freelancerstudent/views/employer/man_hinh_trang_chu_nha_tuyen_dung.dart';
import 'package:freelancerstudent/views/employer/man_hinh_tao_hop_dong.dart';

class FakeAuthViewModelForEmployer extends AuthViewModel {
  @override
  AuthResponseModel? get currentUser => const AuthResponseModel(
        token: 'fake-token-ntd',
        email: 'ntd@techcorp.vn',
        role: 'NhaTuyenDung',
        userId: 99,
        fullName: 'Doanh Nghiệp TechCorp',
        tentaikhoan: 'techcorp_hr',
      );
}

class FakeJobViewModelForEmployer extends JobViewModel {
  final List<JobPostModel> _mockJobs = [
    JobPostModel(
      maJob: 'JOB_001',
      maNhaTuyenDung: 'NTD_01',
      maUser: 99,
      tieude: 'Lập trình Flutter App',
      mota: 'Xây dựng ứng dụng quản lý tiến độ cho sinh viên',
      kynangyeucau: 'Flutter, Dart, REST API',
      thulao: 8000000,
      status: 'DangTuyen',
      soluongtuyen: 2,
      thoigiandukienhoanthanh: DateTime(2026, 11, 30),
    ),
    JobPostModel(
      maJob: 'JOB_002',
      maNhaTuyenDung: 'NTD_01',
      maUser: 99,
      tieude: 'Thiết kế Figma UI/UX',
      mota: 'Thiết kế giao diện mobile và web cho dự án',
      kynangyeucau: 'Figma, UI/UX',
      thulao: 5000000,
      status: 'DaDong',
      soluongtuyen: 1,
      thoigiandukienhoanthanh: DateTime(2026, 10, 25),
    ),
  ];

  @override
  bool get dangTaiJobCuaToi => false;

  @override
  List<JobPostModel> get danhSachJobCuaToi => _mockJobs;

  @override
  Future<void> taiDanhSachJobCuaToi(int maUser) async {}
}

void main() {
  group('Employer Workflow Unit Tests', () {
    test('JobPostModel parses employer specific fields correctly', () {
      final json = {
        'maJob': 'JOB_TEST_99',
        'maNhaTuyenDung': 'NTD_10',
        'maUser': 99,
        'tieude': 'Xây dựng Backend ASP.NET Core',
        'mota': 'Cần bạn sinh viên có kiến thức C#, Web API, SQL Server',
        'kynangyeucau': 'C#, ASP.NET Core, SQL Server',
        'thulao': 10000000,
        'status': 'DangTuyen',
        'soluongtuyen': 1,
        'thoigiandukienhoanthanh': '2026-12-15T00:00:00Z',
        'phiDangBai': 0.0,
      };

      final job = JobPostModel.fromJson(json);

      expect(job.maJob, 'JOB_TEST_99');
      expect(job.maUser, 99);
      expect(job.tieude, 'Xây dựng Backend ASP.NET Core');
      expect(job.thulao, 10000000);
      expect(job.status, 'DangTuyen');
      expect(job.hanHoanThanh.year, 2026);
      expect(job.hanHoanThanh.month, 12);

      final updatedJob = job.copyWith(status: 'DaDong');
      expect(updatedJob.status, 'DaDong');
      expect(updatedJob.maJob, 'JOB_TEST_99');
    });
  });

  group('Employer UI Widget Tests', () {
    testWidgets('EmployerHomeScreen renders real employer stats and job list',
        (tester) async {
      tester.view.physicalSize = const Size(1080, 2400);
      tester.view.devicePixelRatio = 1.0;
      addTearDown(tester.view.resetPhysicalSize);
      addTearDown(tester.view.resetDevicePixelRatio);

      await tester.pumpWidget(
        MultiProvider(
          providers: [
            ChangeNotifierProvider<AuthViewModel>(
              create: (_) => FakeAuthViewModelForEmployer(),
            ),
            ChangeNotifierProvider<JobViewModel>(
              create: (_) => FakeJobViewModelForEmployer(),
            ),
          ],
          child: const MaterialApp(
            home: EmployerHomeScreen(),
          ),
        ),
      );

      await tester.pumpAndSettle();

      expect(find.text('FLsv Doanh Nghiệp'), findsOneWidget);
      expect(find.text('Tổng quan tuyển dụng'), findsOneWidget);
      expect(find.text('Tin tuyển dụng của tôi'), findsOneWidget);
      expect(find.text('Lập trình Flutter App'), findsOneWidget);
      expect(find.text('Thiết kế Figma UI/UX'), findsOneWidget);
      expect(find.text('Đang tuyển'), findsWidgets);
      expect(find.text('Đã đóng'), findsWidgets);
      expect(find.text('Ứng viên'), findsWidgets);
      expect(find.text('Đóng tin'), findsOneWidget);
      expect(find.text('Mở lại'), findsOneWidget);
    });

    testWidgets('TaoHopDongScreen displays applicant info and default contract terms',
        (tester) async {
      const applicant = ApplicationModel(
        maUngTuyen: '101',
        maJob: 'JOB_001',
        maFreelancerStudents: '55',
        thuGioiThieu: 'Em có 2 năm kinh nghiệm Flutter và đã làm 3 dự án.',
        thulaoDeXuat: 8500000,
        trangThaiUngTuyen: 'ChapNhan',
        tieude: 'Lập trình Flutter App',
        tenUser: 'Nguyễn Văn Sinh Viên',
        tenTruong: 'Đại học Bách Khoa',
        tenChuyenNganh: 'Công nghệ thông tin',
      );

      await tester.pumpWidget(
        MultiProvider(
          providers: [
            ChangeNotifierProvider<AuthViewModel>(
              create: (_) => FakeAuthViewModelForEmployer(),
            ),
          ],
          child: const MaterialApp(
            home: TaoHopDongScreen(applicant: applicant),
          ),
        ),
      );

      await tester.pumpAndSettle();

      expect(find.text('Tạo hợp đồng giao kết'), findsOneWidget);
      expect(find.text('Nguyễn Văn Sinh Viên'), findsOneWidget);
      expect(find.text('Đại học Bách Khoa'), findsOneWidget);
      expect(find.text('Làm việc từ xa (Remote)'), findsOneWidget);
      expect(find.text('Khởi tạo dự thảo hợp đồng'), findsOneWidget);
      expect(find.text('Mốc bàn giao #1'), findsOneWidget);
      expect(find.text('Thêm mốc'), findsOneWidget);
      expect(find.byType(TextFormField), findsWidgets);

      // Thêm mốc bàn giao mới (cuộn tới vị trí nếu cần)
      await tester.ensureVisible(find.text('Thêm mốc'));
      await tester.tap(find.text('Thêm mốc'));
      await tester.pumpAndSettle();

      await tester.ensureVisible(find.text('Mốc bàn giao #2'));
      expect(find.text('Mốc bàn giao #2'), findsOneWidget);

      // Xóa mốc bàn giao thứ 2
      final deleteIcons = find.byIcon(Icons.delete_outline);
      expect(deleteIcons, findsNWidgets(2));
      await tester.ensureVisible(deleteIcons.last);
      await tester.tap(deleteIcons.last);
      await tester.pumpAndSettle();

      expect(find.text('Mốc bàn giao #2'), findsNothing);
      expect(find.text('Mốc bàn giao #1'), findsOneWidget);
    });

    test('ApplicationModel correctly tracks contract status with daCoHopDong', () {
      const chuaCo = ApplicationModel(
        maUngTuyen: '1',
        trangThaiUngTuyen: 'ChapNhan',
      );
      expect(chuaCo.daCoHopDong, isFalse);

      const daHuy = ApplicationModel(
        maUngTuyen: '2',
        trangThaiUngTuyen: 'ChapNhan',
        trangThaiHopDong: 'DaHuy',
      );
      expect(daHuy.daCoHopDong, isFalse);

      const daCo = ApplicationModel(
        maUngTuyen: '3',
        trangThaiUngTuyen: 'ChapNhan',
        trangThaiHopDong: 'ChoXacNhan',
        maHopDong: 'HD-2026-001',
      );
      expect(daCo.daCoHopDong, isTrue);
      expect(daCo.maHopDong, equals('HD-2026-001'));
    });

    test('HopDongModel handles rejection reason and status correctly', () {
      final json = {
        'maHD': 'HD_TUCHOI_01',
        'maJob': 'JOB_001',
        'tenCongViec': 'Lập trình Flutter App',
        'maFreelancerStudent': 12,
        'maUngTuyen': 88,
        'tenFreelancer': 'Nguyen Van A',
        'trangThai': 'DaHuy',
        'dieuKhoan': 'Dieu khoan co ban',
        'lyDoTuChoi': 'Mức thù lao chưa phù hợp và thời gian quá gấp',
      };

      final hd = HopDongModel.fromJson(json);
      expect(hd.maHD, equals('HD_TUCHOI_01'));
      expect(hd.maUngTuyen, equals(88));
      expect(hd.daBiTuChoi, isTrue);
      expect(hd.lyDoTuChoi, equals('Mức thù lao chưa phù hợp và thời gian quá gấp'));
    });
  });
}

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'package:freelancerstudent/models/mo_hinh_phan_hoi_xac_thuc.dart';
import 'package:freelancerstudent/models/mo_hinh_nha_tuyen_dung.dart';
import 'package:freelancerstudent/viewmodels/viewmodel_xac_thuc.dart';
import 'package:freelancerstudent/viewmodels/viewmodel_cong_viec.dart';
import 'package:freelancerstudent/viewmodels/viewmodel_nha_tuyen_dung.dart';
import 'package:freelancerstudent/views/home/man_hinh_trang_chu.dart';

class FakeAuthViewModel extends AuthViewModel {
  @override
  bool get isLoading => false;

  @override
  AuthResponseModel? get currentUser => AuthResponseModel(
        token: 'fake-token',
        email: 'freelancer@fpt.edu.vn',
        fullName: 'Lê Văn Sinh Viên',
        role: 'FreelancerStudent',
      );
}

class FakeNhaTuyenDungViewModel extends NhaTuyenDungViewModel {
  final List<NhaTuyenDungModel> _mockData = [
    const NhaTuyenDungModel(
      maNhaTuyenDung: 1,
      maUser: 101,
      tencongty: 'Tập đoàn Công nghệ FPT',
      hotenUser: 'Nguyễn Văn Tuyển Dụng',
      emailUser: 'tuyendung@fpt.com',
      linhvuc: 'Công nghệ thông tin',
      diachi: 'Hà Nội',
      gioithieu: 'Doanh nghiệp công nghệ hàng đầu tại Việt Nam.',
      sosaodanhgia: 4.9,
    ),
    const NhaTuyenDungModel(
      maNhaTuyenDung: 2,
      maUser: 102,
      tencongty: 'VNG Corporation',
      hotenUser: 'Trần Thị Nhân Sự',
      emailUser: 'hr@vng.com.vn',
      linhvuc: 'Game & Internet',
      diachi: 'TP. Hồ Chí Minh',
      gioithieu: 'Kỳ lân công nghệ đầu tiên tại Việt Nam.',
      sosaodanhgia: 4.8,
    ),
  ];

  FakeNhaTuyenDungViewModel() {
    // Khởi tạo trực tiếp mock data
  }

  @override
  bool get dangTai => false;

  @override
  List<NhaTuyenDungModel> get danhSachNhaTuyenDung {
    final kw = tuKhoa.toLowerCase();
    return _mockData.where((item) {
      final matchKw = kw.isEmpty ||
          (item.tencongty ?? '').toLowerCase().contains(kw) ||
          (item.hotenUser ?? '').toLowerCase().contains(kw) ||
          (item.diachi ?? '').toLowerCase().contains(kw);
      final matchLv = linhVucDaChon.isEmpty || item.linhvuc == linhVucDaChon;
      return matchKw && matchLv;
    }).toList();
  }

  @override
  int get tongSoNhaTuyenDung => danhSachNhaTuyenDung.length;

  @override
  List<String> get danhSachLinhVuc => [
        'Tất cả',
        'Công nghệ thông tin',
        'Game & Internet',
      ];

  @override
  Future<void> taiDanhSachNhaTuyenDung() async {}
}

void main() {
  testWidgets(
      'HomeScreen hiển thị thông tin nhà tuyển dụng và tìm kiếm nhà tuyển dụng theo phong cách web',
      (WidgetTester tester) async {
    final fakeNtdVM = FakeNhaTuyenDungViewModel();

    await tester.pumpWidget(
      MultiProvider(
        providers: [
          ChangeNotifierProvider<AuthViewModel>.value(
            value: FakeAuthViewModel(),
          ),
          ChangeNotifierProvider<NhaTuyenDungViewModel>.value(
            value: fakeNtdVM,
          ),
          ChangeNotifierProvider<JobViewModel>(
            create: (_) => JobViewModel(),
          ),
        ],
        child: const MaterialApp(home: HomeScreen()),
      ),
    );

    await tester.pumpAndSettle();

    // 1. Kiểm tra tiêu đề và thông tin chào mừng
    expect(find.text('Khám phá Nhà Tuyển Dụng'), findsOneWidget);
    expect(find.text('Lê Văn Sinh Viên'), findsOneWidget);

    // 2. Kiểm tra danh sách hiển thị tên công ty
    expect(find.text('Tập đoàn Công nghệ FPT'), findsOneWidget);
    expect(find.text('VNG Corporation'), findsOneWidget);

    // 3. Kiểm tra các nút tương tự bên web: "Xem hồ sơ" và "Nhắn tin"
    expect(find.text('Xem hồ sơ'), findsWidgets);
    expect(find.text('Nhắn tin'), findsWidgets);

    // 4. Test tìm kiếm nhà tuyển dụng
    final searchInput = find.byType(TextField).first;
    await tester.enterText(searchInput, 'FPT');
    await tester.pumpAndSettle();

    expect(find.text('Tập đoàn Công nghệ FPT'), findsOneWidget);
    expect(find.text('VNG Corporation'), findsNothing);
  });
}

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../models/mo_hinh_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_cong_viec.dart';
import '../../viewmodels/viewmodel_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_thong_bao.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_yeu_thich.dart';
import '../applications/man_hinh_don_ung_tuyen_cua_toi.dart';
import '../auth/man_hinh_dang_nhap.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';
import '../chat/man_hinh_danh_sach_chat.dart';
import '../danh_gia/man_hinh_danh_gia.dart';
import '../hop_dong/man_hinh_danh_sach_hop_dong.dart';
import '../job_detail/bang_ung_tuyen_cong_viec.dart';
import '../job_detail/man_hinh_chi_tiet_cong_viec.dart';
import '../job_detail/man_hinh_cong_viec_da_luu.dart';
import '../profile/man_hinh_ho_so.dart';
import '../profile/man_hinh_ho_so_nha_tuyen_dung.dart';
import '../student_posts/man_hinh_danh_sach_bai_dang_sinh_vien.dart';
import '../student_posts/man_hinh_quan_ly_bai_dang_sinh_vien.dart';
import '../student_posts/man_hinh_tao_bai_dang_sinh_vien.dart';
import '../thong_bao/man_hinh_thong_bao.dart';
import '../wallet/man_hinh_vi_tien.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  // Bộ điều khiển tìm kiếm
  final TextEditingController _boDieuKhienTimKiemNhaTuyenDung =
      TextEditingController();
  final TextEditingController _boDieuKhienTimKiemViecLam =
      TextEditingController();

  // 0: Nhà tuyển dụng (mặc định), 1: Việc làm
  int _tabHienTai = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      // Tải cả danh sách nhà tuyển dụng và việc làm khi vào màn hình
      context.read<NhaTuyenDungViewModel>().taiDanhSachNhaTuyenDung();
      context.read<JobViewModel>().fetchJobs();
      final authUser = context.read<AuthViewModel>().currentUser;
      if (authUser != null) {
        context.read<JobViewModel>().taiDanhSachJobGoiY(authUser.userId);
        context.read<YeuThichViewModel>().taiDanhSachViecDaLuu(authUser.userId);
        context.read<ThongBaoViewModel>().demChuaDoc(authUser.userId);
      }
    });
  }

  @override
  void dispose() {
    _boDieuKhienTimKiemNhaTuyenDung.dispose();
    _boDieuKhienTimKiemViecLam.dispose();
    super.dispose();
  }

  void _xuLyTimKiemNhaTuyenDung(String tuKhoa) {
    context.read<NhaTuyenDungViewModel>().timKiem(tuKhoa);
  }

  void _xuLyTimKiemViecLam(String tuKhoa) {
    context.read<JobViewModel>().fetchJobs(keyword: tuKhoa.trim());
  }

  @override
  Widget build(BuildContext context) {
    final authUser = context.watch<AuthViewModel>().currentUser;
    final nhaTuyenDungVM = context.watch<NhaTuyenDungViewModel>();
    final jobViewModel = context.watch<JobViewModel>();

    final tenHienThi = authUser?.fullName.isNotEmpty == true
        ? authUser!.fullName
        : 'Freelancer Student';
    final emailHienThi = authUser?.email ?? 'Chưa có email';

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'FLsv',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: AppColors.primary,
            fontSize: 24,
          ),
        ),
        actions: [
          // Chuông thông báo + Badge chưa đọc
          Consumer<ThongBaoViewModel>(
            builder: (context, thongBaoVM, _) {
              final chuaDoc = thongBaoVM.soLuongChuaDoc;
              return IconButton(
                tooltip: 'Thông báo',
                onPressed: () async {
                  await Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const ThongBaoScreen(),
                    ),
                  );
                  if (context.mounted && authUser != null) {
                    context
                        .read<ThongBaoViewModel>()
                        .demChuaDoc(authUser.userId);
                  }
                },
                icon: Stack(
                  clipBehavior: Clip.none,
                  children: [
                    const Icon(
                      Icons.notifications_outlined,
                      color: AppColors.heading,
                      size: 26,
                    ),
                    if (chuaDoc > 0)
                      Positioned(
                        right: -4,
                        top: -4,
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 5,
                            vertical: 1.5,
                          ),
                          decoration: BoxDecoration(
                            color: AppColors.error,
                            borderRadius: BorderRadius.circular(10),
                          ),
                          constraints: const BoxConstraints(
                            minWidth: 16,
                            minHeight: 16,
                          ),
                          child: Text(
                            chuaDoc > 99 ? '99+' : '$chuaDoc',
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              color: Colors.white,
                              fontSize: 10,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
              );
            },
          ),
          PopupMenuButton<String>(
            tooltip: 'Tài khoản',
            offset: const Offset(0, 52),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
            icon: const CircleAvatar(
              radius: 20,
              backgroundColor: AppColors.lightGreenSurface,
              child: Icon(Icons.person, color: Colors.black87),
            ),
            onSelected: (value) async => _handleMenuAction(context, value),
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'chat',
                child: Row(
                  children: [
                    Icon(Icons.chat_bubble_outline, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Chat'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'wallet',
                child: Row(
                  children: [
                    Icon(Icons.wallet_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Ví'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'post',
                child: Row(
                  children: [
                    Icon(Icons.post_add, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Đăng bài tìm việc'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'my_posts',
                child: Row(
                  children: [
                    Icon(Icons.fact_check_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Quản lý bài tìm việc'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'student_market',
                child: Row(
                  children: [
                    Icon(Icons.groups_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Chợ bài đăng SV'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'saved_jobs',
                child: Row(
                  children: [
                    Icon(Icons.bookmark_outline, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Công việc đã lưu'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'contracts',
                child: Row(
                  children: [
                    Icon(Icons.description_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Hợp đồng của tôi'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'reviews',
                child: Row(
                  children: [
                    Icon(Icons.star_outline, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Đánh giá & Nhận xét'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'applications',
                child: Row(
                  children: [
                    Icon(Icons.assignment_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Trạng thái ứng tuyển'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'settings',
                child: Row(
                  children: [
                    Icon(
                      Icons.manage_accounts_outlined,
                      color: AppColors.primary,
                    ),
                    SizedBox(width: 10),
                    Text('Cài đặt tài khoản'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'logout',
                child: Row(
                  children: [
                    Icon(Icons.logout, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Đăng xuất'),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(width: 12),
        ],
      ),
      body: SafeArea(
        child: RefreshIndicator(
          color: AppColors.primary,
          onRefresh: () async {
            if (authUser != null) {
              context.read<ThongBaoViewModel>().demChuaDoc(authUser.userId);
            }
            if (_tabHienTai == 0) {
              await nhaTuyenDungVM.taiDanhSachNhaTuyenDung();
            } else {
              await jobViewModel.refreshJobs(
                keyword: _boDieuKhienTimKiemViecLam.text.trim(),
              );
              if (authUser != null) {
                await jobViewModel.taiDanhSachJobGoiY(authUser.userId);
              }
            }
          },
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              // 1. Thẻ chào mừng người dùng
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(18),
                  border: Border.all(color: AppColors.border),
                  boxShadow: [
                    BoxShadow(
                      color: AppColors.lightGreenSurface.withValues(alpha: 0.15),
                      blurRadius: 8,
                      offset: const Offset(0, 3),
                    ),
                  ],
                ),
                child: Row(
                  children: [
                    CircleAvatar(
                      radius: 24,
                      backgroundColor: AppColors.lightGreenSurface,
                      backgroundImage: authUser?.avatarUrl != null &&
                              authUser!.avatarUrl!.trim().isNotEmpty
                          ? NetworkImage(authUser.anhDaiDienHienThi)
                          : null,
                      onBackgroundImageError: authUser?.avatarUrl != null &&
                              authUser!.avatarUrl!.trim().isNotEmpty
                          ? (context, error) {}
                          : null,
                      child: authUser?.avatarUrl == null ||
                              authUser!.avatarUrl!.trim().isEmpty
                          ? Text(
                              tenHienThi.isNotEmpty
                                  ? tenHienThi[0].toUpperCase()
                                  : 'S',
                              style: const TextStyle(
                                fontSize: 20,
                                fontWeight: FontWeight.bold,
                                color: AppColors.primary,
                              ),
                            )
                          : null,
                    ),
                    const SizedBox(width: 14),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Xin chào,',
                            style: TextStyle(
                              fontSize: 13,
                              color: AppColors.bodyText,
                            ),
                          ),
                          const SizedBox(height: 2),
                          Text(
                            tenHienThi,
                            style: const TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
                          ),
                          const SizedBox(height: 2),
                          Text(
                            emailHienThi,
                            style: const TextStyle(
                              color: AppColors.bodyText,
                              fontSize: 12,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 16),

              // 2. Chuyển đổi Tab (Nhà tuyển dụng / Việc làm)
              Container(
                padding: const EdgeInsets.all(4),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(14),
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: GestureDetector(
                        onTap: () {
                          setState(() {
                            _tabHienTai = 0;
                          });
                        },
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 10),
                          decoration: BoxDecoration(
                            color: _tabHienTai == 0
                                ? Colors.white
                                : Colors.transparent,
                            borderRadius: BorderRadius.circular(10),
                            boxShadow: _tabHienTai == 0
                                ? [
                                    BoxShadow(
                                      color: Colors.black.withValues(alpha: 0.05),
                                      blurRadius: 4,
                                      offset: const Offset(0, 2),
                                    ),
                                  ]
                                : null,
                          ),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(
                                Icons.business,
                                size: 18,
                                color: _tabHienTai == 0
                                    ? AppColors.primary
                                    : AppColors.bodyText,
                              ),
                              const SizedBox(width: 6),
                              Text(
                                'Nhà tuyển dụng',
                                style: TextStyle(
                                  fontWeight: _tabHienTai == 0
                                      ? FontWeight.bold
                                      : FontWeight.w600,
                                  color: _tabHienTai == 0
                                      ? AppColors.primary
                                      : AppColors.bodyText,
                                  fontSize: 14,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                    Expanded(
                      child: GestureDetector(
                        onTap: () {
                          setState(() {
                            _tabHienTai = 1;
                          });
                        },
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 10),
                          decoration: BoxDecoration(
                            color: _tabHienTai == 1
                                ? Colors.white
                                : Colors.transparent,
                            borderRadius: BorderRadius.circular(10),
                            boxShadow: _tabHienTai == 1
                                ? [
                                    BoxShadow(
                                      color: Colors.black.withValues(alpha: 0.05),
                                      blurRadius: 4,
                                      offset: const Offset(0, 2),
                                    ),
                                  ]
                                : null,
                          ),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(
                                Icons.work_outline,
                                size: 18,
                                color: _tabHienTai == 1
                                    ? AppColors.primary
                                    : AppColors.bodyText,
                              ),
                              const SizedBox(width: 6),
                              Text(
                                'Việc làm',
                                style: TextStyle(
                                  fontWeight: _tabHienTai == 1
                                      ? FontWeight.bold
                                      : FontWeight.w600,
                                  color: _tabHienTai == 1
                                      ? AppColors.primary
                                      : AppColors.bodyText,
                                  fontSize: 14,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 18),

              // 3. Nội dung theo Tab đã chọn
              if (_tabHienTai == 0)
                _buildPhanNhaTuyenDung(context, nhaTuyenDungVM)
              else
                _buildPhanViecLam(context, jobViewModel),
            ],
          ),
        ),
      ),
    );
  }

  // ==========================================
  // TAB 1: KHÁM PHÁ & TÌM KIẾM NHÀ TUYỂN DỤNG (Tương tự Search.cshtml bên Web)
  // ==========================================
  Widget _buildPhanNhaTuyenDung(
    BuildContext context,
    NhaTuyenDungViewModel nhaTuyenDungVM,
  ) {
    final dsNhaTuyenDung = nhaTuyenDungVM.danhSachNhaTuyenDung;
    final dsLinhVuc = nhaTuyenDungVM.danhSachLinhVuc;
    final linhVucDaChon = nhaTuyenDungVM.linhVucDaChon;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Tiêu đề & Badge số lượng (tương tự Web Search.cshtml)
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Khám phá Nhà Tuyển Dụng',
                    style: TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: AppColors.primary,
                    ),
                  ),
                  SizedBox(height: 4),
                  Text(
                    'Tìm kiếm doanh nghiệp đối tác & trực tiếp trao đổi',
                    style: TextStyle(
                      fontSize: 13,
                      color: AppColors.bodyText,
                    ),
                  ),
                ],
              ),
            ),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
              decoration: BoxDecoration(
                color: AppColors.lightGreenSurface,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColors.primary),
              ),
              child: Text(
                'Tổng: ${nhaTuyenDungVM.tongSoNhaTuyenDung}',
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                  color: AppColors.primary,
                ),
              ),
            ),
          ],
        ),

        const SizedBox(height: 14),

        // Thanh tìm kiếm nhà tuyển dụng
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 14),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppColors.border),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGreenSurface.withValues(alpha: 0.2),
                blurRadius: 10,
                offset: const Offset(0, 3),
              ),
            ],
          ),
          child: TextField(
            controller: _boDieuKhienTimKiemNhaTuyenDung,
            onChanged: _xuLyTimKiemNhaTuyenDung,
            decoration: InputDecoration(
              border: InputBorder.none,
              hintText: 'Tìm theo tên công ty, người liên hệ, địa chỉ...',
              hintStyle: const TextStyle(fontSize: 13, color: AppColors.bodyText),
              prefixIcon: const Icon(Icons.search, color: AppColors.primary),
              suffixIcon: _boDieuKhienTimKiemNhaTuyenDung.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear, size: 18),
                      onPressed: () {
                        _boDieuKhienTimKiemNhaTuyenDung.clear();
                        _xuLyTimKiemNhaTuyenDung('');
                      },
                    )
                  : null,
            ),
          ),
        ),

        const SizedBox(height: 12),

        // Bộ lọc lĩnh vực hoạt động (Filter chips)
        if (dsLinhVuc.isNotEmpty)
          SizedBox(
            height: 38,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemCount: dsLinhVuc.length,
              separatorBuilder: (context, index) => const SizedBox(width: 8),
              itemBuilder: (context, index) {
                final lv = dsLinhVuc[index];
                final isSelected =
                    (lv == 'Tất cả' && linhVucDaChon.isEmpty) ||
                    (lv == linhVucDaChon);

                return ChoiceChip(
                  label: Text(lv),
                  selected: isSelected,
                  onSelected: (_) => nhaTuyenDungVM.locTheoLinhVuc(lv),
                  selectedColor: AppColors.primary,
                  backgroundColor: Colors.white,
                  labelStyle: TextStyle(
                    fontSize: 12,
                    fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                    color: isSelected ? Colors.white : AppColors.bodyText,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20),
                    side: BorderSide(
                      color: isSelected
                          ? AppColors.primary
                          : AppColors.border,
                    ),
                  ),
                  showCheckmark: false,
                );
              },
            ),
          ),

        const SizedBox(height: 16),

        // Danh sách thẻ Nhà tuyển dụng
        if (nhaTuyenDungVM.dangTai && dsNhaTuyenDung.isEmpty)
          const Center(
            child: Padding(
              padding: EdgeInsets.all(32),
              child: CircularProgressIndicator(color: AppColors.primary),
            ),
          )
        else if (dsNhaTuyenDung.isEmpty)
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 40, horizontal: 20),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              children: [
                const Icon(
                  Icons.business_outlined,
                  size: 48,
                  color: AppColors.bodyText,
                ),
                const SizedBox(height: 12),
                Text(
                  nhaTuyenDungVM.thongBaoLoi ??
                      'Không tìm thấy nhà tuyển dụng nào phù hợp.',
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    color: AppColors.bodyText,
                    fontSize: 14,
                  ),
                ),
              ],
            ),
          )
        else
          ...dsNhaTuyenDung.map(
            (ntd) => Padding(
              padding: const EdgeInsets.only(bottom: 14),
              child: _TheNhaTuyenDung(
                ntd: ntd,
                onXemHoSo: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => NhaTuyenDungProfileScreen(
                        maNhaTuyenDung: ntd.maNhaTuyenDung,
                        maUser: ntd.maUser,
                        initialData: ntd,
                      ),
                    ),
                  );
                },
                onNhanTin: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ChatDetailScreen(
                        receiverId: ntd.maUser.toString(),
                        receiverName: ntd.tenHienThi,
                        avatarUrl: ntd.hinhAnhHienThi,
                      ),
                    ),
                  );
                },
              ),
            ),
          ),
      ],
    );
  }

  // ==========================================
  // TAB 2: KHÁM PHÁ VIỆC LÀM (KÈM BỘ LỌC NÂNG CAO ĐỒNG BỘ WEB)
  // ==========================================
  Widget _buildPhanViecLam(BuildContext context, JobViewModel jobViewModel) {
    final jobs = jobViewModel.jobs;
    final dsGoiY = jobViewModel.danhSachJobGoiY;
    final dangTaiGoiY = jobViewModel.dangTaiGoiY;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Tìm kiếm công việc',
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: AppColors.primary,
          ),
        ),
        const SizedBox(height: 12),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(18),
            border: Border.all(color: AppColors.border),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGreenSurface.withValues(alpha: 0.25),
                blurRadius: 12,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: TextField(
            controller: _boDieuKhienTimKiemViecLam,
            onChanged: _xuLyTimKiemViecLam,
            decoration: const InputDecoration(
              border: InputBorder.none,
              hintText: 'Tìm theo tiêu đề, kỹ năng hoặc công ty...',
              prefixIcon: Icon(Icons.search, color: AppColors.primary),
            ),
          ),
        ),
        const SizedBox(height: 10),

        // Thanh Bộ lọc nâng cao (Mức thù lao, Trạng thái, Sắp xếp)
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            children: [
              // Lọc mức thù lao
              _buildFilterChipMenu(
                label: _nhanMucThuLao(jobViewModel.mucThuLaoLoc),
                isActive: jobViewModel.mucThuLaoLoc != 'tat-ca',
                items: const {
                  'tat-ca': 'Mọi mức thù lao',
                  'duoi-1tr': 'Dưới 1 triệu',
                  '1tr-3tr': '1 - 3 triệu',
                  '3tr-10tr': '3 - 10 triệu',
                  'tren-10tr': 'Trên 10 triệu',
                },
                onSelected: jobViewModel.datBoLocThuLao,
              ),
              const SizedBox(width: 8),

              // Lọc trạng thái
              _buildFilterChipMenu(
                label: _nhanTrangThaiLoc(jobViewModel.trangThaiLoc),
                isActive: jobViewModel.trangThaiLoc != 'tat-ca',
                items: const {
                  'tat-ca': 'Tất cả trạng thái',
                  'DangTuyen': 'Đang mở tuyển',
                  'DaHoanThanh': 'Đã hoàn thành',
                },
                onSelected: jobViewModel.datBoLocTrangThai,
              ),
              const SizedBox(width: 8),

              // Thứ tự sắp xếp
              _buildFilterChipMenu(
                label: _nhanSapXep(jobViewModel.thuTuSapXep),
                isActive: jobViewModel.thuTuSapXep != 'moi-nhat',
                items: const {
                  'moi-nhat': 'Mới nhất',
                  'luong-cao': 'Thù lao cao → thấp',
                  'luong-thap': 'Thù lao thấp → cao',
                },
                onSelected: jobViewModel.datThuTuSapXep,
              ),

              if (jobViewModel.dangCoBoLocNangCao) ...[
                const SizedBox(width: 8),
                TextButton.icon(
                  onPressed: jobViewModel.xoaBoLocNangCao,
                  icon: const Icon(Icons.filter_alt_off_outlined, size: 16),
                  label: const Text('Xóa lọc', style: TextStyle(fontSize: 12)),
                ),
              ],
            ],
          ),
        ),

        // ====================================================
        // KHU VỰC: GỢI Ý VIỆC LÀM PHÙ HỢP KỸ NĂNG (Tối đa 100%)
        // ====================================================
        if (dangTaiGoiY)
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 16),
            child: Center(
              child: Text(
                'Đang đối chiếu kỹ năng của bạn với các công việc...',
                style: TextStyle(fontSize: 13, color: AppColors.bodyText),
              ),
            ),
          )
        else if (dsGoiY.isNotEmpty) ...[
          const SizedBox(height: 20),
          Row(
            children: [
              const Text(
                'Gợi ý việc làm phù hợp',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const Spacer(),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: AppColors.primary),
                ),
                child: Text(
                  '${dsGoiY.length} việc',
                  style: const TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          const Text(
            'Mức độ phù hợp (%) được tính theo tỷ lệ kỹ năng bạn sở hữu đáp ứng yêu cầu tuyển dụng (tối đa 100%)',
            style: TextStyle(fontSize: 12, color: AppColors.bodyText),
          ),
          const SizedBox(height: 12),
          ...dsGoiY.take(5).map(
            (job) => Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: _JobCard(
                job: job,
                isGoiY: true,
                onXemChiTiet: () async {
                  final applied = await Navigator.push<bool>(
                    context,
                    MaterialPageRoute(
                      builder: (_) => JobDetailScreen(job: job),
                    ),
                  );
                  if (applied == true && context.mounted) {
                    await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const MyApplicationsScreen(),
                      ),
                    );
                  }
                },
                onApply: () async {
                  final applied = await showModalBottomSheet<bool>(
                    context: context,
                    isScrollControlled: true,
                    backgroundColor: Colors.transparent,
                    builder: (_) => ApplyJobBottomSheet(job: job),
                  );
                  if (applied == true && context.mounted) {
                    await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const MyApplicationsScreen(),
                      ),
                    );
                  }
                },
              ),
            ),
          ),
        ],

        // ====================================================
        // KHU VỰC: TẤT CẢ VIỆC LÀM
        // ====================================================
        const SizedBox(height: 20),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'Tất cả việc làm',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppColors.heading,
              ),
            ),
            Text(
              '${jobs.length} kết quả',
              style: const TextStyle(
                color: AppColors.primary,
                fontWeight: FontWeight.w600,
                fontSize: 13,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (jobViewModel.isLoading && jobs.isEmpty)
          const Center(
            child: Padding(
              padding: EdgeInsets.all(24),
              child: CircularProgressIndicator(color: AppColors.primary),
            ),
          )
        else if (jobs.isEmpty)
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 24),
            child: Center(
              child: Text(
                'Không tìm thấy công việc nào phù hợp.',
                style: TextStyle(color: AppColors.bodyText),
              ),
            ),
          )
        else
          ...jobs.map(
            (job) => Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: _JobCard(
                job: job,
                onXemChiTiet: () async {
                  final applied = await Navigator.push<bool>(
                    context,
                    MaterialPageRoute(
                      builder: (_) => JobDetailScreen(job: job),
                    ),
                  );
                  if (applied == true && context.mounted) {
                    await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const MyApplicationsScreen(),
                      ),
                    );
                  }
                },
                onApply: () async {
                  final applied = await showModalBottomSheet<bool>(
                    context: context,
                    isScrollControlled: true,
                    builder: (_) => ApplyJobBottomSheet(job: job),
                  );
                  if (applied == true && context.mounted) {
                    await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const MyApplicationsScreen(),
                      ),
                    );
                  }
                },
              ),
            ),
          ),
      ],
    );
  }

  String _nhanMucThuLao(String val) {
    switch (val) {
      case 'duoi-1tr':
        return 'Thù lao: < 1tr';
      case '1tr-3tr':
        return 'Thù lao: 1 - 3tr';
      case '3tr-10tr':
        return 'Thù lao: 3 - 10tr';
      case 'tren-10tr':
        return 'Thù lao: > 10tr';
      default:
        return 'Mức thù lao';
    }
  }

  String _nhanTrangThaiLoc(String val) {
    switch (val) {
      case 'DangTuyen':
        return 'Đang mở tuyển';
      case 'DaHoanThanh':
        return 'Đã hoàn thành';
      default:
        return 'Trạng thái';
    }
  }

  String _nhanSapXep(String val) {
    switch (val) {
      case 'luong-cao':
        return 'Thù lao cao nhất';
      case 'luong-thap':
        return 'Thù lao thấp nhất';
      default:
        return 'Mới nhất';
    }
  }

  Widget _buildFilterChipMenu({
    required String label,
    required bool isActive,
    required Map<String, String> items,
    required void Function(String) onSelected,
  }) {
    return PopupMenuButton<String>(
      onSelected: onSelected,
      itemBuilder: (context) => items.entries
          .map(
            (e) => PopupMenuItem<String>(
              value: e.key,
              child: Text(e.value),
            ),
          )
          .toList(),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 7),
        decoration: BoxDecoration(
          color: isActive ? AppColors.primary : Colors.white,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: isActive ? AppColors.primary : AppColors.border,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              label,
              style: TextStyle(
                fontSize: 12,
                fontWeight: isActive ? FontWeight.bold : FontWeight.w500,
                color: isActive ? Colors.white : AppColors.bodyText,
              ),
            ),
            const SizedBox(width: 4),
            Icon(
              Icons.keyboard_arrow_down,
              size: 16,
              color: isActive ? Colors.white : AppColors.bodyText,
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _handleMenuAction(BuildContext context, String value) async {
    switch (value) {
      case 'chat':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ChatListScreen()),
        );
        break;
      case 'wallet':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const WalletScreen()),
        );
        break;
      case 'post':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const CreateStudentPostScreen()),
        );
        break;
      case 'my_posts':
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => const QuanLyBaiDangSinhVienScreen(),
          ),
        );
        break;
      case 'student_market':
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => const StudentPostListScreen(),
          ),
        );
        break;
      case 'saved_jobs':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ManHinhCongViecDaLuu()),
        );
        break;
      case 'contracts':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const HopDongListScreen()),
        );
        break;
      case 'reviews':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const DanhGiaListScreen()),
        );
        break;
      case 'applications':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const MyApplicationsScreen()),
        );
        break;
      case 'settings':
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ProfileScreen()),
        );
        break;
      case 'logout':
        await context.read<AuthViewModel>().logout();
        if (!context.mounted) return;
        Navigator.of(context).pushAndRemoveUntil(
          MaterialPageRoute(builder: (_) => const LoginScreen()),
          (route) => false,
        );
        break;
    }
  }
}

// ==========================================
// THẺ NHÀ TUYỂN DỤNG (TƯƠNG TỰ CARD BÊN WEB)
// ==========================================
class _TheNhaTuyenDung extends StatelessWidget {
  final NhaTuyenDungModel ntd;
  final VoidCallback onXemHoSo;
  final VoidCallback onNhanTin;

  const _TheNhaTuyenDung({
    required this.ntd,
    required this.onXemHoSo,
    required this.onNhanTin,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
        boxShadow: [
          BoxShadow(
            color: AppColors.lightGreenSurface.withValues(alpha: 0.15),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header Card: Logo + Tên công ty + Lĩnh vực (như Web Search.cshtml)
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(10),
                child: Container(
                  width: 50,
                  height: 50,
                  color: AppColors.lightGreenSurface,
                  child: Image.network(
                    ntd.hinhAnhHienThi,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) => Center(
                      child: Text(
                        ntd.tenHienThi.isNotEmpty
                            ? ntd.tenHienThi[0].toUpperCase()
                            : 'N',
                        style: const TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.bold,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      ntd.tenHienThi,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 2,
                      ),
                      decoration: BoxDecoration(
                        color: AppColors.lightGreenSurface,
                        borderRadius: BorderRadius.circular(4),
                        border: Border.all(
                          color: AppColors.lightGreenSurface,
                        ),
                      ),
                      child: Text(
                        ntd.linhVucHienThi,
                        style: const TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w600,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),

          const SizedBox(height: 12),

          // Giới thiệu ngắn (khung xám nhạt như Web: Giới thiệu: ...)
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: AppColors.lightGreenSurface,
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Giới thiệu:',
                  style: TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                    color: AppColors.bodyText,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  ntd.gioiThieuHienThi,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.bodyText,
                    height: 1.35,
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(height: 10),

          // Địa chỉ & Người đại diện
          Row(
            children: [
              const Icon(
                Icons.location_on_outlined,
                size: 15,
                color: AppColors.primary,
              ),
              const SizedBox(width: 4),
              Expanded(
                child: Text(
                  'Khu vực: ${ntd.diaChiHienThi}',
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Row(
            children: [
              const Icon(
                Icons.person_outline,
                size: 15,
                color: AppColors.primary,
              ),
              const SizedBox(width: 4),
              Expanded(
                child: Text(
                  'Đại diện: ${ntd.nguoiDaiDien}',
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                ),
              ),
            ],
          ),

          const SizedBox(height: 14),

          // 2 Nút: "Xem hồ sơ" & "Nhắn tin" (Tương tự Search.cshtml)
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: onXemHoSo,
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.bodyText,
                    side: const BorderSide(color: AppColors.border),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 10),
                  ),
                  child: const Text(
                    'Xem hồ sơ',
                    style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: ElevatedButton(
                  onPressed: onNhanTin,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    foregroundColor: Colors.white,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 10),
                  ),
                  child: const Text(
                    'Nhắn tin',
                    style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

// ==========================================
// THẺ CÔNG VIỆC
// ==========================================
class _JobCard extends StatelessWidget {
  final JobPostModel job;
  final VoidCallback onApply;
  final VoidCallback? onXemChiTiet;
  final bool isGoiY;

  const _JobCard({
    required this.job,
    required this.onApply,
    this.onXemChiTiet,
    this.isGoiY = false,
  });

  @override
  Widget build(BuildContext context) {
    final location = job.kynangyeucau.isEmpty ? 'Online' : 'Remote';
    final isDaHoanThanh = job.isDaHoanThanh;
    final isDaTuyenDu = job.isDaTuyenDu;
    final type = isDaHoanThanh
        ? 'Đã hoàn thành'
        : (isDaTuyenDu
            ? 'Đã tuyển đủ'
            : (job.status == 'DangTuyen' ? 'Đang tuyển' : 'Đã đóng'));
    final phanTramPhuHop = job.phanTramPhuHop;

    return InkWell(
      borderRadius: BorderRadius.circular(18),
      onTap: onXemChiTiet,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(
            color: isGoiY ? AppColors.primaryLight : AppColors.border,
            width: isGoiY ? 1.5 : 1,
          ),
          boxShadow: isGoiY
              ? [
                  BoxShadow(
                    color: AppColors.primaryLight.withValues(alpha: 0.12),
                    blurRadius: 8,
                    offset: const Offset(0, 3),
                  ),
                ]
              : null,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 44,
                  height: 44,
                  decoration: BoxDecoration(
                    color: isGoiY
                        ? AppColors.lightGreenSurface
                        : AppColors.lightGreenSurface,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Center(
                    child: Text(
                      job.title.isNotEmpty ? job.title[0].toUpperCase() : 'V',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: isGoiY
                            ? AppColors.primaryDark
                            : AppColors.primary,
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(
                            child: Text(
                              job.title,
                              style: const TextStyle(
                                fontSize: 17,
                                fontWeight: FontWeight.bold,
                                color: AppColors.heading,
                              ),
                            ),
                          ),
                          Consumer<YeuThichViewModel>(
                            builder: (context, yeuThichVM, _) {
                              final daLuu = yeuThichVM.daLuuViec(job.id);
                              return IconButton(
                                icon: Icon(
                                  daLuu ? Icons.bookmark : Icons.bookmark_border,
                                  color: daLuu ? AppColors.primary : Colors.grey[500],
                                  size: 22,
                                ),
                                padding: EdgeInsets.zero,
                                constraints: const BoxConstraints(),
                                tooltip: daLuu ? 'Bỏ lưu việc' : 'Lưu công việc',
                                onPressed: () async {
                                  final authUser = context.read<AuthViewModel>().currentUser;
                                  if (authUser == null) return;
                                  final ketQua = await yeuThichVM.toggleLuuViec(authUser.userId, job.id);
                                  if (context.mounted) {
                                    ScaffoldMessenger.of(context).showSnackBar(
                                      SnackBar(
                                        content: Text(ketQua ? 'Đã lưu công việc' : 'Đã bỏ lưu công việc'),
                                        backgroundColor: AppColors.primary,
                                        duration: const Duration(seconds: 1),
                                      ),
                                    );
                                  }
                                },
                              );
                            },
                          ),
                          if (phanTramPhuHop != null) ...[
                            const SizedBox(width: 6),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: AppColors.lightGreenSurface,
                                borderRadius: BorderRadius.circular(8),
                                border: Border.all(
                                  color: AppColors.primaryLight,
                                ),
                              ),
                              child: Text(
                                '$phanTramPhuHop% phù hợp',
                                style: const TextStyle(
                                  fontSize: 11,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.primaryDark,
                                ),
                              ),
                            ),
                          ],
                        ],
                      ),
                      const SizedBox(height: 4),
                      Text(
                        job.kynangyeucau.isEmpty
                            ? 'Công ty tuyển dụng'
                            : job.kynangyeucau,
                        style: const TextStyle(color: AppColors.bodyText, fontSize: 13),
                      ),
                    ],
                  ),
                ),
              ],
            ),

            // Lý do gợi ý & Căn cứ tính độ phù hợp
            if (job.lyDoGoiY != null && job.lyDoGoiY!.trim().isNotEmpty) ...[
              const SizedBox(height: 10),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 7),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: AppColors.border),
                ),
                child: Text(
                  job.lyDoGoiY!,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.primaryDark,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ],

            // Kỹ năng trùng khớp
            if (job.kyNangTrungKhop.isNotEmpty) ...[
              const SizedBox(height: 8),
              Wrap(
                spacing: 6,
                runSpacing: 4,
                children: job.kyNangTrungKhop.map((kn) {
                  return Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                    decoration: BoxDecoration(
                      color: AppColors.infoLight,
                      borderRadius: BorderRadius.circular(6),
                      border: Border.all(color: AppColors.infoBorder),
                    ),
                    child: Text(
                      kn,
                      style: const TextStyle(
                        fontSize: 11,
                        fontWeight: FontWeight.w600,
                        color: AppColors.info,
                      ),
                    ),
                  );
                }).toList(),
              ),
            ],

            const SizedBox(height: 12),
            Row(
              children: [
                _TheThongTin(nhan: location),
                const SizedBox(width: 8),
                _TheThongTin(nhan: type),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Text(
                  '${job.budget.toStringAsFixed(0)} VNĐ',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 15,
                    color: AppColors.primary,
                  ),
                ),
                const Spacer(),
                if ((job.tenNhaTuyenDung ?? '').isNotEmpty)
                  Flexible(
                    child: Text(
                      job.tenNhaTuyenDung!,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppColors.bodyText,
                      ),
                    ),
                  ),
              ],
            ),

            const SizedBox(height: 14),

            // 2 Nút hành động: "Xem chi tiết" & "Ứng tuyển"
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: onXemChiTiet,
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.bodyText,
                      side: const BorderSide(color: AppColors.lightGreenSurface),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 10),
                    ),
                    child: const Text(
                      'Xem chi tiết',
                      style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: ElevatedButton(
                    onPressed: (isDaHoanThanh || isDaTuyenDu) ? null : onApply,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: isDaHoanThanh
                          ? Colors.grey.shade400
                          : (isDaTuyenDu ? Colors.amber.shade200 : AppColors.primary),
                      foregroundColor: isDaTuyenDu ? Colors.brown.shade800 : Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 10),
                    ),
                    child: Text(
                      isDaHoanThanh
                          ? 'Đã hoàn thành'
                          : (isDaTuyenDu ? 'Đã tuyển đủ' : 'Ứng tuyển'),
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _TheThongTin extends StatelessWidget {
  final String nhan;

  const _TheThongTin({required this.nhan});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: AppColors.lightGreenSurface,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        nhan,
        style: const TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.w500,
          color: AppColors.bodyText,
        ),
      ),
    );
  }
}

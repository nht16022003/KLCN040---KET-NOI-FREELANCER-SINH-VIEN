import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_thong_bao.dart';
import '../../viewmodels/viewmodel_thong_bao.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../applications/man_hinh_danh_sach_ung_vien.dart';
import '../applications/man_hinh_don_ung_tuyen_cua_toi.dart';
import '../danh_gia/man_hinh_danh_gia.dart';
import '../hop_dong/man_hinh_chi_tiet_hop_dong.dart';
import '../hop_dong/man_hinh_danh_sach_hop_dong.dart';
import '../job_detail/man_hinh_chi_tiet_cong_viec.dart';
import '../profile/man_hinh_minh_chung_the_sv.dart';
import '../wallet/man_hinh_vi_tien.dart';

class ThongBaoScreen extends StatefulWidget {
  const ThongBaoScreen({super.key});

  @override
  State<ThongBaoScreen> createState() => _ThongBaoScreenState();
}

class _ThongBaoScreenState extends State<ThongBaoScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiThongBao();
    });
  }

  void _taiThongBao() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<ThongBaoViewModel>().taiDanhSachThongBao(user.userId);
    }
  }

  void _danhDauTatCaDaDoc() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<ThongBaoViewModel>().danhDauTatCaDaDoc(user.userId);
    }
  }

  void _nhanThongBao(ThongBaoModel item) {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null && !item.daDoc) {
      context.read<ThongBaoViewModel>().danhDauDaDoc(item.maThongBao, user.userId);
    }

    final link = (item.linkDieuHuong ?? '').trim();
    final loai = item.loaiThongBao.toLowerCase();

    if (link.isNotEmpty || loai.isNotEmpty) {
      final uri = Uri.tryParse(link);
      final maHD = uri?.queryParameters['maHD'] ?? uri?.queryParameters['id'];
      final maJob = uri?.queryParameters['maJob'];

      if (link.toLowerCase().contains('hopdong') ||
          loai.contains('hopdong') ||
          loai.contains('hop_dong')) {
        if (maHD != null && maHD.isNotEmpty) {
          Navigator.push(
            context,
            MaterialPageRoute(builder: (_) => HopDongDetailScreen(maHD: maHD)),
          );
          return;
        }
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const HopDongListScreen()),
        );
        return;
      }

      if (link.toLowerCase().contains('jobpost') &&
          maJob != null &&
          maJob.isNotEmpty) {
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => JobDetailScreen(maJob: maJob)),
        );
        return;
      }

      if (link.toLowerCase().contains('wallet') ||
          loai.contains('vi') ||
          loai.contains('naptien') ||
          loai.contains('ruttien')) {
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const WalletScreen()),
        );
        return;
      }

      if (link.toLowerCase().contains('minhchung') ||
          loai.contains('minhchung')) {
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ManHinhMinhChungTheSV()),
        );
        return;
      }

      if (link.toLowerCase().contains('danhgia') || loai.contains('danhgia')) {
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const DanhGiaListScreen()),
        );
        return;
      }

      if (link.toLowerCase().contains('ungtuyen') ||
          link.toLowerCase().contains('noptuyen') ||
          loai.contains('ungtuyen')) {
        final isEmployer = user?.role.toLowerCase() == 'nhatuyendung';
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => isEmployer
                ? const ApplicantListScreen()
                : const MyApplicationsScreen(),
          ),
        );
        return;
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<ThongBaoViewModel>();

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Thông báo'),
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
        actions: [
          if (vm.soLuongChuaDoc > 0)
            TextButton(
              onPressed: _danhDauTatCaDaDoc,
              style: TextButton.styleFrom(
                foregroundColor: AppColors.primary,
                shape: const RoundedRectangleBorder(
                  borderRadius: BorderRadius.zero,
                ),
              ),
              child: const Text('Đọc tất cả'),
            ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async => _taiThongBao(),
        child: Builder(
          builder: (context) {
            if (vm.dangTai && vm.danhSachThongBao.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (vm.thongBaoLoi != null && vm.danhSachThongBao.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        vm.thongBaoLoi!,
                        textAlign: TextAlign.center,
                        style: const TextStyle(color: AppColors.error),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton(
                        onPressed: _taiThongBao,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero,
                          ),
                        ),
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            if (vm.danhSachThongBao.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Text(
                        'Bạn chưa có thông báo nào.',
                        style: TextStyle(color: AppColors.bodyText),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton(
                        onPressed: _taiThongBao,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero,
                          ),
                        ),
                        child: const Text('Tải lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            return ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: vm.danhSachThongBao.length,
              separatorBuilder: (_, _) => const SizedBox(height: 10),
              itemBuilder: (context, index) {
                final item = vm.danhSachThongBao[index];
                return _buildThongBaoItem(item);
              },
            );
          },
        ),
      ),
    );
  }

  Widget _buildThongBaoItem(ThongBaoModel item) {
    return InkWell(
      onTap: () => _nhanThongBao(item),
      child: Container(
        decoration: BoxDecoration(
          color: item.daDoc ? AppColors.surface : const Color(0xFFF0FDF4),
          border: Border.all(
            color: item.daDoc
                ? AppColors.cardBorder
                : AppColors.primary.withValues(alpha: 0.4),
          ),
        ),
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    item.tieuDe,
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight:
                          item.daDoc ? FontWeight.w600 : FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                ),
                if (!item.daDoc)
                  Container(
                    margin: const EdgeInsets.only(left: 8),
                    padding: const EdgeInsets.symmetric(
                      horizontal: 6,
                      vertical: 2,
                    ),
                    decoration: const BoxDecoration(
                      color: AppColors.primary,
                    ),
                    child: const Text(
                      'Mới',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
              ],
            ),
            const SizedBox(height: 6),
            Text(
              item.noiDung,
              style: const TextStyle(
                fontSize: 13,
                color: AppColors.bodyText,
                height: 1.4,
              ),
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 6,
                    vertical: 2,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.grey.shade100,
                    border: Border.all(color: Colors.grey.shade300),
                  ),
                  child: Text(
                    item.loaiThongBao,
                    style: const TextStyle(fontSize: 11, color: Colors.black54),
                  ),
                ),
                Text(
                  AppFormatters.formatDateTime(item.ngayTao),
                  style: const TextStyle(fontSize: 11, color: Colors.grey),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

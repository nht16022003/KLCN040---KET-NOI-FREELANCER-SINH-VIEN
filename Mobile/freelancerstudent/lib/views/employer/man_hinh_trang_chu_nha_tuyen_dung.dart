import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../services/dich_vu_ung_tuyen.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_cong_viec.dart';
import '../applications/man_hinh_danh_sach_ung_vien.dart';
import '../auth/man_hinh_dang_nhap.dart';
import '../hop_dong/man_hinh_danh_sach_hop_dong.dart';
import '../job_detail/man_hinh_chi_tiet_cong_viec.dart';
import '../wallet/man_hinh_vi_tien.dart';
import 'man_hinh_tao_bai_dang_tuyen_dung.dart';

/// Màn hình chính của Nhà tuyển dụng (Employer Dashboard)
/// Kết nối trực tiếp với API thật:
/// - Bài đăng của tôi: `GET api/JobPost/CuaToi/{maUser}`
/// - Ứng viên: `GET api/JobPost/DanhSachUngTuyen/{maUser}`
/// - Đổi trạng thái bài đăng: `PUT api/JobPost/CapNhat`
class EmployerHomeScreen extends StatefulWidget {
  const EmployerHomeScreen({super.key});

  @override
  State<EmployerHomeScreen> createState() => _EmployerHomeScreenState();
}

class _EmployerHomeScreenState extends State<EmployerHomeScreen> {
  final ApplicationService _applicationService = ApplicationService();

  int _tongSoUngVien = 0;
  bool _dangTaiUngVien = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiDuLieu());
  }

  Future<void> _taiDuLieu() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null || user.userId <= 0) return;

    // Tải bài đăng của tôi
    final jobVM = context.read<JobViewModel>();
    jobVM.taiDanhSachJobCuaToi(user.userId);

    // Tải tổng số ứng viên nộp đơn
    setState(() => _dangTaiUngVien = true);
    final appRes = await _applicationService.getMyApplications(userId: user.userId);
    if (mounted) {
      setState(() {
        _dangTaiUngVien = false;
        _tongSoUngVien = appRes.data?.length ?? 0;
      });
    }
  }

  Future<void> _handleMenuAction(BuildContext context, String value) async {
    switch (value) {
      case 'new-job':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const CreateJobPostScreen()),
        );
        _taiDuLieu();
        break;
      case 'applicants':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ApplicantListScreen()),
        );
        _taiDuLieu();
        break;
      case 'contracts':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const HopDongListScreen()),
        );
        break;
      case 'wallet':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const WalletScreen()),
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

  Future<void> _xacNhanDoiTrangThai(JobPostModel job) async {
    final dangTuyen = job.status == 'DangTuyen';
    final trangThaiMoi = dangTuyen ? 'DaDong' : 'DangTuyen';
    final hanhDong = dangTuyen ? 'Đóng bài tuyển dụng' : 'Mở lại bài tuyển dụng';

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(hanhDong),
        content: Text(
          dangTuyen
              ? 'Bạn có chắc chắn muốn đóng bài đăng "${job.tieude}"? Sinh viên sẽ không thể nộp đơn mới.'
              : 'Bạn có muốn mở lại bài đăng "${job.tieude}" để tiếp tục nhận ứng viên?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(
              backgroundColor: dangTuyen ? AppColors.error : AppColors.primary,
            ),
            child: Text(dangTuyen ? 'Đóng tin' : 'Mở tin'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    final jobVM = context.read<JobViewModel>();
    final ok = await jobVM.doiTrangThaiJob(job, trangThaiMoi);

    if (!mounted) return;
    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            dangTuyen
                ? 'Đã đóng bài tuyển dụng thành công!'
                : 'Đã mở lại bài tuyển dụng!',
          ),
          backgroundColor: AppColors.success,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không thể cập nhật trạng thái bài đăng.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = context.watch<AuthViewModel>().currentUser;
    final jobVM = context.watch<JobViewModel>();

    final dsJob = jobVM.danhSachJobCuaToi;
    final dangTaiJob = jobVM.dangTaiJobCuaToi;
    final soJobDangTuyen = dsJob.where((j) => j.status == 'DangTuyen').length;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: AppColors.surface,
        elevation: 0.5,
        title: const Text(
          'FLsv Doanh Nghiệp',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: AppColors.heading,
            fontSize: 22,
          ),
        ),
        actions: [
          IconButton(
            tooltip: 'Làm mới',
            icon: const Icon(Icons.refresh, color: AppColors.primary),
            onPressed: _taiDuLieu,
          ),
          PopupMenuButton<String>(
            tooltip: 'Tài khoản',
            offset: const Offset(0, 52),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
            icon: const CircleAvatar(
              radius: 18,
              backgroundColor: AppColors.lightGreenSurface,
              child: Icon(Icons.business_center, color: AppColors.primary),
            ),
            onSelected: (value) async => _handleMenuAction(context, value),
            itemBuilder: (context) => const [
              PopupMenuItem(
                value: 'new-job',
                child: Row(
                  children: [
                    Icon(Icons.post_add, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Đăng bài tuyển dụng'),
                  ],
                ),
              ),
              PopupMenuItem(
                value: 'applicants',
                child: Row(
                  children: [
                    Icon(Icons.people_alt_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Danh sách ứng viên'),
                  ],
                ),
              ),
              PopupMenuItem(
                value: 'contracts',
                child: Row(
                  children: [
                    Icon(Icons.assignment_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Quản lý hợp đồng'),
                  ],
                ),
              ),
              PopupMenuItem(
                value: 'wallet',
                child: Row(
                  children: [
                    Icon(Icons.account_balance_wallet_outlined, color: AppColors.primary),
                    SizedBox(width: 10),
                    Text('Ví tiền & Ký quỹ'),
                  ],
                ),
              ),
              PopupMenuDivider(),
              PopupMenuItem(
                value: 'logout',
                child: Row(
                  children: [
                    Icon(Icons.logout, color: Colors.red),
                    SizedBox(width: 10),
                    Text('Đăng xuất', style: TextStyle(color: Colors.red)),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: _taiDuLieu,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              // Lời chào NTD
              if (user != null)
                Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: Text(
                    'Xin chào, ${user.fullName.isNotEmpty ? user.fullName : user.tentaikhoan}',
                    style: const TextStyle(
                      fontSize: 16,
                      color: AppColors.bodyText,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ),

              const Text(
                'Tổng quan tuyển dụng',
                style: TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 14),

              // Thẻ số liệu thật từ API
              Row(
                children: [
                  Expanded(
                    child: _SummaryCard(
                      title: 'Đang tuyển',
                      value: dangTaiJob ? '...' : '$soJobDangTuyen/${dsJob.length}',
                      subtitle: 'Tin tuyển dụng',
                      color: AppColors.primary,
                      icon: Icons.post_add,
                      onTap: null,
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: _SummaryCard(
                      title: 'Ứng viên',
                      value: _dangTaiUngVien ? '...' : '$_tongSoUngVien',
                      subtitle: 'Đơn đã nộp',
                      color: AppColors.warning,
                      icon: Icons.people_alt_outlined,
                      onTap: () => Navigator.push(
                        context,
                        MaterialPageRoute(builder: (_) => const ApplicantListScreen()),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 14),

              // Quick Actions Bar
              Card(
                elevation: 0,
                color: AppColors.surface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                  side: const BorderSide(color: AppColors.border),
                ),
                child: Padding(
                  padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      _actionButton(
                        context,
                        icon: Icons.add_circle_outline,
                        label: 'Đăng tin',
                        onTap: () async {
                          await Navigator.push(
                            context,
                            MaterialPageRoute(builder: (_) => const CreateJobPostScreen()),
                          );
                          _taiDuLieu();
                        },
                      ),
                      _actionButton(
                        context,
                        icon: Icons.assignment_outlined,
                        label: 'Hợp đồng',
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(builder: (_) => const HopDongListScreen()),
                        ),
                      ),
                      _actionButton(
                        context,
                        icon: Icons.account_balance_wallet_outlined,
                        label: 'Ví & Nạp tiền',
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(builder: (_) => const WalletScreen()),
                        ),
                      ),
                      _actionButton(
                        context,
                        icon: Icons.people_outline,
                        label: 'Ứng viên',
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(builder: (_) => const ApplicantListScreen()),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // Tiêu đề danh sách tin tuyển dụng của tôi
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Tin tuyển dụng của tôi',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                  TextButton.icon(
                    onPressed: () async {
                      await Navigator.push(
                        context,
                        MaterialPageRoute(builder: (_) => const CreateJobPostScreen()),
                      );
                      _taiDuLieu();
                    },
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Tạo tin mới'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppColors.primary,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),

              // Danh sách bài đăng thực tế
              if (dangTaiJob && dsJob.isEmpty)
                const Center(
                  child: Padding(
                    padding: EdgeInsets.all(32),
                    child: CircularProgressIndicator(),
                  ),
                )
              else if (dsJob.isEmpty)
                Card(
                  elevation: 0,
                  color: AppColors.surface,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                    side: const BorderSide(color: AppColors.border),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(28),
                    child: Column(
                      children: [
                        const Icon(Icons.work_outline, size: 48, color: Colors.grey),
                        const SizedBox(height: 12),
                        const Text(
                          'Bạn chưa có bài đăng tuyển dụng nào.',
                          style: TextStyle(color: AppColors.bodyText, fontSize: 15),
                        ),
                        const SizedBox(height: 16),
                        FilledButton.icon(
                          onPressed: () async {
                            await Navigator.push(
                              context,
                              MaterialPageRoute(builder: (_) => const CreateJobPostScreen()),
                            );
                            _taiDuLieu();
                          },
                          icon: const Icon(Icons.add),
                          label: const Text('Đăng bài ngay'),
                          style: FilledButton.styleFrom(
                            backgroundColor: AppColors.primary,
                          ),
                        ),
                      ],
                    ),
                  ),
                )
              else
                ...dsJob.map((job) => _EmployerJobItemCard(
                      job: job,
                      onXemUngVien: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => ApplicantListScreen(
                            maJob: job.maJob,
                            tieuDeJob: job.tieude,
                          ),
                        ),
                      ),
                      onDoiTrangThai: () => _xacNhanDoiTrangThai(job),
                      onChiTiet: () => Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => JobDetailScreen(job: job),
                        ),
                      ),
                    )),
            ],
          ),
        ),
      ),
    );
  }

  Widget _actionButton(
    BuildContext context, {
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: AppColors.lightGreenSurface,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, color: AppColors.primary, size: 22),
            ),
            const SizedBox(height: 6),
            Text(
              label,
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: AppColors.heading,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _SummaryCard extends StatelessWidget {
  final String title;
  final String value;
  final String subtitle;
  final Color color;
  final IconData icon;
  final VoidCallback? onTap;

  const _SummaryCard({
    required this.title,
    required this.value,
    required this.subtitle,
    required this.color,
    required this.icon,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(18),
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: AppColors.border),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 13,
                    color: AppColors.bodyText,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                Container(
                  width: 36,
                  height: 36,
                  decoration: BoxDecoration(
                    color: color.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(icon, color: color, size: 20),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: const TextStyle(
                fontSize: 26,
                fontWeight: FontWeight.bold,
                color: AppColors.heading,
              ),
            ),
            const SizedBox(height: 2),
            Text(
              subtitle,
              style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
            ),
          ],
        ),
      ),
    );
  }
}

class _EmployerJobItemCard extends StatelessWidget {
  final JobPostModel job;
  final VoidCallback onXemUngVien;
  final VoidCallback onDoiTrangThai;
  final VoidCallback onChiTiet;

  const _EmployerJobItemCard({
    required this.job,
    required this.onXemUngVien,
    required this.onDoiTrangThai,
    required this.onChiTiet,
  });

  @override
  Widget build(BuildContext context) {
    final dangTuyen = job.status == 'DangTuyen';

    return Container(
      margin: const EdgeInsets.only(bottom: 14),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(
          color: dangTuyen ? AppColors.border : Colors.grey.shade300,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      job.tieude,
                      style: const TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Mã tin: ${job.maJob}',
                      style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                    ),
                  ],
                ),
              ),
              Chip(
                label: Text(
                  dangTuyen ? 'Đang tuyển' : 'Đã đóng',
                  style: TextStyle(
                    color: dangTuyen ? AppColors.success : AppColors.bodyText,
                    fontWeight: FontWeight.bold,
                    fontSize: 12,
                  ),
                ),
                backgroundColor: dangTuyen ? AppColors.lightGreenSurface : Colors.grey.shade200,
                side: BorderSide.none,
                padding: const EdgeInsets.symmetric(horizontal: 4),
              ),
            ],
          ),
          const SizedBox(height: 8),

          Text(
            job.mota,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: AppColors.bodyText, height: 1.4, fontSize: 13),
          ),
          const SizedBox(height: 10),

          // Thông tin thù lao & hạn hoàn thành
          Row(
            children: [
              const Icon(Icons.payments_outlined, size: 16, color: AppColors.primary),
              const SizedBox(width: 6),
              Text(
                AppFormatters.formatCurrency(job.thulao),
                style: const TextStyle(
                  fontWeight: FontWeight.w600,
                  color: AppColors.primary,
                  fontSize: 13,
                ),
              ),
              const SizedBox(width: 14),
              const Icon(Icons.event_outlined, size: 16, color: Colors.grey),
              const SizedBox(width: 6),
              Text(
                'Hạn: ${job.hanHoanThanh.day}/${job.hanHoanThanh.month}/${job.hanHoanThanh.year}',
                style: const TextStyle(color: AppColors.bodyText, fontSize: 12),
              ),
            ],
          ),
          const Divider(height: 20),

          // Các nút hành động
          Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: onXemUngVien,
                  icon: const Icon(Icons.people_outline, size: 16),
                  label: const Text('Ứng viên'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.primary,
                    side: const BorderSide(color: AppColors.primary),
                    padding: const EdgeInsets.symmetric(vertical: 8),
                  ),
                ),
              ),
              const SizedBox(width: 8),
              OutlinedButton(
                onPressed: onDoiTrangThai,
                style: OutlinedButton.styleFrom(
                  foregroundColor: dangTuyen ? AppColors.error : AppColors.success,
                  side: BorderSide(color: dangTuyen ? AppColors.error.withValues(alpha: 0.5) : AppColors.success),
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                ),
                child: Text(dangTuyen ? 'Đóng tin' : 'Mở lại'),
              ),
              const SizedBox(width: 8),
              IconButton(
                tooltip: 'Xem chi tiết',
                icon: const Icon(Icons.arrow_forward_ios, size: 16, color: AppColors.primary),
                onPressed: onChiTiet,
              ),
            ],
          ),
        ],
      ),
    );
  }
}

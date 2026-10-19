import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_ung_tuyen.dart';
import '../../viewmodels/viewmodel_ung_tuyen.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../home/man_hinh_trang_chu.dart';

class MyApplicationsScreen extends StatefulWidget {
  const MyApplicationsScreen({super.key});

  @override
  State<MyApplicationsScreen> createState() => _MyApplicationsScreenState();
}

class _MyApplicationsScreenState extends State<MyApplicationsScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final userId = context.read<AuthViewModel>().currentUser?.userId;
      context.read<ApplicationViewModel>().fetchMyApplications(userId: userId);
    });
  }

  String _statusLabel(String status) {
    final normalized = status.trim();
    if (normalized.isEmpty) return 'Đang chờ duyệt';

    final lower = normalized.toLowerCase();
    if (lower == 'chapnhan' ||
        lower.contains('accept') ||
        lower.contains('approved')) {
      return 'Đã phê duyệt';
    }
    if (lower == 'tuchoi' ||
        lower.contains('reject') ||
        lower.contains('denied')) {
      return 'Bị từ chối';
    }
    return 'Đang chờ duyệt';
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<ApplicationViewModel>();
    final applications = viewModel.applications;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Đơn ứng tuyển của tôi'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () {
            if (Navigator.canPop(context)) {
              Navigator.pop(context);
              return;
            }

            Navigator.pushAndRemoveUntil(
              context,
              MaterialPageRoute(builder: (_) => const HomeScreen()),
              (route) => false,
            );
          },
        ),
      ),
      body: RefreshIndicator(
        onRefresh: () => viewModel.fetchMyApplications(
          userId: context.read<AuthViewModel>().currentUser?.userId,
        ),
        child: Builder(
          builder: (_) {
            if (viewModel.isLoading && applications.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (viewModel.errorMessage != null && applications.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Text(
                    viewModel.errorMessage!,
                    textAlign: TextAlign.center,
                    style: const TextStyle(color: AppColors.error),
                  ),
                ),
              );
            }

            if (applications.isEmpty) {
              return const Center(
                child: Text('Bạn chưa có đơn ứng tuyển nào.', style: TextStyle(color: AppColors.bodyText)),
              );
            }

            return ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: applications.length,
              itemBuilder: (context, index) {
                final item = applications[index];
                return _ApplicationCard(
                  application: item,
                  statusLabel: _statusLabel(item.status),
                );
              },
            );
          },
        ),
      ),
    );
  }
}

class _ApplicationCard extends StatelessWidget {
  final ApplicationModel application;
  final String statusLabel;

  const _ApplicationCard({
    required this.application,
    required this.statusLabel,
  });

  Color _getStatusColor(String label) {
    if (label == 'Đã duyệt' || label == 'Chấp nhận') return AppColors.success;
    if (label == 'Từ chối') return AppColors.error;
    return AppColors.warning;
  }

  @override
  Widget build(BuildContext context) {
    final statusColor = _getStatusColor(statusLabel);
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: ListTile(
        title: Text(
          application.tieude.isEmpty
              ? 'Công việc #${application.jobId}'
              : application.tieude,
          style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 6),
            Text(
              'Trạng thái: $statusLabel',
              style: TextStyle(
                fontWeight: FontWeight.w600,
                color: statusColor,
              ),
            ),
            if (application.coverLetter.isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(
                application.coverLetter,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(color: AppColors.bodyText),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

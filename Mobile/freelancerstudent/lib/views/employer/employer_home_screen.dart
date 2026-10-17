import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../viewmodels/auth_viewmodel.dart';
import '../applications/applicant_list_screen.dart';
import '../auth/login_screen.dart';
import 'create_job_post_screen.dart';

class EmployerHomeScreen extends StatelessWidget {
  const EmployerHomeScreen({super.key});

  Future<void> _handleMenuAction(BuildContext context, String value) async {
    switch (value) {
      case 'new-job':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const CreateJobPostScreen()),
        );
        break;
      case 'applicants':
        await Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => const ApplicantListScreen()),
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF3F9F4),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'FLsv Employer',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: Color(0xFF1D5D3B),
            fontSize: 24,
          ),
        ),
        actions: [
          PopupMenuButton<String>(
            tooltip: 'Tài khoản',
            offset: const Offset(0, 52),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
            icon: const CircleAvatar(
              radius: 18,
              backgroundColor: Color(0xFFBFE7C7),
              child: Icon(Icons.business_center, color: Color(0xFF1D5D3B)),
            ),
            onSelected: (value) async => _handleMenuAction(context, value),
            itemBuilder: (context) => const [
              PopupMenuItem(
                value: 'new-job',
                child: Row(
                  children: [
                    Icon(Icons.post_add, color: Color(0xFF1D5D3B)),
                    SizedBox(width: 10),
                    Text('Đăng bài tuyển dụng'),
                  ],
                ),
              ),
              PopupMenuItem(
                value: 'applicants',
                child: Row(
                  children: [
                    Icon(Icons.people_alt_outlined, color: Color(0xFF1D5D3B)),
                    SizedBox(width: 10),
                    Text('Danh sách ứng viên'),
                  ],
                ),
              ),
              PopupMenuItem(
                value: 'logout',
                child: Row(
                  children: [
                    Icon(Icons.logout, color: Color(0xFF1D5D3B)),
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
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: const [
            Text(
              'Tổng quan tuyển dụng',
              style: TextStyle(
                fontSize: 26,
                fontWeight: FontWeight.bold,
                color: Color(0xFF1D5D3B),
              ),
            ),
            SizedBox(height: 16),
            _SummaryCard(
              title: 'Bài đăng đang hoạt động',
              value: '12',
              color: Color(0xFF2E8B57),
              icon: Icons.post_add,
            ),
            SizedBox(height: 12),
            _SummaryCard(
              title: 'Ứng viên mới',
              value: '48',
              color: Color(0xFFF2C94C),
              icon: Icons.people_alt_outlined,
            ),
            SizedBox(height: 12),
            _SummaryCard(
              title: 'Lượt xem tuần này',
              value: '1.2k',
              color: Color(0xFF5AA9E6),
              icon: Icons.visibility_outlined,
            ),
            SizedBox(height: 18),
            Text(
              'Công việc mới nhất',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Color(0xFF1B3B2A),
              ),
            ),
            SizedBox(height: 12),
            _JobCard(
              title: 'Flutter Developer',
              subtitle: 'Cần 2 freelancer mobile làm app quản lý doanh nghiệp',
              salary: '18 - 25 triệu',
            ),
            SizedBox(height: 12),
            _JobCard(
              title: 'UI/UX Designer',
              subtitle: 'Thiết kế landing page và trải nghiệm người dùng',
              salary: '12 - 15 triệu',
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
  final Color color;
  final IconData icon;

  const _SummaryCard({
    required this.title,
    required this.value,
    required this.color,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: const Color(0xFFDAF0E0)),
      ),
      child: Row(
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.15),
              borderRadius: BorderRadius.circular(14),
            ),
            child: Icon(icon, color: color),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 14,
                    color: Color(0xFF5C7567),
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  value,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF183F2C),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _JobCard extends StatelessWidget {
  final String title;
  final String subtitle;
  final String salary;

  const _JobCard({
    required this.title,
    required this.subtitle,
    required this.salary,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: const Color(0xFFDAF0E0)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  color: const Color(0xFFEAF8EE),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.work_outline, color: Color(0xFF2E8B57)),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF183F2C),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Text(
            subtitle,
            style: const TextStyle(color: Color(0xFF5C7567), height: 1.5),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              const Icon(
                Icons.payments_outlined,
                size: 18,
                color: Color(0xFF2E8B57),
              ),
              const SizedBox(width: 8),
              Text(
                salary,
                style: const TextStyle(
                  fontWeight: FontWeight.w600,
                  color: Color(0xFF1D5D3B),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

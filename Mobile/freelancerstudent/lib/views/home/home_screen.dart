import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/job_post_model.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../../viewmodels/job_viewmodel.dart';
import '../applications/my_applications_screen.dart';
import '../auth/login_screen.dart';
import '../chat/chat_list_screen.dart';
import '../job_detail/apply_job_bottom_sheet.dart';
import '../profile/profile_screen.dart';
import '../student_posts/create_student_post_screen.dart';
import '../wallet/wallet_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<JobViewModel>().fetchJobs();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _searchJobs(String keyword) {
    context.read<JobViewModel>().fetchJobs(keyword: keyword.trim());
  }

  @override
  Widget build(BuildContext context) {
    final authUser = context.watch<AuthViewModel>().currentUser;
    final jobViewModel = context.watch<JobViewModel>();
    final displayName = authUser?.fullName.isNotEmpty == true
        ? authUser!.fullName
        : 'Freelancer Student';
    final displayEmail = authUser?.email ?? 'Chưa có email';
    final jobs = jobViewModel.jobs;

    return Scaffold(
      backgroundColor: const Color(0xFFF3F9F4),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'FLsv',
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
              radius: 20,
              backgroundColor: Color(0xFFE9E9E9),
              child: Icon(Icons.person, color: Colors.black87),
            ),
            onSelected: (value) async => _handleMenuAction(context, value),
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'chat',
                child: Row(
                  children: [
                    Icon(Icons.chat_bubble_outline, color: Color(0xFF1D5D3B)),
                    SizedBox(width: 10),
                    Text('Chat'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'wallet',
                child: Row(
                  children: [
                    Icon(Icons.wallet_outlined, color: Color(0xFF1D5D3B)),
                    SizedBox(width: 10),
                    Text('Ví'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'post',
                child: Row(
                  children: [
                    Icon(Icons.post_add, color: Color(0xFF1D5D3B)),
                    SizedBox(width: 10),
                    Text('Đăng bài tìm việc'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'applications',
                child: Row(
                  children: [
                    Icon(Icons.assignment_outlined, color: Color(0xFF1D5D3B)),
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
                      color: Color(0xFF1D5D3B),
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
          children: [
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(18),
                border: Border.all(color: const Color(0xFFDAF0E0)),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Xin chào,',
                    style: TextStyle(fontSize: 14, color: Color(0xFF5C7567)),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    displayName,
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF183F2C),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    displayEmail,
                    style: const TextStyle(
                      color: Color(0xFF5C7567),
                      fontSize: 13,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            const Text(
              'Tìm kiếm công việc',
              style: TextStyle(
                fontSize: 26,
                fontWeight: FontWeight.bold,
                color: Color(0xFF1D5D3B),
              ),
            ),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(18),
                border: Border.all(color: const Color(0xFFDAF0E0)),
                boxShadow: [
                  BoxShadow(
                    color: const Color(0xFFBFE7C7).withValues(alpha: 0.25),
                    blurRadius: 12,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: TextField(
                controller: _searchController,
                onChanged: _searchJobs,
                decoration: const InputDecoration(
                  border: InputBorder.none,
                  hintText: 'Nhập từ khóa công việc...',
                  prefixIcon: Icon(Icons.search, color: Color(0xFF2E8B57)),
                ),
              ),
            ),
            const SizedBox(height: 22),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: const [
                Text(
                  'Việc làm phù hợp',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF1B3B2A),
                  ),
                ),
                Text(
                  'Xem tất cả',
                  style: TextStyle(
                    color: Color(0xFF2E8B57),
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            if (jobViewModel.isLoading && jobs.isEmpty)
              const Center(
                child: Padding(
                  padding: EdgeInsets.all(24),
                  child: CircularProgressIndicator(),
                ),
              )
            else if (jobs.isEmpty)
              const Padding(
                padding: EdgeInsets.symmetric(vertical: 24),
                child: Center(
                  child: Text(
                    'Chưa có công việc nào được đăng.',
                    style: TextStyle(color: Color(0xFF5C7567)),
                  ),
                ),
              )
            else
              ...jobs.map(
                (job) => Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: _JobCard(
                    job: job,
                    onApply: () {
                      showModalBottomSheet(
                        context: context,
                        isScrollControlled: true,
                        builder: (_) => ApplyJobBottomSheet(job: job),
                      );
                    },
                  ),
                ),
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

class _JobCard extends StatelessWidget {
  final JobPostModel job;
  final VoidCallback onApply;

  const _JobCard({required this.job, required this.onApply});

  @override
  Widget build(BuildContext context) {
    final location = job.kynangyeucau.isEmpty ? 'Online' : 'Remote';
    final type = job.status == 'DangTuyen' ? 'Đang tuyển' : 'Đã đóng';

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
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: const Color(0xFFDFF6E5),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: const Icon(Icons.work_outline, color: Color(0xFF2E8B57)),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      job.title,
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF183F2C),
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      job.kynangyeucau.isEmpty
                          ? 'Công ty tuyển dụng'
                          : job.kynangyeucau,
                      style: const TextStyle(color: Color(0xFF5C7567)),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              _InfoChip(icon: Icons.location_on_outlined, label: location),
              const SizedBox(width: 8),
              _InfoChip(icon: Icons.work_history_outlined, label: type),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                '${job.budget.toStringAsFixed(0)} VNĐ',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF1D5D3B),
                ),
              ),
              TextButton(
                onPressed: onApply,
                style: TextButton.styleFrom(
                  backgroundColor: const Color(0xFFBFE7C7),
                  foregroundColor: const Color(0xFF1D5D3B),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
                child: const Text('Ứng tuyển'),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _InfoChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: const Color(0xFFEAF8EE),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: const Color(0xFF2E8B57)),
          const SizedBox(width: 6),
          Text(
            label,
            style: const TextStyle(fontSize: 12, color: Color(0xFF355B48)),
          ),
        ],
      ),
    );
  }
}

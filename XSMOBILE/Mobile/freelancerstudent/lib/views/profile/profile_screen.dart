import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/api_endpoints.dart';
import '../../models/freelancer_profile_model.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../../viewmodels/freelancer_profile_viewmodel.dart';
import 'edit_profile_screen.dart';

class ProfileScreen extends StatefulWidget {
  final int? freelancerId;
  final bool readOnly;

  const ProfileScreen({super.key, this.freelancerId, this.readOnly = false});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final viewModel = context.read<FreelancerProfileViewModel>();
      if (widget.freelancerId != null) {
        viewModel.loadProfileById(widget.freelancerId!);
        return;
      }
      final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
      if (userId > 0) viewModel.loadProfile(userId);
    });
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<FreelancerProfileViewModel>();
    final profile = viewModel.profile;

    if (viewModel.isLoading && profile == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    if (viewModel.errorMessage != null && profile == null) {
      return Scaffold(
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 48, color: Colors.red),
              const SizedBox(height: 12),
              Text(viewModel.errorMessage ?? 'Không thể tải hồ sơ'),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: () {
                  final profileViewModel = context
                      .read<FreelancerProfileViewModel>();
                  if (widget.freelancerId != null) {
                    profileViewModel.loadProfileById(widget.freelancerId!);
                  } else {
                    final currentUserId =
                        context.read<AuthViewModel>().currentUser?.userId ?? 0;
                    if (currentUserId > 0) {
                      profileViewModel.loadProfile(currentUserId);
                    }
                  }
                },
                child: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      );
    }

    final authUser = context.read<AuthViewModel>().currentUser;
    final canEdit =
        widget.freelancerId == null &&
        !widget.readOnly &&
        authUser?.role == 'FreelancerStudent' &&
        profile?.maFreelancerStudents != null &&
        profile!.maFreelancerStudents > 0;
    final data =
        profile ??
        FreelancerProfileModel(
          maUser: authUser?.userId ?? 0,
          hoTen: authUser?.fullName.isNotEmpty == true
              ? authUser!.fullName
              : 'Sinh viên',
          email: authUser?.email ?? '',
          maFreelancerStudents: 0,
          namThu: 0,
          gpa: 0,
          nienKhoa: '',
          trangThaiNhanViec: true,
          skills: const [],
          projects: const [],
          proofs: const [],
          jobPosts: const [],
        );

    return DefaultTabController(
      length: 4,
      child: Scaffold(
        backgroundColor: const Color(0xFFF3F9F4),
        appBar: AppBar(
          backgroundColor: Colors.white,
          elevation: 0,
          title: Text(
            canEdit ? 'Hồ sơ năng lực' : 'Hồ sơ Freelancer',
            style: TextStyle(
              color: Color(0xFF1D5D3B),
              fontWeight: FontWeight.bold,
            ),
          ),
          actions: [
            if (canEdit)
              IconButton(
                onPressed: () async {
                  final profileViewModel = context
                      .read<FreelancerProfileViewModel>();
                  final saved = await Navigator.push<bool>(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const EditProfileScreen(),
                    ),
                  );
                  if (!mounted || saved != true) return;
                  final id = profileViewModel.profile?.maFreelancerStudents;
                  if (id != null) profileViewModel.loadProfileById(id);
                },
                icon: const Icon(
                  Icons.edit_note_rounded,
                  color: Color(0xFF2E8B57),
                ),
              ),
          ],
          bottom: const TabBar(
            isScrollable: true,
            indicatorColor: Color(0xFF2E8B57),
            labelColor: Color(0xFF1D5D3B),
            unselectedLabelColor: Color(0xFF6D8A7A),
            tabs: [
              Tab(text: 'Tổng quan'),
              Tab(text: 'Giới thiệu'),
              Tab(text: 'Portfolio'),
              Tab(text: 'Minh chứng'),
            ],
          ),
        ),
        body: SafeArea(
          child: TabBarView(
            children: [
              _ProfileOverviewTab(profile: data),
              _ProfileIntroTab(profile: data),
              _ProfilePortfolioTab(
                profile: data,
                onAddProject: canEdit ? () => _addPortfolioProject(data) : null,
              ),
              _ProfileProofTab(profile: data),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _addPortfolioProject(FreelancerProfileModel profile) async {
    final formKey = GlobalKey<FormState>();
    final name = TextEditingController();
    final role = TextEditingController();
    final description = TextEditingController();
    final technologies = TextEditingController();
    final github = TextEditingController();
    final demo = TextEditingController();

    final project = await showDialog<Map<String, String?>>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Thêm dự án portfolio'),
        content: Form(
          key: formKey,
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                _projectField(name, 'Tên dự án', required: true),
                _projectField(role, 'Vai trò'),
                _projectField(description, 'Mô tả', maxLines: 3),
                _projectField(technologies, 'Công nghệ'),
                _projectField(github, 'Link GitHub'),
                _projectField(demo, 'Link Demo'),
              ],
            ),
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () {
              if (!formKey.currentState!.validate()) return;
              Navigator.pop(dialogContext, {
                'name': name.text.trim(),
                'role': role.text.trim(),
                'description': description.text.trim(),
                'technologies': technologies.text.trim(),
                'github': github.text.trim(),
                'demo': demo.text.trim(),
              });
            },
            child: const Text('Thêm'),
          ),
        ],
      ),
    );
    name.dispose();
    role.dispose();
    description.dispose();
    technologies.dispose();
    github.dispose();
    demo.dispose();
    if (project == null || !mounted) return;

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.addPortfolioProject(
      freelancerId: profile.maFreelancerStudents,
      projectName: project['name']!,
      role: project['role'],
      description: project['description'],
      technologies: project['technologies'],
      githubUrl: project['github'],
      demoUrl: project['demo'],
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Đã thêm dự án portfolio.'
              : viewModel.errorMessage ?? 'Không thể thêm dự án.',
        ),
      ),
    );
  }

  Widget _projectField(
    TextEditingController controller,
    String label, {
    bool required = false,
    int maxLines = 1,
  }) => Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: TextFormField(
      controller: controller,
      maxLines: maxLines,
      validator: required
          ? (value) => value == null || value.trim().isEmpty
                ? '$label không được để trống.'
                : null
          : null,
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
      ),
    ),
  );
}

class _ProfileOverviewTab extends StatelessWidget {
  final FreelancerProfileModel profile;

  const _ProfileOverviewTab({required this.profile});

  @override
  Widget build(BuildContext context) {
    final birth = profile.ngaySinh != null
        ? DateFormat('dd/MM/yyyy').format(profile.ngaySinh!)
        : 'Chưa cập nhật';
    final statusText = profile.trangThaiNhanViec
        ? 'Đang nhận việc'
        : 'Tạm ngưng';
    final authUser = context.read<AuthViewModel>().currentUser;
    final displayName = profile.hoTen.trim().isNotEmpty
        ? profile.hoTen
        : (authUser?.fullName.isNotEmpty == true
              ? authUser!.fullName
              : 'Sinh viên');
    final subtitle =
        profile.tenChuyenNganh != null &&
            profile.tenChuyenNganh!.trim().isNotEmpty
        ? profile.tenChuyenNganh!
        : 'Họ tên: $displayName';

    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          Container(
            padding: const EdgeInsets.all(18),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              border: Border.all(color: const Color(0xFFDAF0E0)),
            ),
            child: Row(
              children: [
                CircleAvatar(
                  radius: 32,
                  backgroundColor: const Color(0xFFE8F5E9),
                  backgroundImage:
                      (profile.avatarUrl?.isNotEmpty == true ||
                          authUser?.avatarUrl?.isNotEmpty == true)
                      ? NetworkImage(
                          ApiEndpoints.dinhDangUrlAnh(
                            profile.avatarUrl ?? authUser?.avatarUrl,
                            tenMacDinh: displayName,
                          ),
                        )
                      : null,
                  onBackgroundImageError:
                      (profile.avatarUrl?.isNotEmpty == true ||
                          authUser?.avatarUrl?.isNotEmpty == true)
                      ? (context, error) {}
                      : null,
                  child:
                      (profile.avatarUrl == null ||
                              profile.avatarUrl!.isEmpty) &&
                          (authUser?.avatarUrl == null ||
                              authUser!.avatarUrl!.isEmpty)
                      ? const Icon(
                          Icons.person,
                          size: 34,
                          color: Color(0xFF1D5D3B),
                        )
                      : null,
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        displayName,
                        style: const TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF183F2C),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        subtitle,
                        style: const TextStyle(
                          color: Color(0xFF5E7569),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 10,
                          vertical: 6,
                        ),
                        decoration: BoxDecoration(
                          color: const Color(0xFFEAF8EE),
                          borderRadius: BorderRadius.circular(999),
                        ),
                        child: Text(
                          statusText,
                          style: const TextStyle(
                            color: Color(0xFF1D5D3B),
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          _ProfileInfoRow(label: 'Email', value: profile.email),
          _ProfileInfoRow(
            label: 'Số điện thoại',
            value: profile.sdt ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(label: 'Ngày sinh', value: birth),
          _ProfileInfoRow(
            label: 'Trường',
            value: profile.tenTruong ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(
            label: 'Địa điểm',
            value: profile.diaDiemFreelancerStudent ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(
            label: 'Chuyên ngành',
            value: profile.tenChuyenNganh ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(label: 'Năm thứ', value: '${profile.namThu}'),
          _ProfileInfoRow(label: 'GPA', value: profile.gpa.toStringAsFixed(1)),
          _ProfileInfoRow(label: 'Niên khóa', value: profile.nienKhoa),
          _ProfileInfoRow(
            label: 'Chi phí từ',
            value: profile.chiPhiTu != null
                ? '${NumberFormat('#,###').format(profile.chiPhiTu)}đ'
                : 'Chưa cập nhật',
          ),
        ],
      ),
    );
  }
}

class _ProfileIntroTab extends StatelessWidget {
  final FreelancerProfileModel profile;

  const _ProfileIntroTab({required this.profile});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          Container(
            padding: const EdgeInsets.all(18),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: const Color(0xFFDAF0E0)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Giới thiệu bản thân',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF183F2C),
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  profile.gioiThieu ?? 'Sinh viên chưa cập nhật giới thiệu.',
                  style: const TextStyle(height: 1.7, color: Color(0xFF355B48)),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          _ProfileInfoRow(
            label: 'Ngoại ngữ',
            value: profile.ngonNgu ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(
            label: 'Kỹ năng cơ bản',
            value: profile.kyNangCoBan ?? 'Chưa cập nhật',
          ),
          _ProfileInfoRow(
            label: 'Kỹ năng chuyên môn',
            value: profile.skills.isEmpty
                ? 'Chưa cập nhật'
                : profile.skills.join(', '),
          ),
        ],
      ),
    );
  }
}

class _ProfilePortfolioTab extends StatelessWidget {
  final FreelancerProfileModel profile;
  final VoidCallback? onAddProject;

  const _ProfilePortfolioTab({required this.profile, this.onAddProject});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          if (profile.projects.isEmpty)
            const _EmptyState(title: 'Chưa có portfolio')
          else
            ...profile.projects.map(
              (project) => _PortfolioItem(
                name: project.tenDuAn,
                detail: [
                  project.moTa ?? 'Không có mô tả',
                  if (project.vaiTro != null) 'Vai trò: ${project.vaiTro}',
                  if (project.congNghe != null)
                    'Công nghệ: ${project.congNghe}',
                  if (project.linkGithub != null)
                    'GitHub: ${project.linkGithub}',
                  if (project.linkDemo != null) 'Demo: ${project.linkDemo}',
                ].join('\n'),
              ),
            ),
          const SizedBox(height: 12),
          if (onAddProject != null)
            SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: onAddProject,
                icon: const Icon(Icons.add),
                label: const Text('Thêm dự án'),
                style: FilledButton.styleFrom(
                  backgroundColor: const Color(0xFF7BCB9A),
                  foregroundColor: const Color(0xFF0F2E1A),
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _ProfileProofTab extends StatelessWidget {
  final FreelancerProfileModel profile;

  const _ProfileProofTab({required this.profile});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          if (profile.proofs.isEmpty)
            const _EmptyState(title: 'Chưa có minh chứng')
          else
            ...profile.proofs.map(
              (proof) => Container(
                margin: const EdgeInsets.only(bottom: 12),
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(14),
                  color: Colors.white,
                  border: Border.all(color: const Color(0xFFDAF0E0)),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      width: 42,
                      height: 42,
                      decoration: BoxDecoration(
                        color: const Color(0xFFEAF8EE),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: const Icon(
                        Icons.fact_check,
                        color: Color(0xFF2E8B57),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            proof.loaiMinhChung,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF183F2C),
                            ),
                          ),
                          const SizedBox(height: 6),
                          Text(
                            proof.trangThaiGuiMinhChung ?? 'Chưa cập nhật',
                            style: const TextStyle(color: Color(0xFF5E7569)),
                          ),
                          if (proof.fileMinhChung != null) ...[
                            const SizedBox(height: 6),
                            Text(
                              proof.fileMinhChung!,
                              style: const TextStyle(color: Color(0xFF2E8B57)),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _ProfileInfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _ProfileInfoRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFDAF0E0)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 118,
            child: Text(
              label,
              style: const TextStyle(
                color: Color(0xFF6C8277),
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                color: Color(0xFF183F2C),
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _PortfolioItem extends StatelessWidget {
  final String name;
  final String detail;

  const _PortfolioItem({required this.name, required this.detail});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFDAF0E0)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: const Color(0xFFEAF8EE),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(Icons.folder_open, color: Color(0xFF2E8B57)),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  name,
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF183F2C),
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  detail,
                  style: const TextStyle(color: Color(0xFF5E7569), height: 1.5),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  final String title;

  const _EmptyState({required this.title});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFDAF0E0)),
      ),
      child: Center(
        child: Text(
          title,
          style: const TextStyle(
            color: Color(0xFF5E7569),
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/diem_cuoi_api.dart';
import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../models/mo_hinh_ho_so_freelancer.dart';
import '../../services/dich_vu_danh_gia.dart';
import '../../viewmodels/viewmodel_ho_so_freelancer.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_yeu_thich.dart';
import '../danh_gia/man_hinh_danh_gia.dart';
import '../danh_gia/man_hinh_danh_sach_tat_ca_danh_gia.dart';
import '../student_posts/man_hinh_quan_ly_bai_dang_sinh_vien.dart';
import 'man_hinh_chinh_sua_ho_so.dart';
import 'man_hinh_minh_chung_the_sv.dart';

typedef FreelancerProfileScreen = ProfileScreen;

class ProfileScreen extends StatefulWidget {
  final int? freelancerId;
  final int? maFreelancerStudents;
  final bool readOnly;

  const ProfileScreen({
    super.key,
    this.freelancerId,
    this.maFreelancerStudents,
    this.readOnly = false,
  });

  int? get effectiveFreelancerId => freelancerId ?? maFreelancerStudents;

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _lamMoi();
    });
  }

  Future<void> _lamMoi() async {
    final viewModel = context.read<FreelancerProfileViewModel>();
    final targetId = widget.effectiveFreelancerId;
    if (targetId != null && targetId > 0) {
      await viewModel.loadProfileById(targetId);
      if (!mounted) return;
      final authUser = context.read<AuthViewModel>().currentUser;
      if (authUser != null && authUser.role == 'NhaTuyenDung') {
        context.read<YeuThichViewModel>().kiemTraLuuFreelancer(
              authUser.userId,
              targetId,
            );
      }
      return;
    }
    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (userId > 0) await viewModel.loadProfile(userId);
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
      length: 5,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: AppColors.surface,
          elevation: 0.5,
          title: Text(
            canEdit ? 'Hồ sơ năng lực' : 'Hồ sơ Freelancer',
            style: const TextStyle(
              color: AppColors.heading,
              fontWeight: FontWeight.bold,
            ),
          ),
          actions: [
            if (authUser?.role == 'NhaTuyenDung' &&
                (widget.effectiveFreelancerId != null ||
                    (profile?.maFreelancerStudents != null &&
                        profile!.maFreelancerStudents > 0)))
              Consumer<YeuThichViewModel>(
                builder: (context, yeuThichVM, _) {
                  final targetFLId = widget.effectiveFreelancerId ??
                      profile?.maFreelancerStudents ??
                      0;
                  final daLuu = yeuThichVM.daLuuFreelancer(targetFLId);
                  return IconButton(
                    tooltip: daLuu ? 'Bỏ yêu thích' : 'Lưu Freelancer yêu thích',
                    icon: Icon(
                      daLuu ? Icons.favorite : Icons.favorite_border,
                      color: daLuu ? Colors.red : AppColors.heading,
                    ),
                    onPressed: () async {
                      if (authUser == null || targetFLId <= 0) return;
                      final ketQua = await yeuThichVM.toggleLuuFreelancer(
                          authUser.userId, targetFLId);
                      if (context.mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text(ketQua
                                ? 'Đã lưu ứng viên vào danh sách yêu thích'
                                : 'Đã bỏ yêu thích ứng viên'),
                            backgroundColor: AppColors.primary,
                            duration: const Duration(seconds: 1),
                          ),
                        );
                      }
                    },
                  );
                },
              ),
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
                  color: AppColors.primary,
                ),
              ),
          ],
          bottom: const TabBar(
            isScrollable: true,
            indicatorColor: AppColors.primary,
            labelColor: AppColors.primary,
            unselectedLabelColor: AppColors.bodyText,
            tabs: [
              Tab(text: 'Tổng quan'),
              Tab(text: 'Giới thiệu'),
              Tab(text: 'Portfolio'),
              Tab(text: 'Minh chứng'),
              Tab(text: 'Đánh giá'),
            ],
          ),
        ),
        body: SafeArea(
          child: RefreshIndicator(
            onRefresh: _lamMoi,
            child: TabBarView(
              children: [
                _ProfileOverviewTab(
                  profile: data,
                  canEdit: canEdit,
                ),
                _ProfileIntroTab(profile: data),
                _ProfilePortfolioTab(
                  profile: data,
                  canEdit: canEdit,
                  onAddProject: canEdit ? () => _addPortfolioProject(data) : null,
                  onEditPortfolioInfo: canEdit ? () => _editPortfolioInfo(data) : null,
                  onEditProject: canEdit ? (proj) => _editPortfolioProject(data, proj) : null,
                  onDeleteProject: canEdit ? (proj) => _deletePortfolioProject(data, proj) : null,
                  onToggleFeatured: canEdit ? (proj) => _toggleFeaturedProject(data, proj) : null,
                ),
                _ProfileProofTab(
                  profile: data,
                  canEdit: canEdit,
                  onRefresh: _lamMoi,
                ),
                _ProfileReviewTab(
                  profile: data,
                  canEdit: canEdit,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _editPortfolioInfo(FreelancerProfileModel profile) async {
    final formKey = GlobalKey<FormState>();
    final moTaCtrl = TextEditingController(text: profile.moTaPortfolio ?? '');
    final videoCtrl = TextEditingController(text: profile.urlVideoPortfolio ?? '');

    final result = await showDialog<Map<String, String?>>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Giới thiệu & Video Portfolio'),
        content: Form(
          key: formKey,
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                _projectField(
                  moTaCtrl,
                  'Mô tả tổng quan Portfolio',
                  maxLines: 4,
                ),
                _projectField(
                  videoCtrl,
                  'Link Video giới thiệu (YouTube / Drive)',
                ),
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
              Navigator.pop(dialogContext, {
                'moTa': moTaCtrl.text.trim(),
                'video': videoCtrl.text.trim(),
              });
            },
            child: const Text('Lưu cập nhật'),
          ),
        ],
      ),
    );
    moTaCtrl.dispose();
    videoCtrl.dispose();
    if (result == null || !mounted) return;

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.updatePortfolioInfo(
      freelancerId: profile.maFreelancerStudents,
      moTaBanThan: result['moTa'],
      urlVideo: result['video'],
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Đã cập nhật thông tin Portfolio thành công.'
              : viewModel.errorMessage ?? 'Không thể cập nhật Portfolio.',
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

  Future<void> _editPortfolioProject(
    FreelancerProfileModel profile,
    PortfolioProjectModel item,
  ) async {
    final formKey = GlobalKey<FormState>();
    final name = TextEditingController(text: item.tenDuAn);
    final role = TextEditingController(text: item.vaiTro ?? '');
    final description = TextEditingController(text: item.moTa ?? '');
    final technologies = TextEditingController(text: item.congNghe ?? '');
    final github = TextEditingController(text: item.linkGithub ?? '');
    final demo = TextEditingController(text: item.linkDemo ?? '');
    final linkFile = TextEditingController(text: item.linkFile ?? '');

    final project = await showDialog<Map<String, String?>>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Chỉnh sửa dự án portfolio'),
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
                _projectField(linkFile, 'Link tài liệu đính kèm'),
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
                'linkFile': linkFile.text.trim(),
              });
            },
            child: const Text('Lưu thay đổi'),
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
    linkFile.dispose();
    if (project == null || !mounted) return;

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.updatePortfolioProject(
      freelancerId: profile.maFreelancerStudents,
      maDA: item.maDA ?? '',
      projectName: project['name']!,
      role: project['role'],
      description: project['description'],
      technologies: project['technologies'],
      githubUrl: project['github'],
      demoUrl: project['demo'],
      fileUrl: project['linkFile'],
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Đã cập nhật dự án portfolio thành công.'
              : viewModel.errorMessage ?? 'Không thể cập nhật dự án.',
        ),
      ),
    );
  }

  Future<void> _deletePortfolioProject(
    FreelancerProfileModel profile,
    PortfolioProjectModel item,
  ) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận xóa dự án'),
        content: Text('Bạn có chắc chắn muốn xóa dự án "${item.tenDuAn}" khỏi Portfolio?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xóa dự án'),
          ),
        ],
      ),
    );
    if (confirm != true || !mounted) return;

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.deletePortfolioProject(
      freelancerId: profile.maFreelancerStudents,
      maDA: item.maDA ?? '',
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Đã xóa dự án khỏi Portfolio.'
              : viewModel.errorMessage ?? 'Không thể xóa dự án.',
        ),
      ),
    );
  }

  Future<void> _toggleFeaturedProject(
    FreelancerProfileModel profile,
    PortfolioProjectModel item,
  ) async {
    final targetId = (item.maDA ?? '').trim();
    if (targetId.isEmpty) return;

    final currentFeaturedIds = profile.projects
        .where((p) => p.laDuAnNoiBat && (p.maDA ?? '').trim().isNotEmpty)
        .map((p) => p.maDA!.trim())
        .toList();

    final isCurrentlyFeatured = currentFeaturedIds.contains(targetId);
    if (isCurrentlyFeatured) {
      currentFeaturedIds.remove(targetId);
    } else {
      if (currentFeaturedIds.length >= 2) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Chỉ được chọn tối đa 2 dự án nổi bật! Hãy bỏ chọn 1 dự án trước.'),
          ),
        );
        return;
      }
      currentFeaturedIds.add(targetId);
    }

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.updateFeaturedProjects(
      freelancerId: profile.maFreelancerStudents,
      maDAs: currentFeaturedIds,
    );
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? (isCurrentlyFeatured
                  ? 'Đã bỏ đánh dấu dự án nổi bật.'
                  : 'Đã chọn làm dự án nổi bật!')
              : viewModel.errorMessage ?? 'Không thể cập nhật dự án nổi bật.',
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
  final bool canEdit;

  const _ProfileOverviewTab({
    required this.profile,
    this.canEdit = false,
  });

  Widget _buildVerificationPill(FreelancerProfileModel profile) {
    final bool daXacMinh = profile.daXacMinh;
    final bool hetHan = profile.hetHanNienKhoa;
    final String trangThai = profile.trangThaiXacMinh;

    Color color = AppColors.mutedText;
    String label = 'Chưa xác thực';

    if (hetHan) {
      color = AppColors.error;
      label = 'Hết hạn niên khóa';
    } else if (daXacMinh || trangThai == 'Đã xác minh') {
      color = AppColors.primary;
      label = 'Đã xác minh SV';
    } else if (trangThai == 'Đang gửi') {
      color = Colors.orange.shade800;
      label = 'Chờ duyệt thẻ SV';
    } else if (trangThai == 'Đã từ chối') {
      color = AppColors.error;
      label = 'Thẻ SV bị từ chối';
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        border: Border.all(color: color),
      ),
      child: Text(
        label,
        style: TextStyle(
          color: color,
          fontWeight: FontWeight.bold,
          fontSize: 12,
        ),
      ),
    );
  }

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
          if (profile.hetHanNienKhoa)
            Container(
              margin: const EdgeInsets.only(bottom: 14),
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: AppColors.error.withValues(alpha: 0.1),
                border: Border.all(color: AppColors.error),
              ),
              child: const Text(
                'MINH CHỨNG XÁC NHẬN SINH VIÊN ĐÃ KHÔNG CÒN HỢP LỆ, VÌ NIÊN KHÓA CỦA BẠN ĐÃ VƯỢT QUA THỜI GIAN CHO PHÉP',
                style: TextStyle(
                  color: AppColors.error,
                  fontWeight: FontWeight.bold,
                  fontSize: 12,
                ),
              ),
            ),
          Container(
            padding: const EdgeInsets.all(18),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(20),
              border: Border.all(color: AppColors.border),
            ),
            child: Row(
              children: [
                CircleAvatar(
                  radius: 32,
                  backgroundColor: AppColors.lightGreenSurface,
                  backgroundImage: (profile.avatarUrl?.isNotEmpty == true ||
                          authUser?.avatarUrl?.isNotEmpty == true)
                      ? NetworkImage(
                          ApiEndpoints.dinhDangUrlAnh(
                            profile.avatarUrl ?? authUser?.avatarUrl,
                            tenMacDinh: displayName,
                          ),
                        )
                      : null,
                  onBackgroundImageError: (profile.avatarUrl?.isNotEmpty ==
                              true ||
                          authUser?.avatarUrl?.isNotEmpty == true)
                      ? (context, error) {}
                      : null,
                  child: (profile.avatarUrl == null ||
                              profile.avatarUrl!.isEmpty) &&
                          (authUser?.avatarUrl == null ||
                              authUser!.avatarUrl!.isEmpty)
                      ? const Icon(
                          Icons.person,
                          size: 34,
                          color: AppColors.primary,
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
                          color: AppColors.heading,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        subtitle,
                        style: const TextStyle(
                          color: AppColors.bodyText,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Wrap(
                        spacing: 6,
                        runSpacing: 6,
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 10,
                              vertical: 4,
                            ),
                            decoration: const BoxDecoration(
                              color: AppColors.lightGreenSurface,
                            ),
                            child: Text(
                              statusText,
                              style: const TextStyle(
                                color: AppColors.primary,
                                fontWeight: FontWeight.w600,
                                fontSize: 12,
                              ),
                            ),
                          ),
                          _buildVerificationPill(profile),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          if (canEdit) ...[
            const SizedBox(height: 14),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => const QuanLyBaiDangSinhVienScreen(),
                        ),
                      );
                    },
                    icon: const Icon(Icons.post_add_outlined, size: 18),
                    label: const Text(
                      'Bài đăng tìm việc',
                      style: TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
                    ),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.primary,
                      side: const BorderSide(color: AppColors.primary),
                      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => const DanhGiaListScreen(),
                        ),
                      );
                    },
                    icon: const Icon(Icons.rate_review_outlined, size: 18),
                    label: const Text(
                      'Đánh giá đối tác',
                      style: TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
                    ),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.primary,
                      side: const BorderSide(color: AppColors.primary),
                      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ],
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
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Giới thiệu bản thân',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppColors.heading,
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  profile.gioiThieu ?? 'Sinh viên chưa cập nhật giới thiệu.',
                  style: const TextStyle(height: 1.7, color: AppColors.bodyText),
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
  final bool canEdit;
  final VoidCallback? onAddProject;
  final VoidCallback? onEditPortfolioInfo;
  final void Function(PortfolioProjectModel project)? onEditProject;
  final void Function(PortfolioProjectModel project)? onDeleteProject;
  final void Function(PortfolioProjectModel project)? onToggleFeatured;

  const _ProfilePortfolioTab({
    required this.profile,
    this.canEdit = false,
    this.onAddProject,
    this.onEditPortfolioInfo,
    this.onEditProject,
    this.onDeleteProject,
    this.onToggleFeatured,
  });

  @override
  Widget build(BuildContext context) {
    final featuredCount = profile.projects.where((p) => p.laDuAnNoiBat).length;

    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          // Khối giới thiệu & Video Portfolio
          Container(
            margin: const EdgeInsets.only(bottom: 16),
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Row(
                      children: [
                        Icon(Icons.auto_awesome_mosaic_outlined, color: AppColors.primary, size: 20),
                        SizedBox(width: 8),
                        Text(
                          'Tổng quan Portfolio',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: AppColors.heading,
                          ),
                        ),
                      ],
                    ),
                    if (canEdit && onEditPortfolioInfo != null)
                      TextButton.icon(
                        onPressed: onEditPortfolioInfo,
                        icon: const Icon(Icons.edit_outlined, size: 16),
                        label: const Text('Cập nhật'),
                      ),
                  ],
                ),
                const SizedBox(height: 8),
                Text(
                  (profile.moTaPortfolio != null && profile.moTaPortfolio!.trim().isNotEmpty)
                      ? profile.moTaPortfolio!
                      : 'Chưa có mô tả tổng quan cho Portfolio.',
                  style: const TextStyle(color: AppColors.bodyText, height: 1.5, fontSize: 13),
                ),
                if (profile.urlVideoPortfolio != null &&
                    profile.urlVideoPortfolio!.trim().isNotEmpty) ...[
                  const SizedBox(height: 10),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    decoration: BoxDecoration(
                      color: AppColors.lightGreenSurface,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.play_circle_outline, color: AppColors.primary, size: 18),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Video giới thiệu: ${profile.urlVideoPortfolio}',
                            style: const TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.w600,
                              color: AppColors.primaryDark,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
                if (canEdit) ...[
                  const SizedBox(height: 8),
                  Text(
                    'Dự án nổi bật đã chọn: $featuredCount/2 dự án',
                    style: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: AppColors.mutedText,
                    ),
                  ),
                ],
              ],
            ),
          ),

          if (profile.projects.isEmpty)
            const _EmptyState(title: 'Chưa có dự án nào trong Portfolio')
          else
            ...profile.projects.map(
              (project) => _PortfolioItem(
                project: project,
                canEdit: canEdit,
                onEdit: onEditProject != null ? () => onEditProject!(project) : null,
                onDelete: onDeleteProject != null ? () => onDeleteProject!(project) : null,
                onToggleFeatured:
                    onToggleFeatured != null ? () => onToggleFeatured!(project) : null,
              ),
            ),
          const SizedBox(height: 12),
          if (onAddProject != null)
            SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: onAddProject,
                icon: const Icon(Icons.add),
                label: const Text('Thêm dự án mới'),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  foregroundColor: Colors.white,
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
  final bool canEdit;
  final VoidCallback? onRefresh;

  const _ProfileProofTab({
    required this.profile,
    this.canEdit = false,
    this.onRefresh,
  });

  void _xemAnhLon(BuildContext context, String url) {
    showDialog(
      context: context,
      builder: (ctx) => Dialog(
        backgroundColor: Colors.transparent,
        insetPadding: const EdgeInsets.all(12),
        child: Stack(
          alignment: Alignment.topRight,
          children: [
            InteractiveViewer(
              child: Image.network(
                url,
                fit: BoxFit.contain,
                loadingBuilder: (context, child, progress) {
                  if (progress == null) return child;
                  return Container(
                    height: 250,
                    color: Colors.black54,
                    child: const Center(
                      child: CircularProgressIndicator(color: Colors.white),
                    ),
                  );
                },
                errorBuilder: (_, _, _) => Container(
                  padding: const EdgeInsets.all(20),
                  color: Colors.white,
                  child: const Text('Không thể tải hình ảnh minh chứng gốc.'),
                ),
              ),
            ),
            IconButton(
              icon: const Icon(Icons.close, color: Colors.white, size: 28),
              onPressed: () => Navigator.pop(ctx),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final proof = profile.minhChungChinh;
    final bool hasProof = proof != null;

    final String trangThai = proof?.trangThaiGuiMinhChung ?? 'Chưa gửi';
    Color statusColor = AppColors.mutedText;
    String statusLabel = 'Chưa xác thực';

    if (proof?.hetHanNienKhoa == true) {
      statusColor = AppColors.error;
      statusLabel = 'Hết hạn niên khóa';
    } else if (trangThai == 'Đã xác minh' || proof?.daXacMinh == true) {
      statusColor = AppColors.primary;
      statusLabel = 'Đã xác minh';
    } else if (trangThai == 'Đang gửi') {
      statusColor = Colors.orange.shade800;
      statusLabel = 'Đang chờ duyệt';
    } else if (trangThai == 'Đã từ chối') {
      statusColor = AppColors.error;
      statusLabel = 'Đã từ chối';
    }

    final String? fileUrl = proof?.fileMinhChung?.isNotEmpty == true
        ? ApiEndpoints.dinhDangUrlAnh(proof!.fileMinhChung)
        : null;

    return Padding(
      padding: const EdgeInsets.all(16),
      child: ListView(
        children: [
          // Nút Quét / Nộp lại thẻ (CHỈ DÀNH CHO CHÍNH CHỦ FREELANCERSTUDENT)
          if (canEdit) ...[
            SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: () async {
                  await Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const ManHinhMinhChungTheSV(),
                    ),
                  );
                  onRefresh?.call();
                },
                icon: const Icon(Icons.document_scanner),
                label: Text(
                  hasProof ? 'Cập nhật lại Thẻ Sinh Viên (OCR)' : 'Quét Thẻ Sinh Viên (OCR)',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: const RoundedRectangleBorder(
                    borderRadius: BorderRadius.zero,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 16),
          ],

          // Thông báo hết hạn niên khóa nếu có
          if (proof?.hetHanNienKhoa == true)
            Container(
              margin: const EdgeInsets.only(bottom: 14),
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: AppColors.error.withValues(alpha: 0.1),
                border: Border.all(color: AppColors.error),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'CẢNH BÁO NIÊN KHÓA HẾT HẠN',
                    style: TextStyle(
                      color: AppColors.error,
                      fontWeight: FontWeight.bold,
                      fontSize: 13,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    proof?.thongBao ??
                        'MINH CHỨNG XÁC NHẬN SINH VIÊN ĐÃ KHÔNG CÒN HỢP LỆ, VÌ NIÊN KHÓA CỦA BẠN ĐÃ VƯỢT QUA THỜI GIAN CHO PHÉP',
                    style: const TextStyle(color: AppColors.error, fontSize: 12),
                  ),
                ],
              ),
            ),

          if (!hasProof)
            _EmptyState(
              title: canEdit
                  ? 'Bạn chưa nộp minh chứng Thẻ sinh viên'
                  : 'Freelancer chưa gửi minh chứng Thẻ sinh viên',
            )
          else ...[
            // Card chi tiết thông tin xác minh
            Container(
              margin: const EdgeInsets.only(bottom: 16),
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: AppColors.surface,
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        proof.loaiMinhChung,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppColors.heading,
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: statusColor.withValues(alpha: 0.12),
                          border: Border.all(color: statusColor),
                        ),
                        child: Text(
                          statusLabel,
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: statusColor,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const Divider(height: 20),
                  _dongChiTiet('Mã sinh viên:', proof.mssv ?? 'Đã xác minh theo thẻ'),
                  _dongChiTiet('Họ và tên:', profile.hoTen),
                  _dongChiTiet('Trường ĐH/CĐ:', profile.tenTruong ?? 'Chưa cập nhật'),
                  _dongChiTiet('Chuyên ngành:', profile.tenChuyenNganh ?? 'Chưa cập nhật'),
                  _dongChiTiet('Niên khóa:', proof.nienKhoa ?? profile.nienKhoa),
                  _dongChiTiet('Năm thứ:', 'Năm ${profile.namThu}'),
                  if (proof.ngayNop != null)
                    _dongChiTiet(
                      'Ngày gửi minh chứng:',
                      DateFormat('dd/MM/yyyy HH:mm').format(proof.ngayNop!),
                    ),
                  if (proof.ngayXacMinh != null)
                    _dongChiTiet(
                      'Ngày Ban quản trị duyệt:',
                      DateFormat('dd/MM/yyyy HH:mm').format(proof.ngayXacMinh!),
                    ),
                  if (canEdit && proof.lydoTuChoi != null && proof.lydoTuChoi!.trim().isNotEmpty) ...[
                    const SizedBox(height: 8),
                    Container(
                      width: double.infinity,
                      padding: const EdgeInsets.all(10),
                      color: AppColors.error.withValues(alpha: 0.08),
                      child: Text(
                        'Lý do từ chối: ${proof.lydoTuChoi}',
                        style: const TextStyle(color: AppColors.error, fontSize: 12),
                      ),
                    ),
                  ],
                ],
              ),
            ),

            // Card Hình ảnh Thẻ sinh viên
            if (fileUrl != null)
              Container(
                margin: const EdgeInsets.only(bottom: 16),
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Hình ảnh Thẻ sinh viên',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 4),
                    const Text(
                      'Bấm vào hình ảnh để xem kích thước lớn.',
                      style: TextStyle(fontSize: 11, color: AppColors.mutedText),
                    ),
                    const SizedBox(height: 12),
                    GestureDetector(
                      onTap: () => _xemAnhLon(context, fileUrl),
                      child: Container(
                        width: double.infinity,
                        constraints: const BoxConstraints(maxHeight: 220),
                        decoration: BoxDecoration(
                          color: AppColors.background,
                          border: Border.all(color: AppColors.border),
                        ),
                        child: Image.network(
                          fileUrl,
                          fit: BoxFit.contain,
                          loadingBuilder: (context, child, progress) {
                            if (progress == null) return child;
                            return const Center(
                              child: Padding(
                                padding: EdgeInsets.all(24),
                                child: CircularProgressIndicator(),
                              ),
                            );
                          },
                          errorBuilder: (context, error, stackTrace) => const Padding(
                            padding: EdgeInsets.all(24),
                            child: Center(
                              child: Text(
                                'Không thể tải ảnh thẻ sinh viên',
                                style: TextStyle(fontSize: 12, color: AppColors.mutedText),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
          ],
        ],
      ),
    );
  }

  Widget _dongChiTiet(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 140,
            child: Text(
              label,
              style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: AppColors.heading,
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
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 118,
            child: Text(
              label,
              style: const TextStyle(
                color: AppColors.bodyText,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                color: AppColors.heading,
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
  final PortfolioProjectModel project;
  final bool canEdit;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final VoidCallback? onToggleFeatured;

  const _PortfolioItem({
    required this.project,
    this.canEdit = false,
    this.onEdit,
    this.onDelete,
    this.onToggleFeatured,
  });

  @override
  Widget build(BuildContext context) {
    final detail = [
      project.moTa ?? 'Không có mô tả',
      if (project.vaiTro != null && project.vaiTro!.isNotEmpty)
        'Vai trò: ${project.vaiTro}',
      if (project.congNghe != null && project.congNghe!.isNotEmpty)
        'Công nghệ: ${project.congNghe}',
      if (project.linkGithub != null && project.linkGithub!.isNotEmpty)
        'GitHub: ${project.linkGithub}',
      if (project.linkDemo != null && project.linkDemo!.isNotEmpty)
        'Demo: ${project.linkDemo}',
      if (project.linkFile != null && project.linkFile!.isNotEmpty)
        'Tài liệu: ${project.linkFile}',
    ].join('\n');

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: project.laDuAnNoiBat ? Colors.amber.shade700 : AppColors.border,
          width: project.laDuAnNoiBat ? 1.5 : 1.0,
        ),
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
                  color: project.laDuAnNoiBat
                      ? Colors.amber.withValues(alpha: 0.15)
                      : AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(
                  project.laDuAnNoiBat ? Icons.star_rounded : Icons.folder_open,
                  color: project.laDuAnNoiBat ? Colors.amber.shade800 : AppColors.primary,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            project.tenDuAn,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 15,
                              color: AppColors.heading,
                            ),
                          ),
                        ),
                        if (project.laDuAnNoiBat)
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                            decoration: BoxDecoration(
                              color: Colors.amber.withValues(alpha: 0.16),
                              borderRadius: BorderRadius.circular(6),
                              border: Border.all(color: Colors.amber.shade700),
                            ),
                            child: Text(
                              '★ Dự án nổi bật',
                              style: TextStyle(
                                fontSize: 11,
                                fontWeight: FontWeight.bold,
                                color: Colors.amber.shade900,
                              ),
                            ),
                          ),
                      ],
                    ),
                    const SizedBox(height: 6),
                    Text(
                      detail,
                      style: const TextStyle(color: AppColors.bodyText, height: 1.5),
                    ),
                  ],
                ),
              ),
            ],
          ),
          if (canEdit && (project.maDA ?? '').trim().isNotEmpty) ...[
            const Divider(height: 18),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton.icon(
                  onPressed: onToggleFeatured,
                  icon: Icon(
                    project.laDuAnNoiBat ? Icons.star : Icons.star_border,
                    size: 16,
                    color: Colors.amber.shade800,
                  ),
                  label: Text(
                    project.laDuAnNoiBat ? 'Bỏ nổi bật' : 'Chọn nổi bật',
                    style: TextStyle(
                      fontSize: 12,
                      color: Colors.amber.shade900,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
                const SizedBox(width: 4),
                TextButton.icon(
                  onPressed: onEdit,
                  icon: const Icon(Icons.edit_outlined, size: 16),
                  label: const Text('Sửa', style: TextStyle(fontSize: 12)),
                ),
                const SizedBox(width: 4),
                TextButton.icon(
                  onPressed: onDelete,
                  icon: const Icon(Icons.delete_outline, size: 16, color: AppColors.error),
                  label: const Text(
                    'Xóa',
                    style: TextStyle(fontSize: 12, color: AppColors.error),
                  ),
                ),
              ],
            ),
          ],
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
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: Center(
        child: Text(
          title,
          style: const TextStyle(
            color: AppColors.bodyText,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }
}

class _ProfileReviewTab extends StatefulWidget {
  final FreelancerProfileModel profile;
  final bool canEdit;

  const _ProfileReviewTab({
    required this.profile,
    this.canEdit = false,
  });

  @override
  State<_ProfileReviewTab> createState() => _ProfileReviewTabState();
}

class _ProfileReviewTabState extends State<_ProfileReviewTab> {
  final DanhGiaService _service = DanhGiaService();
  bool _loading = true;
  String? _error;
  ThongKeDanhGiaModel? _thongKe;

  @override
  void initState() {
    super.initState();
    _loadReviews();
  }

  Future<void> _loadReviews() async {
    setState(() {
      _loading = true;
      _error = null;
    });

    final res = await _service.layDanhGiaFreelancer(
      maFreelancerStudent: widget.profile.maFreelancerStudents,
      trang: 1,
      kichThuocTrang: 10,
    );

    if (mounted) {
      setState(() {
        _loading = false;
        if (res.isSuccess) {
          _thongKe = res.data;
        } else {
          _error = res.message;
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null && _thongKe == null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                _error!,
                textAlign: TextAlign.center,
                style: const TextStyle(color: AppColors.error),
              ),
              const SizedBox(height: 12),
              OutlinedButton(
                onPressed: _loadReviews,
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

    final stats = _thongKe;
    final total = stats?.tongLuotDanhGia ?? 0;

    if (total == 0) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.star_outline, size: 54, color: AppColors.mutedText),
              const SizedBox(height: 12),
              const Text(
                'Chưa có đánh giá nào từ nhà tuyển dụng.',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 6),
              const Text(
                'Đánh giá sẽ hiển thị khi bạn hoàn thành các hợp đồng trên nền tảng.',
                textAlign: TextAlign.center,
                style: TextStyle(color: AppColors.bodyText, fontSize: 13),
              ),
              if (widget.canEdit) ...[
                const SizedBox(height: 16),
                FilledButton.icon(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const DanhGiaListScreen(),
                      ),
                    );
                  },
                  icon: const Icon(Icons.rate_review_outlined),
                  label: const Text('Công việc đã hoàn thành & Đánh giá đối tác'),
                  style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
                ),
              ],
            ],
          ),
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadReviews,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (widget.canEdit) ...[
            Container(
              margin: const EdgeInsets.only(bottom: 16),
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const DanhGiaListScreen(),
                    ),
                  );
                },
                icon: const Icon(Icons.rate_review_outlined),
                label: const Text('Công việc đã hoàn thành & Đánh giá đối tác'),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
            ),
          ],
          // Thống kê tổng quan
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      stats!.diemTrungBinh.toStringAsFixed(1),
                      style: const TextStyle(
                        fontSize: 38,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: List.generate(
                            5,
                            (index) => Icon(
                              index < stats.diemTrungBinh.round()
                                  ? Icons.star
                                  : Icons.star_border,
                              size: 20,
                              color: Colors.amber,
                            ),
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          '$total lượt đánh giá',
                          style: const TextStyle(
                            color: AppColors.mutedText,
                            fontSize: 13,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
                const Divider(height: 24),
                // Các tiêu chí chi tiết
                Row(
                  children: [
                    _buildCriterionScore('Chất lượng', stats.trungBinhChatLuong),
                    _buildCriterionScore('Đúng hạn', stats.trungBinhDungHan),
                    _buildCriterionScore('Giao tiếp', stats.trungBinhGiaoTiep),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Đánh giá gần đây (${stats.danhSach.length}/$total)',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              if (total > stats.danhSach.length)
                TextButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ManHinhDanhSachTatCaDanhGia(
                          maFreelancerStudent: widget.profile.maFreelancerStudents,
                          tenDoiTuong: widget.profile.hoTen,
                          laNhaTuyenDung: false,
                        ),
                      ),
                    );
                  },
                  child: const Text('Xem tất cả'),
                ),
            ],
          ),
          const SizedBox(height: 8),
          ...stats.danhSach.map((item) => _buildReviewItem(item)),
          if (total > stats.danhSach.length)
            Padding(
              padding: const EdgeInsets.only(top: 12, bottom: 20),
              child: OutlinedButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ManHinhDanhSachTatCaDanhGia(
                        maFreelancerStudent: widget.profile.maFreelancerStudents,
                        tenDoiTuong: widget.profile.hoTen,
                        laNhaTuyenDung: false,
                      ),
                    ),
                  );
                },
                style: OutlinedButton.styleFrom(
                  shape: const RoundedRectangleBorder(
                    borderRadius: BorderRadius.zero,
                  ),
                  side: const BorderSide(color: AppColors.primary),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
                child: Text('Xem toàn bộ $total đánh giá'),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildCriterionScore(String label, double score) {
    return Expanded(
      child: Column(
        children: [
          Text(
            score > 0 ? score.toStringAsFixed(1) : '-',
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: AppColors.primary,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            label,
            style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
          ),
        ],
      ),
    );
  }

  Widget _buildReviewItem(DanhGiaModel item) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 16,
                backgroundColor: AppColors.lightGreenSurface,
                child: Text(
                  item.tenNguoiDanhGia.isNotEmpty
                      ? item.tenNguoiDanhGia.characters.first.toUpperCase()
                      : 'N',
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.tenNguoiDanhGia,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                    Text(
                      '${item.tenCongViec} · ${item.ngayDanhGia != null ? DateFormat('dd/MM/yyyy').format(item.ngayDanhGia!) : ''}',
                      style: const TextStyle(
                        color: AppColors.mutedText,
                        fontSize: 12,
                      ),
                    ),
                  ],
                ),
              ),
              Row(
                children: [
                  const Icon(Icons.star, size: 16, color: Colors.amber),
                  const SizedBox(width: 2),
                  Text(
                    '${item.soSaoTongThe}',
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 14,
                    ),
                  ),
                ],
              ),
            ],
          ),
          if ((item.nhanXet ?? '').trim().isNotEmpty) ...[
            const SizedBox(height: 10),
            Text(
              item.nhanXet!,
              style: const TextStyle(
                color: AppColors.bodyText,
                fontSize: 13,
                height: 1.4,
              ),
            ),
          ],
        ],
      ),
    );
  }
}

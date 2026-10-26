import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../viewmodels/viewmodel_bai_dang_sinh_vien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import 'man_hinh_chi_tiet_bai_dang_sinh_vien.dart';
import 'man_hinh_tao_bai_dang_sinh_vien.dart';

class StudentPostListScreen extends StatefulWidget {
  const StudentPostListScreen({super.key});

  @override
  State<StudentPostListScreen> createState() => _StudentPostListScreenState();
}

class _StudentPostListScreenState extends State<StudentPostListScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<StudentPostViewModel>().taiDanhSachBaiDangTimViec();
    });
  }

  Future<void> _lamMoi() =>
      context.read<StudentPostViewModel>().taiDanhSachBaiDangTimViec();

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<StudentPostViewModel>();
    final authVM = context.watch<AuthViewModel>();
    final currentUser = authVM.currentUser;
    final isStudent = currentUser != null && !currentUser.isEmployer;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Bài đăng tìm việc'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      floatingActionButton: isStudent
          ? FloatingActionButton.extended(
              onPressed: () async {
                await Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => const CreateStudentPostScreen(),
                  ),
                );
                _lamMoi();
              },
              backgroundColor: AppColors.primary,
              icon: const Icon(Icons.add, color: Colors.white),
              label: const Text('Đăng tin', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
            )
          : null,
      body: RefreshIndicator(
        onRefresh: _lamMoi,
        child: Builder(
          builder: (context) {
            if (vm.dangTai && vm.danhSachBaiDang.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (vm.thongBaoLoi != null && vm.danhSachBaiDang.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(vm.thongBaoLoi!, textAlign: TextAlign.center),
                      const SizedBox(height: 12),
                      FilledButton(
                        onPressed: _lamMoi,
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            if (vm.danhSachBaiDang.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Icon(Icons.assignment_outlined, size: 64, color: AppColors.mutedText),
                      const SizedBox(height: 16),
                      const Text(
                        'Chưa có bài đăng tìm việc nào.',
                        style: TextStyle(fontSize: 16, color: AppColors.mutedText),
                      ),
                      if (isStudent) ...[
                        const SizedBox(height: 16),
                        FilledButton.icon(
                          onPressed: () => Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => const CreateStudentPostScreen(),
                            ),
                          ),
                          icon: const Icon(Icons.add),
                          label: const Text('Tạo bài đăng đầu tiên'),
                          style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
                        ),
                      ],
                    ],
                  ),
                ),
              );
            }

            return ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: vm.danhSachBaiDang.length,
              separatorBuilder: (_, _) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final item = vm.danhSachBaiDang[index];
                return Card(
                  elevation: 1,
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                  child: InkWell(
                    borderRadius: BorderRadius.circular(16),
                    onTap: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => StudentPostDetailScreen(post: item),
                      ),
                    ),
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              CircleAvatar(
                                radius: 22,
                                backgroundImage: NetworkImage(item.hinhAnhHienThi),
                                backgroundColor: AppColors.lightGreenSurface,
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      item.tenSinhVien.isNotEmpty ? item.tenSinhVien : 'Sinh viên',
                                      style: const TextStyle(
                                        fontSize: 15,
                                        fontWeight: FontWeight.bold,
                                        color: AppColors.heading,
                                      ),
                                    ),
                                    if (item.tenTruong != null && item.tenTruong!.isNotEmpty)
                                      Text(
                                        item.tenTruong!,
                                        style: const TextStyle(
                                          fontSize: 12,
                                          color: AppColors.mutedText,
                                        ),
                                        maxLines: 1,
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                  ],
                                ),
                              ),
                              Container(
                                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                decoration: BoxDecoration(
                                  color: AppColors.lightGreenSurface,
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                child: Text(
                                  AppFormatters.formatCurrency(item.mucGiaTu),
                                  style: const TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold,
                                    color: AppColors.primary,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const Divider(height: 20),
                          Text(
                            item.tieuDe,
                            style: const TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
                          ),
                          const SizedBox(height: 6),
                          Text(
                            item.moTa,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
                          ),
                          if (item.kyNang.isNotEmpty) ...[
                            const SizedBox(height: 10),
                            Wrap(
                              spacing: 6,
                              runSpacing: 4,
                              children: item.kyNang
                                  .split(RegExp(r'[,;]'))
                                  .take(3)
                                  .map((s) => s.trim())
                                  .where((s) => s.isNotEmpty)
                                  .map((tag) => Container(
                                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                                        decoration: BoxDecoration(
                                          color: Colors.grey.withAlpha(25),
                                          borderRadius: BorderRadius.circular(4),
                                        ),
                                        child: Text(tag, style: const TextStyle(fontSize: 11, color: AppColors.bodyText)),
                                      ))
                                  .toList(),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ),
                );
              },
            );
          },
        ),
      ),
    );
  }
}

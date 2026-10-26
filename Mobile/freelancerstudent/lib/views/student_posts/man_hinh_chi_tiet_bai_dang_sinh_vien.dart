import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_bai_dang_sinh_vien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';

class StudentPostDetailScreen extends StatelessWidget {
  final StudentPostModel post;

  const StudentPostDetailScreen({super.key, required this.post});

  @override
  Widget build(BuildContext context) {
    final authVM = context.watch<AuthViewModel>();
    final currentUser = authVM.currentUser;
    final isNTD = currentUser?.isEmployer == true;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Chi tiết tìm việc'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Thẻ hồ sơ sinh viên
            Card(
              elevation: 1,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    CircleAvatar(
                      radius: 32,
                      backgroundImage: NetworkImage(post.hinhAnhHienThi),
                      backgroundColor: AppColors.lightGreenSurface,
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            post.tenSinhVien.isNotEmpty ? post.tenSinhVien : 'Sinh viên',
                            style: const TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
                          ),
                          const SizedBox(height: 4),
                          if (post.tenTruong != null && post.tenTruong!.isNotEmpty)
                            Text(
                              post.tenTruong!,
                              style: const TextStyle(
                                fontSize: 13,
                                color: AppColors.bodyText,
                              ),
                            ),
                          if (post.tenChuyenNganh != null && post.tenChuyenNganh!.isNotEmpty)
                            Text(
                              'Chuyên ngành: ${post.tenChuyenNganh}',
                              style: const TextStyle(
                                fontSize: 12,
                                color: AppColors.mutedText,
                              ),
                            ),
                          const SizedBox(height: 4),
                          Row(
                            children: [
                              if (post.namThu > 0)
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                  margin: const EdgeInsets.only(right: 6),
                                  decoration: BoxDecoration(
                                    color: Colors.blue.withAlpha(25),
                                    borderRadius: BorderRadius.circular(4),
                                  ),
                                  child: Text(
                                    'Năm ${post.namThu}',
                                    style: const TextStyle(fontSize: 11, color: Colors.blue, fontWeight: FontWeight.bold),
                                  ),
                                ),
                              if (post.gpa > 0)
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                  decoration: BoxDecoration(
                                    color: Colors.orange.withAlpha(25),
                                    borderRadius: BorderRadius.circular(4),
                                  ),
                                  child: Text(
                                    'GPA: ${post.gpa.toStringAsFixed(1)}',
                                    style: const TextStyle(fontSize: 11, color: Colors.orange, fontWeight: FontWeight.bold),
                                  ),
                                ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // Tiêu đề & Mức lương
            Card(
              elevation: 1,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      post.tieuDe,
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        const Icon(Icons.attach_money, color: AppColors.primary, size: 20),
                        const SizedBox(width: 4),
                        Text(
                          'Mức thù lao mong muốn: ${AppFormatters.formatCurrency(post.mucGiaTu)}',
                          style: const TextStyle(
                            fontSize: 15,
                            fontWeight: FontWeight.bold,
                            color: AppColors.primary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // Kỹ năng
            Card(
              elevation: 1,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Kỹ năng chuyên môn',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: post.kyNang
                          .split(RegExp(r'[,;]'))
                          .map((s) => s.trim())
                          .where((s) => s.isNotEmpty)
                          .map((tag) => Chip(
                                label: Text(tag),
                                backgroundColor: AppColors.lightGreenSurface,
                                labelStyle: const TextStyle(color: AppColors.primary, fontSize: 13),
                                side: BorderSide.none,
                              ))
                          .toList(),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // Mô tả chi tiết
            Card(
              elevation: 1,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Mô tả bản thân & Kinh nghiệm',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Text(
                      post.moTa.isNotEmpty ? post.moTa : 'Chưa có mô tả chi tiết.',
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.bodyText,
                        height: 1.5,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),

            // Nút hành động nếu là Nhà tuyển dụng
            if (isNTD && post.maUser > 0)
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ChatDetailScreen(
                          receiverId: post.maUser.toString(),
                          receiverName: post.tenSinhVien.isNotEmpty ? post.tenSinhVien : 'Sinh viên',
                        ),
                      ),
                    );
                  },
                  icon: const Icon(Icons.chat_bubble_outline),
                  label: const Text('Liên hệ / Phỏng vấn ngay'),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

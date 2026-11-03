import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_freelancer_yeu_thich.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_yeu_thich.dart';
import '../profile/man_hinh_ho_so.dart';

class ManHinhFreelancerYeuThich extends StatefulWidget {
  const ManHinhFreelancerYeuThich({super.key});

  @override
  State<ManHinhFreelancerYeuThich> createState() =>
      _ManHinhFreelancerYeuThichState();
}

class _ManHinhFreelancerYeuThichState extends State<ManHinhFreelancerYeuThich> {
  final currencyFormatter = NumberFormat('#,###', 'vi_VN');

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiDanhSach();
    });
  }

  void _taiDanhSach() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context
          .read<YeuThichViewModel>()
          .taiDanhSachFreelancerYeuThich(user.userId);
    }
  }

  Future<void> _xacNhanBoLuu(FreelancerYeuThichModel item) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
        title: const Text('Bỏ Freelancer yêu thích?',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
        content: Text(
            'Bạn có chắc chắn muốn bỏ lưu ứng viên "${item.hoTen}" khỏi danh sách yêu thích không?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy', style: TextStyle(color: Colors.grey)),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red[700],
              foregroundColor: Colors.white,
              shape:
                  const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
            ),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Bỏ lưu'),
          ),
        ],
      ),
    );

    if (xacNhan == true && mounted) {
      final vm = context.read<YeuThichViewModel>();
      await vm.toggleLuuFreelancer(user.userId, item.maFreelancerStudent);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Đã bỏ yêu thích ứng viên'),
            backgroundColor: AppColors.primary,
            duration: Duration(seconds: 2),
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF7F9FA),
      appBar: AppBar(
        title: const Text(
          'Freelancer yêu thích',
          style: TextStyle(
              fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: Consumer<YeuThichViewModel>(
        builder: (context, vm, child) {
          if (vm.dangTai && vm.danhSachFreelancerYeuThich.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }

          if (vm.thongBaoLoi != null &&
              vm.danhSachFreelancerYeuThich.isEmpty) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline,
                        size: 48, color: Colors.red),
                    const SizedBox(height: 12),
                    Text(
                      vm.thongBaoLoi!,
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: Colors.black87),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero),
                      ),
                      onPressed: _taiDanhSach,
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (vm.danhSachFreelancerYeuThich.isEmpty) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(32.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Container(
                      width: 80,
                      height: 80,
                      decoration: BoxDecoration(
                        color: Colors.grey[200],
                        shape: BoxShape.rectangle,
                      ),
                      child: Icon(Icons.favorite_border,
                          size: 44, color: Colors.grey[500]),
                    ),
                    const SizedBox(height: 16),
                    const Text(
                      'Chưa có Freelancer yêu thích',
                      style: TextStyle(
                          fontSize: 17,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF1E293B)),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Bạn có thể lưu các ứng viên sinh viên xuất sắc vào danh sách này để liên hệ và mời hợp tác bất cứ lúc nào.',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                          fontSize: 13.5, color: Colors.grey[600], height: 1.4),
                    ),
                    const SizedBox(height: 20),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero),
                        padding: const EdgeInsets.symmetric(
                            horizontal: 24, vertical: 12),
                      ),
                      onPressed: () => Navigator.pop(context),
                      child: const Text('Tìm kiếm ứng viên ngay'),
                    ),
                  ],
                ),
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () async => _taiDanhSach(),
            child: ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: vm.danhSachFreelancerYeuThich.length,
              separatorBuilder: (context, index) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final fl = vm.danhSachFreelancerYeuThich[index];
                return _buildFreelancerCard(fl);
              },
            ),
          );
        },
      ),
    );
  }

  Widget _buildFreelancerCard(FreelancerYeuThichModel fl) {
    final avatarUrl = fl.avatar.isNotEmpty
        ? fl.avatar
        : 'https://ui-avatars.com/api/?name=${Uri.encodeComponent(fl.hoTen)}&background=E8F5E9&color=1B5E36';

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: const Color(0xFFE2E8F0)),
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              ClipRRect(
                borderRadius: BorderRadius.zero,
                child: Image.network(
                  avatarUrl,
                  width: 52,
                  height: 52,
                  fit: BoxFit.cover,
                  errorBuilder: (ctx, err, stack) => Container(
                    width: 52,
                    height: 52,
                    color: Colors.grey[300],
                    child: const Icon(Icons.person, color: Colors.grey),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    InkWell(
                      onTap: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (ctx) => FreelancerProfileScreen(
                              maFreelancerStudents: fl.maFreelancerStudent,
                            ),
                          ),
                        );
                      },
                      child: Text(
                        fl.hoTen,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF0F172A),
                        ),
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      fl.chuyenNganh,
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                        color: AppColors.primary,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      '${fl.tenTruong} • Năm ${fl.namThu}',
                      style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                    ),
                  ],
                ),
              ),
              IconButton(
                icon: const Icon(Icons.favorite, color: Colors.red),
                tooltip: 'Bỏ yêu thích',
                onPressed: () => _xacNhanBoLuu(fl),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Container(
            padding: const EdgeInsets.all(10),
            color: const Color(0xFFF8FAFC),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                Column(
                  children: [
                    Text('Đánh giá',
                        style:
                            TextStyle(fontSize: 11, color: Colors.grey[600])),
                    const SizedBox(height: 2),
                    Row(
                      children: [
                        const Icon(Icons.star,
                            size: 14, color: Colors.amber),
                        const SizedBox(width: 2),
                        Text(
                          fl.soLuotDanhGia > 0
                              ? '${fl.diemDanhGiaTrungBinh.toStringAsFixed(1)}/5.0'
                              : 'Chưa có',
                          style: const TextStyle(
                              fontSize: 12.5, fontWeight: FontWeight.bold),
                        ),
                      ],
                    ),
                  ],
                ),
                Container(height: 24, width: 1, color: Colors.grey[300]),
                Column(
                  children: [
                    Text('Điểm GPA',
                        style:
                            TextStyle(fontSize: 11, color: Colors.grey[600])),
                    const SizedBox(height: 2),
                    Text(
                      fl.gpa > 0
                          ? '${fl.gpa.toStringAsFixed(2)}/4.0'
                          : 'Chưa có',
                      style: const TextStyle(
                          fontSize: 12.5, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
                Container(height: 24, width: 1, color: Colors.grey[300]),
                Column(
                  children: [
                    Text('Dự án đã làm',
                        style:
                            TextStyle(fontSize: 11, color: Colors.grey[600])),
                    const SizedBox(height: 2),
                    Text(
                      '${fl.soDuAnHoanThanh}',
                      style: const TextStyle(
                          fontSize: 12.5, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
              ],
            ),
          ),
          if (fl.kyNangCoBan.isNotEmpty) ...[
            const SizedBox(height: 10),
            Wrap(
              spacing: 6,
              runSpacing: 4,
              children: fl.kyNangCoBan
                  .split(RegExp(r'[,;]'))
                  .map((s) => s.trim())
                  .where((s) => s.isNotEmpty)
                  .take(4)
                  .map(
                    (skill) => Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: const Color(0xFFF1F5F9),
                        border: Border.all(color: const Color(0xFFE2E8F0)),
                      ),
                      child: Text(
                        skill,
                        style: const TextStyle(
                            fontSize: 11.5, color: Color(0xFF334155)),
                      ),
                    ),
                  )
                  .toList(),
            ),
          ],
          const Divider(height: 20, thickness: 1, color: Color(0xFFF1F5F9)),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                fl.chiPhiTu != null && fl.chiPhiTu! > 0
                    ? 'Thù lao từ: ${currencyFormatter.format(fl.chiPhiTu)} đ'
                    : 'Thù lao thỏa thuận',
                style: const TextStyle(
                    fontSize: 12.5,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFFDC2626)),
              ),
              ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  foregroundColor: Colors.white,
                  elevation: 0,
                  shape: const RoundedRectangleBorder(
                      borderRadius: BorderRadius.zero),
                  padding:
                      const EdgeInsets.symmetric(horizontal: 14, vertical: 6),
                ),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (ctx) => FreelancerProfileScreen(
                        maFreelancerStudents: fl.maFreelancerStudent,
                      ),
                    ),
                  );
                },
                child: const Text('Xem hồ sơ',
                    style:
                        TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
              ),
            ],
          ),
        ],
      ),
    );
  }
}


import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_luu_viec.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_yeu_thich.dart';
import 'man_hinh_chi_tiet_cong_viec.dart';

class ManHinhCongViecDaLuu extends StatefulWidget {
  const ManHinhCongViecDaLuu({super.key});

  @override
  State<ManHinhCongViecDaLuu> createState() => _ManHinhCongViecDaLuuState();
}

class _ManHinhCongViecDaLuuState extends State<ManHinhCongViecDaLuu> {
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
      context.read<YeuThichViewModel>().taiDanhSachViecDaLuu(user.userId);
    }
  }

  Future<void> _xacNhanBoLuu(CongViecDaLuuModel item) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
        title: const Text('Bỏ lưu công việc?',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
        content: Text('Bạn có chắc chắn muốn bỏ lưu công việc "${item.tieuDe}" không?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy', style: TextStyle(color: Colors.grey)),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red[700],
              foregroundColor: Colors.white,
              shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
            ),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Bỏ lưu'),
          ),
        ],
      ),
    );

    if (xacNhan == true && mounted) {
      final vm = context.read<YeuThichViewModel>();
      await vm.toggleLuuViec(user.userId, item.maJob);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Đã bỏ lưu công việc thành công'),
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
          'Công việc đã lưu',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: Consumer<YeuThichViewModel>(
        builder: (context, vm, child) {
          if (vm.dangTai && vm.danhSachViecDaLuu.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }

          if (vm.thongBaoLoi != null && vm.danhSachViecDaLuu.isEmpty) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline, size: 48, color: Colors.red),
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
                        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                      ),
                      onPressed: _taiDanhSach,
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (vm.danhSachViecDaLuu.isEmpty) {
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
                      child: Icon(Icons.bookmark_border, size: 44, color: Colors.grey[500]),
                    ),
                    const SizedBox(height: 16),
                    const Text(
                      'Bạn chưa lưu công việc nào',
                      style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: Color(0xFF1E293B)),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Hãy duyệt danh sách công việc và nhấn biểu tượng bookmark để lưu lại xem sau nhé.',
                      textAlign: TextAlign.center,
                      style: TextStyle(fontSize: 13.5, color: Colors.grey[600], height: 1.4),
                    ),
                    const SizedBox(height: 20),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                        padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                      ),
                      onPressed: () => Navigator.pop(context),
                      child: const Text('Khám phá công việc ngay'),
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
              itemCount: vm.danhSachViecDaLuu.length,
              separatorBuilder: (context, index) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final job = vm.danhSachViecDaLuu[index];
                return _buildJobCard(job);
              },
            ),
          );
        },
      ),
    );
  }

  Widget _buildJobCard(CongViecDaLuuModel job) {
    final String statusText = job.daTuyenDu
        ? 'Đã tuyển đủ'
        : (job.status.toLowerCase() == 'closed' ? 'Đã đóng tuyển' : 'Đang tuyển');
    final Color statusColor = job.daTuyenDu || job.status.toLowerCase() == 'closed'
        ? Colors.grey
        : AppColors.primary;

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
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      job.tenNhaTuyenDung,
                      style: TextStyle(
                        fontSize: 12.5,
                        fontWeight: FontWeight.w600,
                        color: Colors.grey[700],
                      ),
                    ),
                    const SizedBox(height: 4),
                    InkWell(
                      onTap: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (ctx) => JobDetailScreen(maJob: job.maJob),
                          ),
                        );
                      },
                      child: Text(
                        job.tieuDe,
                        style: const TextStyle(
                          fontSize: 15.5,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF0F172A),
                          height: 1.3,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              IconButton(
                icon: const Icon(Icons.bookmark, color: AppColors.primary),
                tooltip: 'Bỏ lưu việc',
                onPressed: () => _xacNhanBoLuu(job),
              ),
            ],
          ),
          const SizedBox(height: 8),
          if (job.moTa.isNotEmpty)
            Text(
              job.moTa,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(fontSize: 13, color: Colors.grey[600], height: 1.4),
            ),
          const SizedBox(height: 12),
          Row(
            children: [
              Text(
                job.thuLao > 0
                    ? '${currencyFormatter.format(job.thuLao)} VNĐ'
                    : 'Thỏa thuận',
                style: const TextStyle(
                  fontSize: 14.5,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFFDC2626),
                ),
              ),
              const Spacer(),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.1),
                  border: Border.all(color: statusColor.withValues(alpha: 0.4)),
                ),
                child: Text(
                  statusText,
                  style: TextStyle(
                    fontSize: 11.5,
                    fontWeight: FontWeight.w600,
                    color: statusColor,
                  ),
                ),
              ),
            ],
          ),
          const Divider(height: 20, thickness: 1, color: Color(0xFFF1F5F9)),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                job.ngayLuu != null
                    ? 'Đã lưu: ${DateFormat('dd/MM/yyyy').format(job.ngayLuu!)}'
                    : '',
                style: TextStyle(fontSize: 11.5, color: Colors.grey[500]),
              ),
              TextButton(
                style: TextButton.styleFrom(
                  padding: EdgeInsets.zero,
                  minimumSize: Size.zero,
                  tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (ctx) => JobDetailScreen(maJob: job.maJob),
                    ),
                  );
                },
                child: const Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text('Xem chi tiết', style: TextStyle(fontSize: 12.5, fontWeight: FontWeight.bold, color: AppColors.primary)),
                    SizedBox(width: 4),
                    Icon(Icons.arrow_forward_ios, size: 11, color: AppColors.primary),
                  ],
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}


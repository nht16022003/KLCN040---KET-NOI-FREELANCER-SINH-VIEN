import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_ung_thue.dart';
import '../../viewmodels/viewmodel_ung_thue.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';

class ManHinhYeuCauThueFreelancer extends StatefulWidget {
  const ManHinhYeuCauThueFreelancer({super.key});

  @override
  State<ManHinhYeuCauThueFreelancer> createState() =>
      _ManHinhYeuCauThueFreelancerState();
}

class _ManHinhYeuCauThueFreelancerState
    extends State<ManHinhYeuCauThueFreelancer> {
  final currencyFormatter = NumberFormat('#,###', 'vi_VN');
  final dateFormatter = DateFormat('dd/MM/yyyy HH:mm');
  String _boLocTrangThai = 'TatCa'; // TatCa, ChoPhanHoi, DaChapNhan, DaTuChoi

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiDanhSach());
  }

  void _taiDanhSach() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<UngThueViewModel>().taiDanhSachTheoFreelancer(user.userId);
    }
  }

  Future<void> _xacNhanChapNhan(UngThueModel item) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: const Text(
          'Chấp nhận yêu cầu thuê?',
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        content: Text(
          'Bạn đồng ý nhận lời mời hợp tác cho công việc "${item.tenHienThiCongViec}" từ ${item.tenNhaTuyenDung}? Nhà tuyển dụng sẽ tiến hành lập hợp đồng và ký quỹ.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy', style: TextStyle(color: Colors.grey)),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primary,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(6),
              ),
            ),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Đồng ý nhận việc'),
          ),
        ],
      ),
    );

    if (xacNhan == true && mounted) {
      final vm = context.read<UngThueViewModel>();
      final ok = await vm.chapNhanYeuCau(item.maYeuCau, user.userId);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(ok
                ? 'Đã chấp nhận lời mời thuê!'
                : (vm.thongBaoLoi ?? 'Không thể chấp nhận yêu cầu.')),
            backgroundColor: ok ? AppColors.primary : Colors.red[700],
          ),
        );
      }
    }
  }

  Future<void> _moHopThoaiTuChoi(UngThueModel item) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final lyDoController = TextEditingController();
    final formKeyDialog = GlobalKey<FormState>();

    final xacNhan = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) {
        return Padding(
          padding: EdgeInsets.only(
            left: 20,
            right: 20,
            top: 20,
            bottom: MediaQuery.of(ctx).viewInsets.bottom + 20,
          ),
          child: Form(
            key: formKeyDialog,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text(
                      'Từ chối yêu cầu thuê',
                      style: TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.close),
                      onPressed: () => Navigator.pop(ctx, false),
                    ),
                  ],
                ),
                Text(
                  'Dự án: ${item.tenHienThiCongViec}',
                  style: const TextStyle(fontSize: 13, color: AppColors.mutedText),
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: lyDoController,
                  maxLines: 3,
                  autofocus: true,
                  decoration: const InputDecoration(
                    labelText: 'Lý do từ chối *',
                    hintText:
                        'Ví dụ: Tôi đang bận dự án khác, ngân sách chưa phù hợp...',
                    border: OutlineInputBorder(),
                    alignLabelWithHint: true,
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nêu lý do từ chối';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 18),
                SizedBox(
                  width: double.infinity,
                  height: 44,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.red[700],
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(6),
                      ),
                    ),
                    onPressed: () {
                      if (formKeyDialog.currentState!.validate()) {
                        Navigator.pop(ctx, true);
                      }
                    },
                    child: const Text(
                      'Xác nhận từ chối',
                      style:
                          TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );

    if (xacNhan == true && mounted) {
      final vm = context.read<UngThueViewModel>();
      final ok = await vm.tuChoiYeuCau(
        item.maYeuCau,
        user.userId,
        lyDoController.text.trim(),
      );
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(ok
                ? 'Đã gửi từ chối yêu cầu thuê.'
                : (vm.thongBaoLoi ?? 'Lỗi khi từ chối yêu cầu.')),
            backgroundColor: ok ? AppColors.primary : Colors.red[700],
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text(
          'Yêu cầu thuê trực tiếp',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: Column(
        children: [
          // Filter Chips
          Container(
            color: Colors.white,
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: [
                  _buildFilterChip('Tất cả', 'TatCa'),
                  const SizedBox(width: 8),
                  _buildFilterChip('Chờ phản hồi', 'ChoPhanHoi'),
                  const SizedBox(width: 8),
                  _buildFilterChip('Đã chấp nhận', 'DaChapNhan'),
                  const SizedBox(width: 8),
                  _buildFilterChip('Đã tạo hợp đồng', 'DaTaoHopDong'),
                  const SizedBox(width: 8),
                  _buildFilterChip('Đã từ chối', 'DaTuChoi'),
                ],
              ),
            ),
          ),

          const Divider(height: 1, color: Color(0xFFE2E8F0)),

          // List content
          Expanded(
            child: Consumer<UngThueViewModel>(
              builder: (context, vm, _) {
                if (vm.dangTai && vm.danhSachFreelancer.isEmpty) {
                  return const Center(child: CircularProgressIndicator());
                }

                if (vm.thongBaoLoi != null && vm.danhSachFreelancer.isEmpty) {
                  return Center(
                    child: Padding(
                      padding: const EdgeInsets.all(20),
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(Icons.error_outline,
                              size: 48, color: Colors.red),
                          const SizedBox(height: 12),
                          Text(
                            vm.thongBaoLoi!,
                            textAlign: TextAlign.center,
                            style: const TextStyle(fontSize: 14),
                          ),
                          const SizedBox(height: 16),
                          ElevatedButton(
                            onPressed: _taiDanhSach,
                            child: const Text('Thử lại'),
                          ),
                        ],
                      ),
                    ),
                  );
                }

                final ds = vm.danhSachFreelancer.where((item) {
                  if (_boLocTrangThai == 'TatCa') return true;
                  return item.trangThai == _boLocTrangThai;
                }).toList();

                if (ds.isEmpty) {
                  return RefreshIndicator(
                    onRefresh: () async => _taiDanhSach(),
                    child: ListView(
                      physics: const AlwaysScrollableScrollPhysics(),
                      children: const [
                        SizedBox(height: 100),
                        Center(
                          child: Column(
                            children: [
                              Icon(Icons.inbox_outlined,
                                  size: 64, color: Colors.grey),
                              SizedBox(height: 12),
                              Text(
                                'Chưa có yêu cầu thuê nào từ Nhà tuyển dụng.',
                                style: TextStyle(
                                  fontSize: 15,
                                  color: Colors.grey,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  );
                }

                return RefreshIndicator(
                  onRefresh: () async => _taiDanhSach(),
                  child: ListView.separated(
                    padding: const EdgeInsets.all(16),
                    itemCount: ds.length,
                    separatorBuilder: (_, __) => const SizedBox(height: 12),
                    itemBuilder: (context, index) {
                      final item = ds[index];
                      return _buildRequestCard(item);
                    },
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFilterChip(String label, String value) {
    final isSelected = _boLocTrangThai == value;
    return ChoiceChip(
      label: Text(label),
      selected: isSelected,
      labelStyle: TextStyle(
        fontSize: 13,
        fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
        color: isSelected ? Colors.white : AppColors.heading,
      ),
      selectedColor: AppColors.primary,
      backgroundColor: const Color(0xFFF1F5F9),
      side: BorderSide(
        color: isSelected ? AppColors.primary : const Color(0xFFE2E8F0),
      ),
      onSelected: (selected) {
        if (selected) {
          setState(() => _boLocTrangThai = value);
        }
      },
    );
  }

  Widget _buildRequestCard(UngThueModel item) {
    final avatar = item.avatarNhaTuyenDung != null &&
            item.avatarNhaTuyenDung!.isNotEmpty
        ? item.avatarNhaTuyenDung!
        : 'https://ui-avatars.com/api/?name=${Uri.encodeComponent(item.tenNhaTuyenDung)}&background=E8F5E9&color=1B5E36';

    Color badgeBg;
    Color badgeText;
    String badgeTitle;
    IconData badgeIcon;

    switch (item.trangThai) {
      case 'DaChapNhan':
        badgeBg = const Color(0xFFDCFCE7);
        badgeText = const Color(0xFF166534);
        badgeTitle = 'Đã chấp nhận';
        badgeIcon = Icons.check_circle_outline;
        break;
      case 'DaTaoHopDong':
        badgeBg = const Color(0xFFE0F2FE);
        badgeText = const Color(0xFF0369A1);
        badgeTitle = 'Đã tạo hợp đồng';
        badgeIcon = Icons.assignment_turned_in_outlined;
        break;
      case 'DaTuChoi':
        badgeBg = const Color(0xFFFEE2E2);
        badgeText = const Color(0xFF991B1B);
        badgeTitle = 'Đã từ chối';
        badgeIcon = Icons.cancel_outlined;
        break;
      case 'ChoPhanHoi':
      default:
        badgeBg = const Color(0xFFFEF3C7);
        badgeText = const Color(0xFF92400E);
        badgeTitle = 'Chờ phản hồi';
        badgeIcon = Icons.hourglass_top_outlined;
        break;
    }

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: const Color(0xFFE2E8F0)),
        boxShadow: const [
          BoxShadow(
            color: Color(0x08000000),
            blurRadius: 4,
            offset: Offset(0, 2),
          ),
        ],
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header: Employer Avatar + Name + Status Badge
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(20),
                child: Image.network(
                  avatar,
                  width: 42,
                  height: 42,
                  fit: BoxFit.cover,
                  errorBuilder: (ctx, err, stack) => Container(
                    width: 42,
                    height: 42,
                    color: Colors.grey[200],
                    child: const Icon(Icons.business, color: Colors.grey),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.tenNhaTuyenDung,
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    if (item.ngayGui != null) ...[
                      const SizedBox(height: 2),
                      Text(
                        'Nhận lúc: ${dateFormatter.format(item.ngayGui!)}',
                        style: TextStyle(fontSize: 11.5, color: Colors.grey[600]),
                      ),
                    ],
                  ],
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: badgeBg,
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(badgeIcon, size: 14, color: badgeText),
                    const SizedBox(width: 4),
                    Text(
                      badgeTitle,
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: badgeText,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),

          const SizedBox(height: 12),
          const Divider(height: 1, color: Color(0xFFF1F5F9)),
          const SizedBox(height: 10),

          // Job Title & Details
          Text(
            item.tenHienThiCongViec,
            style: const TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.bold,
              color: Color(0xFF0F172A),
            ),
          ),

          if (item.moTaCongViec != null &&
              item.moTaCongViec!.trim().isNotEmpty) ...[
            const SizedBox(height: 6),
            Text(
              item.moTaCongViec!,
              style: TextStyle(fontSize: 13, color: Colors.grey[700]),
            ),
          ],

          const SizedBox(height: 10),

          // Budget & Deadline (Sử dụng Wrap chống tràn màn hình)
          Wrap(
            spacing: 16,
            runSpacing: 6,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              if (item.nganSachDeNghi != null && item.nganSachDeNghi! > 0)
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.payments_outlined,
                        size: 16, color: Color(0xFFDC2626)),
                    const SizedBox(width: 4),
                    Text(
                      '${currencyFormatter.format(item.nganSachDeNghi)} đ',
                      style: const TextStyle(
                        fontSize: 13.5,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFFDC2626),
                      ),
                    ),
                  ],
                ),
              if (item.thoiHanDuKien != null)
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.event_outlined,
                        size: 16, color: AppColors.primary),
                    const SizedBox(width: 4),
                    Text(
                      'Hạn: ${DateFormat('dd/MM/yyyy').format(item.thoiHanDuKien!)}',
                      style: const TextStyle(
                        fontSize: 13,
                        color: AppColors.bodyText,
                      ),
                    ),
                  ],
                ),
            ],
          ),

          // Rejection reason if DaTuChoi
          if (item.laDaTuChoi &&
              item.lyDoTuChoi != null &&
              item.lyDoTuChoi!.trim().isNotEmpty) ...[
            const SizedBox(height: 10),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: const Color(0xFFFEF2F2),
                borderRadius: BorderRadius.circular(6),
                border: Border.all(color: const Color(0xFFFECACA)),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Lý do bạn từ chối:',
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF991B1B),
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    item.lyDoTuChoi!,
                    style: const TextStyle(
                      fontSize: 12.5,
                      color: Color(0xFF7F1D1D),
                    ),
                  ),
                ],
              ),
            ),
          ],

          // Thông báo trạng thái khi DaChapNhan (Đồng bộ với Website)
          if (item.laDaChapNhan) ...[
            const SizedBox(height: 12),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              decoration: BoxDecoration(
                color: const Color(0xFFF0FDF4),
                borderRadius: BorderRadius.circular(6),
                border: Border.all(color: const Color(0xFFBBF7D0)),
              ),
              child: const Row(
                children: [
                  Icon(Icons.info_outline, size: 16, color: Color(0xFF166534)),
                  SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Đang chờ Nhà tuyển dụng lập hợp đồng công việc và ký quỹ.',
                      style: TextStyle(
                        fontSize: 12.5,
                        fontWeight: FontWeight.w500,
                        color: Color(0xFF166534),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],

          // Thông báo trạng thái khi DaTaoHopDong
          if (item.laDaTaoHopDong) ...[
            const SizedBox(height: 12),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              decoration: BoxDecoration(
                color: const Color(0xFFF0FDF4),
                borderRadius: BorderRadius.circular(6),
                border: Border.all(color: const Color(0xFFBBF7D0)),
              ),
              child: const Row(
                children: [
                  Icon(Icons.check_circle_outline, size: 16, color: Color(0xFF166534)),
                  SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Nhà tuyển dụng đã tạo hợp đồng cho công việc này. Vui lòng vào mục Hợp đồng để kiểm tra và xác nhận.',
                      style: TextStyle(
                        fontSize: 12.5,
                        fontWeight: FontWeight.w500,
                        color: Color(0xFF166534),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],

          const SizedBox(height: 14),
          const Divider(height: 1, color: Color(0xFFF1F5F9)),
          const SizedBox(height: 10),

          // Action buttons: Nhắn tin trao đổi & Chấp nhận / Từ chối
          Row(
            children: [
              OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.primary,
                  side: const BorderSide(color: AppColors.primary),
                  padding:
                      const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(6),
                  ),
                ),
                icon: const Icon(Icons.chat_outlined, size: 16),
                label: const Text(
                  'Nhắn tin',
                  style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold),
                ),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ChatDetailScreen(
                        receiverId: item.maUserNTD.toString(),
                        receiverName: item.tenNhaTuyenDung,
                        avatarUrl: item.avatarNhaTuyenDung,
                        maJob: item.maJob,
                        maFreelancerStudent: item.maFreelancerStudent,
                      ),
                    ),
                  );
                },
              ),
              if (item.laChoPhanHoi) ...[
                const SizedBox(width: 8),
                Expanded(
                  child: OutlinedButton(
                    style: OutlinedButton.styleFrom(
                      foregroundColor: Colors.red[700],
                      side: BorderSide(color: Colors.red[300]!),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(6),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 8),
                    ),
                    onPressed: () => _moHopThoaiTuChoi(item),
                    child: const Text(
                      'Từ chối',
                      style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      foregroundColor: Colors.white,
                      elevation: 0,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(6),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 8),
                    ),
                    onPressed: () => _xacNhanChapNhan(item),
                    child: const Text(
                      'Chấp nhận',
                      style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
              ],
            ],
          ),
        ],
      ),
    );
  }
}


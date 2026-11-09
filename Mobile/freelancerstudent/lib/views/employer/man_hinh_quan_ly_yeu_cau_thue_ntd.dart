import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_ung_thue.dart';
import '../../models/mo_hinh_ung_tuyen.dart';
import '../../viewmodels/viewmodel_ung_thue.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';
import 'man_hinh_tao_hop_dong.dart';

class ManHinhQuanLyYeuCauThueNTD extends StatefulWidget {
  const ManHinhQuanLyYeuCauThueNTD({super.key});

  @override
  State<ManHinhQuanLyYeuCauThueNTD> createState() =>
      _ManHinhQuanLyYeuCauThueNTDState();
}

class _ManHinhQuanLyYeuCauThueNTDState
    extends State<ManHinhQuanLyYeuCauThueNTD> {
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
      context.read<UngThueViewModel>().taiDanhSachTheoNTD(user.userId);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text(
          'Yêu cầu thuê đã gửi',
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
                if (vm.dangTai && vm.danhSachNTD.isEmpty) {
                  return const Center(child: CircularProgressIndicator());
                }

                if (vm.thongBaoLoi != null && vm.danhSachNTD.isEmpty) {
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

                final ds = vm.danhSachNTD.where((item) {
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
                                'Không có yêu cầu thuê nào.',
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
    final avatar = item.avatarFreelancer != null &&
            item.avatarFreelancer!.isNotEmpty
        ? item.avatarFreelancer!
        : 'https://ui-avatars.com/api/?name=${Uri.encodeComponent(item.tenFreelancer)}&background=E8F5E9&color=1B5E36';

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
          // Header: Freelancer Avatar + Name + Status Badge
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
                    child: const Icon(Icons.person, color: Colors.grey),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.tenFreelancer,
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    if (item.ngayGui != null) ...[
                      const SizedBox(height: 2),
                      Text(
                        'Gửi lúc: ${dateFormatter.format(item.ngayGui!)}',
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
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(fontSize: 13, color: Colors.grey[700]),
            ),
          ],

          const SizedBox(height: 10),

          // Budget & Deadline (Sử dụng Wrap chống tràn giao diện)
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
                    'Lý do từ chối từ Freelancer:',
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

          const SizedBox(height: 14),
          const Divider(height: 1, color: Color(0xFFF1F5F9)),
          const SizedBox(height: 10),

          // Action Buttons: Nhắn tin trao đổi & Tạo hợp đồng ngay
          Row(
            children: [
              OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.primary,
                  side: const BorderSide(color: AppColors.primary),
                  padding:
                      const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
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
                        receiverId: item.maUserFreelancer.toString(),
                        receiverName: item.tenFreelancer,
                        avatarUrl: item.avatarFreelancer,
                        maJob: item.maJob,
                        maFreelancerStudent: item.maFreelancerStudent,
                      ),
                    ),
                  );
                },
              ),
              if (item.laDaChapNhan) ...[
                const SizedBox(width: 10),
                Expanded(
                  child: ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      foregroundColor: Colors.white,
                      elevation: 0,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 12, vertical: 9),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(6),
                      ),
                    ),
                    icon: const Icon(Icons.handshake_outlined, size: 17),
                    label: const Text(
                      'Tạo hợp đồng ngay',
                      style:
                          TextStyle(fontSize: 13.5, fontWeight: FontWeight.bold),
                      overflow: TextOverflow.ellipsis,
                    ),
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => CreateContractScreen(
                            applicant: ApplicationModel(
                              maJob: item.maJob ?? '',
                              maFreelancerStudents:
                                  item.maFreelancerStudent.toString(),
                              tieude: item.tenHienThiCongViec,
                              tenUser: item.tenFreelancer,
                              avatar: item.avatarFreelancer,
                              thulaoDeXuat: item.nganSachDeNghi ?? 0,
                            ),
                          ),
                        ),
                      );
                    },
                  ),
                ),
              ],
              if (item.laDaTaoHopDong) ...[
                const SizedBox(width: 10),
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    decoration: BoxDecoration(
                      color: const Color(0xFFF0FDF4),
                      borderRadius: BorderRadius.circular(6),
                      border: Border.all(color: const Color(0xFF86EFAC)),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: const [
                        Icon(Icons.check_circle_outline, size: 16, color: Color(0xFF166534)),
                        SizedBox(width: 6),
                        Flexible(
                          child: Text(
                            'Đã lập hợp đồng công việc',
                            style: TextStyle(
                              fontSize: 12.5,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF166534),
                            ),
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      ],
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

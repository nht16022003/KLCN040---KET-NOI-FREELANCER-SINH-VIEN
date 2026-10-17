import 'package:flutter/material.dart';

import '../../../models/nha_tuyen_dung_model.dart';
import '../../chat/chat_detail_screen.dart';

class NhaTuyenDungDetailBottomSheet extends StatelessWidget {
  final NhaTuyenDungModel ntd;

  const NhaTuyenDungDetailBottomSheet({super.key, required this.ntd});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      padding: EdgeInsets.only(
        top: 12,
        left: 20,
        right: 20,
        bottom: MediaQuery.of(context).viewInsets.bottom + 24,
      ),
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Thanh kéo nhỏ
            Center(
              child: Container(
                width: 44,
                height: 5,
                decoration: BoxDecoration(
                  color: Colors.grey.shade300,
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 16),

            // Header: Avatar + Tên công ty + Lĩnh vực
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Container(
                    width: 64,
                    height: 64,
                    color: const Color(0xFFE8F5E9),
                    child: Image.network(
                      ntd.hinhAnhHienThi,
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) => Center(
                        child: Text(
                          ntd.tenHienThi.isNotEmpty
                              ? ntd.tenHienThi[0].toUpperCase()
                              : 'C',
                          style: const TextStyle(
                            fontSize: 26,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF1D5D3B),
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        ntd.tenHienThi,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF183F2C),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 8,
                          vertical: 3,
                        ),
                        decoration: BoxDecoration(
                          color: const Color(0xFFEAF8EE),
                          borderRadius: BorderRadius.circular(6),
                          border: Border.all(color: const Color(0xFFBFE7C7)),
                        ),
                        child: Text(
                          ntd.linhVucHienThi,
                          style: const TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.w600,
                            color: Color(0xFF1D5D3B),
                          ),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          const Icon(Icons.star, color: Colors.amber, size: 16),
                          const SizedBox(width: 4),
                          Text(
                            '${ntd.diemDanhGia.toStringAsFixed(1)} / 5.0',
                            style: const TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.w600,
                              color: Color(0xFF5C7567),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 20),
            const Divider(height: 1),
            const SizedBox(height: 16),

            // Thông tin chi tiết
            const Text(
              'Thông tin liên hệ & địa chỉ',
              style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.bold,
                color: Color(0xFF183F2C),
              ),
            ),
            const SizedBox(height: 10),

            _ThongTinRow(
              icon: Icons.person_outline,
              label: 'Người đại diện',
              value: ntd.nguoiDaiDien,
            ),
            if (ntd.emailUser != null && ntd.emailUser!.isNotEmpty)
              _ThongTinRow(
                icon: Icons.email_outlined,
                label: 'Email liên hệ',
                value: ntd.emailUser!,
              ),
            if (ntd.sdtUser != null && ntd.sdtUser!.isNotEmpty)
              _ThongTinRow(
                icon: Icons.phone_outlined,
                label: 'Số điện thoại',
                value: ntd.sdtUser!,
              ),
            _ThongTinRow(
              icon: Icons.location_on_outlined,
              label: 'Khu vực / Trụ sở',
              value: ntd.diaChiHienThi,
            ),

            const SizedBox(height: 16),
            const Text(
              'Giới thiệu doanh nghiệp',
              style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.bold,
                color: Color(0xFF183F2C),
              ),
            ),
            const SizedBox(height: 8),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: const Color(0xFFF9FBF9),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: const Color(0xFFE0EBE2)),
              ),
              child: Text(
                ntd.gioiThieuHienThi,
                style: const TextStyle(
                  fontSize: 13,
                  height: 1.5,
                  color: Color(0xFF3B4D43),
                ),
              ),
            ),

            const SizedBox(height: 24),

            // Nút hành động
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.pop(context),
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 12),
                      side: const BorderSide(color: Color(0xFFC0D2C6)),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10),
                      ),
                    ),
                    child: const Text(
                      'Đóng',
                      style: TextStyle(color: Color(0xFF5C7567)),
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      Navigator.pop(context);
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => ChatDetailScreen(
                            receiverId: ntd.maUser.toString(),
                            receiverName: ntd.tenHienThi,
                            avatarUrl: ntd.hinhAnhHienThi,
                          ),
                        ),
                      );
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF1D5D3B),
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(vertical: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10),
                      ),
                    ),
                    icon: const Icon(Icons.chat_bubble_outline, size: 18),
                    label: const Text('Nhắn tin'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _ThongTinRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _ThongTinRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 5),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 18, color: const Color(0xFF2E8B57)),
          const SizedBox(width: 10),
          SizedBox(
            width: 110,
            child: Text(
              label,
              style: const TextStyle(
                fontSize: 13,
                color: Color(0xFF7A9384),
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w600,
                color: Color(0xFF183F2C),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

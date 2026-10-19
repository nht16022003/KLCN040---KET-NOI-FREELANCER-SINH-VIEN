import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../services/dich_vu_vi_tien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_vi_tien.dart';

class DepositScreen extends StatefulWidget {
  const DepositScreen({super.key});

  @override
  State<DepositScreen> createState() => _DepositScreenState();
}

class _DepositScreenState extends State<DepositScreen> {
  final TextEditingController _soTienController = TextEditingController();
  final WalletService _walletService = WalletService();
  bool _dangXuLy = false;
  Map<String, dynamic>? _ketQuaGiaoDich;

  final List<double> _mucTienGoiY = [100000, 200000, 500000, 1000000, 2000000];

  @override
  void dispose() {
    _soTienController.dispose();
    super.dispose();
  }

  Future<void> _taoGiaoDich() async {
    final rawText = _soTienController.text.replaceAll(RegExp(r'[^0-9]'), '');
    final soTien = double.tryParse(rawText) ?? 0;

    if (soTien < 100000 || soTien % 50000 != 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Số tiền nạp tối thiểu là 100.000đ và phải là bội số của 50.000đ.',
          ),
        ),
      );
      return;
    }

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để nạp tiền.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    final res = await _walletService.taoGiaoDichNapTien(user.userId, soTien);

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (res.isSuccess && res.data != null) {
      setState(() {
        _ketQuaGiaoDich = res.data;
      });
      // Tải lại số dư ví
      context.read<WalletViewModel>().taiThongTinVi(user.userId);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            res.message.isNotEmpty ? res.message : 'Tạo giao dịch nạp tiền thất bại.',
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Nạp tiền vào ví'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: _ketQuaGiaoDich != null
          ? _buildKetQua(context)
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                const Text(
                  'Chọn hoặc nhập số tiền cần nạp',
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.heading),
                ),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: _mucTienGoiY.map((tien) {
                    return ActionChip(
                      label: Text(AppFormatters.formatCurrency(tien)),
                      onPressed: () {
                        setState(() {
                          _soTienController.text = tien.toStringAsFixed(0);
                        });
                      },
                    );
                  }).toList(),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: _soTienController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: 'Số tiền (VNĐ)',
                    hintText: 'Tối thiểu 100.000đ',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.attach_money),
                  ),
                ),
                const SizedBox(height: 12),
                const Text(
                  'Quy định: Số tiền tối thiểu 100.000đ và là bội số của 50.000đ. Hệ thống xác thực tự động qua chuyển khoản SePay.',
                  style: TextStyle(fontSize: 12, color: AppColors.bodyText),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton(
                    onPressed: _dangXuLy ? null : _taoGiaoDich,
                    style: FilledButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      backgroundColor: AppColors.primary,
                    ),
                    child: _dangXuLy
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Text('Tạo giao dịch nạp tiền'),
                  ),
                ),
              ],
            ),
    );
  }

  Widget _buildKetQua(BuildContext context) {
    final kq = _ketQuaGiaoDich!;
    final maGiaoDich = kq['maGiaoDich']?.toString() ?? '';
    final noiDung = kq['noiDungChuyenKhoan']?.toString() ?? maGiaoDich;
    final soTien = (kq['soTien'] is num) ? (kq['soTien'] as num).toDouble() : 0.0;

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.lightGreenSurface,
            borderRadius: BorderRadius.circular(16),
          ),
          child: Column(
            children: [
              const Icon(
                Icons.check_circle_outline,
                color: AppColors.primary,
                size: 56,
              ),
              const SizedBox(height: 12),
              const Text(
                'Tạo đơn nạp tiền thành công!',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Số tiền: ${AppFormatters.formatCurrency(soTien)}',
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: AppColors.primary,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 20),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Thông tin chuyển khoản',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                ),
                const Divider(),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Mã giao dịch'),
                  subtitle: Text(
                    maGiaoDich,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Nội dung chuyển khoản (bắt buộc chính xác)'),
                  subtitle: SelectableText(
                    noiDung,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Colors.red,
                      fontSize: 16,
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                const Text(
                  'Vui lòng ghi đúng nội dung chuyển khoản ở trên để hệ thống tự động cộng tiền vào ví trong vòng 1-3 phút.',
                  style: TextStyle(fontSize: 13, color: Colors.black54),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 20),
        FilledButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Hoàn tất và quay lại ví'),
        ),
      ],
    );
  }
}

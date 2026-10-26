import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../viewmodels/viewmodel_vi_tien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../profile/man_hinh_minh_chung_the_sv.dart';

class WithdrawScreen extends StatefulWidget {
  const WithdrawScreen({super.key});

  @override
  State<WithdrawScreen> createState() => _WithdrawScreenState();
}

class _WithdrawScreenState extends State<WithdrawScreen> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _soTienController = TextEditingController();
  final TextEditingController _soTaiKhoanController = TextEditingController();
  final TextEditingController _tenChuTKController = TextEditingController();
  final TextEditingController _ghiChuController = TextEditingController();

  String _tenNganHang = 'Vietcombank';
  bool _dangXuLy = false;

  final List<String> _danhSachNganHang = [
    'Vietcombank',
    'MB Bank',
    'Techcombank',
    'BIDV',
    'Agribank',
    'VietinBank',
    'ACB',
    'VPBank',
    'TPBank',
    'Sacombank',
    'HDBank',
    'OCB',
    'VIB',
    'SHB',
  ];

  @override
  void dispose() {
    _soTienController.dispose();
    _soTaiKhoanController.dispose();
    _tenChuTKController.dispose();
    _ghiChuController.dispose();
    super.dispose();
  }

  Future<void> _guiYeuCauRutTien() async {
    if (!_formKey.currentState!.validate()) return;

    final walletVM = context.read<WalletViewModel>();
    final soDuKhaDung = walletVM.vi?.soDuKhaDung ?? 0;

    final rawSoTien = _soTienController.text.replaceAll(RegExp(r'[^0-9]'), '');
    final soTien = double.tryParse(rawSoTien) ?? 0;

    if (soTien < 50000) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Số tiền rút tối thiểu là 50.000đ.')),
      );
      return;
    }

    if (soTien > soDuKhaDung) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Số tiền rút vượt quá số dư khả dụng hiện tại.')),
      );
      return;
    }

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để rút tiền.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    final res = await walletVM.taoYeuCauRutTien(
      user.userId,
      soTienRut: soTien,
      tenNganHang: _tenNganHang,
      soTaiKhoan: _soTaiKhoanController.text.trim(),
      tenChuTaiKhoan: _tenChuTKController.text.trim().toUpperCase(),
    );

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (res.isSuccess) {
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (ctx) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          title: const Row(
            children: [
              Icon(Icons.check_circle, color: AppColors.primary),
              SizedBox(width: 8),
              Text('Gửi yêu cầu thành công'),
            ],
          ),
          content: Text(
            'Yêu cầu rút ${AppFormatters.formatCurrency(soTien)} đã được ghi nhận. '
            'Hệ thống sẽ đối soát và giải ngân về tài khoản của bạn trong thời gian sớm nhất.',
          ),
          actions: [
            FilledButton(
              onPressed: () {
                Navigator.pop(ctx);
                Navigator.pop(context); // Về lại màn hình ví
              },
              child: const Text('Đã hiểu'),
            ),
          ],
        ),
      );
    } else {
      final loi = walletVM.thongBaoLoi ?? 'Đã có lỗi xảy ra khi gửi yêu cầu rút tiền.';
      if (loi.contains('xác thực') || loi.contains('xác minh')) {
        showDialog(
          context: context,
          builder: (ctx) => AlertDialog(
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
            title: const Row(
              children: [
                Icon(Icons.shield_outlined, color: Colors.orange),
                SizedBox(width: 8),
                Text('Yêu cầu xác thực', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
              ],
            ),
            content: Text(loi),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(ctx),
                child: const Text('Đóng'),
              ),
              ElevatedButton(
                style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
                onPressed: () {
                  Navigator.pop(ctx);
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => const ManHinhMinhChungTheSV()),
                  );
                },
                child: const Text('Xác thực ngay', style: TextStyle(color: Colors.white)),
              ),
            ],
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(loi), backgroundColor: AppColors.error),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final walletVM = context.watch<WalletViewModel>();
    final soDuKhaDung = walletVM.vi?.soDuKhaDung ?? 0;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Rút tiền về tài khoản'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Card thông tin số dư
              Card(
                elevation: 0,
                color: AppColors.lightGreenSurface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                  side: BorderSide(color: AppColors.primary.withAlpha(50)),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        'Số dư khả dụng:',
                        style: TextStyle(
                          fontSize: 14,
                          color: AppColors.bodyText,
                        ),
                      ),
                      Text(
                        AppFormatters.formatCurrency(soDuKhaDung),
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: AppColors.primary,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // Số tiền muốn rút
              const Text(
                'Số tiền cần rút (VND)',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _soTienController,
                keyboardType: TextInputType.number,
                decoration: InputDecoration(
                  hintText: 'Tối thiểu 50.000đ',
                  prefixIcon: const Icon(Icons.attach_money),
                  suffixText: 'VND',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập số tiền muốn rút';
                  }
                  final numVal = double.tryParse(val.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
                  if (numVal < 50000) {
                    return 'Số tiền rút tối thiểu là 50.000đ';
                  }
                  if (numVal > soDuKhaDung) {
                    return 'Số tiền vượt quá số dư khả dụng';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Ngân hàng
              const Text(
                'Ngân hàng thụ hưởng',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              DropdownButtonFormField<String>(
                initialValue: _tenNganHang,
                decoration: InputDecoration(
                  prefixIcon: const Icon(Icons.account_balance),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                items: _danhSachNganHang.map((bank) {
                  return DropdownMenuItem(value: bank, child: Text(bank));
                }).toList(),
                onChanged: (val) {
                  if (val != null) setState(() => _tenNganHang = val);
                },
              ),
              const SizedBox(height: 16),

              // Số tài khoản
              const Text(
                'Số tài khoản',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _soTaiKhoanController,
                keyboardType: TextInputType.number,
                decoration: InputDecoration(
                  hintText: 'Nhập số tài khoản ngân hàng',
                  prefixIcon: const Icon(Icons.credit_card),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập số tài khoản';
                  }
                  if (val.trim().length < 6) {
                    return 'Số tài khoản không hợp lệ';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Tên chủ tài khoản
              const Text(
                'Tên chủ tài khoản (viết hoa không dấu)',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _tenChuTKController,
                textCapitalization: TextCapitalization.characters,
                decoration: InputDecoration(
                  hintText: 'Ví dụ: NGUYEN VAN A',
                  prefixIcon: const Icon(Icons.person_outline),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập tên chủ tài khoản';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Ghi chú
              const Text(
                'Ghi chú (tùy chọn)',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _ghiChuController,
                maxLines: 2,
                decoration: InputDecoration(
                  hintText: 'Nhập ghi chú nếu có...',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
              ),
              const SizedBox(height: 24),

              // Nút xác nhận rút tiền
              FilledButton(
                onPressed: _dangXuLy ? null : _guiYeuCauRutTien,
                style: FilledButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  backgroundColor: AppColors.primary,
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
                child: _dangXuLy
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                      )
                    : const Text(
                        'XÁC NHẬN RÚT TIỀN',
                        style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

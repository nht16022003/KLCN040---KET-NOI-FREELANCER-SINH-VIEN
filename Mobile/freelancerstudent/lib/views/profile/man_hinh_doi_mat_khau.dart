import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../services/dich_vu_xac_thuc.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _matKhauCuController = TextEditingController();
  final TextEditingController _matKhauMoiController = TextEditingController();
  final TextEditingController _xacNhanMKController = TextEditingController();

  final AuthService _authService = AuthService();
  bool _dangXuLy = false;
  bool _anMKCu = true;
  bool _anMKMoi = true;
  bool _anXacNhan = true;

  @override
  void dispose() {
    _matKhauCuController.dispose();
    _matKhauMoiController.dispose();
    _xacNhanMKController.dispose();
    super.dispose();
  }

  Future<void> _doiMatKhau() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để thực hiện đổi mật khẩu.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    final res = await _authService.doiMatKhau(
      maUser: user.userId,
      matKhauHienTai: _matKhauCuController.text.trim(),
      matKhauMoi: _matKhauMoiController.text.trim(),
      xacNhanMatKhauMoi: _xacNhanMKController.text.trim(),
    );

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (res.isSuccess) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Đổi mật khẩu thành công!'),
          backgroundColor: AppColors.primary,
        ),
      );
      Navigator.pop(context);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Đổi mật khẩu thất bại, vui lòng kiểm tra lại mật khẩu cũ.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Đổi mật khẩu'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Text(
                'Mật khẩu hiện tại',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _matKhauCuController,
                obscureText: _anMKCu,
                decoration: InputDecoration(
                  hintText: 'Nhập mật khẩu hiện tại của bạn',
                  prefixIcon: const Icon(Icons.lock_outline),
                  suffixIcon: IconButton(
                    icon: Icon(_anMKCu ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _anMKCu = !_anMKCu),
                  ),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập mật khẩu hiện tại';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              const Text(
                'Mật khẩu mới',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _matKhauMoiController,
                obscureText: _anMKMoi,
                decoration: InputDecoration(
                  hintText: 'Nhập mật khẩu mới (tối thiểu 6 ký tự)',
                  prefixIcon: const Icon(Icons.lock),
                  suffixIcon: IconButton(
                    icon: Icon(_anMKMoi ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _anMKMoi = !_anMKMoi),
                  ),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập mật khẩu mới';
                  }
                  if (val.trim().length < 6) {
                    return 'Mật khẩu mới phải có ít nhất 6 ký tự';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              const Text(
                'Xác nhận mật khẩu mới',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _xacNhanMKController,
                obscureText: _anXacNhan,
                decoration: InputDecoration(
                  hintText: 'Nhập lại mật khẩu mới',
                  prefixIcon: const Icon(Icons.check_circle_outline),
                  suffixIcon: IconButton(
                    icon: Icon(_anXacNhan ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _anXacNhan = !_anXacNhan),
                  ),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng xác nhận mật khẩu mới';
                  }
                  if (val.trim() != _matKhauMoiController.text.trim()) {
                    return 'Mật khẩu xác nhận không khớp với mật khẩu mới';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 28),

              FilledButton(
                onPressed: _dangXuLy ? null : _doiMatKhau,
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
                        'CẬP NHẬT MẬT KHẨU',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

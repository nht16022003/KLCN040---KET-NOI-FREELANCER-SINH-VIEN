import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../employer/man_hinh_trang_chu_nha_tuyen_dung.dart';
import '../home/man_hinh_trang_chu.dart';
import 'man_hinh_dang_ky.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  Future<void> _handleLogin() async {
    final email = _emailController.text.trim();
    final password = _passwordController.text.trim();

    if (email.isEmpty || password.isEmpty) {
      _showMessage('Vui lòng nhập email và mật khẩu');
      return;
    }

    final authViewModel = context.read<AuthViewModel>();
    final success = await authViewModel.login(email, password);

    if (!mounted) return;

    if (success) {
      final role = authViewModel.currentUser?.role ?? 'FreelancerStudent';
      final targetScreen = role == 'NhaTuyenDung' || role == 'Employer'
          ? const EmployerHomeScreen()
          : const HomeScreen();

      Navigator.of(context)
          .pushReplacement(MaterialPageRoute(builder: (_) => targetScreen));
      return;
    }

    _showMessage(authViewModel.errorMessage ?? 'Đăng nhập thất bại');
  }

  void _showMessage(String message) {
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text(message)));
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<AuthViewModel>(
      builder: (context, authViewModel, _) {
        return Scaffold(
          backgroundColor: AppColors.background,
          body: SafeArea(
            child: Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const SizedBox(height: 20),
                    Container(
                      width: 120,
                      height: 120,
                      decoration: const BoxDecoration(
                        color: AppColors.primary,
                        shape: BoxShape.circle,
                      ),
                      child: const Center(
                        child: Text(
                          'FLsv',
                          style: TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.w800,
                            color: Colors.white,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 18),
                    const Text(
                      'Đăng nhập',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 28,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 28),
                    TextField(
                      controller: _emailController,
                      keyboardType: TextInputType.text,
                      decoration: const InputDecoration(
                        labelText: 'Tên tài khoản',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.person, color: AppColors.primary),
                      ),
                    ),
                    const SizedBox(height: 16),
                    TextField(
                      controller: _passwordController,
                      obscureText: true,
                      decoration: const InputDecoration(
                        labelText: 'Mật khẩu',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.lock_outline, color: AppColors.primary),
                      ),
                    ),
                    const SizedBox(height: 24),
                    ElevatedButton(
                      onPressed: authViewModel.isLoading ? null : _handleLogin,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                      child: authViewModel.isLoading
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : const Text(
                              'Đăng nhập',
                              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                            ),
                    ),
                    const SizedBox(height: 12),
                    TextButton(
                      onPressed: () {},
                      style: TextButton.styleFrom(foregroundColor: AppColors.primary),
                      child: const Text('Quên mật khẩu?'),
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const RegisterScreen(),
                          ),
                        );
                      },
                      style: TextButton.styleFrom(foregroundColor: AppColors.primaryDark),
                      child: const Text(
                        'Đăng ký mới',
                        style: TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}

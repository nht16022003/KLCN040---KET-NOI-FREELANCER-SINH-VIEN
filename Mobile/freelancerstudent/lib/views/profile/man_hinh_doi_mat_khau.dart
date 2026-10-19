import 'package:flutter/material.dart';

class ChangePasswordScreen extends StatelessWidget {
  const ChangePasswordScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Đổi mật khẩu')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              obscureText: true,
              decoration: const InputDecoration(labelText: 'Mật khẩu cũ'),
            ),
            const SizedBox(height: 12),
            TextField(
              obscureText: true,
              decoration: const InputDecoration(labelText: 'Mật khẩu mới'),
            ),
            const SizedBox(height: 12),
            TextField(
              obscureText: true,
              decoration: const InputDecoration(labelText: 'Xác nhận mật khẩu'),
            ),
            const SizedBox(height: 20),
            FilledButton(onPressed: () {}, child: const Text('Cập nhật')),
          ],
        ),
      ),
    );
  }
}

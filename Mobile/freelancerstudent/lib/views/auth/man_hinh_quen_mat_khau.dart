import 'package:flutter/material.dart';

class ForgotPasswordScreen extends StatelessWidget {
  const ForgotPasswordScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Quên mật khẩu')),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Nhập email để đặt lại mật khẩu'),
            const SizedBox(height: 20),
            TextField(decoration: const InputDecoration(labelText: 'Email')),
            const SizedBox(height: 20),
            ElevatedButton(onPressed: () {}, child: const Text('Gửi yêu cầu')),
          ],
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';

class EditProfileScreen extends StatelessWidget {
  const EditProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Chỉnh sửa hồ sơ')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(decoration: const InputDecoration(labelText: 'Họ tên')),
            const SizedBox(height: 12),
            TextField(
              decoration: const InputDecoration(labelText: 'Số điện thoại'),
            ),
            const SizedBox(height: 12),
            TextField(
              maxLines: 3,
              decoration: const InputDecoration(labelText: 'Giới thiệu'),
            ),
            const SizedBox(height: 20),
            FilledButton(onPressed: () {}, child: const Text('Lưu thay đổi')),
          ],
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';

class CreateStudentPostScreen extends StatelessWidget {
  const CreateStudentPostScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Tạo bài tìm việc')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(decoration: const InputDecoration(labelText: 'Tiêu đề')),
            const SizedBox(height: 12),
            TextField(
              maxLines: 4,
              decoration: const InputDecoration(labelText: 'Mô tả kỹ năng'),
            ),
            const SizedBox(height: 12),
            TextField(
              decoration: const InputDecoration(
                labelText: 'Mức lương mong muốn',
              ),
            ),
            const SizedBox(height: 20),
            FilledButton(onPressed: () {}, child: const Text('Đăng bài')),
          ],
        ),
      ),
    );
  }
}

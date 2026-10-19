import 'package:flutter/material.dart';

class StudentPostDetailScreen extends StatelessWidget {
  const StudentPostDetailScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Chi tiết sinh viên')),
      body: const Padding(
        padding: EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Tên sinh viên',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 12),
            Text('Kỹ năng: Flutter, Dart, .NET'),
            SizedBox(height: 12),
            Text('Mức lương mong muốn: 12 triệu'),
          ],
        ),
      ),
    );
  }
}

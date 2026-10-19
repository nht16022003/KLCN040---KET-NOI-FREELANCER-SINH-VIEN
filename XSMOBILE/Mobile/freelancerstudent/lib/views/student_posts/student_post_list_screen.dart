import 'package:flutter/material.dart';

class StudentPostListScreen extends StatelessWidget {
  const StudentPostListScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Bài đăng tìm việc')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: const [
          Card(
            child: ListTile(
              title: Text('Sinh viên Flutter'),
              subtitle: Text('Tôi có kinh nghiệm làm UI/UX và API'),
            ),
          ),
          Card(
            child: ListTile(
              title: Text('Sinh viên Backend'),
              subtitle: Text('ASP.NET Core, SQL Server'),
            ),
          ),
        ],
      ),
    );
  }
}

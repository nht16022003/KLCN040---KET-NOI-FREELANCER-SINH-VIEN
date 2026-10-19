import 'package:flutter/material.dart';

class ChatListScreen extends StatelessWidget {
  const ChatListScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Tin nhắn')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: const [
          ListTile(
            leading: CircleAvatar(child: Text('A')),
            title: Text('Nguyễn Văn A'),
            subtitle: Text('Xin chào, em có thể bắt đầu không?'),
          ),
        ],
      ),
    );
  }
}
